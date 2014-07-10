using Routine.Soa.Configuration;

namespace Routine.Soa.Builder
{
	public class SoaConfigurationBuilder
	{
		public GenericSoaConfiguration FromScratch()
		{
			return new GenericSoaConfiguration();
		}

		public GenericSoaConfiguration FromBasic()
		{
			return FromScratch()
				.MaxResultLengthIs(2*1024*1024)
				.ExtractExceptionResult.OnFailReturn(new SoaExceptionResult());
		}
	}
}

