using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Routine.Core.Interceptor
{
	public class DecoratorInterceptor<TContext, TVariableType> : BaseSingleInterceptor<DecoratorInterceptor<TContext, TVariableType>, TContext>
		where TContext : InterceptionContext
	{
		private static object variableNameLock = new object();
		private static int instanceCount = 0;
		private static string NextVariableName()
		{
			lock (variableNameLock)
			{
				return "__decoratorVariable_" + (instanceCount++);
			}
		}

		private readonly string variableName;
		private readonly Func<TContext, TVariableType> beforeDelegate;
		private Action<TContext, TVariableType> successDelegate;
		private Action<TContext, TVariableType> failDelegate;
		private Action<TContext, TVariableType> afterDelegate;

		public DecoratorInterceptor(Func<TContext, TVariableType> beforeDelegate)
		{
			this.variableName = NextVariableName();
			this.beforeDelegate = beforeDelegate;

			Success(obj => { });
			Fail(obj => { });
			After(obj => { });
		}

		public DecoratorInterceptor<TContext, TVariableType> Success(Action<TVariableType> successDelegate) { return Success((ctx, obj) => successDelegate(obj)); }
		public DecoratorInterceptor<TContext, TVariableType> Success(Action<TContext, TVariableType> successDelegate) { this.successDelegate = successDelegate; return this; }

		public DecoratorInterceptor<TContext, TVariableType> Fail(Action<TVariableType> failDelegate) { return Fail((ctx, obj) => failDelegate(obj)); }
		public DecoratorInterceptor<TContext, TVariableType> Fail(Action<TContext, TVariableType> failDelegate) { this.failDelegate = failDelegate; return this; }

		public DecoratorInterceptor<TContext, TVariableType> After(Action<TVariableType> afterDelegate) { return After((ctx, obj) => afterDelegate(obj)); }
		public DecoratorInterceptor<TContext, TVariableType> After(Action<TContext, TVariableType> afterDelegate) { this.afterDelegate = afterDelegate; return this; }

		private string ExceptionVariableName { get { return variableName + "_exception"; } }
		private bool ExceptionOccuredOnBefore(TContext context) { return context[ExceptionVariableName] != null; }

		protected override void OnBefore(TContext context)
		{
			try
			{
				context[variableName] = beforeDelegate(context);
			}
			catch (Exception ex)
			{
				context[ExceptionVariableName] = ex;
				throw ex;
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
