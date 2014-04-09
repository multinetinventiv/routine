using System;

namespace Routine.Core.Interceptor.Chain
{
	public class LastChainLinkInterceptor<TContext> : IChainLinkInterceptor<TContext>
		where TContext : InterceptionContext
	{
		public IChainLinkInterceptor<TContext> Next { get { return null; } set { } }

		public object Intercept(TContext context, Func<object> invocation)
		{
			return invocation();
		}
	}
}
