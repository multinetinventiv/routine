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

        Assert.AreEqual(4, actual.Count);
        Assert.AreEqual("1", actual[0]);
        Assert.AreEqual("2", actual[1]);
        Assert.AreEqual("3", actual[2]);
        Assert.AreEqual("4", actual[3]);
    }

    [Test]
    public void Returns_empty_list_when_no_items_are_added()
    {
        var testing = new ListConfiguration<string, string>("dummy", "test");

        var actual = testing.Get();

        Assert.AreEqual(0, actual.Count);
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

        Assert.AreEqual(4, actual.Count);
        Assert.AreEqual("1", actual[0]);
        Assert.AreEqual("2", actual[1]);
        Assert.AreEqual("3", actual[2]);
        Assert.AreEqual("4", actual[3]);
    }

    [Test]
    public void Facade_add_params_array()
    {
        var testing = new ListConfiguration<string, string>("dummy", "test");

        testing.Add("1", "2");
        var actual = testing.Get();

        Assert.AreEqual(2, actual.Count);
        Assert.AreEqual("1", actual[0]);
        Assert.AreEqual("2", actual[1]);
    }

    [Test]
    public void Facade_when_adding_item__parent_configuration_can_be_used()
    {
        var testing = new ListConfiguration<string, string>("dummy", "test");

        testing.Add(s => s.ToCharArray().Select(c => c.ToString(CultureInfo.InvariantCulture)));
        testing.Add(s => s.Replace("d", "s").Replace("m", "n"));

        var actual = testing.Get();

        Assert.AreEqual(6, actual.Count);
        Assert.AreEqual("d", actual[0]);
        Assert.AreEqual("u", actual[1]);
        Assert.AreEqual("m", actual[2]);
        Assert.AreEqual("m", actual[3]);
        Assert.AreEqual("y", actual[4]);
        Assert.AreEqual("sunny", actual[5]);
    }
}
