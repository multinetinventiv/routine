using System;
using System.Web.Routing;
using Routine.Core;
using Routine.Engine;

namespace Routine.Service.Context
{
	public class DefaultServiceContext : IServiceContext
	{
		public IServiceConfiguration ServiceConfiguration { get; }
		public IObjectService ObjectService { get; }
		public ICoreContext CoreContext { get; }
        public Func<IServiceContext, IRouteHandler> HandlerFactory { get;  }

		public DefaultServiceContext(ICoreContext coreContext, IServiceConfiguration serviceConfiguration, IObjectService objectService, Func<IServiceContext, IRouteHandler> handlerFactory)
		{
			ServiceConfiguration = serviceConfiguration;
			ObjectService = objectService;
            HandlerFactory = handlerFactory;
            CoreContext = coreContext;

			RegisterRoutes();
		}

		private void RegisterRoutes()
		{
			var routeHandler = HandlerFactory(this);
            RouteTable.Routes.Add(Constants.SERVICE_ROUTE_NAME_BASE + "handler-index", new Route(url: "handler/{action}", defaults: new RouteValueDictionary()
			{
				{ "action", ServiceHttpHandler.IndexAction }
			}, constraints: new RouteValueDictionary()
			{
				{ "action",ServiceHttpHandler.IndexAction }
			}, routeHandler: routeHandler));

			RouteTable.Routes.Add(Constants.SERVICE_ROUTE_NAME_BASE + "handler-application-model", new Route(url: "handler/{action}", defaults: null, constraints: new RouteValueDictionary()
			{
				{ "action", ServiceHttpHandler.ApplicationModelAction }
			}, routeHandler: routeHandler));

			RouteTable.Routes.Add(Constants.SERVICE_ROUTE_NAME_BASE + "handler-configuration", new Route(url: "handler/{action}", defaults: null, constraints: new RouteValueDictionary()
			{
				{ "action", ServiceHttpHandler.ConfigurationAction }
			}, routeHandler: routeHandler));

			RouteTable.Routes.Add(Constants.SERVICE_ROUTE_NAME_BASE + "handler-file", new Route(url: "handler/{action}", defaults: null, constraints: new RouteValueDictionary()
			{
				{ "action", ServiceHttpHandler.FileAction }
			}, routeHandler: routeHandler));

			RouteTable.Routes.Add(Constants.SERVICE_ROUTE_NAME_BASE + "handler-fonts", new Route(url: "handler/{action}/{fileName}/f", defaults: null, constraints: new RouteValueDictionary()
			{
				{ "action", ServiceHttpHandler.FontsAction }
			}, routeHandler: routeHandler));

			RouteTable.Routes.Add(Constants.SERVICE_ROUTE_NAME_BASE + "handler-handle", new Route(url: "handler/{modelId}/{idOrViewModelIdOrOperation}/{viewModelIdOrOperation}/{operation}", defaults: new RouteValueDictionary()
			{
				{ "action", ServiceHttpHandler.HandleAction },
				{ "idOrViewModelIdOrOperation", string.Empty },
				{ "viewModelIdOrOperation", string.Empty },
				{ "operation", string.Empty }
			}, constraints: new RouteValueDictionary()
			{
				{ "modelId", ".*"},
			}, routeHandler: routeHandler));
		}

		public ReferenceData GetObjectReference(object @object)
		{
			return CoreContext.CreateDomainObject(@object).GetReferenceData();
		}

		public string GetModelId(IType type)
		{
			return CoreContext.GetDomainType(type).Id;
		}

		public IType GetType(string modelId)
		{
			return CoreContext.GetDomainType(modelId).Type;
		}

		public object GetObject(ReferenceData reference)
		{
			return CoreContext.GetObject(reference);
		}

		public object GetObject(IType type, string id)
		{
			return CoreContext.GetDomainType(type).Locate(id);
		}

		private string Path(string path)
		{
			var rootPath = ServiceConfiguration.GetRootPath() ?? string.Empty;

			if (rootPath.StartsWith("/"))
			{
				rootPath = rootPath.After("/");
			}

			if (!string.IsNullOrEmpty(rootPath) && !rootPath.EndsWith("/"))
			{
				rootPath += "/";
			}

			if (path.StartsWith("/"))
			{
				path = path.After("/");
			}

			return rootPath + path;
		}
	}
}
