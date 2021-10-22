using Routine.Interception;
using System;
using System.Threading.Tasks;

namespace Routine.Test.Interception.Stubs
{
    public class TestAsyncAroundInterceptor : AsyncAroundInterceptorBase<TestAsyncAroundInterceptor, TestContext<string>>, ITestAroundInterceptor<TestAsyncAroundInterceptor>
    {
        private readonly int delay;

        public TestAsyncAroundInterceptor(int delay)
        {
            this.delay = delay;
        }

        private Exception exceptionBefore;
        public void FailOnBeforeWith(Exception exceptionBefore) => this.exceptionBefore = exceptionBefore;

        private object result;
        public void CancelAndReturn(object result) => this.result = result;

        private object resultOverride;
        public void OverrideActualResultWith(object resultOverride) => this.resultOverride = resultOverride;

        private Exception exceptionSuccess;
        public void FailOnSuccessWith(Exception exceptionSuccess) => this.exceptionSuccess = exceptionSuccess;

        private object resultOnFail;
        public void HideFailAndReturn(object resultOnFail) => this.resultOnFail = resultOnFail;

        private Exception exception;
        public void OverrideExceptionWith(Exception exception) => this.exception = exception;

        protected override async Task OnBefore(TestContext<string> context)
        {
            await Task.Delay(delay);

            if (exceptionBefore != null)
            {
                throw exceptionBefore;
            }

            context["before"] = true;

            if (result != null)
            {
                context.Canceled = true;
                context.Result = result;
            }
        }

        protected override async Task OnSuccess(TestContext<string> context)
        {
            await Task.Delay(delay);

            if (exceptionSuccess != null)
            {
                throw exceptionSuccess;
            }

            context["success"] = true;

            if (resultOverride != null)
            {
                context.Result = resultOverride;
            }
        }

        protected override async Task OnFail(TestContext<string> context)
        {
            await Task.Delay(delay);

            context["fail"] = true;

            if (resultOnFail != null)
            {
                context.ExceptionHandled = true;
                context.Result = resultOnFail;
            }

            if (exception != null)
            {
                context.Exception = exception;
            }
        }

        protected override async Task OnAfter(TestContext<string> context)
        {
            await Task.Delay(delay);

            context["after"] = true;
        }

        protected override bool CanIntercept(TestContext<string> context) =>
            key != null && context[key] != null || base.CanIntercept(context);

        private string key;
        public TestAsyncAroundInterceptor WhenContextHas(string key)
        {
            this.key = key;

            return this;
        }
    }
}