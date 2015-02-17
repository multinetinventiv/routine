using System.Web;
using Routine.Test.Domain.Configuration;

namespace Routine.Test.Business.Service
{
	public class SoaApplication : HttpApplication
	{
		protected void Application_Start()
		{
			Configurer.ConfigureSoaApplication();
		}
	}
}
