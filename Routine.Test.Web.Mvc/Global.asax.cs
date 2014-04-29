using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Routine.Test.Domain.Configuration;

namespace Routine.Test.Web.Mvc
{
	public class MvcApplication : HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			GlobalFilters.Filters.Add(new HandleErrorAttribute());
			RouteTable.Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			Configurer.ConfigureMvcApplication();
		}
	}
}
