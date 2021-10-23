using System;

namespace Routine.Test.Interception.Stubs.Interceptors
{
    public interface IInterceptor<out TConcrete> : Routine.Interception.IInterceptor<TestContext>
        where TConcrete : IInterceptor<TConcrete>
    {
        void FailOnBeforeWith(Exception exceptionBefore);
        void CancelAndReturn(object result);
        void OverrideActualResultWith(object resultOverride);
        void FailOnSuccessWith(Exception exceptionSuccess);
        void HideFailAndReturn(object resultOnFail);
        void OverrideExceptionWith(Exception exception);

        TConcrete When(Func<TestContext, bool> whenDelegate);
        TConcrete WhenContextHas(string key);
    }
}