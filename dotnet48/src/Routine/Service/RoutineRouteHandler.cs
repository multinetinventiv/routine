using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using Routine.Core.Rest;
using Routine.Service.RequestHandlers;

namespace Routine.Service
{
	public class RoutineRouteHandler : IRoutineRouteHandler
	{
		private readonly IServiceContext serviceContext;
		private readonly IJsonSerializer jsonSerializer;

		public Dictionary<string, Func<HttpContextBase, IRequestHandler>> RequestHandlers { get; }

		public RoutineRouteHandler(IServiceContext serviceContext, IJsonSerializer jsonSerializer)
		{
			this.serviceContext = serviceContext;
			this.jsonSerializer = jsonSerializer;

			RequestHandlers = new Dictionary<string, Func<HttpContextBase, IRequestHandler>>();
		}

		public virtual void RegisterRoutes()
		{
			Add(hcb => new IndexRequestHandler(serviceContext, jsonSerializer, hcb));
			Add(hcb => new FileRequestHandler(serviceContext, jsonSerializer, hcb));
			Add(path: "{action}/{fileName}/f", factory: hcb => new FontsRequestHandler(serviceContext, jsonSerializer, hcb, "fileName"));
			Add(hcb => new ConfigurationRequestHandler(serviceContext, jsonSerializer, hcb));
			Add(hcb => new ApplicationModelRequestHandler(serviceContext, jsonSerializer, hcb));
			Add(
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

		public IHttpHandler GetHttpHandler(RequestContext requestContext)
		{
			var action = $"{requestContext.RouteData.Values["action"]}".ToLowerInvariant();
			var requestHandler = RequestHandlers[action](requestContext.HttpContext);

			return new ProxyHttpHandler(requestHandler);
		}

		private void Add<T>(Func<HttpContextBase, T> factory, object defaults = null)
			where T : IRequestHandler
		{
			Add(
				factory: factory,
				path: ActionNameFor<T>(),
				defaults: defaults
			);
		}
		private void Add<T>(Func<HttpContextBase, T> factory, string path, object defaults = null) where T : IRequestHandler
		{
			Add(
				factory: hc => factory(hc),
				path: path,
				action: ActionNameFor<T>(),
				defaults: defaults,
				index: typeof(IIndexRequestHandler).IsAssignableFrom(typeof(T))
			);
		}
		private void Add(
			Func<HttpContextBase, IRequestHandler> factory,
			string path,
			string action,
			object defaults = null,
			bool index = false
		)
		{
			if (defaults == null) { defaults = new { }; }

			RequestHandlers[action] = factory;

			if (index)
			{
				RouteTable.Routes.Add(Guid.NewGuid().ToString(format: "N"),
					new Route(
						url: string.Empty,
						defaults: new RouteValueDictionary(defaults) { { "action", action } },
						routeHandler: this
					)
				);
			}
			else
			{
				RouteTable.Routes.Add(Guid.NewGuid().ToString(format: "N"),
					new Route(
						url: serviceContext.ServiceConfiguration.GetPath(path),
						defaults: new RouteValueDictionary(defaults) { { "action", action } },
						routeHandler: this
					)
				);
			}
		}

		private string ActionNameFor<T>() where T : IRequestHandler
		{
			return typeof(T).Name.BeforeLast("RequestHandler").ToLowerInvariant();
		}
	}
}
