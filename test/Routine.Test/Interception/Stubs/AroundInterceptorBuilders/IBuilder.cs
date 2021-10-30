using Routine.Interception;
using System;

namespace Routine.Test.Interception.Stubs.AroundInterceptorBuilders
{
    public interface IBuilder
    {
        IInterceptor<Context> Build(
            Action before = null, Action success = null, Action fail = null, Action after = null,
            Action<Context> beforeCtx = null, Action<Context> successCtx = null, Action<Context> failCtx = null, Action<Context> afterCtx = null
        );
        IInterceptor<Context> FacadeBefore(Action before = null, Action<Context> beforeCtx = null);
        IInterceptor<Context> FacadeSuccess(Action success = null, Action<Context> successCtx = null);
        IInterceptor<Context> FacadeFail(Action fail = null, Action<Context> failCtx = null);
        IInterceptor<Context> FacadeAfter(Action after = null, Action<Context> afterCtx = null);
    }
}