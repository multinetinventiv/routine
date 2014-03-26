using System;

namespace Routine.Core.Interceptor
{
	public class Interception : Interception<InterceptionContext> 
	{
		public Interception(IInterceptor<InterceptionContext> interceptor, InterceptionContext context)
			: base(interceptor, context) { }
	}

	public class Interception<TContext>
		where TContext : InterceptionContext
	{
		private readonly IInterceptor<TContext> interceptor;
		private readonly TContext context;

		public Interception(IInterceptor<TContext> interceptor, TContext context)
		{
			this.interceptor = interceptor;
			this.context = context;
		}

		public object Do(Func<object> invocation)
		{
			try
			{
				interceptor.OnBefore(context);

				if (!context.Canceled)
				{
					context.Result = invocation();
				}

				interceptor.OnAfter(context);
			}
			catch (Exception ex)
			{
				context.Exception = ex;
				interceptor.OnError(context);
				if (!context.ExceptionHandled)
				{
					throw context.Exception;
				}
			}

			return context.Result;
		}
	}
}
