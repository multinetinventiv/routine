using Routine.Interception;
using Routine.Test.Core;
using Routine.Test.Interception.Stubs.Invocations;
using Routine.Test.Interception.Stubs;

using IInvocation = Routine.Test.Interception.Stubs.Invocations.IInvocation;

namespace Routine.Test.Interception;

[TestFixture(typeof(Sync))]
[TestFixture(typeof(Async))]
public class ChainInterceptorTest<TInvocation> : CoreTestBase
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
    public void First_added_interceptor_wraps_the_second_one_and_invocation_happens_last()
    {
        var interceptor = new ChainInterceptor<Context>();

        interceptor.Add(BuildRoutine.Interceptor<Context>().Do()
            .Before(ctx => ctx.Value += " - before1")
            .Success(ctx => ctx.Value += " - success1")
            .After(ctx => ctx.Value += " - after1"));
        interceptor.Add(BuildRoutine.Interceptor<Context>().Do()
            .Before(ctx => ctx.Value += " - before2")
            .Success(ctx => ctx.Value += " - success2")
            .After(ctx => ctx.Value += " - after2"));

        var testing = interceptor;

        _invocation.Context.Value = "begin";

        _invocation.Returns("actual");

        var actual = _invocation.Intercept(testing);

        Assert.That(actual, Is.EqualTo("actual"));

        Assert.That(_invocation.Context.Value, Is.EqualTo("begin - before1 - before2 - success2 - after2 - success1 - after1"));
        Assert.That(_invocation.Count, Is.EqualTo(1));
    }

    [Test]
    public void When_invocation_fails_all_interceptors_work_in_the_reverse_order__since_first_wraps_second()
    {
        var interceptor = new ChainInterceptor<Context>();

        interceptor.Add(BuildRoutine.Interceptor<Context>().Do()
            .Before(ctx => ctx.Value += " - before1")
            .Fail(ctx => ctx.Value += " - fail1")
            .After(ctx => ctx.Value += " - after1"));
        interceptor.Add(BuildRoutine.Interceptor<Context>().Do()
            .Before(ctx => ctx.Value += " - before2")
            .Fail(ctx => ctx.Value += " - fail2")
            .After(ctx => ctx.Value += " - after2"));

        var testing = interceptor;

        _invocation.Context.Value = "begin";

        _invocation.FailsWith(new Exception());

        Assert.That(() => _invocation.Intercept(testing), Throws.TypeOf<Exception>());

        Assert.That(_invocation.Context.Value, Is.EqualTo("begin - before1 - before2 - fail2 - after2 - fail1 - after1"));
        Assert.That(_invocation.Count, Is.EqualTo(1));
    }

    [Test]
    public void When_invocation_fails_and_second_interceptor_handles_the_exception__first_interceptor_does_not_know_about_the_exception()
    {
        var interceptor = new ChainInterceptor<Context>();

        interceptor.Add(BuildRoutine.Interceptor<Context>().Do()
            .Before(ctx => ctx.Value += " - before1")
            .Success(ctx => ctx.Value += " - success1")
            .After(ctx => ctx.Value += " - after1"));
        interceptor.Add(BuildRoutine.Interceptor<Context>().Do()
            .Before(ctx => ctx.Value += " - before2")
            .Fail(ctx =>
            {
                ctx.Value += " - fail2 (handled)";
                ctx.ExceptionHandled = true;
                ctx.Result = "result2";
            })
            .After(ctx => ctx.Value += " - after2"));

        var testing = interceptor;

        _invocation.Context.Value = "begin";

        _invocation.FailsWith(new Exception());

        var actual = _invocation.Intercept(testing);

        Assert.That(actual, Is.EqualTo("result2"));

        Assert.That(_invocation.Context.Value, Is.EqualTo("begin - before1 - before2 - fail2 (handled) - after2 - success1 - after1"));
        Assert.That(_invocation.Count, Is.EqualTo(1));
    }

    [Test]
    public void When_first_interceptor_fails_on_success__second_interceptor_does_not_know_about_the_exception()
    {
        var interceptor = new ChainInterceptor<Context>();

        interceptor.Add(BuildRoutine.Interceptor<Context>().Do()
            .Before(ctx => ctx.Value += " - before1")
            .Success(ctx =>
            {
                ctx.Value += " - success1";
                throw new Exception();
            })
            .Fail(ctx => ctx.Value += " - fail1")
            .After(ctx => ctx.Value += " - after1"));

        interceptor.Add(BuildRoutine.Interceptor<Context>().Do()
            .Before(ctx => ctx.Value += " - before2")
            .Success(ctx => ctx.Value += " - success2")
            .After(ctx => ctx.Value += " - after2"));

        var testing = interceptor;

        _invocation.Context.Value = "begin";

        _invocation.Returns("actual");

        Assert.That(() => _invocation.Intercept(testing), Throws.TypeOf<Exception>());

        Assert.That(_invocation.Context.Value, Is.EqualTo("begin - before1 - before2 - success2 - after2 - success1 - fail1 - after1"));
        Assert.That(_invocation.Count, Is.EqualTo(1));
    }

    [Test]
    public void Merging_two_chains_connects_given_chain_to_the_last_link_of_the_root_chain()
    {
        var interceptor1 = new ChainInterceptor<Context>();
        interceptor1.Add(BuildRoutine.Interceptor<Context>().Before(ctx => ctx.Value += " - before1"));
        interceptor1.Add(BuildRoutine.Interceptor<Context>().Before(ctx => ctx.Value += " - before2"));

        var interceptor2 = new ChainInterceptor<Context>();
        interceptor2.Add(BuildRoutine.Interceptor<Context>().Before(ctx => ctx.Value += " - before3"));
        interceptor2.Add(BuildRoutine.Interceptor<Context>().Before(ctx => ctx.Value += " - before4"));

        interceptor1.Merge(interceptor2);

        var testing = interceptor1;

        _invocation.Context.Value = "begin";
        _invocation.Intercept(testing);

        Assert.That(_invocation.Context.Value, Is.EqualTo("begin - before1 - before2 - before3 - before4"));
        Assert.That(_invocation.Count, Is.EqualTo(1));
    }

    [Test]
    public void After_merging_two_chains__new_interceptor_is_added_to_the_very_last_chain_link()
    {
        var interceptor1 = new ChainInterceptor<Context>();
        interceptor1.Add(BuildRoutine.Interceptor<Context>().Before(ctx => ctx.Value += " - before1"));
        interceptor1.Add(BuildRoutine.Interceptor<Context>().Before(ctx => ctx.Value += " - before2"));

        var interceptor2 = new ChainInterceptor<Context>();
        interceptor2.Add(BuildRoutine.Interceptor<Context>().Before(ctx => ctx.Value += " - before3"));
        interceptor2.Add(BuildRoutine.Interceptor<Context>().Before(ctx => ctx.Value += " - before4"));

        interceptor1.Merge(interceptor2);

        interceptor1.Add(BuildRoutine.Interceptor<Context>().Before(ctx => ctx.Value += " - before5"));

        var testing = interceptor1;

        _invocation.Context.Value = "begin";
        _invocation.Intercept(testing);

        Assert.That(_invocation.Context.Value, Is.EqualTo("begin - before1 - before2 - before3 - before4 - before5"));
        Assert.That(_invocation.Count, Is.EqualTo(1));
    }

    [Test]
    public void When_a_chain_is_merged_with_an_empty_chain_nothing_changes()
    {
        var interceptor1 = new ChainInterceptor<Context>();
        interceptor1.Add(BuildRoutine.Interceptor<Context>().Before(ctx => ctx.Value += " - before1"));
        interceptor1.Add(BuildRoutine.Interceptor<Context>().Before(ctx => ctx.Value += " - before2"));

        var interceptor2 = new ChainInterceptor<Context>();

        interceptor1.Merge(interceptor2);

        var testing = interceptor1;

        _invocation.Context.Value = "begin";
        _invocation.Intercept(testing);

        Assert.That(_invocation.Context.Value, Is.EqualTo("begin - before1 - before2"));
        Assert.That(_invocation.Count, Is.EqualTo(1));
    }

    [Test]
    public void When_an_empty_chain_is_merged_with_a_chain__given_chain_s_links_added_to_the_empty_one()
    {
        var interceptor1 = new ChainInterceptor<Context>();

        var interceptor2 = new ChainInterceptor<Context>();
        interceptor2.Add(BuildRoutine.Interceptor<Context>().Before(ctx => ctx.Value += " - before1"));
        interceptor2.Add(BuildRoutine.Interceptor<Context>().Before(ctx => ctx.Value += " - before2"));

        interceptor1.Merge(interceptor2);

        var testing = interceptor1;

        _invocation.Context.Value = "begin";
        _invocation.Intercept(testing);

        Assert.That(_invocation.Context.Value, Is.EqualTo("begin - before1 - before2"));
        Assert.That(_invocation.Count, Is.EqualTo(1));
    }

    [Test]
    public void When_an_empty_chain_intercepts_some_invocation__invocation_is_directly_called()
    {
        var testing = new ChainInterceptor<Context>();

        _invocation.Returns("actual");

        var actual = _invocation.Intercept(testing);

        Assert.That(actual, Is.EqualTo("actual"));
        Assert.That(_invocation.Count, Is.EqualTo(1));
    }
}
