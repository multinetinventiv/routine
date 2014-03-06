using NUnit.Framework;

namespace Routine.Test.Core
{
	public abstract class CoreTestBase
	{
		public abstract string[] DomainTypeRootNamespaces{get;}

		[SetUp]
		public virtual void SetUp()
		{
			TypeInfo.AddDomainTypeRootNamespace(DomainTypeRootNamespaces);
		}
	}
}

