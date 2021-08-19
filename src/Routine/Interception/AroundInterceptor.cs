using System;

namespace Routine.Interception
{
	public class AroundInterceptor<TContext> : BaseAroundInterceptor<AroundInterceptor<TContext>, TContext>
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

		public AroundInterceptor<TContext> Before(Action beforeDelegate) { return Before(_ => beforeDelegate()); }
		public AroundInterceptor<TContext> Before(Action<TContext> beforeDelegate) { this.beforeDelegate = beforeDelegate; return this; }

		public AroundInterceptor<TContext> Success(Action successDelegate) { return Success(_ => successDelegate()); }
		public AroundInterceptor<TContext> Success(Action<TContext> successDelegate) { this.successDelegate = successDelegate; return this; }

		public AroundInterceptor<TContext> Fail(Action failDelegate) { return Fail(_ => failDelegate()); }
		public AroundInterceptor<TContext> Fail(Action<TContext> failDelegate) { this.failDelegate = failDelegate; return this; }

		public AroundInterceptor<TContext> After(Action afterDelegate) { return After(_ => afterDelegate()); }
		public AroundInterceptor<TContext> After(Action<TContext> afterDelegate) { this.afterDelegate = afterDelegate; return this; }

		protected override void OnBefore(TContext context)
		{
			beforeDelegate(context);
		}

		protected override void OnSuccess(TContext context)
		{
			successDelegate(context);
		}

		protected override void OnFail(TContext context)
		{
			failDelegate(context);
		}

		protected override void OnAfter(TContext context)
		{
			afterDelegate(context);
		}
	}
}
