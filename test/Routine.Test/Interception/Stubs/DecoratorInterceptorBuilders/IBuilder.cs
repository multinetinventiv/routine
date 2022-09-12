using Routine.Interception;
using System;

namespace Routine.Test.Interception.Stubs.DecoratorInterceptorBuilders;

public interface IBuilder
{
    IInterceptor<Context> Build<TVariableType>(
        Func<TVariableType> before = null, Action<TVariableType> success = null,
        Action<TVariableType> fail = null, Action<TVariableType> after = null,
        Func<Context, TVariableType> beforeCtx = null, Action<Context, TVariableType> successCtx = null,
        Action<Context, TVariableType> failCtx = null, Action<Context, TVariableType> afterCtx = null);
}
