using System.Web;
using Routine.Test.Domain.Configuration;

namespace Routine.Test.Web.Mvc
{
	public class MvcApplication : HttpApplication
	{
		protected void Application_Start()
		{
			Configurer.ConfigureMvcApplication(Configurer.Mvc.DefaultTheme());
		}
	}
}
