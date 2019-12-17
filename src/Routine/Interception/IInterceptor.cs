using System;

namespace Routine.Interception
{
	public interface IInterceptor<in TContext>
		where TContext : InterceptionContext
	{
		object Intercept(TContext context, Func<object> invocation);
	}
}
