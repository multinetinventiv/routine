using Routine.Api.Generator.Configuration;

namespace Routine.Api.Generator.Builder
{
	public class ApiGenerationConfigurationBuilder
	{
		public GenericApiGenerationConfiguration FromScratch()
		{
			return new GenericApiGenerationConfiguration();
		}

		public GenericApiGenerationConfiguration FromBasic()
		{
			return FromScratch()
					.ExtractReferencedTypeIsClientType.OnFailReturn(false)
			;
		}
	}
}
