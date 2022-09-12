using Routine.Interception.Configuration;
using Routine.Interception;
using System;

namespace Routine.Test.Interception.Stubs.DecoratorInterceptorBuilders;

public class Sync : IBuilder
{
    private static InterceptorBuilder<Context> Builder => BuildRoutine.Interceptor<Context>();

    public IInterceptor<Context> Build<TVariableType>(
        Func<TVariableType> before = null, Action<TVariableType> success = null,
        Action<TVariableType> fail = null, Action<TVariableType> after = null,
        Func<Context, TVariableType> beforeCtx = null, Action<Context, TVariableType> successCtx = null,
        Action<Context, TVariableType> failCtx = null, Action<Context, TVariableType> afterCtx = null
    ) => (before != null ? Builder.ByDecorating(before) : Builder.ByDecorating(beforeCtx))
        .Success(success).Fail(fail).After(after)
        .Success(successCtx).Fail(failCtx).After(afterCtx);
}
