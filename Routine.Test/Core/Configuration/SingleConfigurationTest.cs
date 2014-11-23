using NUnit.Framework;

namespace Routine.Test.Core.Configuration
{
	[TestFixture]
	public class SingleConfigurationTest : CoreTestBase
	{
		[Test]
		public void Returns_configured_value()
		{
			Assert.Fail();
		}

		[Test]
		public void When_set_more_than_once__returns_last_configured_value()
		{
			Assert.Fail();
		}

		[Test]
		public void When_not_set_and_not_required_returns_default_value()
		{
			Assert.Fail();
		}

		[Test]
		public void When_not_set_and_required_throws_ConfigurationException()
		{
			Assert.Fail();
		}
	}
}