using Routine.Api.Configuration;

namespace Routine.Api.Builder
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
					.ExtractReferencedTypeIsValueType.OnFailReturn(false)
					.ExtractTargetValueType.OnFailReturn(null);
			;
		}
	}
}
