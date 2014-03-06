namespace Routine.Core.Reflection.Optimization
{
	public class ReflectionMethodInvoker : IMethodInvoker
	{
		private readonly System.Reflection.MethodBase method;

		public ReflectionMethodInvoker(System.Reflection.MethodBase method)
		{
			this.method = method;
		}

		public object Invoke(object target, params object[] args)
		{
			if(method.IsConstructor)
			{
				var ctor = method as System.Reflection.ConstructorInfo;

				return ctor.Invoke(args);
			}

			return method.Invoke(target, args);
		}
	}
}
