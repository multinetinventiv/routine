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

    public class LaterAddedType
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

        [Test]
        public void Refreshes_added_types_within_coding_style_when_a_new_type_is_added_so_that_old_IType_instances_are_not_stored()
        {
            var codingStyle =
                BuildRoutine.CodingStyle().FromBasic()
                    .AddTypes(typeof(CachedBusiness))
                    .IdExtractor.Set(c => c.IdByProperty(m => m.Returns<string>("Id")))
                    .Locator.Set(c => c.Locator(l => l.Constant(null)))
                    .ValueExtractor.Set(c => c.Value(e => e.By(obj => string.Format("{0}", obj))));

            var testing = new DefaultCoreContext(codingStyle, new DictionaryCache());

            testing.GetDomainTypes();

            var expected = testing.GetDomainType(type.of<CachedBusiness>());

            codingStyle.AddTypes(typeof(LaterAddedType));

            testing = new DefaultCoreContext(codingStyle, new DictionaryCache());

            testing.GetDomainTypes();

            var actual = testing.GetDomainType(type.of<CachedBusiness>());

            Assert.AreNotSame(expected.Type, actual.Type);
        }
    }
}