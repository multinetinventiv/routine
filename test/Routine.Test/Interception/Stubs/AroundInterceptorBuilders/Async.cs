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
    ) => Builder.DoAsync()
        .Before(Wrap(before)).Success(Wrap(success)).Fail(Wrap(fail)).After(Wrap(after))
        .Before(Wrap(beforeCtx)).Success(Wrap(successCtx)).Fail(Wrap(failCtx)).After(Wrap(afterCtx));

    public IInterceptor<Context> FacadeBefore(
        Action before = null, Action<Context> beforeCtx = null
    ) => before != null ? Builder.BeforeAsync(Wrap(before)) : Builder.BeforeAsync(Wrap(beforeCtx));

    public IInterceptor<Context> FacadeSuccess(
        Action success = null, Action<Context> successCtx = null
    ) => success != null ? Builder.SuccessAsync(Wrap(success)) : Builder.SuccessAsync(Wrap(successCtx));

    public IInterceptor<Context> FacadeFail(
        Action fail = null, Action<Context> failCtx = null
    ) => fail != null ? Builder.FailAsync(Wrap(fail)) : Builder.FailAsync(Wrap(failCtx));

    public IInterceptor<Context> FacadeAfter(
        Action after = null, Action<Context> afterCtx = null
    ) => after != null ? Builder.AfterAsync(Wrap(after)) : Builder.AfterAsync(Wrap(afterCtx));
}
