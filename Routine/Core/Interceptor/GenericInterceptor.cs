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
		private Action<TContext> afterDelegate;
		private Action<TContext> errorDelegate;

		public GenericInterceptor()
		{
			Before(() => { });
			After(() => { });
			Error(() => { });
		}

		public GenericInterceptor<TContext> Before(Action beforeDelegate) { return Before(ctx => beforeDelegate()); }
		public GenericInterceptor<TContext> Before(Action<TContext> beforeDelegate) { this.beforeDelegate = beforeDelegate; return this; }

		public GenericInterceptor<TContext> After(Action afterDelegate) { return After(ctx => afterDelegate()); }
		public GenericInterceptor<TContext> After(Action<TContext> afterDelegate) { this.afterDelegate = afterDelegate; return this; }

		public GenericInterceptor<TContext> Error(Action errorDelegate) { return Error(ctx => errorDelegate()); }
		public GenericInterceptor<TContext> Error(Action<TContext> errorDelegate) { this.errorDelegate = errorDelegate; return this; }

		protected override void OnBefore(TContext context)
		{
			beforeDelegate(context);
		}

		protected override void OnAfter(TContext context)
		{
			afterDelegate(context);
		}

		protected override void OnError(TContext context)
		{
			errorDelegate(context);
		}
	}
}
