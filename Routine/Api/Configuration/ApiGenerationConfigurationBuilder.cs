namespace Routine.Api.Configuration
{
	public class ApiGenerationConfigurationBuilder
	{
		public ConventionalApiGenerationConfiguration FromBasic()
		{
			return new ConventionalApiGenerationConfiguration()
					.ReferencedTypeIsValueType.Set(false)
					.TargetValueType.SetDefault()
					.TypeIsRendered.Set(true)
					.InitializerIsRendered.Set(true)
					.MemberIsRendered.Set(true)
					.OperationIsRendered.Set(true)

					.NextLayer()
			;
		}
	}
}
