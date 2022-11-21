using Routine.Core.Cache;
using Routine.Core;
using Routine.Engine.Configuration.ConventionBased;
using Routine.Engine.Context;
using Routine.Engine;
using Routine.Test.Core;

namespace Routine.Test.Engine;

public abstract class ObjectServiceTestBase : CoreTestBase
{
    protected Dictionary<string, object> _objectRepository;

    protected ICoreContext _ctx;
    protected ConventionBasedCodingStyle _codingStyle;

    protected ObjectService _testing;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        _objectRepository = new();

        _codingStyle = BuildRoutine.CodingStyle().FromBasic()
            .AddTypes(GetType().Assembly, t => t.IsPublic && t.Namespace != null && t.Namespace.StartsWith(RootNamespace))

            .Initializers.Add(c => c.Constructors().When(t => t.IsValueType && t.Namespace?.StartsWith(RootNamespace) == true))
            .Datas.Add(c => c.PublicProperties(m => !m.IsInherited()).When(t => t.Namespace?.StartsWith(RootNamespace) == true))
            .Operations.Add(c => c.PublicMethods(m => !m.IsInherited()).When(t => t.Namespace?.StartsWith(RootNamespace) == true))

            .IdExtractor.Set(c => c.IdByProperty(p => p.Returns<string>("Id")).When(t => t.Namespace != null && t.Namespace.StartsWith(RootNamespace)))
            .Locator.Set(c => c.Locator(l => l.SingleBy(id => _objectRepository[id])).When(t => t.Namespace != null && t.Namespace.StartsWith(RootNamespace) && t.Properties.Any(m => m.Returns<string>("Id"))))

            .NextLayer()
            ;

        _ctx = new DefaultCoreContext(_codingStyle);
        _testing = new(_ctx, new DictionaryCache());
    }

    protected void AddToRepository(object obj)
    {
        var _ = _testing.ApplicationModel;

        var idExtractor = _ctx.CodingStyle.GetIdExtractor(obj.GetTypeInfo());
        var id = idExtractor.GetId(obj);
        _objectRepository.Add(id, obj);
    }

    protected ReferenceData IdNull() => Id(null, null, null, true);
    protected new virtual ReferenceData Id(string id) => Id(id, DefaultModelId);
    protected new ReferenceData Id(string id, string modelId) => Id(id, modelId, modelId);
    protected new ReferenceData Id(string id, string actualModelId, string viewModelId) => Id(id, actualModelId, viewModelId, false);
    protected new ReferenceData Id(string id, string actualModelId, string viewModelId, bool isNull) =>
        isNull
            ? null
            : new ReferenceData { Id = id, ModelId = actualModelId, ViewModelId = viewModelId };

    protected abstract string DefaultModelId { get; }
    protected abstract string RootNamespace { get; }
}
