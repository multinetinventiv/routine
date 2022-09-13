using Routine.Test.Core;
using Routine.Test.Interception.Stubs.Interceptors;

using AsyncInvocation = Routine.Test.Interception.Stubs.Invocations.Async;
using SyncInvocation = Routine.Test.Interception.Stubs.Invocations.Sync;
using IInvocation = Routine.Test.Interception.Stubs.Invocations.IInvocation;

namespace Routine.Test.Interception;

[TestFixture(typeof(SyncAround), typeof(SyncInvocation))]
[TestFixture(typeof(SyncAround), typeof(AsyncInvocation))]
[TestFixture(typeof(AsyncAround), typeof(SyncInvocation))]
[TestFixture(typeof(AsyncAround), typeof(AsyncInvocation))]
public class AroundInterceptorBaseTest<TInterceptor, TInvocation> : CoreTestBase
    where TInterceptor : IAroundInterceptor<TInterceptor>, new()
    where TInvocation : IInvocation, new()
{
    private IInvocation invocation;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        invocation = new TInvocation();
    }

    [Test]
    public void A_successful_invocation_calls_OnBefore__OnSuccess_and_OnAfter_respectively()
    {
        var testing = new TInterceptor();

        invocation.Returns("result");

        var actual = invocation.Intercept(testing);

        Assert.AreEqual("result", actual);

        Assert.IsTrue((bool)invocation.Context["before"]);
        Assert.IsTrue((bool)invocation.Context["invocation"]);
        Assert.IsTrue((bool)invocation.Context["success"]);
        Assert.IsNull(invocation.Context["fail"]);
        Assert.IsTrue((bool)invocation.Context["after"]);
        Assert.AreEqual(1, invocation.Count);
    }

    [Test]
    public void An_unsuccessful_invocation_calls_OnBefore__OnFail_and_OnAfter_respectively()
    {
        var testing = new TInterceptor();

        invocation.FailsWith(new Exception());

        Assert.Throws<Exception>(() => invocation.Intercept(testing));

        Assert.IsTrue((bool)invocation.Context["before"]);
        Assert.IsNull(invocation.Context["invocation"]);
        Assert.IsNull(invocation.Context["success"]);
        Assert.IsTrue((bool)invocation.Context["fail"]);
        Assert.IsTrue((bool)invocation.Context["after"]);
    }

    [Test]
    public void Can_cancel_actual_invocation_and_return_some_other_result()
    {
        var testing = new TInterceptor();

        invocation.Returns("actual");
        testing.CancelAndReturn("cancel");

        var actual = invocation.Intercept(testing);

        Assert.AreEqual("cancel", actual);

        Assert.IsTrue((bool)invocation.Context["before"]);
        Assert.IsNull(invocation.Context["invocation"]);
        Assert.IsTrue((bool)invocation.Context["success"]);
        Assert.IsNull(invocation.Context["fail"]);
        Assert.IsTrue((bool)invocation.Context["after"]);
    }

    [Test]
    public void Can_alter_actual_invocation_result_after_a_successful_invocation()
    {
        var testing = new TInterceptor();

        invocation.Returns("actual");
        testing.OverrideActualResultWith("override");

        invocation.Intercept(testing);

        Assert.IsTrue((bool)invocation.Context["before"]);
        Assert.IsTrue((bool)invocation.Context["invocation"]);
        Assert.IsTrue((bool)invocation.Context["success"]);
        Assert.IsNull(invocation.Context["fail"]);
        Assert.IsTrue((bool)invocation.Context["after"]);
    }

    [Test]
    public void Can_hide_the_exception_and_return_a_result()
    {
        var testing = new TInterceptor();

        invocation.FailsWith(new Exception());
        testing.HideFailAndReturn("override");

        var actual = invocation.Intercept(testing);

        Assert.AreEqual("override", actual);

        Assert.IsTrue((bool)invocation.Context["before"]);
        Assert.IsNull(invocation.Context["invocation"]);
        Assert.IsNull(invocation.Context["success"]);
        Assert.IsTrue((bool)invocation.Context["fail"]);
        Assert.IsTrue((bool)invocation.Context["after"]);
    }

    [Test]
    public void Can_alter_the_exception_thrown_by_invocation()
    {
        var testing = new TInterceptor();

        invocation.FailsWith(new ArgumentNullException());
        testing.OverrideExceptionWith(new FormatException());

        Assert.Throws<FormatException>(() => invocation.Intercept(testing));

        Assert.IsTrue((bool)invocation.Context["before"]);
        Assert.IsNull(invocation.Context["invocation"]);
        Assert.IsNull(invocation.Context["success"]);
        Assert.IsTrue((bool)invocation.Context["fail"]);
        Assert.IsTrue((bool)invocation.Context["after"]);
    }

    [Test]
    public void When_exception_is_not_changed__preserves_stack_trace()
    {
        var testing = new TInterceptor();

        invocation.FailsWith(new ArgumentNullException());

        try
        {
            invocation.Intercept(testing);
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine(ex.StackTrace);
            Assert.IsTrue(ex.StackTrace?.Contains(invocation.ExceptionStackTraceLookupText), ex.StackTrace);
        }
    }

    [Test]
    public void When_an_exception_is_thrown_OnBefore__OnFail_is_still_called()
    {
        var testing = new TInterceptor();

        testing.FailOnBeforeWith(new Exception());

        Assert.Throws<Exception>(() => invocation.Intercept(testing));

        Assert.IsNull(invocation.Context["before"]);
        Assert.IsNull(invocation.Context["invocation"]);
        Assert.IsNull(invocation.Context["success"]);
        Assert.IsTrue((bool)invocation.Context["fail"]);
        Assert.IsTrue((bool)invocation.Context["after"]);
    }

    [Test]
    public void When_an_exception_is_thrown_OnSuccess__OnFail_is_still_called()
    {
        var testing = new TInterceptor();

        testing.FailOnSuccessWith(new Exception());

        Assert.Throws<Exception>(() => invocation.Intercept(testing));

        Assert.IsTrue((bool)invocation.Context["before"]);
        Assert.IsTrue((bool)invocation.Context["invocation"]);
        Assert.IsNull(invocation.Context["success"]);
        Assert.IsTrue((bool)invocation.Context["fail"]);
        Assert.IsTrue((bool)invocation.Context["after"]);
    }

    [Test]
    public void By_default_Intercepts_any_given_invocation()
    {
        var testing = new TInterceptor();

        invocation.Intercept(testing);

        Assert.IsTrue((bool)invocation.Context["before"]);
        Assert.IsTrue((bool)invocation.Context["invocation"]);
        Assert.IsTrue((bool)invocation.Context["success"]);
        Assert.IsNull(invocation.Context["fail"]);
        Assert.IsTrue((bool)invocation.Context["after"]);
    }

    [Test]
    public void Intercepts_only_when_clause_returns_true()
    {
        var testing = new TInterceptor();

        testing.When(_ => false);

        invocation.Intercept(testing);

        Assert.IsNull(invocation.Context["before"]);
        Assert.IsTrue((bool)invocation.Context["invocation"]);
        Assert.IsNull(invocation.Context["success"]);
        Assert.IsNull(invocation.Context["fail"]);
        Assert.IsNull(invocation.Context["after"]);

        testing.When(_ => true);

        invocation.Context["invocation"] = null;

        invocation.Intercept(testing);

        Assert.IsTrue((bool)invocation.Context["before"]);
        Assert.IsTrue((bool)invocation.Context["invocation"]);
        Assert.IsTrue((bool)invocation.Context["success"]);
        Assert.IsNull(invocation.Context["fail"]);
        Assert.IsTrue((bool)invocation.Context["after"]);
    }

    [Test]
    public void Custom_when_clauses_can_be_defined_by_sub_classes()
    {
        var testing = new TInterceptor();

        invocation.Context["override-base"] = true;

        testing.When(_ => false).WhenContextHas("override-base");

        invocation.Intercept(testing);

        Assert.IsTrue((bool)invocation.Context["before"]);
        Assert.IsTrue((bool)invocation.Context["invocation"]);
        Assert.IsTrue((bool)invocation.Context["success"]);
        Assert.IsNull(invocation.Context["fail"]);
        Assert.IsTrue((bool)invocation.Context["after"]);
    }
}
