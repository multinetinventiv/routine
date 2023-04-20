using Routine.Test.Core;
using Routine.Test.Interception.Stubs.DecoratorInterceptorBuilders;

using AsyncBuilder = Routine.Test.Interception.Stubs.DecoratorInterceptorBuilders.Async;
using AsyncInvocation = Routine.Test.Interception.Stubs.Invocations.Async;
using IInvocation = Routine.Test.Interception.Stubs.Invocations.IInvocation;
using SyncInvocation = Routine.Test.Interception.Stubs.Invocations.Sync;
using SyncOverAsyncBuilder = Routine.Test.Interception.Stubs.DecoratorInterceptorBuilders.SyncOverAsync;

namespace Routine.Test.Interception;

[TestFixture(typeof(AsyncBuilder), typeof(SyncInvocation))]
[TestFixture(typeof(AsyncBuilder), typeof(AsyncInvocation))]
[TestFixture(typeof(SyncOverAsyncBuilder), typeof(SyncInvocation))]
[TestFixture(typeof(SyncOverAsyncBuilder), typeof(AsyncInvocation))]
public class DecoratorInterceptorTest<TBuilder, TInvocation> : CoreTestBase
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
    public void Decorates_an_invocation_with_a_variable_of_given_type_that_is_created_OnBefore()
    {
        var testing = _builder.Build(
            before: () => "test string",
            success: actual => Assert.That(actual, Is.EqualTo("test string")),
            fail: actual => Assert.That(actual, Is.EqualTo("test string")),
            after: actual => Assert.That(actual, Is.EqualTo("test string"))
        );

        _invocation.Intercept(testing);

        _invocation.FailsWith(new Exception());

        Assert.That(() => _invocation.Intercept(testing), Throws.TypeOf<Exception>());
    }

    [Test]
    public void When_multiple_instances_intercept_using_same_context__variables_does_not_conflict()
    {
        var testing = _builder.Build(
            before: () => "interceptor1",
            success: actual => Assert.That(actual, Is.EqualTo("interceptor1")),
            fail: actual => Assert.That(actual, Is.EqualTo("interceptor1")),
            after: actual => Assert.That(actual, Is.EqualTo("interceptor1"))
        );

        var testingOther = _builder.Build(
            before: () => 2,
            success: actual => Assert.That(actual, Is.EqualTo(2)),
            fail: actual => Assert.That(actual, Is.EqualTo(2)),
            after: actual => Assert.That(actual, Is.EqualTo(2))
        );

        _invocation.Intercept(testing);
        _invocation.Intercept(testingOther);
    }

    [Test]
    public void By_default_nothing_happens_OnSuccess__OnFail_and_OnAfter()
    {
        var testing = _builder.Build(before: () => "dummy");

        _invocation.Intercept(testing);

        _invocation.FailsWith(new Exception());

        Assert.That(() => _invocation.Intercept(testing), Throws.TypeOf<Exception>());
    }

    [Test]
    public void Context_can_be_used_during_interception()
    {
        var testing = _builder.Build(
            beforeCtx: ctx => ctx["value"] as string,
            successCtx: (ctx, actual) => Assert.That(actual, Is.SameAs(ctx["value"])),
            failCtx: (ctx, actual) => Assert.That(actual, Is.SameAs(ctx["value"])),
            afterCtx: (ctx, actual) => Assert.That(actual, Is.SameAs(ctx["value"]))
        );

        _invocation.Context["value"] = "dummy";

        _invocation.Intercept(testing);

        _invocation.FailsWith(new Exception());

        Assert.That(() => _invocation.Intercept(testing), Throws.TypeOf<Exception>());
    }

    [Test]
    public void When_variable_could_not_be_retrieved_during_before_delegate__fail_and_after_are_skipped()
    {
        var testing = _builder.Build(
            before: () => Throw<string>(new Exception()),
            success: _ => Assert.Fail("should not be called"),
            fail: _ => Assert.Fail("should be skipped"),
            after: _ => Assert.Fail("should be skipped")
        );

        _invocation.Context["value"] = "dummy";

        Assert.That(() => _invocation.Intercept(testing), Throws.TypeOf<Exception>());
    }
}
