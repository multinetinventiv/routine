namespace Routine.Service.Configuration
{
	public class ServiceConfigurationBuilder
	{
		public ConventionBasedServiceConfiguration FromBasic()
		{
			return new ConventionBasedServiceConfiguration()
				.RootPath.Set(ServiceController.ControllerName)
				.AllowGet.Set(false)
				.ExceptionResult.Set(new ExceptionResult())
				.ResponseHeaderValue.SetDefault()

				.NextLayer()
			;
		}
	}
}

