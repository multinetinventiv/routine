using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Routine.Core.Interceptor
{
	public class GenericInterceptor<TContext> : BaseSingleInterceptor<GenericInterceptor<TContext>, TContext>
		where TContext : InterceptionContext
	{
		private Action<TContext> beforeDelegate;
		private Action<TContext> successDelegate;
		private Action<TContext> failDelegate;
		private Action<TContext> afterDelegate;

		public GenericInterceptor()
		{
			Before(() => { });
			Success(() => { });
			Fail(() => { });
			After(() => { });
		}

		public GenericInterceptor<TContext> Before(Action beforeDelegate) { return Before(ctx => beforeDelegate()); }
		public GenericInterceptor<TContext> Before(Action<TContext> beforeDelegate) { this.beforeDelegate = beforeDelegate; return this; }

		public GenericInterceptor<TContext> Success(Action successDelegate) { return Success(ctx => successDelegate()); }
		public GenericInterceptor<TContext> Success(Action<TContext> successDelegate) { this.successDelegate = successDelegate; return this; }

		public GenericInterceptor<TContext> Fail(Action failDelegate) { return Fail(ctx => failDelegate()); }
		public GenericInterceptor<TContext> Fail(Action<TContext> failDelegate) { this.failDelegate = failDelegate; return this; }

		public GenericInterceptor<TContext> After(Action afterDelegate) { return After(ctx => afterDelegate()); }
		public GenericInterceptor<TContext> After(Action<TContext> afterDelegate) { this.afterDelegate = afterDelegate; return this; }

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
