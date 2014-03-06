using System;
using Castle.MicroKernel;

namespace Routine.Windsor
{
	public class WindsorFactory : IFactory
	{
		private readonly IKernel kernel;

		public WindsorFactory(IKernel kernel)
		{
			this.kernel = kernel;
		}

		public T Create<T>()
		{
			return kernel.Resolve<T>();
		}

		public object Create(Type type)
		{
			return kernel.Resolve(type);
		}

	}
}

