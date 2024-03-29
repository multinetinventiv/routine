using Routine.Engine;
using Routine.Engine.Context;
using Routine.Engine.Reflection;
using Routine.Test.Core;
using Routine.Test.Engine.Context.Domain;

namespace Routine.Test.Engine.Context;

[TestFixture]
public class DefaultCoreContextTest : CoreTestBase
{
    private ICoreContext _testing;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        var codingStyle = BuildRoutine.CodingStyle().FromBasic()
            .AddTypes(GetType().Assembly, t => !string.IsNullOrEmpty(t.Namespace) && t.Namespace.StartsWith("Routine.Test.Engine.Context.Domain"))
            .IdExtractor.Set(c => c.IdByProperty(m => m.Returns<string>("Id")))
            .Locator.Set(c => c.Locator(l => l.Constant(null)))
            .ValueExtractor.Set(c => c.Value(e => e.By(obj => $"{obj}")));

        _testing = new DefaultCoreContext(codingStyle);

        TypeInfo.Clear();
    }

    [Test]
    public void Cannot_access_a_domain_type_before_context_is_initialized()
    {
        Assert.That(() => { var _ = _testing.DomainTypes; }, Throws.TypeOf<InvalidOperationException>());
        Assert.That(() => _testing.GetDomainType(type.of<CachedBusiness>()), Throws.TypeOf<InvalidOperationException>());
    }

    [Test]
    public void Caches_domain_types_by_object_model_id()
    {
        _testing.BuildDomainTypes();

        var domainType = _testing.GetDomainType(type.of<CachedBusiness>());

        var expected = _testing.GetDomainType(domainType.Id);
        var actual = _testing.GetDomainType(domainType.Id);

        Assert.That(actual, Is.SameAs(expected));
    }

    [Test]
    public void Build_domain_types_rebuilds_everytime_it_is_called()
    {
        _testing.BuildDomainTypes();
        var oldDomainType = _testing.GetDomainType(type.of<CachedBusiness>());

        _testing.BuildDomainTypes();
        var newDomainType = _testing.GetDomainType(type.of<CachedBusiness>());

        Assert.That(newDomainType, Is.Not.SameAs(oldDomainType));
    }

    [Test]
    public void Adding_a_type_later_on_is_reflected_over_existing_proxy_types_of_existing_members()
    {
        var codingStyle = BuildRoutine.CodingStyle().FromBasic().AddTypes(typeof(CachedBusiness));
        var proxyOverAProperty = (ProxyTypeInfo)type.of<CachedBusiness>().GetProperty(nameof(CachedBusiness.LaterAddedType)).PropertyType;

        Assert.That(proxyOverAProperty.Real, Is.InstanceOf<ReflectedTypeInfo>());

        codingStyle.AddTypes(typeof(LaterAddedType));

        Assert.That(proxyOverAProperty.Real, Is.InstanceOf<OptimizedTypeInfo>());
    }
}
