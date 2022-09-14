using Routine.Core.Cache;
using Routine.Core;

using static Routine.Constants;

namespace Routine.Engine.Context;

public class DefaultCoreContext : ICoreContext
{
    private readonly ICodingStyle codingStyle;
    private readonly ICache cache;

    private bool initialized;

    private Dictionary<string, DomainType> DomainTypes
    {
        get => cache[CACHE_DOMAIN_TYPES] as Dictionary<string, DomainType>;
        init
        {
            lock (cache)
            {
                cache.Add(CACHE_DOMAIN_TYPES, value);
            }
        }
    }

    public DefaultCoreContext(ICodingStyle codingStyle, ICache cache)
    {
        this.codingStyle = codingStyle;
        this.cache = cache;

        DomainTypes = new Dictionary<string, DomainType>();
        initialized = false;
    }

    public ICodingStyle CodingStyle => codingStyle;

    public DomainType GetDomainType(string typeId)
    {
        if (!DomainTypes.TryGetValue(typeId, out var result))
        {
            throw new TypeNotFoundException(typeId);
        }

        return result;
    }

    public DomainType GetDomainType(IType type) =>
        GetDomainType(((ICoreContext)this).BuildTypeId(
            CodingStyle.GetModule(type),
            CodingStyle.GetName(type)
        ));

    public List<DomainType> GetDomainTypes()
    {
        if (initialized) { return DomainTypes.Values.ToList(); }

        foreach (var type in CodingStyle.GetTypes())
        {
            var domainType = new DomainType(this, type);

            try
            {
                DomainTypes.Add(domainType.Id, domainType);
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException($"{domainType.Id} was attempted to be added more than once.", ex);
            }
        }

        foreach (var domainType in DomainTypes.Values.ToList())
        {
            domainType.Initialize();
        }

        initialized = true;

        return DomainTypes.Values.ToList();
    }

    public DomainObject CreateDomainObject(object @object, DomainType viewDomainType)
    {
        var type = @object == null ? null : CodingStyle.GetType(@object);
        var actualDomainType = GetDomainType(type);

        viewDomainType ??= actualDomainType;

        return new DomainObject(@object, actualDomainType, viewDomainType);
    }

    public DomainObject CreateDomainObject(ReferenceData referenceData)
    {
        var actualDomainType = GetActualDomainType(referenceData);
        var viewDomainType = referenceData.ModelId != referenceData.ViewModelId
            ? GetDomainType(referenceData.ViewModelId)
            : actualDomainType;

        return new DomainObject(actualDomainType.Locate(referenceData), actualDomainType, viewDomainType);
    }

    private DomainType GetActualDomainType(ReferenceData aReference) => aReference == null
        ? GetDomainType((IType)null)
        : GetDomainType(aReference.ModelId);

    public object GetObject(ReferenceData aReference) => GetActualDomainType(aReference).Locate(aReference);
}

public class TypeNotFoundException : Exception
{
    public string TypeId { get; }

    public TypeNotFoundException(string typeId)
        : base(
            $"Type could not be found with given type id: '{typeId}'. Make sure type id is correct and corresponding type is configured. " +
            "This can occur when a client with old version of service model tries to connect to server with a new version of service model. " +
            "Also make sure that ObjectService.GetApplicationModel is called before any other ObjectService methods are called " +
            "(This is because domain type of the expected type should be accessed via IType before trying to access via type id).")
    {
        TypeId = typeId;
    }
}
