namespace Routine.Ui.Context
{
	public class DefaultMvcContext : IMvcContext
	{
		public IMvcConfiguration Configuration { get; private set; }
		public ApplicationViewModel Application { get; private set; }

		public DefaultMvcContext(IMvcConfiguration configuration, ApplicationViewModel application)
		{
			Configuration = configuration;
			Application = application;
		}
	}
}