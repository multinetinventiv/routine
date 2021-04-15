namespace Routine.Core.Reflection
{
	public static class SystemReflectionFacadeExtensions
	{
		public static IMethodInvoker CreateInvoker(this System.Reflection.MethodBase source)
		{
			return new ProxyMethodInvoker(source);
		}
	}

	internal class ProxyMethodInvoker : IMethodInvoker
	{
		private readonly System.Reflection.MethodBase method;

		public ProxyMethodInvoker(System.Reflection.MethodBase method)
		{
			this.method = method;

			// ReflectionOptimizer.AddToOptimizeList(method);
		}

		private IMethodInvoker real;
		public IMethodInvoker Real
		{
			get
			{
				if (real == null)
				{
					//todo: burasi da duzenlenmeli
					//  real = ReflectionOptimizer.CreateInvoker(method);
				}

				return real;
			}
		}

		public object Invoke(object target, params object[] args)
		{
			return Real.Invoke(target, args);
		}
	}
}