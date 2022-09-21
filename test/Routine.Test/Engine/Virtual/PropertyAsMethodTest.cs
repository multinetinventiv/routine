using Routine.Engine;
using Routine.Engine.Virtual;
using Routine.Test.Core;

namespace Routine.Test.Engine.Virtual;

[TestFixture]
public class PropertyAsMethodTest : CoreTestBase
{
    [Test]
    public void Wraps_property_to_act_as_a_method_with_get_prefix()
    {
        IProperty property = type.of<string>().GetProperty(nameof(string.Length));
        var testing = new PropertyAsMethod(property);

        Assert.That(testing.Name, Is.EqualTo($"Get{property.Name}"));
        Assert.That(testing.GetCustomAttributes(), Is.EqualTo(property.GetCustomAttributes()));

        Assert.That(testing.ParentType, Is.EqualTo(property.ParentType));
        Assert.That(testing.ReturnType, Is.EqualTo(property.ReturnType));
        Assert.That(testing.GetReturnTypeCustomAttributes(), Is.EqualTo(property.GetReturnTypeCustomAttributes()));

        Assert.That(testing.Parameters, Is.Empty);
        Assert.That(testing.IsPublic, Is.EqualTo(property.IsPublic));
        Assert.That(testing.GetDeclaringType(true), Is.EqualTo(property.GetDeclaringType(true)));
    }

    [Test]
    public void Property_cannot_be_null()
    {
        Assert.Throws<ArgumentNullException>(() => new PropertyAsMethod(null));
    }

    [Test]
    public void Name_prefix_overrides_default_get_prefix()
    {
        IProperty property = type.of<string>().GetProperty(nameof(string.Length));
        var testing = new PropertyAsMethod(property, namePrefix: "Fetch");

        Assert.That(testing.Name, Is.EqualTo("FetchLength"));
    }

    [Test]
    public void Null_prefix_is_treated_as_empty_string()
    {
        IProperty property = type.of<string>().GetProperty(nameof(string.Length));
        var testing = new PropertyAsMethod(property, namePrefix: null);

        Assert.That(testing.Name, Is.EqualTo("Length"));
    }

    [Test]
    public async Task Perform_on_is_forwarded_to_fetch_from()
    {
        IProperty property = type.of<string>().GetProperty(nameof(string.Length));
        var testing = new PropertyAsMethod(property);

        var actual = testing.PerformOn("test");

        Assert.That(actual, Is.EqualTo(4));

        actual = await testing.PerformOnAsync("test");

        Assert.That(actual, Is.EqualTo(4));
    }
}
