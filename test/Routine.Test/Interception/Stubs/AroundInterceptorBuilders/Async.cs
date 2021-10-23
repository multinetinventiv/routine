using Routine.Interception;
using Routine.Interception.Configuration;
using System;
using System.Threading.Tasks;

namespace Routine.Test.Interception.Stubs.AroundInterceptorBuilders
{
    public class Async : IBuilder
    {
        private static Func<Task> Wrap(Action @delegate) => @delegate == null ? null : async () => { await Task.Delay(1); @delegate(); };
        private static Func<TestContext, Task> Wrap(Action<TestContext> @delegate) => @delegate == null ? null : async ctx => { await Task.Delay(1); @delegate(ctx); };

        private static InterceptorBuilder<TestContext> Builder => BuildRoutine.Interceptor<TestContext>();

        public IInterceptor<TestContext> Build(
            Action before = null, Action success = null, Action fail = null, Action after = null,
            Action<TestContext> beforeCtx = null, Action<TestContext> successCtx = null,
            Action<TestContext> failCtx = null, Action<TestContext> afterCtx = null
        ) => Builder.DoAsync()
            .Before(Wrap(before)).Success(Wrap(success)).Fail(Wrap(fail)).After(Wrap(after))
            .Before(Wrap(beforeCtx)).Success(Wrap(successCtx)).Fail(Wrap(failCtx)).After(Wrap(afterCtx));

        public IInterceptor<TestContext> FacadeBefore(
            Action before = null, Action<TestContext> beforeCtx = null
        ) => before != null ? Builder.BeforeAsync(Wrap(before)) : Builder.BeforeAsync(Wrap(beforeCtx));

        public IInterceptor<TestContext> FacadeSuccess(
            Action success = null, Action<TestContext> successCtx = null
        ) => success != null ? Builder.SuccessAsync(Wrap(success)) : Builder.SuccessAsync(Wrap(successCtx));

        public IInterceptor<TestContext> FacadeFail(
            Action fail = null, Action<TestContext> failCtx = null
        ) => fail != null ? Builder.FailAsync(Wrap(fail)) : Builder.FailAsync(Wrap(failCtx));

        public IInterceptor<TestContext> FacadeAfter(
            Action after = null, Action<TestContext> afterCtx = null
        ) => after != null ? Builder.AfterAsync(Wrap(after)) : Builder.AfterAsync(Wrap(afterCtx));
    }
}