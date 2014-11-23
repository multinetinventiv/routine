using System;
using NUnit.Framework;
using Routine.Engine;
using Routine.Engine.Locator;
using Routine.Test.Core;

namespace Routine.Test.Engine.Locator
{
	[TestFixture]
	public class DelegateLocatorTest : CoreTestBase
	{
		[Test]
		public void Uses_delegate_to_locate_an_object()
		{
			var testing = new DelegateBasedLocator((t, id) => "located: " + id);
			var testingInterface = (ILocator)testing;

			Assert.AreEqual("located: test", testingInterface.Locate(type.of<string>(), "test"));
		}

		[Test]
		public void When_no_delegate_was_given_throws_ArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new DelegateBasedLocator(null));
		}
	}
}

