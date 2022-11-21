using Routine.Core;

namespace Routine.Engine.Context;

public class DefaultCoreContext : ICoreContext
{
    private readonly ICodingStyle _codingStyle;

    private readonly object _domainTypesLock = new();
    private volatile Dictionary<string, DomainType> _domainTypes;

    private Dictionary<string, DomainType> DomainTypes
    {
        get => _domainTypes ?? throw new InvalidOperationException("Context was not initialized yet");
        set => _domainTypes = value;
    }

    public DefaultCoreContext(ICodingStyle codingStyle)
    {
        _codingStyle = codingStyle;
    }

    public ICodingStyle CodingStyle => _codingStyle;

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

    private void BuildDomainTypes()
    {
        lock (_domainTypesLock)
        {
            DomainTypes = new();
            foreach (var type in CodingStyle.GetTypes())
            {
                var domainType = new DomainType(this, type);

                DomainTypes[domainType.Id] = domainType;
            }

            foreach (var domainType in DomainTypes.Values.ToList())
            {
                domainType.Initialize();
            }
        }
    }

    public async Task<object> GetObjectAsync(ReferenceData referenceData) =>
        await GetActualDomainType(referenceData).LocateAsync(referenceData);

    public async Task<DomainObject> GetDomainObjectAsync(ReferenceData referenceData)
    {
        var (actualDomainType, viewDomainType) = GetActualAndViewDomainType(referenceData);

        return new(await actualDomainType.LocateAsync(referenceData), actualDomainType, viewDomainType);
    }

    public DomainObject CreateDomainObject(object @object, DomainType viewDomainType)
    {
        var type = @object == null ? null : CodingStyle.GetType(@object);
        var actualDomainType = GetDomainType(type);

        viewDomainType ??= actualDomainType;

        return new(@object, actualDomainType, viewDomainType);
    }

    private (DomainType, DomainType) GetActualAndViewDomainType(ReferenceData referenceData)
    {
        var actualDomainType = GetActualDomainType(referenceData);
        var viewDomainType = referenceData.ModelId != referenceData.ViewModelId
            ? GetDomainType(referenceData.ViewModelId)
            : actualDomainType;

        return (actualDomainType, viewDomainType);
    }

    private DomainType GetActualDomainType(ReferenceData referenceData) => referenceData == null
        ? GetDomainType((IType)null)
        : GetDomainType(referenceData.ModelId);

    List<DomainType> ICoreContext.DomainTypes => DomainTypes.Values.ToList();
    void ICoreContext.BuildDomainTypes() => BuildDomainTypes();
}
