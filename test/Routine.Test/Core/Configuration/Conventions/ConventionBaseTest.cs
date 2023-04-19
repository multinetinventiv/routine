using Routine.Core.Configuration.Convention;
using Routine.Core.Configuration;

namespace Routine.Test.Core.Configuration.Conventions;

[TestFixture]
public class ConventionBaseTest : CoreTestBase
{
    private class TestConvention<T> : ConventionBase<T, string>
    {
        protected override string Apply(T obj) => obj.ToString();
    }

    [Test]
    public void When_no_condition_is_given__AppliesTo_always_returns_true()
    {
        IConvention<string, string> testing = new TestConvention<string>();

        Assert.That(testing.AppliesTo("any"), Is.True);
    }

    [Test]
    public void When_given__AppliesTo_returns_condition_s_output()
    {
        IConvention<string, string> testing = new TestConvention<string>().When(o => o == "valid");

        Assert.That(testing.AppliesTo("valid"), Is.True);
        Assert.That(testing.AppliesTo("invalid"), Is.False);
    }

    [Test]
    public void When_it_does_not_apply_to_given_object__Apply_throws_ConfigurationException()
    {
        IConvention<string, string> testing = new TestConvention<string>().When(o => o == "valid");

        try
        {
            testing.Apply(null);
            Assert.Fail("exception not thrown");
        }
        catch (ConfigurationException) { }
    }

    [Test]
    public void Calling_when_more_than_once_chains_predicates_with_and()
    {
        IConvention<string, string> testing = new TestConvention<string>().When(o => o.Contains("valid")).When(o => !o.Contains("in"));

        Assert.That(testing.AppliesTo("valid"), Is.True);
        Assert.That(testing.AppliesTo("invalid"), Is.False);
    }

    [Test]
    public void Facade__When()
    {
        IConvention<string, string> testing = new TestConvention<string>().When("match");

        Assert.That(testing.AppliesTo("match"), Is.True);
        Assert.That(testing.AppliesTo("nomatch"), Is.False);
    }

    [Test]
    public void Facade__WhenDefault()
    {
        IConvention<string, string> testing = new TestConvention<string>().WhenDefault();

        Assert.That(testing.AppliesTo(null), Is.True);
        Assert.That(testing.AppliesTo("string"), Is.False);
    }
}
