using System;

namespace Routine.Interception
{
	public class AdapterChainLinkInterceptor<TContext> : IChainLinkInterceptor<TContext>
		where TContext : InterceptionContext
	{
		private readonly IInterceptor<TContext> real;
		private IChainLinkInterceptor<TContext> next;

		public AdapterChainLinkInterceptor(IInterceptor<TContext> real)
		{
			this.real = real;

			next = new LastChainLinkInterceptor<TContext>();
		}

		public IChainLinkInterceptor<TContext> Next
		{
			get => next;
            set
            {
                value ??= new LastChainLinkInterceptor<TContext>();

                next = value;
            }
		}

		public object Intercept(TContext context, Func<object> invocation) => real.Intercept(context, () => next.Intercept(context, invocation));
    }
}
