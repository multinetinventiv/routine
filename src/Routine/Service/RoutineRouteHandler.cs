using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Routine.Core.Rest;
using Routine.Service.RequestHandlers;

namespace Routine.Service
{
	public class RoutineRouteHandler : IRoutineRouteHandler
	{
		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly IServiceContext serviceContext;
		private readonly IJsonSerializer jsonSerializer;

		public Dictionary<string, Func<IHttpContextAccessor, IRequestHandler>> RequestHandlers { get; }

		public RoutineRouteHandler(IServiceContext serviceContext, IJsonSerializer jsonSerializer, IHttpContextAccessor httpContextAccessor)
		{
			this.serviceContext = serviceContext;
			this.jsonSerializer = jsonSerializer;
			this.httpContextAccessor = httpContextAccessor;
			RequestHandlers = new Dictionary<string, Func<IHttpContextAccessor, IRequestHandler>>();
		}

		public virtual void RegisterRoutes(IApplicationBuilder applicationBuilder)
		{
			Add(hcb => new IndexRequestHandler(serviceContext, jsonSerializer, hcb), applicationBuilder);
			Add(hcb => new FileRequestHandler(serviceContext, jsonSerializer, hcb), applicationBuilder);
			Add(applicationBuilder: applicationBuilder, path: "{action}/{fileName}/f", factory: hcb => new FontsRequestHandler(serviceContext, jsonSerializer, hcb, "fileName"));
			Add(hcb => new ConfigurationRequestHandler(serviceContext, jsonSerializer, hcb), applicationBuilder);
			Add(hcb => new ApplicationModelRequestHandler(serviceContext, jsonSerializer, hcb), applicationBuilder);
			Add(applicationBuilder: applicationBuilder,
				path: "{modelId}/{idOrViewModelIdOrOperation}/{viewModelIdOrOperation}/{operation}",
				defaults: new { idOrViewModelIdOrOperation = string.Empty, viewModelIdOrOperation = string.Empty, operation = string.Empty },
				factory: hcb =>
					new HandleRequestHandler(serviceContext, jsonSerializer, hcb,
						modelIdRouteKey: "modelId",
						idOrViewModelIdOrOperationRouteKey: "idOrViewModelIdOrOperation",
						viewModelIdOrOperationRouteKey: "viewModelIdOrOperation",
						operationRouteKey: "operation",
						actionFactory: resolution => resolution.HasOperation
							? new DoRequestHandler(serviceContext, jsonSerializer, hcb, resolution) as IRequestHandler
							: new GetRequestHandler(serviceContext, jsonSerializer, hcb, resolution)
					)
			);
		}

		// todo: 0 referansi var. Ihtiyac yoksa silinmeli varsa muadili yazilmali
		// public IHttpHandler GetHttpHandler(RequestContext requestContext)
		// {
		// 	var action = $"{requestContext.RouteData.Values["action"]}".ToLowerInvariant();
		// 	var requestHandler = RequestHandlers[action](requestContext.HttpContext);

		// 	return new ProxyHttpHandler(requestHandler);
		// }

		private void Add<T>(Func<IHttpContextAccessor, T> factory, IApplicationBuilder applicationBuilder, object defaults = null)
			where T : IRequestHandler
		{
			Add(
				factory: factory,
				applicationBuilder:applicationBuilder,
				path: ActionNameFor<T>(),
				defaults: defaults
			);
		}
		private void Add<T>(Func<IHttpContextAccessor, T> factory, IApplicationBuilder applicationBuilder, string path, object defaults = null) where T : IRequestHandler
		{
			Add(
				factory: hc => factory(hc),
				applicationBuilder: applicationBuilder,
				path: path,
				action: ActionNameFor<T>(),
				defaults: defaults,
				index: typeof(IIndexRequestHandler).IsAssignableFrom(typeof(T))
			);
		}
		private void Add(
			Func<IHttpContextAccessor, IRequestHandler> factory,
            IApplicationBuilder applicationBuilder,
			string path,
			string action,
			object defaults = null,
			bool index = false
		)
		{
			if (defaults == null) { defaults = new { }; }

			RequestHandlers[action] = factory;

			applicationBuilder.Map($"/{path}", (app) => {
                app.Run(async context => {
                    await Task.Run(() =>
                    {
                        var requestHandler = RequestHandlers[action](httpContextAccessor);
                        requestHandler.WriteResponse();
                    });
                });
			});

			// if (index)
			// {
			// 	RouteTable.Routes.Add(Guid.NewGuid().ToString(format: "N"),
			// 		new Route(
			// 			url: string.Empty,
			// 			defaults: new RouteValueDictionary(defaults) { { "action", action } },
			// 			routeHandler: this
			// 		)
			// 	);
			// }
			// else
			// {
			// 	RouteTable.Routes.Add(Guid.NewGuid().ToString(format: "N"),
			// 		new Route(
			// 			url: serviceContext.ServiceConfiguration.GetPath(path),
			// 			defaults: new RouteValueDictionary(defaults) { { "action", action } },
			// 			routeHandler: this
			// 		)
			// 	);
			// }
		}

		private string ActionNameFor<T>() where T : IRequestHandler
		{
			return typeof(T).Name.BeforeLast("RequestHandler").ToLowerInvariant();
		}

        public RequestDelegate GetRequestHandler(HttpContext httpContext, RouteData routeData)
        {
            throw new NotImplementedException();
        }
    }
}
