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
		private Action<TContext, TVariableType> afterDelegate;
		private Action<TContext, TVariableType> errorDelegate;

		public DecoratorInterceptor(Func<TContext, TVariableType> beforeDelegate)
		{
			this.variableName = NextVariableName();
			this.beforeDelegate = beforeDelegate;

			After(obj => { });
			Error(obj => { });
		}

		public DecoratorInterceptor<TContext, TVariableType> After(Action<TVariableType> afterDelegate) { return After((ctx, obj) => afterDelegate(obj)); }
		public DecoratorInterceptor<TContext, TVariableType> After(Action<TContext, TVariableType> afterDelegate) { this.afterDelegate = afterDelegate; return this; }

		public DecoratorInterceptor<TContext, TVariableType> Error(Action<TVariableType> errorDelegate) { return Error((ctx, obj) => errorDelegate(obj)); }
		public DecoratorInterceptor<TContext, TVariableType> Error(Action<TContext, TVariableType> errorDelegate) { this.errorDelegate = errorDelegate; return this; }

		protected override void OnBefore(TContext context)
		{
			context[variableName] = beforeDelegate(context);
		}

		protected override void OnAfter(TContext context)
		{
			afterDelegate(context, (TVariableType)context[variableName]);
		}

		protected override void OnError(TContext context)
		{
			errorDelegate(context, (TVariableType)context[variableName]);
		}
	}
}
