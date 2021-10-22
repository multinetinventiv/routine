using Routine.Interception;
using System;

namespace Routine.Test.Interception.Stubs
{
    public interface ITestAroundInterceptor<out TConcrete> : IInterceptor<TestContext<string>>
        where TConcrete : ITestAroundInterceptor<TConcrete>
    {
        void FailOnBeforeWith(Exception exceptionBefore);
        void CancelAndReturn(object result);
        void OverrideActualResultWith(object resultOverride);
        void FailOnSuccessWith(Exception exceptionSuccess);
        void HideFailAndReturn(object resultOnFail);
        void OverrideExceptionWith(Exception exception);

        TConcrete When(Func<TestContext<string>, bool> whenDelegate);
        TConcrete WhenContextHas(string key);
    }
}