using NUnit.Framework;
using Routine.Interception;

namespace Routine.Test.Interception
{
	[TestFixture]
	public class InterceptionContextTest
	{
		[Test]
		public void Can_contain_extra_data()
		{
			var testing = new InterceptionContext("test");

			testing["key"] = "value";

			Assert.AreEqual("value", testing["key"]);
		}

		[Test]
		public void Returns_null_when_given_key_does_not_exists()
		{
			var testing = new InterceptionContext("test");

			Assert.IsNull(testing["key"]);
		}
	}
}
