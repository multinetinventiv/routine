using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Routine.Core;

namespace Routine.Test.Core.Interceptor
{
	[TestFixture]
	public class InterceptionContextTest
	{
		[Test]
		public void ContainsExtraData()
		{
			var testing = new InterceptionContext();

			testing["key"] = "value";

			Assert.AreEqual("value", testing["key"]);
		}

		[Test]
		public void ReturnsNullWhenGivenKeyDoesNotExists()
		{
			var testing = new InterceptionContext();

			Assert.IsNull(testing["key"]);
		}
	}
}
