namespace Routine.Service.Configuration
{
	public class ServiceConfigurationBuilder
	{
		public ConventionalServiceConfiguration FromBasic()
		{
			return new ConventionalServiceConfiguration()
				.RootPath.Set(ServiceController.ControllerName)
				.MaxResultLength.Set(Constants.DEFAULT_MAX_RESULT_LENGTH)
				.ExceptionResult.Set(new ExceptionResult())
				.ResponseHeaderValue.SetDefault()

				.NextLayer()
			;
		}
	}
}

