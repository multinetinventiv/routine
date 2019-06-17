namespace Routine.Service.Configuration
{
	public class ServiceConfigurationBuilder
	{
		public ConventionBasedServiceConfiguration FromBasic()
		{
			return new ConventionBasedServiceConfiguration()
				.RootPath.Set(ServiceController.ControllerName)
				.MaxResultLength.Set(Constants.DEFAULT_MAX_RESULT_LENGTH)
				.AllowGet.Set(false)
				.ExceptionResult.Set(new ExceptionResult())
				.ResponseHeaderValue.SetDefault()

				.NextLayer()
			;
		}
	}
}

