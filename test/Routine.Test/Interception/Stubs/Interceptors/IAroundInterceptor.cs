using System;

namespace Routine.Test.Interception.Stubs.Interceptors;

public interface IAroundInterceptor<out TConcrete> : Routine.Interception.IInterceptor<Context>
    where TConcrete : IAroundInterceptor<TConcrete>
{
    void FailOnBeforeWith(Exception exceptionBefore);
    void CancelAndReturn(object result);
    void OverrideActualResultWith(object resultOverride);
    void FailOnSuccessWith(Exception exceptionSuccess);
    void HideFailAndReturn(object resultOnFail);
    void OverrideExceptionWith(Exception exception);

    TConcrete When(Func<Context, bool> whenDelegate);
    TConcrete WhenContextHas(string key);
}
