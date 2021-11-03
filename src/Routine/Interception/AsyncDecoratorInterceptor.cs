using System;
using System.Threading.Tasks;

namespace Routine.Interception
{
    public class AsyncDecoratorInterceptor<TContext, TVariable> : AsyncAroundInterceptorBase<AsyncDecoratorInterceptor<TContext, TVariable>, TContext>
        where TContext : InterceptionContext
    {
        private static readonly Func<TContext, TVariable, Task> EMPTY = (_, _) => Task.CompletedTask;
        private static Func<TContext, TVariable, Task> Wrap(Action<TVariable> @delegate) => @delegate == null ? null : (_, obj) => { @delegate(obj); return Task.CompletedTask; };
        private static Func<TContext, TVariable, Task> Wrap(Action<TContext, TVariable> @delegate) => @delegate == null ? null : (ctx, obj) => { @delegate(ctx, obj); return Task.CompletedTask; };
        private static Func<TContext, TVariable, Task> Wrap(Func<TVariable, Task> @delegate) => @delegate == null ? null : async (_, obj) => await @delegate(obj);

        private readonly Func<TContext, Task<TVariable>> beforeDelegate;
        private readonly string variableName;

        private Func<TContext, TVariable, Task> successDelegate = EMPTY;
        private Func<TContext, TVariable, Task> failDelegate = EMPTY;
        private Func<TContext, TVariable, Task> afterDelegate = EMPTY;


        public AsyncDecoratorInterceptor(Func<TVariable> beforeDelegate) : this(_ => Task.FromResult(beforeDelegate())) { }
        public AsyncDecoratorInterceptor(Func<TContext, TVariable> beforeDelegate) : this(ctx => Task.FromResult(beforeDelegate(ctx))) { }
        public AsyncDecoratorInterceptor(Func<Task<TVariable>> beforeDelegate) : this(async _ => await beforeDelegate()) { }
        public AsyncDecoratorInterceptor(Func<TContext, Task<TVariable>> beforeDelegate)
        {
            this.beforeDelegate = beforeDelegate ?? throw new ArgumentNullException(nameof(beforeDelegate));

            variableName = DecoratorInterceptorVariableNameFactory.NextVariableName();
        }

        public AsyncDecoratorInterceptor<TContext, TVariable> Success(Action<TVariable> successDelegate) => Success(Wrap(successDelegate));
        public AsyncDecoratorInterceptor<TContext, TVariable> Success(Action<TContext, TVariable> successDelegate) => Success(Wrap(successDelegate));
        public AsyncDecoratorInterceptor<TContext, TVariable> Success(Func<TVariable, Task> successDelegate) => Success(Wrap(successDelegate));
        public AsyncDecoratorInterceptor<TContext, TVariable> Success(Func<TContext, TVariable, Task> successDelegate) { this.successDelegate = successDelegate ?? this.successDelegate; return this; }
        
        public AsyncDecoratorInterceptor<TContext, TVariable> Fail(Action<TVariable> failDelegate) => Fail(Wrap(failDelegate));
        public AsyncDecoratorInterceptor<TContext, TVariable> Fail(Action<TContext, TVariable> failDelegate) => Fail(Wrap(failDelegate));
        public AsyncDecoratorInterceptor<TContext, TVariable> Fail(Func<TVariable, Task> failDelegate) => Fail(Wrap(failDelegate));
        public AsyncDecoratorInterceptor<TContext, TVariable> Fail(Func<TContext, TVariable, Task> failDelegate) { this.failDelegate = failDelegate ?? this.failDelegate; return this; }
        
        public AsyncDecoratorInterceptor<TContext, TVariable> After(Action<TVariable> afterDelegate) => After(Wrap(afterDelegate));
        public AsyncDecoratorInterceptor<TContext, TVariable> After(Action<TContext, TVariable> afterDelegate) => After(Wrap(afterDelegate));
        public AsyncDecoratorInterceptor<TContext, TVariable> After(Func<TVariable, Task> afterDelegate) => After(Wrap(afterDelegate));
        public AsyncDecoratorInterceptor<TContext, TVariable> After(Func<TContext, TVariable, Task> afterDelegate) { this.afterDelegate = afterDelegate ?? this.afterDelegate; return this; }

        private string ExceptionVariableName => variableName + "_exception";
        private bool ExceptionOccuredOnBefore(TContext context) => context[ExceptionVariableName] != null;

        protected override async Task OnBefore(TContext context)
        {
            try
            {
                context[variableName] = await beforeDelegate(context);
            }
            catch (Exception ex)
            {
                context[ExceptionVariableName] = ex;
                throw;
            }
        }

        protected override async Task OnSuccess(TContext context)
        {
            if (ExceptionOccuredOnBefore(context)) { return; }

            await successDelegate(context, (TVariable)context[variableName]);
        }

        protected override async Task OnFail(TContext context)
        {
            if (ExceptionOccuredOnBefore(context)) { return; }

            await failDelegate(context, (TVariable)context[variableName]);
        }

        protected override async Task OnAfter(TContext context)
        {
            if (ExceptionOccuredOnBefore(context)) { return; }

            await afterDelegate(context, (TVariable)context[variableName]);
        }
    }
}