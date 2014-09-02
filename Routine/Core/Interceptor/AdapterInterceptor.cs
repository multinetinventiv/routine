using System;

namespace Routine.Core.Interceptor
{
	public class AdapterInterceptor<TContext, TAdaptedContext> : IInterceptor<TContext>
		where TContext : TAdaptedContext
		where TAdaptedContext : InterceptionContext
	{
		private readonly IInterceptor<TAdaptedContext> wrappedInterceptor;

		public AdapterInterceptor(IInterceptor<TAdaptedContext> wrappedInterceptor)
		{
			this.wrappedInterceptor = wrappedInterceptor;
		}

		public object Intercept(TContext context, Func<object> invocation)
		{
			return wrappedInterceptor.Intercept(context, invocation);
		}
	}
}
