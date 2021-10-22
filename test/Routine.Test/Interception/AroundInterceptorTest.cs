using System;
using NUnit.Framework;
using Routine.Interception;
using Routine.Test.Interception.Stubs;
using System.Threading.Tasks;

namespace Routine.Test.Interception
{
    [TestFixture]
    public class AroundInterceptorSyncTest : AroundInterceptorTest<object>
    {
        protected override object Intercept(IInterceptor<TestContext<string>> testing, TestContext<string> context, Func<object> invocation) => UseIntercept(testing, context, invocation);
        protected override object Convert(object result) => result;
    }

    [TestFixture]
    public class AroundInterceptorAsyncTest : AroundInterceptorTest<Task<object>>
    {
        protected override object Intercept(IInterceptor<TestContext<string>> testing, TestContext<string> context, Func<Task<object>> invocation) => UseInterceptAsync(testing, context, invocation);
        protected override Task<object> Convert(object result) => Task.FromResult(result);
    }

    public abstract class AroundInterceptorTest<TResult> : InterceptorTestBase<TResult>
    {
        [Test]
        public void Implement_async_overload()
        {
            Assert.Fail("not implemented");
        }

        [Test]
        public void Before_success_fail_actions_can_be_defined_by_delegates()
        {
            context = String();
            context.Value = "begin";

            var testing = BuildRoutine.Interceptor<TestContext<string>>().Do()
                .Before(() => context.Value += " - before")
                .Success(() => context.Value += " - success")
                .Fail(() => context.Value += " - fail")
                .After(() => context.Value += " - after");

            Intercept(testing, context, invocation);

            Assert.AreEqual("begin - before - success - after", context.Value);

            context.Value = "begin";
            InvocationFailsWith(new Exception());

            Assert.Throws<Exception>(() => Intercept(testing, context, invocation));

            Assert.AreEqual("begin - before - fail - after", context.Value);
        }

        [Test]
        public void Nothing_happens_when_nothing_is_defined()
        {
            var testing = BuildRoutine.Interceptor<TestContext<string>>().Do();
            context = String();

            Intercept(testing, context, invocation);
        }

        [Test]
        public void Context_can_be_used_during_interception()
        {
            var testing = BuildRoutine.Interceptor<TestContext<string>>().Do()
                .Before(ctx => ctx.Value += " - before")
                .Success(ctx => ctx.Value += " - success")
                .Fail(ctx => ctx.Value += " - fail")
                .After(ctx => ctx.Value += " - after");

            context = String();
            context.Value = "begin";

            Intercept(testing, context, invocation);

            Assert.AreEqual("begin - before - success - after", context.Value);

            context.Value = "begin";
            InvocationFailsWith(new Exception());

            Assert.Throws<Exception>(() => Intercept(testing, context, invocation));

            Assert.AreEqual("begin - before - fail - after", context.Value);
        }

        [Test]
        public void Facade_Before()
        {
            var testing = BuildRoutine.Interceptor<TestContext<string>>().Before(ctx => ctx.Value = "before");
            context = String();

            Intercept(testing, context, invocation);

            Assert.AreEqual("before", context.Value);
        }

        [Test]
        public void Facade_Success()
        {
            var testing = BuildRoutine.Interceptor<TestContext<string>>().Success(ctx => ctx.Value = "success");
            context = String();

            Intercept(testing, context, invocation);

            Assert.AreEqual("success", context.Value);
        }

        [Test]
        public void Facade_Fail()
        {
            var testing = BuildRoutine.Interceptor<TestContext<string>>().Fail(ctx => ctx.Value = "fail");
            context = String();

            InvocationFailsWith(new Exception());

            Assert.Throws<Exception>(() => Intercept(testing, context, invocation));

            Assert.AreEqual("fail", context.Value);
        }

        [Test]
        public void Facade_After()
        {
            var testing = BuildRoutine.Interceptor<TestContext<string>>().After(ctx => ctx.Value = "after");
            context = String();

            Intercept(testing, context, invocation);

            Assert.AreEqual("after", context.Value);
        }
    }
}
