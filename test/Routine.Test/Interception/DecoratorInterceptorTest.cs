using System;
using NUnit.Framework;
using Routine.Interception;
using System.Threading.Tasks;

namespace Routine.Test.Interception
{
    [TestFixture]
    public class DecoratorInterceptorSyncTest : DecoratorInterceptorTest<object>
    {
        protected override object Intercept(IInterceptor<TestContext<string>> testing, TestContext<string> context, Func<object> invocation) => UseIntercept(testing, context, invocation);
        protected override object Convert(object result) => result;
    }

    [TestFixture]
    public class DecoratorInterceptorAsyncTest : DecoratorInterceptorTest<Task<object>>
    {
        protected override object Intercept(IInterceptor<TestContext<string>> testing, TestContext<string> context, Func<Task<object>> invocation) => UseInterceptAsync(testing, context, invocation);
        protected override Task<object> Convert(object result) => Task.FromResult(result);
    }

    public abstract class DecoratorInterceptorTest<TResult> : InterceptorTestBase<TResult>
    {
        [Test]
        public void Decorates_an_invocation_with_a_variable_of_given_type_that_is_created_OnBefore()
        {
            var testing = BuildRoutine.Interceptor<TestContext<string>>()
                .ByDecorating(() => "test string")
                .Success(actual => Assert.AreEqual("test string", actual))
                .Fail(actual => Assert.AreEqual("test string", actual))
                .After(actual => Assert.AreEqual("test string", actual));

            context = String();

            Intercept(testing, context, invocation);

            InvocationFailsWith(new Exception());

            Assert.Throws<Exception>(() => Intercept(testing, context, invocation));
        }

        [Test]
        public void When_multiple_instances_intercept_using_same_context__variables_does_not_conflict()
        {
            var testing = BuildRoutine.Interceptor<TestContext<string>>()
                .ByDecorating(() => "interceptor1")
                .Success(actual => Assert.AreEqual("interceptor1", actual))
                .Fail(actual => Assert.AreEqual("interceptor1", actual))
                .After(actual => Assert.AreEqual("interceptor1", actual));

            var testingOther = BuildRoutine.Interceptor<TestContext<string>>()
                .ByDecorating(() => 2)
                .Success(actual => Assert.AreEqual(2, actual))
                .Fail(actual => Assert.AreEqual(2, actual))
                .After(actual => Assert.AreEqual(2, actual));

            context = String();

            Intercept(testing, context, invocation);
            Intercept(testingOther, context, invocation);
        }

        [Test]
        public void By_default_nothing_happens_OnSuccess__OnFail_and_OnAfter()
        {
            var testing = BuildRoutine.Interceptor<TestContext<string>>()
                .ByDecorating(() => "dummy");

            context = String();

            Intercept(testing, context, invocation);

            InvocationFailsWith(new Exception());

            Assert.Throws<Exception>(() => Intercept(testing, context, invocation));
        }

        [Test]
        public void Context_can_be_used_during_interception()
        {
            var testing = BuildRoutine.Interceptor<TestContext<string>>()
                .ByDecorating(ctx => ctx["value"] as string)
                .Success((ctx, actual) => Assert.AreSame(ctx["value"], actual))
                .Fail((ctx, actual) => Assert.AreSame(ctx["value"], actual))
                .After((ctx, actual) => Assert.AreSame(ctx["value"], actual));

            context = String();
            context["value"] = "dummy";

            Intercept(testing, context, invocation);

            InvocationFailsWith(new Exception());

            Assert.Throws<Exception>(() => Intercept(testing, context, invocation));
        }

        [Test]
        public void When_variable_could_not_be_retrieved_during_before_delegate__fail_and_after_are_skipped()
        {
            var testing = BuildRoutine.Interceptor<TestContext<string>>()
                .ByDecorating(() => Throw<string>(new Exception()))
                .Success(_ => Assert.Fail("should not be called"))
                .Fail(_ => Assert.Fail("should be skipped"))
                .After(_ => Assert.Fail("should be skipped"));

            context = String();
            context["value"] = "dummy";

            Assert.Throws<Exception>(() => Intercept(testing, context, invocation));
        }
    }
}
