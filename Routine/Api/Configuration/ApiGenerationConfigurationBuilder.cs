namespace Routine.Api.Configuration
{
	public class ApiGenerationConfigurationBuilder
	{
		public ConventionalApiGenerationConfiguration FromScratch()
		{
			return new ConventionalApiGenerationConfiguration();
		}

		public ConventionalApiGenerationConfiguration FromBasic()
		{
			return FromScratch()
					.ReferencedTypeIsValueType.OnFailReturn(false)
					.TargetValueType.OnFailReturn(null)
					.TypeIsRendered.OnFailReturn(true)
					.InitializerIsRendered.OnFailReturn(true)
					.MemberIsRendered.OnFailReturn(true)
					.OperationIsRendered.OnFailReturn(true)
			;
		}
	}
}
