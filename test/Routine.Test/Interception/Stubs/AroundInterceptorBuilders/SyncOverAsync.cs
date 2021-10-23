using Routine.Interception;
using Routine.Interception.Configuration;
using System;

namespace Routine.Test.Interception.Stubs.AroundInterceptorBuilders
{
    public class SyncOverAsync : IBuilder
    {
        private static InterceptorBuilder<TestContext> Builder => BuildRoutine.Interceptor<TestContext>();

        public IInterceptor<TestContext> Build(
            Action before = null, Action success = null, Action fail = null, Action after = null,
            Action<TestContext> beforeCtx = null, Action<TestContext> successCtx = null,
            Action<TestContext> failCtx = null, Action<TestContext> afterCtx = null
        ) => Builder.DoAsync()
            .Before(before).Success(success).Fail(fail).After(after)
            .Before(beforeCtx).Success(successCtx).Fail(failCtx).After(afterCtx);

        public IInterceptor<TestContext> FacadeBefore(
            Action before = null, Action<TestContext> beforeCtx = null
        ) => before != null ? Builder.BeforeAsync(before) : Builder.BeforeAsync(beforeCtx);

        public IInterceptor<TestContext> FacadeSuccess(
            Action success = null, Action<TestContext> successCtx = null
        ) => success != null ? Builder.SuccessAsync(success) : Builder.SuccessAsync(successCtx);

        public IInterceptor<TestContext> FacadeFail(
            Action fail = null, Action<TestContext> failCtx = null
        ) => fail != null ? Builder.FailAsync(fail) : Builder.FailAsync(failCtx);

        public IInterceptor<TestContext> FacadeAfter(
            Action after = null, Action<TestContext> afterCtx = null
        ) => after != null ? Builder.AfterAsync(after) : Builder.AfterAsync(afterCtx);
    }
}