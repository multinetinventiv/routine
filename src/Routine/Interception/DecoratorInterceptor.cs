using System;

namespace Routine.Interception
{
	internal class DecoratorInterceptorVariableNameFactory
	{
		private static readonly object VARIABLE_NAME_LOCK = new();
		private static int instanceCount;
		internal static string NextVariableName()
		{
			lock (VARIABLE_NAME_LOCK)
			{
				return "__decoratorVariable_" + (instanceCount++);
			}
		}
	}

	public class DecoratorInterceptor<TContext, TVariableType> : AroundInterceptorBase<DecoratorInterceptor<TContext, TVariableType>, TContext>
		where TContext : InterceptionContext
	{
		private readonly string variableName;
		private readonly Func<TContext, TVariableType> beforeDelegate;
		private Action<TContext, TVariableType> successDelegate;
		private Action<TContext, TVariableType> failDelegate;
		private Action<TContext, TVariableType> afterDelegate;

		public DecoratorInterceptor(Func<TContext, TVariableType> beforeDelegate)
		{
			this.beforeDelegate = beforeDelegate;
			
			variableName = DecoratorInterceptorVariableNameFactory.NextVariableName();

			Success(_ => { });
			Fail(_ => { });
			After(_ => { });
		}

		public DecoratorInterceptor<TContext, TVariableType> Success(Action<TVariableType> successDelegate) => Success((_, obj) => successDelegate(obj));
        public DecoratorInterceptor<TContext, TVariableType> Success(Action<TContext, TVariableType> successDelegate) { this.successDelegate = successDelegate; return this; }

		public DecoratorInterceptor<TContext, TVariableType> Fail(Action<TVariableType> failDelegate) => Fail((_, obj) => failDelegate(obj));
        public DecoratorInterceptor<TContext, TVariableType> Fail(Action<TContext, TVariableType> failDelegate) { this.failDelegate = failDelegate; return this; }

		public DecoratorInterceptor<TContext, TVariableType> After(Action<TVariableType> afterDelegate) => After((_, obj) => afterDelegate(obj));
        public DecoratorInterceptor<TContext, TVariableType> After(Action<TContext, TVariableType> afterDelegate) { this.afterDelegate = afterDelegate; return this; }

		private string ExceptionVariableName => variableName + "_exception";
        private bool ExceptionOccuredOnBefore(TContext context) => context[ExceptionVariableName] != null;

        protected override void OnBefore(TContext context)
		{
			try
			{
				context[variableName] = beforeDelegate(context);
			}
			catch (Exception ex)
			{
				context[ExceptionVariableName] = ex;
				throw;
			}
		}

		protected override void OnSuccess(TContext context)
		{
			if (ExceptionOccuredOnBefore(context)) { return; }

			successDelegate(context, (TVariableType)context[variableName]);
		}

		protected override void OnFail(TContext context)
		{
			if (ExceptionOccuredOnBefore(context)) { return; }

			failDelegate(context, (TVariableType)context[variableName]);
		}

		protected override void OnAfter(TContext context)
		{
			if (ExceptionOccuredOnBefore(context)) { return; }

			afterDelegate(context, (TVariableType)context[variableName]);
		}
	}
}
