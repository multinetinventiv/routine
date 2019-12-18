namespace Routine.Core.Reflection
{
	public interface IMethodInvoker
	{
		object Invoke(object target, params object[] args);
	}
}

