using Routine.Interception;
using Routine.Interception.Configuration;
using System;
using System.Threading.Tasks;

namespace Routine.Test.Interception.Stubs.DecoratorInterceptorBuilders
{
    public class Async : IBuilder
    {
        private static Func<Task<TVariable>> Wrap<TVariable>(Func<TVariable> @delegate) => @delegate == null ? null : async () => { await Task.Delay(1); return @delegate(); };
        private static Func<TestContext, Task<TVariable>> Wrap<TVariable>(Func<TestContext, TVariable> @delegate) => @delegate == null ? null : async ctx => { await Task.Delay(1); return @delegate(ctx); };
        private static Func<TVariable, Task> Wrap<TVariable>(Action<TVariable> @delegate) => @delegate == null ? null : async obj => { await Task.Delay(1); @delegate(obj); };
        private static Func<TestContext, TVariable, Task> Wrap<TVariable>(Action<TestContext, TVariable> @delegate) => @delegate == null ? null : async (ctx, obj) => { await Task.Delay(1); @delegate(ctx, obj); };

        private static InterceptorBuilder<TestContext> Builder => BuildRoutine.Interceptor<TestContext>();

        public IInterceptor<TestContext> Build<TVariable>(
            Func<TVariable> before = null, Action<TVariable> success = null,
            Action<TVariable> fail = null, Action<TVariable> after = null,
            Func<TestContext, TVariable> beforeCtx = null, Action<TestContext, TVariable> successCtx = null,
            Action<TestContext, TVariable> failCtx = null, Action<TestContext, TVariable> afterCtx = null
        ) => (before != null ? Builder.ByDecoratingAsync(Wrap(before)) : Builder.ByDecoratingAsync(Wrap(beforeCtx)))
            .Success(Wrap(success)).Fail(Wrap(fail)).After(Wrap(after))
            .Success(Wrap(successCtx)).Fail(Wrap(failCtx)).After(Wrap(afterCtx));
    }
}