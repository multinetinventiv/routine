using NUnit.Framework;
using Routine.Core.Configuration;

namespace Routine.Test.Core.Configuration
{
    [TestFixture]
    public class SingleConfigurationTest : CoreTestBase
    {
        [Test]
        public void Returns_configured_value()
        {
            var testing = new SingleConfiguration<string, string>("dummy", "test");

            testing.Set("expected");

            Assert.AreEqual("expected", testing.Get());
        }

        [Test]
        public void When_set_more_than_once__returns_last_configured_value()
        {
            var testing = new SingleConfiguration<string, string>("dummy", "test");

            testing.Set("expected");
            testing.Set("expected2");
            testing.Set("expected3");

            Assert.AreEqual("expected3", testing.Get());
        }

        [Test]
        public void When_not_set_and_not_required_returns_default_value()
        {
            var testing = new SingleConfiguration<string, string>("dummy", "test");

            Assert.IsNull(testing.Get());

            var testingInt = new SingleConfiguration<string, int>("dummy", "test");

            Assert.AreEqual(0, testingInt.Get());
        }

        [Test]
        public void When_not_set_and_required_throws_ConfigurationException()
        {
            var testing = new SingleConfiguration<string, string>("dummy", "test", true);

            Assert.Throws<ConfigurationException>(() => testing.Get());
        }

        [Test]
        public void When_setting_value__parent_configuration_can_be_used()
        {
            var testing = new SingleConfiguration<string, string>("dummy", "test");

            testing.Set(s => s.Replace("d", "s").Replace("m", "n"));

            Assert.AreEqual("sunny", testing.Get());
        }
    }
}