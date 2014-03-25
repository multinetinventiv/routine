using System;

namespace Routine.Core.Interceptor
{
	public abstract class BaseSingleInterceptor<TConcrete, TContext> : IInterceptor<TContext>
		where TConcrete : BaseSingleInterceptor<TConcrete, TContext>
		where TContext : InterceptionContext
	{
		private Func<TContext, bool> whenDelegate;

		protected BaseSingleInterceptor()
		{
			When(ctx => true);
		}

		public TConcrete When(Func<TContext, bool> whenDelegate) {this.whenDelegate = whenDelegate; return (TConcrete)this;}

		protected virtual bool CanIntercept(TContext context)
		{
			return whenDelegate(context);
		}

		private void OptionalOnBefore(TContext context)
		{
			if (!CanIntercept(context)) { return; }

			OnBefore(context);
		}

		protected abstract void OnBefore(TContext context); 

		private void OptionalOnAfter(TContext context)
		{
			if (!CanIntercept(context)) { return; }

			OnAfter(context);
		}

		protected abstract void OnAfter(TContext context); 

		private void OptionalOnError(TContext context)
		{
			if (!CanIntercept(context)) { return; }

			OnError(context);
		}

		protected abstract void OnError(TContext context); 
		
		#region IInterceptor implementation

		void IInterceptor<TContext>.OnBefore(TContext context)
		{
			OptionalOnBefore(context);
		}

		void IInterceptor<TContext>.OnAfter(TContext context)
		{
			OptionalOnAfter(context);
		}

		void IInterceptor<TContext>.OnError(TContext context)
		{
			OptionalOnError(context);
		} 

		#endregion
	}
}
