using NUnit.Framework;
using Routine.Engine.Locator;
using Routine.Engine;
using Routine.Test.Core;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Routine.Test.Engine.Locator
{
    [TestFixture]
    public class DelegateLocatorTest : CoreTestBase
    {
        [Test]
        public void Uses_delegate_to_locate_an_object()
        {
            var testing = new DelegateBasedLocator((_, ids) => ids.Select(id => "located: " + id).Cast<object>().ToList());
            var testingInterface = (ILocator)testing;

            var actual = testingInterface.Locate(type.of<string>(), new List<string> { "test1", "test2" });

            Assert.AreEqual("located: test1", actual[0]);
            Assert.AreEqual("located: test2", actual[1]);
        }

        [Test]
        public void When_no_delegate_was_given_throws_ArgumentNullException()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new DelegateBasedLocator(null));
        }
    }
}
