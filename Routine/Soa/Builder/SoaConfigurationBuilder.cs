using Routine.Soa.Configuration;

namespace Routine.Soa.Builder
{
	public class SoaConfigurationBuilder
	{
		public ConventionalSoaConfiguration FromScratch()
		{
			return new ConventionalSoaConfiguration();
		}

		public ConventionalSoaConfiguration FromBasic()
		{
			return FromScratch()
				.MaxResultLength.Set(2*1024*1024)
				.ExceptionResult.OnFailReturn(new SoaExceptionResult());
		}
	}
}

