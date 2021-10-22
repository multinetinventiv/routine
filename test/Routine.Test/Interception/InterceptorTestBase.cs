using System;
using NUnit.Framework;
using Routine.Core.Runtime;
using Routine.Interception;
using Routine.Test.Core;
using Routine.Test.Interception.Stubs;
using System.Threading.Tasks;

namespace Routine.Test.Interception
{
    public abstract class InterceptorTestBase<TResult> : CoreTestBase
    {
        protected class TestConfiguration { }

        protected TestContext<string> String() => String(null);
        protected TestContext<string> String(string value) => Ctx(value);
        protected TestContext<T> Ctx<T>() => Ctx(default(T));
        protected TestContext<T> Ctx<T>(T value) => new("test") { Value = value };
        protected TestConfiguration DummyConfiguration() => new();

        protected TestContext<string> context;
        protected Func<TResult> invocation;

        private int invocationCount;

        private object result;
        protected void InvocationReturns(object result) => this.result = result;

        private Exception exception;
        protected void InvocationFailsWith(Exception exception) => this.exception = exception;
        protected string ExceptionStackTraceLookupText => "InterceptorTestBase`1.<SetUp>";

        protected T Throw<T>(Exception ex) => throw ex;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            context = String();
            invocationCount = 0;
            result = null;
            exception = null;
            invocation = () =>
            {
                invocationCount++;

                if (exception != null) { throw exception; }

                context["invocation"] = true;

                return Convert(result);
            };
        }

        protected void AssertInvocationWasCalledOnlyOnce()
        {
            Assert.AreEqual(1, invocationCount);
        }

        protected abstract object Intercept(IInterceptor<TestContext<string>> testing, TestContext<string> context, Func<TResult> invocation);

        protected object UseIntercept(IInterceptor<TestContext<string>> testing, TestContext<string> context, Func<object> invocation) => testing.Intercept(context, invocation);
        protected object UseInterceptAsync(IInterceptor<TestContext<string>> testing, TestContext<string> context, Func<Task<object>> invocation) => testing.InterceptAsync(context, invocation).WaitAndGetResult();

        protected abstract TResult Convert(object result);
    }
}