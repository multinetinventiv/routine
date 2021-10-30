using System;

namespace Routine.Interception
{
    public abstract class AroundInterceptorBase<TConcrete, TContext> : InterceptorBase<TContext>
        where TConcrete : AroundInterceptorBase<TConcrete, TContext>
        where TContext : InterceptionContext
    {
        private Func<TContext, bool> whenDelegate;

        protected AroundInterceptorBase()
        {
            When(_ => true);
        }

        public TConcrete When(Func<TContext, bool> whenDelegate) { this.whenDelegate = whenDelegate; return (TConcrete)this; }

        protected virtual bool CanIntercept(TContext context) => whenDelegate(context);

        protected override object Intercept(TContext context, Func<object> invocation)
        {
            if (!CanIntercept(context)) { return invocation(); }

            try
            {
                OnBefore(context);

                if (!context.Canceled)
                {
                    context.Result = invocation();
                }

                OnSuccess(context);
            }
            catch (Exception ex)
            {
                context.Exception = ex;
                OnFail(context);
                if (!context.ExceptionHandled)
                {
                    if (ex == context.Exception) // if exception was not changed, preserve stack trace
                    {
                        throw;
                    }
                    else
                    {
                        throw context.Exception;
                    }
                }
            }
            finally
            {
                OnAfter(context);
            }

            return context.Result;
        }

        protected abstract void OnBefore(TContext context);
        protected abstract void OnSuccess(TContext context);
        protected abstract void OnFail(TContext context);
        protected abstract void OnAfter(TContext context);
    }
}
