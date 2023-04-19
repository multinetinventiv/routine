using Routine.Core.Configuration;
using System.Globalization;

namespace Routine.Test.Core.Configuration;

[TestFixture]
public class ListConfigurationTest : CoreTestBase
{
    [Test]
    public void Returns_added_items()
    {
        var testing = new ListConfiguration<string, string>("dummy", "test");

        testing.Add(new List<string> { "1", "2" });
        testing.Add(new List<string> { "3", "4" });

        var actual = testing.Get();

        Assert.That(actual.Count, Is.EqualTo(4));
        Assert.That(actual[0], Is.EqualTo("1"));
        Assert.That(actual[1], Is.EqualTo("2"));
        Assert.That(actual[2], Is.EqualTo("3"));
        Assert.That(actual[3], Is.EqualTo("4"));
    }

    [Test]
    public void Returns_empty_list_when_no_items_are_added()
    {
        var testing = new ListConfiguration<string, string>("dummy", "test");

        var actual = testing.Get();

        Assert.That(actual.Count, Is.EqualTo(0));
    }

    [Test]
    public void Merges_with_other_ListConfiguration_adding_other_s_items_to_the_end()
    {
        var testing = new ListConfiguration<string, string>("dummy", "test");
        var testingOther = new ListConfiguration<string, string>("dummy", "test");

        testing.Add("1", "2");
        testingOther.Add("3", "4");

        testing.Merge(testingOther);

        var actual = testing.Get();

        Assert.That(actual.Count, Is.EqualTo(4));
        Assert.That(actual[0], Is.EqualTo("1"));
        Assert.That(actual[1], Is.EqualTo("2"));
        Assert.That(actual[2], Is.EqualTo("3"));
        Assert.That(actual[3], Is.EqualTo("4"));
    }

    [Test]
    public void Facade_add_params_array()
    {
        var testing = new ListConfiguration<string, string>("dummy", "test");

        testing.Add("1", "2");
        var actual = testing.Get();

        Assert.That(actual.Count, Is.EqualTo(2));
        Assert.That(actual[0], Is.EqualTo("1"));
        Assert.That(actual[1], Is.EqualTo("2"));
    }

    [Test]
    public void Facade_when_adding_item__parent_configuration_can_be_used()
    {
        var testing = new ListConfiguration<string, string>("dummy", "test");

        testing.Add(s => s.ToCharArray().Select(c => c.ToString(CultureInfo.InvariantCulture)));
        testing.Add(s => s.Replace("d", "s").Replace("m", "n"));

        var actual = testing.Get();

        Assert.That(actual.Count, Is.EqualTo(6));
        Assert.That(actual[0], Is.EqualTo("d"));
        Assert.That(actual[1], Is.EqualTo("u"));
        Assert.That(actual[2], Is.EqualTo("m"));
        Assert.That(actual[3], Is.EqualTo("m"));
        Assert.That(actual[4], Is.EqualTo("y"));
        Assert.That(actual[5], Is.EqualTo("sunny"));
    }
}
