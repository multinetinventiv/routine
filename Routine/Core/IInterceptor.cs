using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Routine.Core
{
	public interface IInterceptor<TContext>
	{
		void OnBefore(TContext context);
		void OnAfter(TContext context);
		void OnError(TContext context);
	}
}
