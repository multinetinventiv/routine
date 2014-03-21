using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Routine.Core.Interceptor
{
	public class Interception<TContext, TResult>
	{
		private IInterceptor<Interception<TContext, TResult>> interceptor;

		public Interception(IInterceptor<Interception<TContext, TResult>> interceptor, TContext context)
		{
			this.interceptor = interceptor;

			Context = context;
		}

		public TContext Context { get; set; }
		public TResult Result { get; set; }
		public Exception Exception { get; set; }
		public bool ExceptionHandled { get; set; }

		internal TResult Do(Func<Interception<TContext, TResult>, TResult> operation)
		{
			try
			{
				interceptor.OnBefore(this);

				Result = operation(this);

				interceptor.OnAfter(this);
			}
			catch (Exception ex)
			{
				Exception = ex;
				interceptor.OnError(this);
				if (!ExceptionHandled)
				{
					throw Exception;
				}
			}

			return Result;
		}
	}
}
