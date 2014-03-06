using System;

namespace Routine.Core.Reflection.Optimization
{
	public interface IMethodInvoker
	{
		object Invoke(object target, params object[] args);
	}
}

