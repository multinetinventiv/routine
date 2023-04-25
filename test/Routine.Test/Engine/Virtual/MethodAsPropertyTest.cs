using Routine.Engine;
using Routine.Engine.Virtual;
using Routine.Test.Core;

namespace Routine.Test.Engine.Virtual;

[TestFixture]
public class MethodAsPropertyTest : CoreTestBase
{
    [Test]
    public void Wraps_method_to_act_as_a_property()
    {
        IMethod method = type.of<string>().GetMethods(nameof(string.ToUpper)).First();
        var testing = new MethodAsProperty(method);

        Assert.That(testing.Name, Is.EqualTo(method.Name));
        Assert.That(testing.GetCustomAttributes(), Is.EqualTo(method.GetCustomAttributes()));
        Assert.That(testing.ParentType, Is.EqualTo(method.ParentType));
        Assert.That(testing.ReturnType, Is.EqualTo(method.ReturnType));
        Assert.That(testing.GetReturnTypeCustomAttributes(), Is.EqualTo(method.GetReturnTypeCustomAttributes()));
        Assert.That(testing.IsPublic, Is.EqualTo(method.IsPublic));
        Assert.That(testing.GetDeclaringType(true), Is.EqualTo(method.GetDeclaringType(true)));
    }

    [Test]
    public void Method_cannot_be_null()
    {
        Assert.That(() => new MethodAsProperty(null), Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    public void Ignore_prefix_removes_given_prefix_from_method_name()
    {
        IMethod method = type.of<string>().GetMethods(nameof(string.ToUpper)).First();
        var testing = new MethodAsProperty(method, ignorePrefix: "To");

        Assert.That(testing.Name, Is.EqualTo("Upper"));
    }

    [Test]
    public void Ignore_prefix_is_empty_when_not_given()
    {
        IMethod method = type.of<string>().GetMethods(nameof(string.ToUpper)).First();
        var testing = new MethodAsProperty(method, ignorePrefix: null);

        Assert.That(testing.Name, Is.EqualTo(nameof(string.ToUpper)));
    }

    [Test]
    public void Void_method_are_not_allowed()
    {
        var method = type.of<MethodAsPropertyTest>().GetMethod(nameof(Void_method_are_not_allowed));

        Assert.That(() => new MethodAsProperty(method), Throws.TypeOf<ArgumentException>());
    }

    [Test]
    public void Fetching_property_is_forwarded_to_method_perform()
    {
        IMethod method = type.of<string>().GetMethods(nameof(string.ToUpper)).First();
        var testing = new MethodAsProperty(method);

        var actual = testing.FetchFrom("test");

        Assert.That(actual, Is.EqualTo("TEST"));
    }

    [Test]
    public void Parameters_are_given_to_fetch_without_asking_parameters()
    {
        IMethod method = type.of<string>().GetMethods(nameof(string.Substring)).First();
        var testing = new MethodAsProperty(method, 1);

        var actual = testing.FetchFrom("test");

        Assert.That(actual, Is.EqualTo("est"));
    }

    [Test]
    public void Parameter_length_must_match_required_number_of_parameters()
    {
        IMethod method = type.of<string>().GetMethods(nameof(string.Substring)).First();
        Assert.That(() => new MethodAsProperty(method), Throws.TypeOf<ArgumentException>());
    }
}
