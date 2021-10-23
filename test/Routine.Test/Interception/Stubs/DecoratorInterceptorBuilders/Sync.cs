using Routine.Interception;
using Routine.Interception.Configuration;
using System;

namespace Routine.Test.Interception.Stubs.DecoratorInterceptorBuilders
{
    public class Sync : IBuilder
    {
        private static InterceptorBuilder<TestContext> Builder => BuildRoutine.Interceptor<TestContext>();

        public IInterceptor<TestContext> Build<TVariableType>(
            Func<TVariableType> before = null, Action<TVariableType> success = null,
            Action<TVariableType> fail = null, Action<TVariableType> after = null,
            Func<TestContext, TVariableType> beforeCtx = null, Action<TestContext, TVariableType> successCtx = null,
            Action<TestContext, TVariableType> failCtx = null, Action<TestContext, TVariableType> afterCtx = null
        ) => (before != null ? Builder.ByDecorating(before) : Builder.ByDecorating(beforeCtx))
            .Success(success).Fail(fail).After(after)
            .Success(successCtx).Fail(failCtx).After(afterCtx);
    }
}