using System.Web;
using Routine.Test.Domain.Configuration;

namespace Routine.Test.Business.Service
{
	public class ServiceApplication : HttpApplication
	{
		protected void Application_Start()
		{
			Configurer.ConfigureServiceApplication();
		}
	}
}
