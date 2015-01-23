using Routine.Api;
using Routine.Api.Configuration;
using Routine.Api.Template;

namespace Routine.Test.Api.Template
{
	public abstract class ClientApiTestBase : ApiTestBase
	{
		protected override IApiTemplate DefaultTestTemplate { get { return new ClientApiTemplate("ClientApi"); } }
		protected override ConventionalApiConfiguration BaseConfiguration() { return BuildRoutine.ApiConfig().ClientApi(); }
	}
}