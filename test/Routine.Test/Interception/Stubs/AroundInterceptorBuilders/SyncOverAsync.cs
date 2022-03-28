using Routine.Interception.Configuration;
using Routine.Interception;
using System;

namespace Routine.Test.Interception.Stubs.AroundInterceptorBuilders
{
    public class SyncOverAsync : IBuilder
    {
        private static InterceptorBuilder<Context> Builder => BuildRoutine.Interceptor<Context>();

        public IInterceptor<Context> Build(
            Action before = null, Action success = null, Action fail = null, Action after = null,
            Action<Context> beforeCtx = null, Action<Context> successCtx = null,
            Action<Context> failCtx = null, Action<Context> afterCtx = null
        ) => Builder.DoAsync()
            .Before(before).Success(success).Fail(fail).After(after)
            .Before(beforeCtx).Success(successCtx).Fail(failCtx).After(afterCtx);

        public IInterceptor<Context> FacadeBefore(
            Action before = null, Action<Context> beforeCtx = null
        ) => before != null ? Builder.BeforeAsync(before) : Builder.BeforeAsync(beforeCtx);

        public IInterceptor<Context> FacadeSuccess(
            Action success = null, Action<Context> successCtx = null
        ) => success != null ? Builder.SuccessAsync(success) : Builder.SuccessAsync(successCtx);

        public IInterceptor<Context> FacadeFail(
            Action fail = null, Action<Context> failCtx = null
        ) => fail != null ? Builder.FailAsync(fail) : Builder.FailAsync(failCtx);

        public IInterceptor<Context> FacadeAfter(
            Action after = null, Action<Context> afterCtx = null
        ) => after != null ? Builder.AfterAsync(after) : Builder.AfterAsync(afterCtx);
    }
}
