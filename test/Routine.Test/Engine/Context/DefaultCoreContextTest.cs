using Routine.Core.Cache;
using Routine.Engine.Context;
using Routine.Engine;
using Routine.Test.Core;
using Routine.Test.Engine.Context.Domain;

namespace Routine.Test.Engine.Context;

[TestFixture]
public class DefaultCoreContextTest : CoreTestBase
{
    [Test]
    public void Caches_domain_types_by_object_model_id()
    {
        ICodingStyle codingStyle =
            BuildRoutine.CodingStyle().FromBasic()
                .AddTypes(GetType().Assembly, t => !string.IsNullOrEmpty(t.Namespace) && t.Namespace.StartsWith("Routine.Test.Engine.Context.Domain"))
                .IdExtractor.Set(c => c.IdByProperty(m => m.Returns<string>("Id")))
                .Locator.Set(c => c.Locator(l => l.Constant(null)))
                .ValueExtractor.Set(c => c.Value(e => e.By(obj => $"{obj}")));

        var testing = new DefaultCoreContext(codingStyle, new DictionaryCache());

        testing.GetDomainTypes();

        var domainType = testing.GetDomainType(type.of<CachedBusiness>());

        var expected = testing.GetDomainType(domainType.Id);
        var actual = testing.GetDomainType(domainType.Id);

        Assert.AreSame(expected, actual);
    }
}
