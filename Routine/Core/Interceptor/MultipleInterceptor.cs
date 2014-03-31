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
		public TConfiguration Done(IInterceptor<TContext> interceptor) { Add(interceptor); return configuration; }

		public MultipleInterceptor<TConfiguration, TContext> Add(IInterceptor<TContext> interceptor)
		{
			this.interceptors.Add(interceptor);

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

		protected void OnSuccess(TContext context)
		{
			for (int i = interceptors.Count - 1; i >= 0; i--)
			{
				interceptors[i].OnSuccess(context);
			}
		}

		protected void OnFail(TContext context)
		{
			for (int i = interceptors.Count - 1; i >= 0; i--)
			{
				interceptors[i].OnFail(context);
			}
		}

		protected void OnAfter(TContext context)
		{
			for (int i = interceptors.Count - 1; i >= 0; i--)
			{
				interceptors[i].OnAfter(context);
			}
		}

		#region IInterceptor<TContext> implementation

		void IInterceptor<TContext>.OnBefore(TContext context) { OnBefore(context); }
		void IInterceptor<TContext>.OnSuccess(TContext context) { OnSuccess(context); }
		void IInterceptor<TContext>.OnFail(TContext context) { OnFail(context); }
		void IInterceptor<TContext>.OnAfter(TContext context) { OnAfter(context); }
 
		#endregion	
	}
}
