using Routine.Interception;
using System;

namespace Routine.Test.Interception.Stubs.DecoratorInterceptorBuilders
{
    public interface IBuilder
    {
        IInterceptor<TestContext> Build<TVariableType>(
            Func<TVariableType> before = null, Action<TVariableType> success = null,
            Action<TVariableType> fail = null, Action<TVariableType> after = null,
            Func<TestContext, TVariableType> beforeCtx = null, Action<TestContext, TVariableType> successCtx = null,
            Action<TestContext, TVariableType> failCtx = null, Action<TestContext, TVariableType> afterCtx = null);
    }
}