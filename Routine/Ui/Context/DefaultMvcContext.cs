using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;

namespace Routine.Ui.Context
{
	public class DefaultMvcContext : IMvcContext
	{
		public IMvcConfiguration Configuration { get; private set; }
		public ApplicationViewModel Application { get; private set; }

		public DefaultMvcContext(IMvcConfiguration configuration, ApplicationViewModel application)
		{
			Configuration = configuration;
			Application = application;

			RegisterRoutes();
			RegisterVirtualPathProvider();
		}

		private void RegisterRoutes()
		{
			RouteTable.Routes.IgnoreRoute("{*staticfile}", new
			{
				//To treat a configured extension as a static file (e.g. -> ".*\.(css|js|png)(/.*)?")
				staticfile = string.Format(@".*\.({0})(/.*)?", string.Join("|", Configuration.GetStaticFileExtensions()))
			}); 
			
			RouteTable.Routes.MapRoute(
				Constants.MVC_PERFORM_AS_ROUTE_NAME,
				Path("{actualModelId}/{id}/{viewModelId}/Perform/{operationModelId}"),
				new { controller = RoutineController.ControllerName, action = RoutineController.PerformAsAction }
			);

			RouteTable.Routes.MapRoute(
				Constants.MVC_PERFORM_ROUTE_NAME,
				Path("{modelId}/{id}/Perform/{operationModelId}"),
				new { controller = RoutineController.ControllerName, action = RoutineController.PerformAction }
			);

			RouteTable.Routes.MapRoute(
				Constants.MVC_GET_AS_ROUTE_NAME,
				Path("{actualModelId}/{id}/{viewModelId}"),
				new { controller = RoutineController.ControllerName, action = RoutineController.GetAsAction, id = Configuration.GetDefaultObjectId() }
			);

			RouteTable.Routes.MapRoute(
				Constants.MVC_GET_ROUTE_NAME,
				Path("{modelId}/{id}"),
				new { controller = RoutineController.ControllerName, action = RoutineController.GetAction, id = Configuration.GetDefaultObjectId() }
			);
		}

		private string Path(string path)
		{
			var rootPath = Configuration.GetRootPath() ?? string.Empty;

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

		private void RegisterVirtualPathProvider()
		{
			HostingEnvironment.RegisterVirtualPathProvider(
				new EmbeddedResourceVirtualPathProvider(
					Configuration,
					HostingEnvironment.VirtualPathProvider
				)
			);
		}
	}
}