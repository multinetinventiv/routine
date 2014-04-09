using NUnit.Framework;
using Routine.Core;

namespace Routine.Test.Core.Interceptor
{
	[TestFixture]
	public class InterceptionContextTest
	{
		[Test]
		public void Can_contain_extra_data()
		{
			var testing = new InterceptionContext();

			testing["key"] = "value";

			Assert.AreEqual("value", testing["key"]);
		}

		[Test]
		public void Returns_null_when_given_key_does_not_exists()
		{
			var testing = new InterceptionContext();

			Assert.IsNull(testing["key"]);
		}
	}
}
