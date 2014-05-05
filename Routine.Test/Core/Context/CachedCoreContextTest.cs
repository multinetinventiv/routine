using Moq;
using NUnit.Framework;
using Routine.Core;
using Routine.Core.Cache;
using Routine.Core.Context;

namespace Routine.Test.Core.Context.Domain
{
	public class CachedBusiness
	{
		public string Id{get;set;}
	}
}

namespace Routine.Test.Core.Context
{
	[TestFixture]
	public class CachedCoreContextTest : CoreTestBase
	{
		public override string[] DomainTypeRootNamespaces { get { return new[] { "Routine.Test.Core.Context.Domain" }; } }

		[Test]
		public void CachesDomainTypesByObjectModelId()
		{
			ICodingStyle codingStyle = 
				BuildRoutine.CodingStyle().FromBasic()
					.SerializeModelId.Done(s => s.SerializeBy(t => t.FullName).DeserializeBy(id => id.ToType()))
					.ExtractId.Done(e => e.ByPublicProperty(p => p.Returns<string>("Id")));

			var testing = new CachedCoreContext(codingStyle, new DictionaryCache());

			var modelId = "Routine.Test.Core.Context.Domain.CachedBusiness";

			var expected = testing.GetDomainType(modelId);
			var actual = testing.GetDomainType(modelId);

			Assert.AreSame(expected, actual);
		}
	}
}

