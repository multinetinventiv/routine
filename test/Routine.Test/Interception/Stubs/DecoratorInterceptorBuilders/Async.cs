using Routine.Interception.Configuration;
using Routine.Interception;
using System.Threading.Tasks;
using System;

namespace Routine.Test.Interception.Stubs.DecoratorInterceptorBuilders
{
    public class Async : IBuilder
    {
        private static Func<Task<TVariable>> Wrap<TVariable>(Func<TVariable> @delegate) => @delegate == null ? null : async () => { await Task.Delay(1); return @delegate(); };
        private static Func<Context, Task<TVariable>> Wrap<TVariable>(Func<Context, TVariable> @delegate) => @delegate == null ? null : async ctx => { await Task.Delay(1); return @delegate(ctx); };
        private static Func<TVariable, Task> Wrap<TVariable>(Action<TVariable> @delegate) => @delegate == null ? null : async obj => { await Task.Delay(1); @delegate(obj); };
        private static Func<Context, TVariable, Task> Wrap<TVariable>(Action<Context, TVariable> @delegate) => @delegate == null ? null : async (ctx, obj) => { await Task.Delay(1); @delegate(ctx, obj); };

        private static InterceptorBuilder<Context> Builder => BuildRoutine.Interceptor<Context>();

        public IInterceptor<Context> Build<TVariable>(
            Func<TVariable> before = null, Action<TVariable> success = null,
            Action<TVariable> fail = null, Action<TVariable> after = null,
            Func<Context, TVariable> beforeCtx = null, Action<Context, TVariable> successCtx = null,
            Action<Context, TVariable> failCtx = null, Action<Context, TVariable> afterCtx = null
        ) => (before != null ? Builder.ByDecoratingAsync(Wrap(before)) : Builder.ByDecoratingAsync(Wrap(beforeCtx)))
            .Success(Wrap(success)).Fail(Wrap(fail)).After(Wrap(after))
            .Success(Wrap(successCtx)).Fail(Wrap(failCtx)).After(Wrap(afterCtx));
    }
}
