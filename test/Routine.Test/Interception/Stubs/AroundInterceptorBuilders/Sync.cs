using Routine.Interception;
using Routine.Interception.Configuration;
using System;

namespace Routine.Test.Interception.Stubs.AroundInterceptorBuilders
{
    public class Sync : IBuilder
    {
        private static InterceptorBuilder<TestContext> Builder => BuildRoutine.Interceptor<TestContext>();

        public IInterceptor<TestContext> Build(
            Action before = null, Action success = null, Action fail = null, Action after = null,
            Action<TestContext> beforeCtx = null, Action<TestContext> successCtx = null,
            Action<TestContext> failCtx = null, Action<TestContext> afterCtx = null
        ) => Builder.Do()
            .Before(before).Success(success).Fail(fail).After(after)
            .Before(beforeCtx).Success(successCtx).Fail(failCtx).After(afterCtx);

        public IInterceptor<TestContext> FacadeBefore(
            Action before = null, Action<TestContext> beforeCtx = null
        ) => before != null ? Builder.Before(before) : Builder.Before(beforeCtx);

        public IInterceptor<TestContext> FacadeSuccess(
            Action success = null, Action<TestContext> successCtx = null
        ) => success != null ? Builder.Success(success) : Builder.Success(successCtx);

        public IInterceptor<TestContext> FacadeFail(
            Action fail = null, Action<TestContext> failCtx = null
        ) => fail != null ? Builder.Fail(fail) : Builder.Fail(failCtx);

        public IInterceptor<TestContext> FacadeAfter(
            Action after = null, Action<TestContext> afterCtx = null
        ) => after != null ? Builder.After(after) : Builder.After(afterCtx);
    }
}