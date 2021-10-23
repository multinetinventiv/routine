using Routine.Interception;
using System;

namespace Routine.Test.Interception.Stubs.AroundInterceptorBuilders
{
    public interface IBuilder
    {
        IInterceptor<TestContext> Build(
            Action before = null, Action success = null, Action fail = null, Action after = null,
            Action<TestContext> beforeCtx = null, Action<TestContext> successCtx = null, Action<TestContext> failCtx = null, Action<TestContext> afterCtx = null
        );
        IInterceptor<TestContext> FacadeBefore(Action before = null, Action<TestContext> beforeCtx = null);
        IInterceptor<TestContext> FacadeSuccess(Action success = null, Action<TestContext> successCtx = null);
        IInterceptor<TestContext> FacadeFail(Action fail = null, Action<TestContext> failCtx = null);
        IInterceptor<TestContext> FacadeAfter(Action after = null, Action<TestContext> afterCtx = null);
    }
}