using Routine.Core.Configuration;

namespace Routine.Test.Core.Configuration;

[TestFixture]
public class SingleConfigurationTest : CoreTestBase
{
    [Test]
    public void Returns_configured_value()
    {
        var testing = new SingleConfiguration<string, string>("dummy", "test");

        testing.Set("expected");

        Assert.That(testing.Get(), Is.EqualTo("expected"));
    }

    [Test]
    public void When_set_more_than_once__returns_last_configured_value()
    {
        var testing = new SingleConfiguration<string, string>("dummy", "test");

        testing.Set("expected");
        testing.Set("expected2");
        testing.Set("expected3");

        Assert.That(testing.Get(), Is.EqualTo("expected3"));
    }

    [Test]
    public void When_not_set_and_not_required_returns_default_value()
    {
        var testing = new SingleConfiguration<string, string>("dummy", "test");

        Assert.That(testing.Get(), Is.Null);

        var testingInt = new SingleConfiguration<string, int>("dummy", "test");

        Assert.That(testingInt.Get(), Is.EqualTo(0));
    }

    [Test]
    public void When_not_set_and_required_throws_ConfigurationException()
    {
        var testing = new SingleConfiguration<string, string>("dummy", "test", true);

        Assert.That(() => testing.Get(), Throws.TypeOf<ConfigurationException>());
    }

    [Test]
    public void When_setting_value__parent_configuration_can_be_used()
    {
        var testing = new SingleConfiguration<string, string>("dummy", "test");

        testing.Set(s => s.Replace("d", "s").Replace("m", "n"));

        Assert.That(testing.Get(), Is.EqualTo("sunny"));
    }
}
