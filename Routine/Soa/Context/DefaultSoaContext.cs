using System.Web.Mvc;
using System.Web.Routing;
using Routine.Core;
using Routine.Engine;

namespace Routine.Soa.Context
{
	public class DefaultSoaContext : ISoaContext
	{
		public ISoaConfiguration SoaConfiguration { get; private set; }
		public IObjectService ObjectService { get; private set; }
		public ICoreContext CoreContext { get; private set; }

		public DefaultSoaContext(ICoreContext coreContext, ISoaConfiguration soaConfiguration, IObjectService objectService)
		{
			SoaConfiguration = soaConfiguration;
			ObjectService = objectService;
			CoreContext = coreContext;

			RegisterRoutes();
		}

		private void RegisterRoutes()
		{
			RouteTable.Routes.MapRoute(
				Constants.SOA_ROUTE_NAME,
				SoaConfiguration.GetRootPath() + "/{action}/{id}",
				new { controller = SoaController.ControllerName, action = SoaController.DefaultAction, id = "" }
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
