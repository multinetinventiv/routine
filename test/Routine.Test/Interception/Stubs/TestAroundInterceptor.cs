using Routine.Interception;
using System;

namespace Routine.Test.Interception.Stubs
{
    public class TestAroundInterceptor : AroundInterceptorBase<TestAroundInterceptor, TestContext<string>>, ITestAroundInterceptor<TestAroundInterceptor>
    {
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

        protected override void OnBefore(TestContext<string> context)
        {
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

        protected override void OnSuccess(TestContext<string> context)
        {
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

        protected override void OnFail(TestContext<string> context)
        {
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

        protected override void OnAfter(TestContext<string> context) => context["after"] = true;

        protected override bool CanIntercept(TestContext<string> context) =>
            key != null && context[key] != null || base.CanIntercept(context);

        private string key;
        public TestAroundInterceptor WhenContextHas(string key)
        {
            this.key = key;

            return this;
        }
    }
}