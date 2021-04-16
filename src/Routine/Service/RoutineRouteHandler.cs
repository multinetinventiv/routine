using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Routine.Core.Rest;
using Routine.Service.RequestHandlers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Routine.Service
{
    public class RoutineRouteHandler : IRoutineRouteHandler
	{
		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly IServiceContext serviceContext;
		private readonly IJsonSerializer jsonSerializer;

		public Dictionary<string, Func<IHttpContextAccessor, IRequestHandler>> RequestHandlers { get; }

        public List<string> RequestHandlersList => RequestHandlers.Keys.Select(q => q.ToLowerInvariant()).ToList();

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
		
			//todo: URL pattern tipler RouteTable'a eklenmeli. Uygulama ayaga kalkmasi icin gelen istek request path den cozuldu.
            applicationBuilder.Use(async (context, next) =>
            {
                var path = context.Request.Path.ToString().ToLowerInvariant();

                var key = string.IsNullOrEmpty(path) || path == "/"
                    ? "index"
                    : RequestHandlersList.FirstOrDefault(r =>
                        context.Request.Path.ToString().ToLowerInvariant().Contains(r));

                if (string.IsNullOrEmpty(key))
                {
					context.Response.StatusCode = StatusCodes.Status404NotFound;
				}
                else
                {
					var requestHandler = RequestHandlers[key](httpContextAccessor);
                    requestHandler.WriteResponse();
				}

                await next();
            });


		}

      
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
