using Routine.Core.Configuration.Convention;

namespace Routine.Test.Core.Configuration.Conventions;

[TestFixture]
public class DelegateBasedConventionTest : CoreTestBase
{
    [Test]
    public void Applies_given_delegate_to_given_object()
    {
        IConvention<object, string> testing =
            new DelegateBasedConvention<object, string>()
                .Return(o => o.ToString());

        Assert.That(testing.Apply(1), Is.EqualTo("1"));
    }

    [Test]
    public void When_no_delegate_was_given_returns_default_value()
    {
        IConvention<int, string> testing1 = new DelegateBasedConvention<int, string>();
        Assert.That(testing1.Apply(1), Is.EqualTo(null));

        IConvention<string, int> testing2 = new DelegateBasedConvention<string, int>();
        Assert.That(testing2.Apply("string"), Is.EqualTo(0));
    }

    [Test]
    public void Facade__Can_return_constant_result_no_matter_what()
    {
        IConvention<string, string> testing = BuildRoutine.Convention<string, string>().Constant("constant_result");

        Assert.That(testing.Apply("test1"), Is.EqualTo("constant_result"));
        Assert.That(testing.Apply("test2"), Is.EqualTo("constant_result"));
    }
}
