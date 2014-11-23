namespace Routine.Api.Context
{
	public class DefaultApiGenerationContext : IApiGenerationContext
	{
		public IApiGenerationConfiguration Configuration { get; private set; }
		public ApplicationCodeModel Application { get; private set; }

		public DefaultApiGenerationContext(IApiGenerationConfiguration configuration, ApplicationCodeModel application)
		{
			Configuration = configuration;
			Application = application;
		}
	}
}