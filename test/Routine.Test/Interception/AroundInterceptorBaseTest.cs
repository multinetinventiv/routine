using Routine.Test.Core;
using Routine.Test.Interception.Stubs.Interceptors;

using AsyncInvocation = Routine.Test.Interception.Stubs.Invocations.Async;
using SyncInvocation = Routine.Test.Interception.Stubs.Invocations.Sync;
using IInvocation = Routine.Test.Interception.Stubs.Invocations.IInvocation;

namespace Routine.Test.Interception;

[TestFixture(typeof(SyncInvocation))]
[TestFixture(typeof(AsyncInvocation))]
public class AroundInterceptorBaseTest<TInvocation> : CoreTestBase
    where TInvocation : IInvocation, new()
{
    private IInvocation _invocation;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        _invocation = new TInvocation();
    }

    [Test]
    public void A_successful_invocation_calls_OnBefore__OnSuccess_and_OnAfter_respectively()
    {
        var testing = new Around();

        _invocation.Returns("result");

        var actual = _invocation.Intercept(testing);

        Assert.That(actual, Is.EqualTo("result"));

        Assert.That((bool)_invocation.Context["before"], Is.True);
        Assert.That((bool)_invocation.Context["invocation"], Is.True);
        Assert.That((bool)_invocation.Context["success"], Is.True);
        Assert.That(_invocation.Context["fail"], Is.Null);
        Assert.That((bool)_invocation.Context["after"], Is.True);
        Assert.That(_invocation.Count, Is.EqualTo(1));
    }

    [Test]
    public void An_unsuccessful_invocation_calls_OnBefore__OnFail_and_OnAfter_respectively()
    {
        var testing = new Around();

        _invocation.FailsWith(new Exception());

        Assert.That(() => _invocation.Intercept(testing), Throws.TypeOf<Exception>());

        Assert.That((bool)_invocation.Context["before"], Is.True);
        Assert.That(_invocation.Context["invocation"], Is.Null);
        Assert.That(_invocation.Context["success"], Is.Null);
        Assert.That((bool)_invocation.Context["fail"], Is.True);
        Assert.That((bool)_invocation.Context["after"], Is.True);
    }

    [Test]
    public void Can_cancel_actual_invocation_and_return_some_other_result()
    {
        var testing = new Around();

        _invocation.Returns("actual");
        testing.CancelAndReturn("cancel");

        var actual = _invocation.Intercept(testing);

        Assert.That(actual, Is.EqualTo("cancel"));

        Assert.That((bool)_invocation.Context["before"], Is.True);
        Assert.That(_invocation.Context["invocation"], Is.Null);
        Assert.That((bool)_invocation.Context["success"], Is.True);
        Assert.That(_invocation.Context["fail"], Is.Null);
        Assert.That((bool)_invocation.Context["after"], Is.True);
    }

    [Test]
    public void Can_alter_actual_invocation_result_after_a_successful_invocation()
    {
        var testing = new Around();

        _invocation.Returns("actual");
        testing.OverrideActualResultWith("override");

        _invocation.Intercept(testing);

        Assert.That((bool)_invocation.Context["before"], Is.True);
        Assert.That((bool)_invocation.Context["invocation"], Is.True);
        Assert.That((bool)_invocation.Context["success"], Is.True);
        Assert.That(_invocation.Context["fail"], Is.Null);
        Assert.That((bool)_invocation.Context["after"], Is.True);
    }

    [Test]
    public void Can_hide_the_exception_and_return_a_result()
    {
        var testing = new Around();

        _invocation.FailsWith(new Exception());
        testing.HideFailAndReturn("override");

        var actual = _invocation.Intercept(testing);

        Assert.That(actual, Is.EqualTo("override"));

        Assert.That((bool)_invocation.Context["before"], Is.True);
        Assert.That(_invocation.Context["invocation"], Is.Null);
        Assert.That(_invocation.Context["success"], Is.Null);
        Assert.That((bool)_invocation.Context["fail"], Is.True);
        Assert.That((bool)_invocation.Context["after"], Is.True);
    }

    [Test]
    public void Can_alter_the_exception_thrown_by_invocation()
    {
        var testing = new Around();

        _invocation.FailsWith(new ArgumentNullException());
        testing.OverrideExceptionWith(new FormatException());

        Assert.That(() => _invocation.Intercept(testing), Throws.TypeOf<FormatException>());

        Assert.That((bool)_invocation.Context["before"], Is.True);
        Assert.That(_invocation.Context["invocation"], Is.Null);
        Assert.That(_invocation.Context["success"], Is.Null);
        Assert.That((bool)_invocation.Context["fail"], Is.True);
        Assert.That((bool)_invocation.Context["after"], Is.True);
    }

    [Test]
    public void When_exception_is_not_changed__preserves_stack_trace()
    {
        var testing = new Around();

        _invocation.FailsWith(new ArgumentNullException());

        try
        {
            _invocation.Intercept(testing);
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine(ex.StackTrace);
            Assert.That(ex.StackTrace?.Contains(_invocation.ExceptionStackTraceLookupText), Is.True, ex.StackTrace);
        }
    }

    [Test]
    public void When_an_exception_is_thrown_OnBefore__OnFail_is_still_called()
    {
        var testing = new Around();

        testing.FailOnBeforeWith(new Exception());

        Assert.That(() => _invocation.Intercept(testing), Throws.TypeOf<Exception>());

        Assert.That(_invocation.Context["before"], Is.Null);
        Assert.That(_invocation.Context["invocation"], Is.Null);
        Assert.That(_invocation.Context["success"], Is.Null);
        Assert.That((bool)_invocation.Context["fail"], Is.True);
        Assert.That((bool)_invocation.Context["after"], Is.True);
    }

    [Test]
    public void When_an_exception_is_thrown_OnSuccess__OnFail_is_still_called()
    {
        var testing = new Around();

        testing.FailOnSuccessWith(new Exception());

        Assert.That(() => _invocation.Intercept(testing), Throws.TypeOf<Exception>());

        Assert.That((bool)_invocation.Context["before"], Is.True);
        Assert.That((bool)_invocation.Context["invocation"], Is.True);
        Assert.That(_invocation.Context["success"], Is.Null);
        Assert.That((bool)_invocation.Context["fail"], Is.True);
        Assert.That((bool)_invocation.Context["after"], Is.True);
    }

    [Test]
    public void By_default_Intercepts_any_given_invocation()
    {
        var testing = new Around();

        _invocation.Intercept(testing);

        Assert.That((bool)_invocation.Context["before"], Is.True);
        Assert.That((bool)_invocation.Context["invocation"], Is.True);
        Assert.That((bool)_invocation.Context["success"], Is.True);
        Assert.That(_invocation.Context["fail"], Is.Null);
        Assert.That((bool)_invocation.Context["after"], Is.True);
    }

    [Test]
    public void Intercepts_only_when_clause_returns_true()
    {
        var testing = new Around();

        testing.When(_ => false);

        _invocation.Intercept(testing);

        Assert.That(_invocation.Context["before"], Is.Null);
        Assert.That((bool)_invocation.Context["invocation"], Is.True);
        Assert.That(_invocation.Context["success"], Is.Null);
        Assert.That(_invocation.Context["fail"], Is.Null);
        Assert.That(_invocation.Context["after"], Is.Null);

        testing.When(_ => true);

        _invocation.Context["invocation"] = null;

        _invocation.Intercept(testing);

        Assert.That((bool)_invocation.Context["before"], Is.True);
        Assert.That((bool)_invocation.Context["invocation"], Is.True);
        Assert.That((bool)_invocation.Context["success"], Is.True);
        Assert.That(_invocation.Context["fail"], Is.Null);
        Assert.That((bool)_invocation.Context["after"], Is.True);
    }

    [Test]
    public void Custom_when_clauses_can_be_defined_by_sub_classes()
    {
        var testing = new Around();

        _invocation.Context["override-base"] = true;

        testing.When(_ => false).WhenContextHas("override-base");

        _invocation.Intercept(testing);

        Assert.That((bool)_invocation.Context["before"], Is.True);
        Assert.That((bool)_invocation.Context["invocation"], Is.True);
        Assert.That((bool)_invocation.Context["success"], Is.True);
        Assert.That(_invocation.Context["fail"], Is.Null);
        Assert.That((bool)_invocation.Context["after"], Is.True);
    }
}
