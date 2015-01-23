using NUnit.Framework;

namespace Routine.Test.Api.Template
{
	public abstract class ClientApiFeatureTestBase : ClientApiTestBase
	{
		[Test]
		public void Feature_supports_referenced_client_api()
		{
			Referenced_client_api_support_case();
		}

		protected abstract void Referenced_client_api_support_case();

		[Test]
		public void Feature_supports_list_input_and_output()
		{
			List_input_and_output_case();
		}

		protected abstract void List_input_and_output_case();
	}
}