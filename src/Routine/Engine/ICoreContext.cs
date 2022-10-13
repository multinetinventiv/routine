using Routine.Core;

namespace Routine.Engine;

public interface ICoreContext
{
    ICodingStyle CodingStyle { get; }
    List<DomainType> DomainTypes { get; }

    void BuildDomainTypes();

    DomainType GetDomainType(string typeId);
    DomainType GetDomainType(IType type);

    Task<object> GetObjectAsync(ReferenceData reference);
    Task<DomainObject> GetDomainObjectAsync(ReferenceData reference);

    DomainObject CreateDomainObject(object @object, DomainType viewDomainType);

    public async Task<DomainObject> GetDomainObjectAsync(string id, string modelId) => await GetDomainObjectAsync(id, modelId, modelId);
    public async Task<DomainObject> GetDomainObjectAsync(string id, string modelId, string viewModelId) =>
        await GetDomainObjectAsync(new()
        {
            Id = id,
            ModelId = modelId,
            ViewModelId = viewModelId
        });

    public DomainObject CreateDomainObject(object @object) => CreateDomainObject(@object, null);

    internal VariableData CreateValueData(object @object, bool isList, DomainType viewDomainType, bool eager) => CreateValueData(@object, isList, viewDomainType, Constants.FIRST_DEPTH, eager);
    internal VariableData CreateValueData(object @object, bool isList, DomainType viewDomainType, int currentDepth, bool eager)
    {
        var result = new VariableData { IsList = isList };

        if (@object == null) { return result; }

        if (isList)
        {
            if (@object is not ICollection list) { return result; }

            foreach (var item in list)
            {
                result.Values.Add(CreateDomainObject(item, viewDomainType).GetObjectData(currentDepth, eager));
            }
        }
        else
        {
            result.Values.Add(CreateDomainObject(@object, viewDomainType).GetObjectData(currentDepth, eager));
        }

        return result;
    }

    internal string BuildTypeId(string module, string name) => string.IsNullOrEmpty(module) ? name : $"{module}.{name}";
}
