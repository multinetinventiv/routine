using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Routine.Core.Interceptor
{
	public class MultipleInterceptor<TConfiguration, TContext> : IInterceptor<TContext>
		where TContext : InterceptionContext
	{
		private readonly TConfiguration configuration;
		private readonly List<IInterceptor<TContext>> interceptors;

		public MultipleInterceptor(TConfiguration configuration)
		{
			this.configuration = configuration;
			this.interceptors = new List<IInterceptor<TContext>>();
		}

		public TConfiguration Done() { return configuration; }
		public TConfiguration Done(IInterceptor<TContext> selector) { Add(selector); return configuration; }

		public MultipleInterceptor<TConfiguration, TContext> Add(IInterceptor<TContext> selector)
		{
			this.interceptors.Add(selector);

			return this;
		}

		public MultipleInterceptor<TConfiguration, TContext> Merge(MultipleInterceptor<TConfiguration, TContext> other)
		{
			interceptors.AddRange(other.interceptors);

			return this;
		}

		protected void OnBefore(TContext context)
		{
			foreach (var interceptor in interceptors)
			{
				interceptor.OnBefore(context);
			}
		}

		protected void OnAfter(TContext context)
		{
			for (int i = interceptors.Count - 1; i >= 0; i--)
			{
				interceptors[i].OnAfter(context);
			}
		}

		protected void OnError(TContext context)
		{
			for (int i = interceptors.Count - 1; i >= 0; i--)
			{
				interceptors[i].OnError(context);
			}
		}

		#region IInterceptor<TContext> implementation

		void IInterceptor<TContext>.OnBefore(TContext context) { OnBefore(context); }
		void IInterceptor<TContext>.OnAfter(TContext context) { OnAfter(context); }
		void IInterceptor<TContext>.OnError(TContext context) { OnError(context); }
 
		#endregion	
	}
}
