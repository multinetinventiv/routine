using Routine.Api.SoaClientConfiguration;

namespace Routine.Api.Builder
{
	public class SoaClientConfigurationBuilder
	{
		public GenericSoaClientConfiguration FromScratch()
		{
			return new GenericSoaClientConfiguration();
		}

		public GenericSoaClientConfiguration FromBasic()
		{
			return FromScratch();
		}
	}
}

