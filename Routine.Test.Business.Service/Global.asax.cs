using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Routine.Test.Domain.Configuration;

namespace Routine.Test.Business.Service
{
	public class SoaApplication : HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			GlobalFilters.Filters.Add(new HandleErrorAttribute());
			RouteTable.Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			Configurer.ConfigureSoaApplication();
		}
	}
}
