using System.Web.Mvc;
using System.Web.Routing;
using Routine.Core;
using Routine.Engine;

namespace Routine.Service.Context
{
	public class DefaultServiceContext : IServiceContext
	{
		public IServiceConfiguration ServiceConfiguration { get; private set; }
		public IObjectService ObjectService { get; private set; }
		public ICoreContext CoreContext { get; private set; }

		public DefaultServiceContext(ICoreContext coreContext, IServiceConfiguration serviceConfiguration, IObjectService objectService)
		{
			ServiceConfiguration = serviceConfiguration;
			ObjectService = objectService;
			CoreContext = coreContext;

			RegisterRoutes();
		}

		private void RegisterRoutes()
		{
			RouteTable.Routes.MapRoute(
				Constants.SERVICE_ROUTE_NAME,
				ServiceConfiguration.GetRootPath() + "/{action}/{id}",
				new { controller = ServiceController.ControllerName, action = ServiceController.DefaultAction, id = "" }
			);
		}

		public ObjectReferenceData GetObjectReference(object @object)
		{
			return CoreContext.CreateDomainObject(@object).GetReferenceData();
		}

		public object GetObject(ObjectReferenceData reference)
		{
			return CoreContext.GetObject(reference);
		}

		public object GetObject(IType type, string id)
		{
			return CoreContext.GetDomainType(type).Locate(id);
		}
	}
}
