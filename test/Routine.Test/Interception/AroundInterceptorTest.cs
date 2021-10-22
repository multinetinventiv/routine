using System;
using NUnit.Framework;
using Routine.Interception;
using Routine.Interception.Configuration;
using Routine.Test.Interception.Stubs;
using System.Threading.Tasks;

namespace Routine.Test.Interception
{
    [TestFixture]
    public class AroundInterceptorSyncTest : AroundInterceptorTest<object>.Base
    {
        protected override object Intercept(IInterceptor<TestContext<string>> testing, TestContext<string> context, Func<object> invocation) => UseIntercept(testing, context, invocation);
        protected override object Convert(object result) => result;
    }

    [TestFixture]
    public class AroundInterceptorAsyncTest : AroundInterceptorTest<Task<object>>.Base
    {
        protected override object Intercept(IInterceptor<TestContext<string>> testing, TestContext<string> context, Func<Task<object>> invocation) => UseInterceptAsync(testing, context, invocation);
        protected override Task<object> Convert(object result) => Task.FromResult(result);
    }

    [TestFixture]
    public class AsyncAroundInterceptorSyncTest : AroundInterceptorTest<object>.AsyncBase
    {
        protected override object Intercept(IInterceptor<TestContext<string>> testing, TestContext<string> context, Func<object> invocation) => UseIntercept(testing, context, invocation);
        protected override object Convert(object result) => result;

        [Test]
        public void Test_async_overloads()
        {
            Assert.Fail("not tested");
        }
    }

    [TestFixture]
    public class AsyncAroundInterceptorAsyncTest : AroundInterceptorTest<Task<object>>.AsyncBase
    {
        protected override object Intercept(IInterceptor<TestContext<string>> testing, TestContext<string> context, Func<Task<object>> invocation) => UseInterceptAsync(testing, context, invocation);
        protected override Task<object> Convert(object result) => Task.FromResult(result);

        [Test]
        public void Test_async_overloads()
        {
            Assert.Fail("not tested");
        }
    }

    public abstract class AroundInterceptorTest<TResult> : InterceptorTestBase<TResult>
    {
        #region Abstraction

        protected abstract IInterceptor<TestContext<string>> Build(
            Action before = null, Action success = null, Action fail = null, Action after = null,
            Action<TestContext<string>> beforeCtx = null, Action<TestContext<string>> successCtx = null, Action<TestContext<string>> failCtx = null, Action<TestContext<string>> afterCtx = null
        );
        protected abstract IInterceptor<TestContext<string>> FacadeBefore(Action before = null, Action<TestContext<string>> beforeCtx = null);
        protected abstract IInterceptor<TestContext<string>> FacadeSuccess(Action success = null, Action<TestContext<string>> successCtx = null);
        protected abstract IInterceptor<TestContext<string>> FacadeFail(Action fail = null, Action<TestContext<string>> failCtx = null);
        protected abstract IInterceptor<TestContext<string>> FacadeAfter(Action after = null, Action<TestContext<string>> afterCtx = null);

        public abstract class Base : AroundInterceptorTest<TResult>
        {
            protected static InterceptorBuilder<TestContext<string>> Builder => BuildRoutine.Interceptor<TestContext<string>>();

            protected override IInterceptor<TestContext<string>> Build(
                Action before = null, Action success = null, Action fail = null, Action after = null,
                Action<TestContext<string>> beforeCtx = null, Action<TestContext<string>> successCtx = null,
                Action<TestContext<string>> failCtx = null, Action<TestContext<string>> afterCtx = null
            ) => Builder.Do()
                .Before(before).Success(success).Fail(fail).After(after)
                .Before(beforeCtx).Success(successCtx).Fail(failCtx).After(afterCtx);

            protected override IInterceptor<TestContext<string>> FacadeBefore(
                Action before = null, Action<TestContext<string>> beforeCtx = null
            ) => before != null ? Builder.Before(before) : Builder.Before(beforeCtx);

            protected override IInterceptor<TestContext<string>> FacadeSuccess(
                Action success = null, Action<TestContext<string>> successCtx = null
            ) => success != null ? Builder.Success(success) : Builder.Success(successCtx);

            protected override IInterceptor<TestContext<string>> FacadeFail(
                Action fail = null, Action<TestContext<string>> failCtx = null
            ) => fail != null ? Builder.Fail(fail) : Builder.Fail(failCtx);

            protected override IInterceptor<TestContext<string>> FacadeAfter(
                Action after = null, Action<TestContext<string>> afterCtx = null
            ) => after != null ? Builder.After(after) : Builder.After(afterCtx);
        }

        public abstract class AsyncBase : AroundInterceptorTest<TResult>
        {
            protected static InterceptorBuilder<TestContext<string>> Builder => BuildRoutine.Interceptor<TestContext<string>>();

            protected override IInterceptor<TestContext<string>> Build(
                Action before = null, Action success = null, Action fail = null, Action after = null,
                Action<TestContext<string>> beforeCtx = null, Action<TestContext<string>> successCtx = null,
                Action<TestContext<string>> failCtx = null, Action<TestContext<string>> afterCtx = null
            ) => Builder.DoAsync()
                .Before(before).Success(success).Fail(fail).After(after)
                .Before(beforeCtx).Success(successCtx).Fail(failCtx).After(afterCtx);

            protected override IInterceptor<TestContext<string>> FacadeBefore(
                Action before = null, Action<TestContext<string>> beforeCtx = null
            ) => before != null ? Builder.BeforeAsync(before) : Builder.BeforeAsync(beforeCtx);

            protected override IInterceptor<TestContext<string>> FacadeSuccess(
                Action success = null, Action<TestContext<string>> successCtx = null
            ) => success != null ? Builder.SuccessAsync(success) : Builder.SuccessAsync(successCtx);

            protected override IInterceptor<TestContext<string>> FacadeFail(
                Action fail = null, Action<TestContext<string>> failCtx = null
            ) => fail != null ? Builder.FailAsync(fail) : Builder.FailAsync(failCtx);

            protected override IInterceptor<TestContext<string>> FacadeAfter(
                Action after = null, Action<TestContext<string>> afterCtx = null
            ) => after != null ? Builder.AfterAsync(after) : Builder.AfterAsync(afterCtx);
        }

        #endregion

        [Test]
        public void Before_success_fail_actions_can_be_defined_by_delegates()
        {
            context = String();
            context.Value = "begin";

            var testing = Build(
                before: () => context.Value += " - before",
                success: () => context.Value += " - success",
                fail: () => context.Value += " - fail",
                after: () => context.Value += " - after"
            );

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
            var testing = Build();
            context = String();

            Intercept(testing, context, invocation);
        }

        [Test]
        public void Context_can_be_used_during_interception()
        {
            var testing = Build(
                beforeCtx: ctx => ctx.Value += " - before",
                successCtx: ctx => ctx.Value += " - success",
                failCtx: ctx => ctx.Value += " - fail",
                afterCtx: ctx => ctx.Value += " - after"
            );

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
            var testing = FacadeBefore(beforeCtx: ctx => ctx.Value = "before");
            context = String();

            Intercept(testing, context, invocation);

            Assert.AreEqual("before", context.Value);
        }

        [Test]
        public void Facade_Success()
        {
            var testing = FacadeSuccess(successCtx: ctx => ctx.Value = "success");
            context = String();

            Intercept(testing, context, invocation);

            Assert.AreEqual("success", context.Value);
        }

        [Test]
        public void Facade_Fail()
        {
            var testing = FacadeFail(failCtx: ctx => ctx.Value = "fail");
            context = String();

            InvocationFailsWith(new Exception());

            Assert.Throws<Exception>(() => Intercept(testing, context, invocation));

            Assert.AreEqual("fail", context.Value);
        }

        [Test]
        public void Facade_After()
        {
            var testing = FacadeAfter(afterCtx: ctx => ctx.Value = "after");
            context = String();

            Intercept(testing, context, invocation);

            Assert.AreEqual("after", context.Value);
        }
    }
}
