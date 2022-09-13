using Routine.Test.Core;
using Routine.Test.Interception.Stubs.AroundInterceptorBuilders;
using Routine.Test.Interception.Stubs.Invocations;

using AsyncInvocation = Routine.Test.Interception.Stubs.Invocations.Async;
using AsyncBuilder = Routine.Test.Interception.Stubs.AroundInterceptorBuilders.Async;
using SyncInvocation = Routine.Test.Interception.Stubs.Invocations.Sync;
using SyncBuilder = Routine.Test.Interception.Stubs.AroundInterceptorBuilders.Sync;
using SyncOverAsyncBuilder = Routine.Test.Interception.Stubs.AroundInterceptorBuilders.SyncOverAsync;
using IInvocation = Routine.Test.Interception.Stubs.Invocations.IInvocation;

namespace Routine.Test.Interception;

[TestFixture(typeof(SyncBuilder), typeof(SyncInvocation))]
[TestFixture(typeof(SyncBuilder), typeof(AsyncInvocation))]
[TestFixture(typeof(AsyncBuilder), typeof(SyncInvocation))]
[TestFixture(typeof(AsyncBuilder), typeof(AsyncInvocation))]
[TestFixture(typeof(SyncOverAsyncBuilder), typeof(SyncInvocation))]
[TestFixture(typeof(SyncOverAsyncBuilder), typeof(AsyncInvocation))]
public class AroundInterceptorTest<TBuilder, TInvocation> : CoreTestBase
    where TBuilder : IBuilder, new()
    where TInvocation : IInvocation, new()
{
    private IBuilder builder;
    private IInvocation invocation;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        builder = new TBuilder();
        invocation = new TInvocation();
    }

    [Test]
    public void Before_success_fail_actions_can_be_defined_by_delegates()
    {
        invocation.Context.Value = "begin";

        var testing = builder.Build(
            before: () => invocation.Context.Value += " - before",
            success: () => invocation.Context.Value += " - success",
            fail: () => invocation.Context.Value += " - fail",
            after: () => invocation.Context.Value += " - after"
        );

        invocation.Intercept(testing);

        Assert.AreEqual("begin - before - success - after", invocation.Context.Value);

        invocation.Context.Value = "begin";
        invocation.FailsWith(new Exception());

        Assert.Throws<Exception>(() => invocation.Intercept(testing));
        Assert.AreEqual("begin - before - fail - after", invocation.Context.Value);
    }

    [Test]
    public void Nothing_happens_when_nothing_is_defined()
    {
        var testing = builder.Build();

        invocation.Intercept(testing);
    }

    [Test]
    public void Context_can_be_used_during_interception()
    {
        var testing = builder.Build(
            beforeCtx: ctx => ctx.Value += " - before",
            successCtx: ctx => ctx.Value += " - success",
            failCtx: ctx => ctx.Value += " - fail",
            afterCtx: ctx => ctx.Value += " - after"
        );

        invocation.Context.Value = "begin";

        invocation.Intercept(testing);

        Assert.AreEqual("begin - before - success - after", invocation.Context.Value);

        invocation.Context.Value = "begin";
        invocation.FailsWith(new Exception());

        Assert.Throws<Exception>(() => invocation.Intercept(testing));

        Assert.AreEqual("begin - before - fail - after", invocation.Context.Value);
    }

    [Test]
    public void Facade_Before()
    {
        var testing = builder.FacadeBefore(beforeCtx: ctx => ctx.Value = "before");

        invocation.Intercept(testing);

        Assert.AreEqual("before", invocation.Context.Value);
    }

    [Test]
    public void Facade_Success()
    {
        var testing = builder.FacadeSuccess(successCtx: ctx => ctx.Value = "success");

        invocation.Intercept(testing);

        Assert.AreEqual("success", invocation.Context.Value);
    }

    [Test]
    public void Facade_Fail()
    {
        var testing = builder.FacadeFail(failCtx: ctx => ctx.Value = "fail");

        invocation.FailsWith(new Exception());

        Assert.Throws<Exception>(() => invocation.Intercept(testing));

        Assert.AreEqual("fail", invocation.Context.Value);
    }

    [Test]
    public void Facade_After()
    {
        var testing = builder.FacadeAfter(afterCtx: ctx => ctx.Value = "after");

        invocation.Intercept(testing);

        Assert.AreEqual("after", invocation.Context.Value);
    }
}
