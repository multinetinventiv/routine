using Routine.Test.Core;
using Routine.Test.Interception.Stubs.AroundInterceptorBuilders;

using AsyncInvocation = Routine.Test.Interception.Stubs.Invocations.Async;
using AsyncBuilder = Routine.Test.Interception.Stubs.AroundInterceptorBuilders.Async;
using SyncInvocation = Routine.Test.Interception.Stubs.Invocations.Sync;
using SyncOverAsyncBuilder = Routine.Test.Interception.Stubs.AroundInterceptorBuilders.SyncOverAsync;
using IInvocation = Routine.Test.Interception.Stubs.Invocations.IInvocation;

namespace Routine.Test.Interception;

[TestFixture(typeof(AsyncBuilder), typeof(SyncInvocation))]
[TestFixture(typeof(AsyncBuilder), typeof(AsyncInvocation))]
[TestFixture(typeof(SyncOverAsyncBuilder), typeof(SyncInvocation))]
[TestFixture(typeof(SyncOverAsyncBuilder), typeof(AsyncInvocation))]
public class AroundInterceptorTest<TBuilder, TInvocation> : CoreTestBase
    where TBuilder : IBuilder, new()
    where TInvocation : IInvocation, new()
{
    private IBuilder _builder;
    private IInvocation _invocation;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        _builder = new TBuilder();
        _invocation = new TInvocation();
    }

    [Test]
    public void Before_success_fail_actions_can_be_defined_by_delegates()
    {
        _invocation.Context.Value = "begin";

        var testing = _builder.Build(
            before: () => _invocation.Context.Value += " - before",
            success: () => _invocation.Context.Value += " - success",
            fail: () => _invocation.Context.Value += " - fail",
            after: () => _invocation.Context.Value += " - after"
        );

        _invocation.Intercept(testing);

        Assert.AreEqual("begin - before - success - after", _invocation.Context.Value);

        _invocation.Context.Value = "begin";
        _invocation.FailsWith(new Exception());

        Assert.Throws<Exception>(() => _invocation.Intercept(testing));
        Assert.AreEqual("begin - before - fail - after", _invocation.Context.Value);
    }

    [Test]
    public void Nothing_happens_when_nothing_is_defined()
    {
        var testing = _builder.Build();

        _invocation.Intercept(testing);
    }

    [Test]
    public void Context_can_be_used_during_interception()
    {
        var testing = _builder.Build(
            beforeCtx: ctx => ctx.Value += " - before",
            successCtx: ctx => ctx.Value += " - success",
            failCtx: ctx => ctx.Value += " - fail",
            afterCtx: ctx => ctx.Value += " - after"
        );

        _invocation.Context.Value = "begin";

        _invocation.Intercept(testing);

        Assert.AreEqual("begin - before - success - after", _invocation.Context.Value);

        _invocation.Context.Value = "begin";
        _invocation.FailsWith(new Exception());

        Assert.Throws<Exception>(() => _invocation.Intercept(testing));

        Assert.AreEqual("begin - before - fail - after", _invocation.Context.Value);
    }

    [Test]
    public void Facade_Before()
    {
        var testing = _builder.FacadeBefore(beforeCtx: ctx => ctx.Value = "before");

        _invocation.Intercept(testing);

        Assert.AreEqual("before", _invocation.Context.Value);
    }

    [Test]
    public void Facade_Success()
    {
        var testing = _builder.FacadeSuccess(successCtx: ctx => ctx.Value = "success");

        _invocation.Intercept(testing);

        Assert.AreEqual("success", _invocation.Context.Value);
    }

    [Test]
    public void Facade_Fail()
    {
        var testing = _builder.FacadeFail(failCtx: ctx => ctx.Value = "fail");

        _invocation.FailsWith(new Exception());

        Assert.Throws<Exception>(() => _invocation.Intercept(testing));

        Assert.AreEqual("fail", _invocation.Context.Value);
    }

    [Test]
    public void Facade_After()
    {
        var testing = _builder.FacadeAfter(afterCtx: ctx => ctx.Value = "after");

        _invocation.Intercept(testing);

        Assert.AreEqual("after", _invocation.Context.Value);
    }
}
