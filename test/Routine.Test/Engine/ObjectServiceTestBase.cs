using NUnit.Framework;
using Routine.Core.Cache;
using Routine.Core;
using Routine.Engine.Configuration.ConventionBased;
using Routine.Engine.Context;
using Routine.Engine;
using Routine.Test.Core;
using System.Collections.Generic;
using System.Linq;

namespace Routine.Test.Engine;

public abstract class ObjectServiceTestBase : CoreTestBase
{
    protected Dictionary<string, object> objectRepository;

    protected ICoreContext ctx;
    protected ConventionBasedCodingStyle codingStyle;

    protected ObjectService testing;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        objectRepository = new Dictionary<string, object>();

        codingStyle = BuildRoutine.CodingStyle().FromBasic()
            .AddTypes(GetType().Assembly, t => t.IsPublic && t.Namespace != null && t.Namespace.StartsWith(RootNamespace))

            .Initializers.Add(c => c.Constructors().When(t => t.IsValueType && t.Namespace?.StartsWith(RootNamespace) == true))
            .Datas.Add(c => c.PublicProperties(m => !m.IsInherited()).When(t => t.Namespace?.StartsWith(RootNamespace) == true))
            .Operations.Add(c => c.PublicMethods(m => !m.IsInherited()).When(t => t.Namespace?.StartsWith(RootNamespace) == true))

            .IdExtractor.Set(c => c.IdByProperty(p => p.Returns<string>("Id")).When(t => t.Namespace != null && t.Namespace.StartsWith(RootNamespace)))
            .Locator.Set(c => c.Locator(l => l.SingleBy(id => objectRepository[id])).When(t => t.Namespace != null && t.Namespace.StartsWith(RootNamespace) && t.Properties.Any(m => m.Returns<string>("Id"))))

            .NextLayer()
            ;

        var cache = new DictionaryCache();
        ctx = new DefaultCoreContext(codingStyle, cache);

        testing = new ObjectService(ctx, cache);
    }

    protected void AddToRepository(object obj)
    {
        var _ = testing.ApplicationModel;

        var idExtractor = ctx.CodingStyle.GetIdExtractor(obj.GetTypeInfo());
        var id = idExtractor.GetId(obj);
        objectRepository.Add(id, obj);
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
