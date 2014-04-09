using System;
using Routine.Core.Interceptor.Chain;

namespace Routine.Core.Interceptor
{
	public class ChainInterceptor<TConfiguration, TContext> : IInterceptor<TContext>
		where TContext : InterceptionContext
	{
		private readonly TConfiguration configuration;
		private IChainLinkInterceptor<TContext> first;
		private IChainLinkInterceptor<TContext> last;

		public ChainInterceptor(TConfiguration configuration)
		{
			this.configuration = configuration;
		}

		public TConfiguration Done() { return configuration; }
		public TConfiguration Done(IInterceptor<TContext> interceptor) { Add(interceptor); return configuration; }

		public ChainInterceptor<TConfiguration, TContext> Add(IInterceptor<TContext> interceptor)
		{
			var newLink = new AdapterChainLinkInterceptor<TContext>(interceptor);

			if (first == null || last == null)
			{
				first = last = newLink;

				return this;
			}

			last.Next = newLink;
			last = newLink;

			return this;
		}

		public ChainInterceptor<TConfiguration, TContext> Merge(ChainInterceptor<TConfiguration, TContext> other)
		{
			if (other.first == null || other.last == null) { return this; }

			if (first == null || last == null)
			{
				first = other.first;
				last = other.last;
				return this;
			}

			last.Next = other.first;
			last = other.last;

			return this;
		}

		private object Intercept(TContext context, Func<object> invocation)
		{
			if (first == null)
			{
				return invocation();
			}

			return first.Intercept(context, invocation);
		}

		#region IInterceptor<TContext> implementation

		object IInterceptor<TContext>.Intercept(TContext context, Func<object> invocation) { return Intercept(context, invocation); }
 
		#endregion
	}
}
