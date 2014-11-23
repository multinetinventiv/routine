using NUnit.Framework;
using Routine.Engine;
using Routine.Engine.Locator;
using Routine.Test.Core;

namespace Routine.Test.Engine.Locator
{
	[TestFixture]
	public class BaseLocatorTest : CoreTestBase
	{
		private class TestLocator : BaseLocator<TestLocator>
		{
			protected override object Locate(IType type, string id){ return null; }
		}

		private TestLocator testing;
		private ILocator testingInterface;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			testingInterface = testing = new TestLocator();
		}

		[Test]
		public void Locate_throws_cannot_locate_exception_when_result_is_null_and_locator_does_not_accept_null()
		{
			testing.AcceptNullResult(true);

			var actual = testingInterface.Locate(type.of<string>(), "dummy");
			Assert.IsNull(actual);

			testing.AcceptNullResult(false);

			try
			{
				testingInterface.Locate(type.of<string>(), "dummy");
				Assert.Fail("exception not thrown");
			}
			catch(CannotLocateException){}
		}
	}
}

