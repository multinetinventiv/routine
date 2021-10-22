using System;
using NUnit.Framework;
using Routine.Interception;
using Routine.Test.Interception.Stubs;
using System.Threading.Tasks;

namespace Routine.Test.Interception
{
    [TestFixture]
    public class AroundInterceptorBaseSyncTest : AroundInterceptorBaseTest<TestAroundInterceptor, object>
    {
        protected override object Intercept(IInterceptor<TestContext<string>> testing, TestContext<string> context, Func<object> invocation) => UseIntercept(testing, context, invocation); 
        protected override object Convert(object result) => result;
        protected override ITestAroundInterceptor<TestAroundInterceptor> NewInterceptor() => new TestAroundInterceptor();
    }

    [TestFixture]
    public class AroundInterceptorBaseAsyncTest : AroundInterceptorBaseTest<TestAroundInterceptor, Task<object>>
    {
        protected override object Intercept(IInterceptor<TestContext<string>> testing, TestContext<string> context, Func<Task<object>> invocation) => UseInterceptAsync(testing, context, invocation);
        protected override Task<object> Convert(object result) => Task.FromResult(result);
        protected override ITestAroundInterceptor<TestAroundInterceptor> NewInterceptor() => new TestAroundInterceptor();
    }

    [TestFixture]
    public class AsyncAroundInterceptorBaseSyncTest : AroundInterceptorBaseTest<TestAsyncAroundInterceptor, object>
    {
        protected override object Intercept(IInterceptor<TestContext<string>> testing, TestContext<string> context, Func<object> invocation) => UseIntercept(testing, context, invocation); 
        protected override object Convert(object result) => result;
        protected override ITestAroundInterceptor<TestAsyncAroundInterceptor> NewInterceptor() => new TestAsyncAroundInterceptor(1);
    }

    [TestFixture]
    public class AsyncAroundInterceptorBaseAsyncTest : AroundInterceptorBaseTest<TestAsyncAroundInterceptor, Task<object>>
    {
        protected override object Intercept(IInterceptor<TestContext<string>> testing, TestContext<string> context, Func<Task<object>> invocation) => UseInterceptAsync(testing, context, invocation);
        protected override Task<object> Convert(object result) => Task.FromResult(result);
        protected override ITestAroundInterceptor<TestAsyncAroundInterceptor> NewInterceptor() => new TestAsyncAroundInterceptor(1);
    }

    public abstract class AroundInterceptorBaseTest<TInterceptor, TResult> : InterceptorTestBase<TResult> 
        where TInterceptor : ITestAroundInterceptor<TInterceptor>
    {
        protected abstract ITestAroundInterceptor<TInterceptor> NewInterceptor();

        [Test]
        public void A_successful_invocation_calls_OnBefore__OnSuccess_and_OnAfter_respectively()
        {
            var testing = NewInterceptor();

            InvocationReturns("result");

            var actual = Intercept(testing, context, invocation);

            Assert.AreEqual("result", actual);

            Assert.IsTrue((bool)context["before"]);
            Assert.IsTrue((bool)context["invocation"]);
            Assert.IsTrue((bool)context["success"]);
            Assert.IsNull(context["fail"]);
            Assert.IsTrue((bool)context["after"]);
            AssertInvocationWasCalledOnlyOnce();
        }

        [Test]
        public void An_unsuccessful_invocation_calls_OnBefore__OnFail_and_OnAfter_respectively()
        {
            var testing = NewInterceptor();

            InvocationFailsWith(new Exception());

            Assert.Throws<Exception>(() => Intercept(testing, context, invocation));

            Assert.IsTrue((bool)context["before"]);
            Assert.IsNull(context["invocation"]);
            Assert.IsNull(context["success"]);
            Assert.IsTrue((bool)context["fail"]);
            Assert.IsTrue((bool)context["after"]);
        }

        [Test]
        public void Can_cancel_actual_invocation_and_return_some_other_result()
        {
            var testing = NewInterceptor();

            InvocationReturns("actual");
            testing.CancelAndReturn("cancel");

            var actual = Intercept(testing, context, invocation);

            Assert.AreEqual("cancel", actual);

            Assert.IsTrue((bool)context["before"]);
            Assert.IsNull(context["invocation"]);
            Assert.IsTrue((bool)context["success"]);
            Assert.IsNull(context["fail"]);
            Assert.IsTrue((bool)context["after"]);
        }

        [Test]
        public void Can_alter_actual_invocation_result_after_a_successful_invocation()
        {
            var testing = NewInterceptor();

            InvocationReturns("actual");
            testing.OverrideActualResultWith("override");

            Intercept(testing, context, invocation);

            Assert.IsTrue((bool)context["before"]);
            Assert.IsTrue((bool)context["invocation"]);
            Assert.IsTrue((bool)context["success"]);
            Assert.IsNull(context["fail"]);
            Assert.IsTrue((bool)context["after"]);
        }

        [Test]
        public void Can_hide_the_exception_and_return_a_result()
        {
            var testing = NewInterceptor();

            InvocationFailsWith(new Exception());
            testing.HideFailAndReturn("override");

            var actual = Intercept(testing, context, invocation);

            Assert.AreEqual("override", actual);

            Assert.IsTrue((bool)context["before"]);
            Assert.IsNull(context["invocation"]);
            Assert.IsNull(context["success"]);
            Assert.IsTrue((bool)context["fail"]);
            Assert.IsTrue((bool)context["after"]);
        }

        [Test]
        public void Can_alter_the_exception_thrown_by_invocation()
        {
            var testing = NewInterceptor();

            InvocationFailsWith(new ArgumentNullException());
            testing.OverrideExceptionWith(new FormatException());

            Assert.Throws<FormatException>(() => Intercept(testing, context, invocation));

            Assert.IsTrue((bool)context["before"]);
            Assert.IsNull(context["invocation"]);
            Assert.IsNull(context["success"]);
            Assert.IsTrue((bool)context["fail"]);
            Assert.IsTrue((bool)context["after"]);
        }

        [Test]
        public void When_exception_is_not_changed__preserves_stack_trace()
        {
            var testing = NewInterceptor();

            InvocationFailsWith(new ArgumentNullException());

            try
            {
                Intercept(testing, context, invocation);
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.StackTrace);
                Assert.IsTrue(ex.StackTrace?.Contains(ExceptionStackTraceLookupText), ex.StackTrace);
            }
        }

        [Test]
        public void When_an_exception_is_thrown_OnBefore__OnFail_is_still_called()
        {
            var testing = NewInterceptor();

            testing.FailOnBeforeWith(new Exception());

            Assert.Throws<Exception>(() => Intercept(testing, context, invocation));

            Assert.IsNull(context["before"]);
            Assert.IsNull(context["invocation"]);
            Assert.IsNull(context["success"]);
            Assert.IsTrue((bool)context["fail"]);
            Assert.IsTrue((bool)context["after"]);
        }

        [Test]
        public void When_an_exception_is_thrown_OnSuccess__OnFail_is_still_called()
        {
            var testing = NewInterceptor();

            testing.FailOnSuccessWith(new Exception());

            Assert.Throws<Exception>(() => Intercept(testing, context, invocation));

            Assert.IsTrue((bool)context["before"]);
            Assert.IsTrue((bool)context["invocation"]);
            Assert.IsNull(context["success"]);
            Assert.IsTrue((bool)context["fail"]);
            Assert.IsTrue((bool)context["after"]);
        }

        [Test]
        public void By_default_intercepts_any_given_invocation()
        {
            var testing = NewInterceptor();

            Intercept(testing, context, invocation);

            Assert.IsTrue((bool)context["before"]);
            Assert.IsTrue((bool)context["invocation"]);
            Assert.IsTrue((bool)context["success"]);
            Assert.IsNull(context["fail"]);
            Assert.IsTrue((bool)context["after"]);
        }

        [Test]
        public void Intercepts_only_when_clause_returns_true()
        {
            var testing = NewInterceptor();

            testing.When(_ => false);

            Intercept(testing, context, invocation);

            Assert.IsNull(context["before"]);
            Assert.IsTrue((bool)context["invocation"]);
            Assert.IsNull(context["success"]);
            Assert.IsNull(context["fail"]);
            Assert.IsNull(context["after"]);

            testing.When(_ => true);

            context["invocation"] = null;

            Intercept(testing, context, invocation);

            Assert.IsTrue((bool)context["before"]);
            Assert.IsTrue((bool)context["invocation"]);
            Assert.IsTrue((bool)context["success"]);
            Assert.IsNull(context["fail"]);
            Assert.IsTrue((bool)context["after"]);
        }

        [Test]
        public void Custom_when_clauses_can_be_defined_by_sub_classes()
        {
            var testing = NewInterceptor();

            context["override-base"] = true;

            testing.When(_ => false).WhenContextHas("override-base");

            Intercept(testing, context, invocation);

            Assert.IsTrue((bool)context["before"]);
            Assert.IsTrue((bool)context["invocation"]);
            Assert.IsTrue((bool)context["success"]);
            Assert.IsNull(context["fail"]);
            Assert.IsTrue((bool)context["after"]);
        }
    }
}
