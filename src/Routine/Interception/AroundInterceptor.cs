using System;
using System.Threading.Tasks;

namespace Routine.Interception
{
    public class AroundInterceptor<TContext> : AroundInterceptorBase<AroundInterceptor<TContext>, TContext>
        where TContext : InterceptionContext
    {
        private static readonly Action<TContext> EMPTY = _ => { };
        private static Action<TContext> Wrap(Action @delegate) => @delegate == null ? null : _ => @delegate();

        private Action<TContext> beforeDelegate = EMPTY;
        private Action<TContext> successDelegate = EMPTY;
        private Action<TContext> failDelegate = EMPTY;
        private Action<TContext> afterDelegate = EMPTY;

        public AroundInterceptor<TContext> Before(Action beforeDelegate) => Before(Wrap(beforeDelegate));
        public AroundInterceptor<TContext> Before(Action<TContext> beforeDelegate) { this.beforeDelegate = beforeDelegate ?? this.beforeDelegate; return this; }

        public AroundInterceptor<TContext> Success(Action successDelegate) => Success(Wrap(successDelegate));
        public AroundInterceptor<TContext> Success(Action<TContext> successDelegate) { this.successDelegate = successDelegate ?? this.successDelegate; return this; }

        public AroundInterceptor<TContext> Fail(Action failDelegate) => Fail(Wrap(failDelegate));
        public AroundInterceptor<TContext> Fail(Action<TContext> failDelegate) { this.failDelegate = failDelegate ?? this.failDelegate; return this; }

        public AroundInterceptor<TContext> After(Action afterDelegate) => After(Wrap(afterDelegate));
        public AroundInterceptor<TContext> After(Action<TContext> afterDelegate) { this.afterDelegate = afterDelegate ?? this.afterDelegate; return this; }

        protected override void OnBefore(TContext context) => beforeDelegate(context);
        protected override void OnSuccess(TContext context) => successDelegate(context);
        protected override void OnFail(TContext context) => failDelegate(context);
        protected override void OnAfter(TContext context) => afterDelegate(context);
    }

    public class AsyncAroundInterceptor<TContext> : AsyncAroundInterceptorBase<AsyncAroundInterceptor<TContext>, TContext>
        where TContext : InterceptionContext
    {
        private static readonly Func<TContext, Task> EMPTY = _ => Task.CompletedTask;
        private static Func<TContext, Task> Wrap(Action @delegate) => @delegate == null ? null : _ => { @delegate(); return Task.CompletedTask; };
        private static Func<TContext, Task> Wrap(Action<TContext> @delegate) => @delegate == null ? null : ctx => { @delegate(ctx); return Task.CompletedTask; };
        private static Func<TContext, Task> Wrap(Func<Task> @delegate) => @delegate == null ? null : async _ => await @delegate();

        private Func<TContext, Task> beforeDelegate = EMPTY;
        private Func<TContext, Task> successDelegate = EMPTY;
        private Func<TContext, Task> failDelegate = EMPTY;
        private Func<TContext, Task> afterDelegate = EMPTY;

        public AsyncAroundInterceptor<TContext> Before(Action beforeDelegate) => Before(Wrap(beforeDelegate));
        public AsyncAroundInterceptor<TContext> Before(Action<TContext> beforeDelegate) => Before(Wrap(beforeDelegate));
        public AsyncAroundInterceptor<TContext> Before(Func<Task> beforeDelegate) => Before(Wrap(beforeDelegate));
        public AsyncAroundInterceptor<TContext> Before(Func<TContext, Task> beforeDelegate) { this.beforeDelegate = beforeDelegate ?? this.beforeDelegate; return this; }

        public AsyncAroundInterceptor<TContext> Success(Action successDelegate) => Success(Wrap(successDelegate));
        public AsyncAroundInterceptor<TContext> Success(Action<TContext> successDelegate) => Success(Wrap(successDelegate));
        public AsyncAroundInterceptor<TContext> Success(Func<Task> successDelegate) => Success(Wrap(successDelegate));
        public AsyncAroundInterceptor<TContext> Success(Func<TContext, Task> successDelegate) { this.successDelegate = successDelegate ?? this.successDelegate; return this; }

        public AsyncAroundInterceptor<TContext> Fail(Action failDelegate) => Fail(Wrap(failDelegate));
        public AsyncAroundInterceptor<TContext> Fail(Action<TContext> failDelegate) => Fail(Wrap(failDelegate));
        public AsyncAroundInterceptor<TContext> Fail(Func<Task> failDelegate) => Fail(Wrap(failDelegate));
        public AsyncAroundInterceptor<TContext> Fail(Func<TContext, Task> failDelegate) { this.failDelegate = failDelegate ?? this.failDelegate; return this; }

        public AsyncAroundInterceptor<TContext> After(Action afterDelegate) => After(Wrap(afterDelegate));
        public AsyncAroundInterceptor<TContext> After(Action<TContext> afterDelegate) => After(Wrap(afterDelegate));
        public AsyncAroundInterceptor<TContext> After(Func<Task> afterDelegate) => After(Wrap(afterDelegate));
        public AsyncAroundInterceptor<TContext> After(Func<TContext, Task> afterDelegate) { this.afterDelegate = afterDelegate ?? this.afterDelegate; return this; }

        protected override async Task OnBefore(TContext context) => await beforeDelegate(context);
        protected override async Task OnSuccess(TContext context) => await successDelegate(context);
        protected override async Task OnFail(TContext context) => await failDelegate(context);
        protected override async Task OnAfter(TContext context) => await afterDelegate(context);
    }
}
