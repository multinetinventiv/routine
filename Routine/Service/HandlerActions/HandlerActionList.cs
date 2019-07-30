using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;

namespace Routine.Service.HandlerActions
{
	public class HandlerActionList
	{
		private readonly ServiceRouteHandler routeHandler;
		private readonly IServiceContext serviceContext;
		private readonly Dictionary<string, Func<HttpContextBase, IHandlerAction>> actions;

		public HandlerActionList(ServiceRouteHandler routeHandler, IServiceContext serviceContext)
		{
			this.routeHandler = routeHandler;
			this.serviceContext = serviceContext;

			actions = new Dictionary<string, Func<HttpContextBase, IHandlerAction>>();
		}

		public HandlerActionList Add<T>(Func<HttpContextBase, T> factory) where T : IHandlerAction
		{
			return Add(factory, ActionNameFor<T>());
		}

		public HandlerActionList Add<T>(Func<HttpContextBase, T> factory, string path) where T : IHandlerAction
		{
			return Add(
				action: ActionNameFor<T>(),
				path: path,
				factory: hc => factory(hc),
				index: typeof(IIndexHandlerAction).IsAssignableFrom(typeof(T))
			);
		}

		public HandlerActionList Add(Func<HttpContextBase, IHandlerAction> factory, string path, string action, bool index = false)
		{
			actions[action] = factory;

			if (index)
			{
				RouteTable.Routes.Add(Guid.NewGuid().ToString(format: "N"),
					new Route(
						url: string.Empty,
						defaults: new RouteValueDictionary { { "action", action } },
						routeHandler: routeHandler
					)
				);
			}
			else
			{
				RouteTable.Routes.Add(Guid.NewGuid().ToString(format: "N"),
					new Route(
						url: serviceContext.ServiceConfiguration.GetPath(path),
						defaults: new RouteValueDictionary { { "action", action } },
						routeHandler: routeHandler
					)
				);
			}

			return this;
		}

		private string ActionNameFor<T>() where T : IHandlerAction
		{
			return typeof(T).Name.BeforeLast("HandlerAction").ToLowerInvariant();
		}

		public IHandlerAction Get(HttpContextBase httpContext)
		{
			var routeData = httpContext.Request.RequestContext.RouteData;
			var action = $"{routeData.Values["action"]}".ToLowerInvariant();

			return actions[action](httpContext);
		}
	}
}