using Routine.Interception;
using System;

namespace Routine.Test.Interception.Stubs.Invocations
{
    public abstract class InvocationBase<TResult> : IInvocation
    {
        protected TestContext context;
        private int count;
        private object result;
        private Exception exception;
        protected Func<TResult> invocation;

        protected InvocationBase()
        {
            context = new TestContext("test");
            count = 0;
            result = null;
            exception = null;
            invocation = () =>
            {
                count++;

                if (exception != null) { throw exception; }

                context["invocation"] = true;

                return Convert(result);
            };
        }

        public TestContext Context => context;
        public int Count => count;
        public string ExceptionStackTraceLookupText => "InvocationBase`1.<.ctor>";

        public void Returns(object result) => this.result = result;
        public void FailsWith(Exception exception) => this.exception = exception;

        protected abstract object Intercept(IInterceptor<TestContext> testing);
        protected abstract TResult Convert(object result);

        object IInvocation.Intercept(IInterceptor<TestContext> testing) => Intercept(testing);
    }
}