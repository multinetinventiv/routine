using Castle.MicroKernel;
using System;

namespace Routine.Test.Common.Domain.Windsor
{
	public class WindsorDomainContext : IDomainContext
	{
		private readonly IKernel kernel;

		public WindsorDomainContext(IKernel kernel)
		{
			this.kernel = kernel;
		}

		public T New<T>()
		{
			return kernel.Resolve<T>();
		}

		public T Get<T>()
		{
			return kernel.Resolve<T>();
		}

		public object New(Type type)
		{
			return kernel.Resolve(type);
		}

		public object Get(Type type)
		{
			return kernel.Resolve(type);
		}
	}
}

