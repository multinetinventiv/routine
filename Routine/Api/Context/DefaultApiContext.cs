namespace Routine.Api.Context
{
	public class DefaultApiContext : IApiContext
	{
		public IApiConfiguration Configuration { get; private set; }
		public ApplicationCodeModel Application { get; private set; }

		public DefaultApiContext(IApiConfiguration configuration, ApplicationCodeModel application)
		{
			Configuration = configuration;
			Application = application;
		}
	}
}