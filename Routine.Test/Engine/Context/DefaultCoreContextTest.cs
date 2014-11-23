using NUnit.Framework;
using Routine.Core.Cache;
using Routine.Engine;
using Routine.Engine.Context;
using Routine.Test.Core;
using Routine.Test.Engine.Context.Domain;

namespace Routine.Test.Engine.Context.Domain
{
	public class CachedBusiness
	{
		public string Id { get; set; }
	}
}

namespace Routine.Test.Engine.Context
{
	[TestFixture]
	public class DefaultCoreContextTest : CoreTestBase
	{
		[Test]
		public void Caches_domain_types_by_object_model_id()
		{
			ICodingStyle codingStyle = 
				BuildRoutine.CodingStyle().FromBasic()
					.AddTypes(GetType().Assembly, t => t.Namespace.StartsWith("Routine.Test.Engine.Context.Domain"))
					.TypeId.Set(c => c.By(t => t.FullName))
					.IdExtractor.Set(c => c.IdByMember(m => m.Returns<string>("Id")))
					.ObjectLocator.Set(c => c.Locator(l => l.Constant(null)))
					.ValueExtractor.Set(c => c.Value(e => e.By(obj => string.Format("{0}", obj))));

			var testing = new DefaultCoreContext(codingStyle, new DictionaryCache());

			var domainType = testing.GetDomainType(type.of<CachedBusiness>());

			var expected = testing.GetDomainType(domainType.Id);
			var actual = testing.GetDomainType(domainType.Id);

			Assert.AreSame(expected, actual);
		}
	}
}