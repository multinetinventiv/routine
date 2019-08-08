using Castle.MicroKernel;
using System;
using NHibernate;
using Routine.Test.Common;

namespace Routine.Test.Domain.Windsor
{
	internal class WindsorDomainContext : IDomainContext
	{
		private readonly IKernel kernel;

		public WindsorDomainContext(IKernel kernel)
		{
			this.kernel = kernel;
		}

		public object Resolve(Type type)
		{
			return kernel.Resolve(type);
		}

		public object Find(TypedGuid typedUid)
		{
			return kernel.Resolve<ISession>().Get(typedUid.Type, typedUid.Uid);
		}
	}
}

