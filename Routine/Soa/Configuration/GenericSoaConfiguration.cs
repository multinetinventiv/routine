using System.Web.Mvc;
using System.Web.Routing;

namespace Routine.Soa.Configuration
{
	public class GenericSoaConfiguration : ISoaConfiguration
	{
		public GenericSoaConfiguration()
		{
			RegisterRoutes();
		}

		private void RegisterRoutes()
		{
			RouteTable.Routes.MapRoute(
				"Soa",
				"Soa/{action}/{id}",
				new {controller="Soa", action="Index", id=""}
			);
		}
	}
}

