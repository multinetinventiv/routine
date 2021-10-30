using Routine.Interception;
using Routine.Interception.Configuration;
using System;

namespace Routine.Test.Interception.Stubs.AroundInterceptorBuilders
{
    public class Sync : IBuilder
    {
        private static InterceptorBuilder<Context> Builder => BuildRoutine.Interceptor<Context>();

        public IInterceptor<Context> Build(
            Action before = null, Action success = null, Action fail = null, Action after = null,
            Action<Context> beforeCtx = null, Action<Context> successCtx = null,
            Action<Context> failCtx = null, Action<Context> afterCtx = null
        ) => Builder.Do()
            .Before(before).Success(success).Fail(fail).After(after)
            .Before(beforeCtx).Success(successCtx).Fail(failCtx).After(afterCtx);

        public IInterceptor<Context> FacadeBefore(
            Action before = null, Action<Context> beforeCtx = null
        ) => before != null ? Builder.Before(before) : Builder.Before(beforeCtx);

        public IInterceptor<Context> FacadeSuccess(
            Action success = null, Action<Context> successCtx = null
        ) => success != null ? Builder.Success(success) : Builder.Success(successCtx);

        public IInterceptor<Context> FacadeFail(
            Action fail = null, Action<Context> failCtx = null
        ) => fail != null ? Builder.Fail(fail) : Builder.Fail(failCtx);

        public IInterceptor<Context> FacadeAfter(
            Action after = null, Action<Context> afterCtx = null
        ) => after != null ? Builder.After(after) : Builder.After(afterCtx);
    }
}