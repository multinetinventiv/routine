using System.Web;
using Routine.Test.Domain.Configuration;

namespace Routine.Test.Web.Mvc.LeftMenu
{
	public class MvcApplication : HttpApplication
	{
		protected void Application_Start()
		{
			Configurer.ConfigureMvcApplication();
		}
	}
}