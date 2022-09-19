using Routine.Interception.Configuration;
using Routine.Interception;

namespace Routine.Test.Interception.Stubs.AroundInterceptorBuilders;

public class Async : IBuilder
{
    private static Func<Task> Wrap(Action @delegate) => @delegate == null ? null : async () => { await Task.Delay(1); @delegate(); };
    private static Func<Context, Task> Wrap(Action<Context> @delegate) => @delegate == null ? null : async ctx => { await Task.Delay(1); @delegate(ctx); };

    private static InterceptorBuilder<Context> Builder => BuildRoutine.Interceptor<Context>();

    public IInterceptor<Context> Build(
        Action before = null, Action success = null, Action fail = null, Action after = null,
        Action<Context> beforeCtx = null, Action<Context> successCtx = null,
        Action<Context> failCtx = null, Action<Context> afterCtx = null
    ) => Builder.Do()
        .Before(Wrap(before)).Success(Wrap(success)).Fail(Wrap(fail)).After(Wrap(after))
        .Before(Wrap(beforeCtx)).Success(Wrap(successCtx)).Fail(Wrap(failCtx)).After(Wrap(afterCtx));

    public IInterceptor<Context> FacadeBefore(
        Action before = null, Action<Context> beforeCtx = null
    ) => before != null ? Builder.Before(Wrap(before)) : Builder.Before(Wrap(beforeCtx));

    public IInterceptor<Context> FacadeSuccess(
        Action success = null, Action<Context> successCtx = null
    ) => success != null ? Builder.Success(Wrap(success)) : Builder.Success(Wrap(successCtx));

    public IInterceptor<Context> FacadeFail(
        Action fail = null, Action<Context> failCtx = null
    ) => fail != null ? Builder.Fail(Wrap(fail)) : Builder.Fail(Wrap(failCtx));

    public IInterceptor<Context> FacadeAfter(
        Action after = null, Action<Context> afterCtx = null
    ) => after != null ? Builder.After(Wrap(after)) : Builder.After(Wrap(afterCtx));
}
