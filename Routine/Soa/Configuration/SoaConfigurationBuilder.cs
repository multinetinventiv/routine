namespace Routine.Soa.Configuration
{
	public class SoaConfigurationBuilder
	{
		public ConventionalSoaConfiguration FromBasic()
		{
			return new ConventionalSoaConfiguration()
				.RootPath.Set(SoaController.ControllerName)
				.MaxResultLength.Set(Constants.DEFAULT_MAX_RESULT_LENGTH)
				.ExceptionResult.Set(new SoaExceptionResult())

				.NextLayer()
			;
		}
	}
}

