using NUnit.Framework;
using Routine.Test.Core;
using Routine.Test.Interception.Stubs.DecoratorInterceptorBuilders;
using Routine.Test.Interception.Stubs.Invocations;
using System;
using AsyncBuilder = Routine.Test.Interception.Stubs.DecoratorInterceptorBuilders.Async;
using AsyncInvocation = Routine.Test.Interception.Stubs.Invocations.Async;
using SyncBuilder = Routine.Test.Interception.Stubs.DecoratorInterceptorBuilders.Sync;
using SyncInvocation = Routine.Test.Interception.Stubs.Invocations.Sync;
using SyncOverAsyncBuilder = Routine.Test.Interception.Stubs.DecoratorInterceptorBuilders.SyncOverAsync;

namespace Routine.Test.Interception;

[TestFixture(typeof(SyncBuilder), typeof(SyncInvocation))]
[TestFixture(typeof(SyncBuilder), typeof(AsyncInvocation))]
[TestFixture(typeof(AsyncBuilder), typeof(SyncInvocation))]
[TestFixture(typeof(AsyncBuilder), typeof(AsyncInvocation))]
[TestFixture(typeof(SyncOverAsyncBuilder), typeof(SyncInvocation))]
[TestFixture(typeof(SyncOverAsyncBuilder), typeof(AsyncInvocation))]
public class DecoratorInterceptorTest<TBuilder, TInvocation> : CoreTestBase
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
    public void Decorates_an_invocation_with_a_variable_of_given_type_that_is_created_OnBefore()
    {
        var testing = builder.Build(
            before: () => "test string",
            success: actual => Assert.AreEqual("test string", actual),
            fail: actual => Assert.AreEqual("test string", actual),
            after: actual => Assert.AreEqual("test string", actual)
        );

        invocation.Intercept(testing);

        invocation.FailsWith(new Exception());

        Assert.Throws<Exception>(() => invocation.Intercept(testing));
    }

    [Test]
    public void When_multiple_instances_intercept_using_same_context__variables_does_not_conflict()
    {
        var testing = builder.Build(
            before: () => "interceptor1",
            success: actual => Assert.AreEqual("interceptor1", actual),
            fail: actual => Assert.AreEqual("interceptor1", actual),
            after: actual => Assert.AreEqual("interceptor1", actual)
        );

        var testingOther = builder.Build(
            before: () => 2,
            success: actual => Assert.AreEqual(2, actual),
            fail: actual => Assert.AreEqual(2, actual),
            after: actual => Assert.AreEqual(2, actual)
        );

        invocation.Intercept(testing);
        invocation.Intercept(testingOther);
    }

    [Test]
    public void By_default_nothing_happens_OnSuccess__OnFail_and_OnAfter()
    {
        var testing = builder.Build(before: () => "dummy");

        invocation.Intercept(testing);

        invocation.FailsWith(new Exception());

        Assert.Throws<Exception>(() => invocation.Intercept(testing));
    }

    [Test]
    public void Context_can_be_used_during_interception()
    {
        var testing = builder.Build(
            beforeCtx: ctx => ctx["value"] as string,
            successCtx: (ctx, actual) => Assert.AreSame(ctx["value"], actual),
            failCtx: (ctx, actual) => Assert.AreSame(ctx["value"], actual),
            afterCtx: (ctx, actual) => Assert.AreSame(ctx["value"], actual)
        );

        invocation.Context["value"] = "dummy";

        invocation.Intercept(testing);

        invocation.FailsWith(new Exception());

        Assert.Throws<Exception>(() => invocation.Intercept(testing));
    }

    [Test]
    public void When_variable_could_not_be_retrieved_during_before_delegate__fail_and_after_are_skipped()
    {
        var testing = builder.Build(
            before: () => Throw<string>(new Exception()),
            success: _ => Assert.Fail("should not be called"),
            fail: _ => Assert.Fail("should be skipped"),
            after: _ => Assert.Fail("should be skipped")
        );

        invocation.Context["value"] = "dummy";

        Assert.Throws<Exception>(() => invocation.Intercept(testing));
    }
}
