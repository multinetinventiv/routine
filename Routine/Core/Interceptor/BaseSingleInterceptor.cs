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

		private void OptionalOnSuccess(TContext context)
		{
			if (!CanIntercept(context)) { return; }

			OnSuccess(context);
		}

		protected abstract void OnSuccess(TContext context); 

		private void OptionalOnFail(TContext context)
		{
			if (!CanIntercept(context)) { return; }

			OnFail(context);
		}

		protected abstract void OnFail(TContext context);

		private void OptionalOnAfter(TContext context)
		{
			if (!CanIntercept(context)) { return; }

			OnAfter(context);
		}

		protected abstract void OnAfter(TContext context); 
		
		#region IInterceptor implementation

		void IInterceptor<TContext>.OnBefore(TContext context)
		{
			OptionalOnBefore(context);
		}

		void IInterceptor<TContext>.OnSuccess(TContext context)
		{
			OptionalOnSuccess(context);
		}

		void IInterceptor<TContext>.OnFail(TContext context)
		{
			OptionalOnFail(context);
		}

		void IInterceptor<TContext>.OnAfter(TContext context)
		{
			OptionalOnAfter(context);
		} 

		#endregion
	}
}
