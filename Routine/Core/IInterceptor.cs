using System;
using System.Collections.Generic;

namespace Routine.Core
{
	public interface IInterceptor<TContext>
		where TContext : InterceptionContext
	{
		void OnBefore(TContext context);
		void OnAfter(TContext context);
		void OnError(TContext context);
	}

	public class InterceptionContext
	{
		protected readonly Dictionary<string, object> data;

		public InterceptionContext()
		{
			data = new Dictionary<string, object>();
		}

		public virtual object this[string key] 
		{
			get
			{
				object result; 
				data.TryGetValue(key, out result); 
				return result; 
			} 
			set 
			{ 
				data[key] = value; 
			} 
		}

		public virtual object Result { get; set; }
		public virtual bool Canceled { get; set; }
		public virtual Exception Exception { get; set; }
		public virtual bool ExceptionHandled { get; set; }
	}
}
