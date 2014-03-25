using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Routine.Core.Interceptor
{
	public static class InterceptionFactory
	{
		public static Interception<TContext> Intercept<TContext>(this IInterceptor<TContext> source, TContext context) where TContext : InterceptionContext
		{
			return new Interception<TContext>(source, context);
		}
	}
}
