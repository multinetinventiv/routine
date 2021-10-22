using System;
using System.Threading.Tasks;

namespace Routine.Interception
{
	public class AroundInterceptor<TContext> : AroundInterceptorBase<AroundInterceptor<TContext>, TContext>
		where TContext : InterceptionContext
	{
		private Action<TContext> beforeDelegate;
		private Action<TContext> successDelegate;
		private Action<TContext> failDelegate;
		private Action<TContext> afterDelegate;

		public AroundInterceptor()
		{
			Before(() => { });
			Success(() => { });
			Fail(() => { });
			After(() => { });
		}

		public AroundInterceptor<TContext> Before(Action beforeDelegate) => Before(_ => beforeDelegate());
        public AroundInterceptor<TContext> Before(Action<TContext> beforeDelegate) { this.beforeDelegate = beforeDelegate; return this; }

		public AroundInterceptor<TContext> Success(Action successDelegate) => Success(_ => successDelegate());
        public AroundInterceptor<TContext> Success(Action<TContext> successDelegate) { this.successDelegate = successDelegate; return this; }

		public AroundInterceptor<TContext> Fail(Action failDelegate) => Fail(_ => failDelegate());
        public AroundInterceptor<TContext> Fail(Action<TContext> failDelegate) { this.failDelegate = failDelegate; return this; }

		public AroundInterceptor<TContext> After(Action afterDelegate) => After(_ => afterDelegate());
        public AroundInterceptor<TContext> After(Action<TContext> afterDelegate) { this.afterDelegate = afterDelegate; return this; }

		protected override void OnBefore(TContext context) => beforeDelegate(context);
        protected override void OnSuccess(TContext context) => successDelegate(context);
        protected override void OnFail(TContext context) => failDelegate(context);
        protected override void OnAfter(TContext context) => afterDelegate(context);
    }

    public class AsyncAroundInterceptor<TContext> : AsyncAroundInterceptorBase<AsyncAroundInterceptor<TContext>, TContext>
        where TContext : InterceptionContext
    {
        private Func<TContext, Task> beforeDelegate;
        private Func<TContext, Task> successDelegate;
        private Func<TContext, Task> failDelegate;
        private Func<TContext, Task> afterDelegate;

        public AsyncAroundInterceptor()
        {
            Before(() => { });
            Success(() => { });
            Fail(() => { });
            After(() => { });
        }

        public AsyncAroundInterceptor<TContext> Before(Action beforeDelegate) => Before(_ => beforeDelegate());
        public AsyncAroundInterceptor<TContext> Before(Action<TContext> beforeDelegate) => Before(ctx => { beforeDelegate(ctx); return Task.CompletedTask; });
        public AsyncAroundInterceptor<TContext> Before(Func<Task> beforeDelegate) => Before(async _ => await beforeDelegate());
        public AsyncAroundInterceptor<TContext> Before(Func<TContext, Task> beforeDelegate) { this.beforeDelegate = beforeDelegate; return this; }

        public AsyncAroundInterceptor<TContext> Success(Action successDelegate) => Success(_ => successDelegate());
        public AsyncAroundInterceptor<TContext> Success(Action<TContext> successDelegate) => Success(ctx => { successDelegate(ctx); return Task.CompletedTask; });
        public AsyncAroundInterceptor<TContext> Success(Func<Task> successDelegate) => Success(async _ => await successDelegate());
        public AsyncAroundInterceptor<TContext> Success(Func<TContext, Task> successDelegate) { this.successDelegate = successDelegate; return this; }

        public AsyncAroundInterceptor<TContext> Fail(Action failDelegate) => Fail(_ => failDelegate());
        public AsyncAroundInterceptor<TContext> Fail(Action<TContext> failDelegate) => Fail(ctx => { failDelegate(ctx); return Task.CompletedTask; });
        public AsyncAroundInterceptor<TContext> Fail(Func<Task> failDelegate) => Fail(async _ => await failDelegate());
        public AsyncAroundInterceptor<TContext> Fail(Func<TContext, Task> failDelegate) { this.failDelegate = failDelegate; return this; }

        public AsyncAroundInterceptor<TContext> After(Action afterDelegate) => After(_ => afterDelegate());
        public AsyncAroundInterceptor<TContext> After(Action<TContext> afterDelegate) => After(ctx => { afterDelegate(ctx); return Task.CompletedTask; });
        public AsyncAroundInterceptor<TContext> After(Func<Task> afterDelegate) => After(async _ => await afterDelegate());
        public AsyncAroundInterceptor<TContext> After(Func<TContext, Task> afterDelegate) { this.afterDelegate = afterDelegate; return this; }

        protected override async Task OnBefore(TContext context) => await beforeDelegate(context);
        protected override async Task OnSuccess(TContext context) => await successDelegate(context);
        protected override async Task OnFail(TContext context) => await failDelegate(context);
        protected override async Task OnAfter(TContext context) => await afterDelegate(context);
    }
}
