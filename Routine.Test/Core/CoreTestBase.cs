using NUnit.Framework;

namespace Routine.Test.Core
{
	public abstract class CoreTestBase
	{
		[SetUp]
		public virtual void SetUp()
		{
			TypeInfo.SetProxyMatcher(t => t.Name.Contains("Proxy"), t => t.BaseType);
		}
	}
}

