using System.Linq;
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
				Constants.SERVICE_ROUTE_NAME_BASE + "index",
				Path("Index"),
				new
				{
					controller = ServiceController.ControllerName,
					action = ServiceController.IndexAction
				}
			);

			RouteTable.Routes.MapRoute(
				Constants.SERVICE_ROUTE_NAME_BASE + "configuration",
				Path("Configuration"),
				new
				{
					controller = ServiceController.ControllerName,
					action = ServiceController.ConfigurationAction
				}
			);

			RouteTable.Routes.MapRoute(
				Constants.SERVICE_ROUTE_NAME_BASE + "file",
				Path("File"),
				new
				{
					controller = ServiceController.ControllerName,
					action = ServiceController.FileAction
				}
			);

			RouteTable.Routes.MapRoute(
				Constants.SERVICE_ROUTE_NAME_BASE + "fonts",
				Path("Fonts/{fileName}/f"),
				new
				{
					controller = ServiceController.ControllerName,
					action = ServiceController.FontsAction
				}
			);

			RouteTable.Routes.MapRoute(
				Constants.SERVICE_ROUTE_NAME_BASE + "applicationmodel",
				Path("ApplicationModel"),
				new
				{
					controller = ServiceController.ControllerName,
					action = ServiceController.ApplicationModelAction
				}
			);

			RouteTable.Routes.MapRoute(
				Constants.SERVICE_ROUTE_NAME_BASE + "handle",
				Path("{modelId}/{idOrViewModelIdOrOperation}/{viewModelIdOrOperation}/{operation}"),
				new
				{
					controller = ServiceController.ControllerName,
					action = ServiceController.HandleAction,
					idOrViewModelIdOrOperation = UrlParameter.Optional,
					viewModelIdOrOperation = UrlParameter.Optional,
					operation = UrlParameter.Optional
				}
			);

			var jsonValueProviderFactory = ValueProviderFactories.Factories.OfType<JsonValueProviderFactory>().FirstOrDefault();

			if (jsonValueProviderFactory != null) ValueProviderFactories.Factories.Remove(jsonValueProviderFactory);
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
