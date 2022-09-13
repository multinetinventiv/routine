using Routine.Core;

namespace Routine.Engine;

public class DomainObject
{
    private readonly DomainType actualDomainType;
    private readonly DomainType viewDomainType;
    private readonly object actualTarget;
    private readonly object viewTarget;

    public DomainObject(object target, DomainType actualDomainType, DomainType viewDomainType)
    {
        this.actualDomainType = actualDomainType ?? throw new ArgumentNullException(nameof(actualDomainType));
        this.viewDomainType = viewDomainType ?? throw new ArgumentNullException(nameof(viewDomainType));

        actualTarget = target;
        viewTarget = actualDomainType.Convert(target, viewDomainType);
    }

    public string GetId() => actualDomainType.IdExtractor == null
        ? string.Empty
        : actualDomainType.IdExtractor.GetId(actualTarget);

    public string GetDisplay()
    {
        if (viewDomainType.IsValueModel)
        {
            return GetId();
        }

        if (actualTarget == null || viewDomainType.ValueExtractor == null)
        {
            return actualDomainType.ValueExtractor == null
                ? string.Empty
                : actualDomainType.ValueExtractor.GetValue(actualTarget);
        }

        return viewDomainType.ValueExtractor.GetValue(viewTarget);
    }

    public ReferenceData GetReferenceData()
    {
        if (actualTarget == null)
        {
            return null;
        }

        var result = new ReferenceData
        {
            ModelId = actualDomainType.Id,
            ViewModelId = viewDomainType.Id,
            Id = GetId()
        };

        return result;
    }

    public ObjectData GetObjectData(bool eager) => GetObjectData(Constants.FIRST_DEPTH, eager);
    internal ObjectData GetObjectData(int currentDepth, bool eager)
    {
        if (actualTarget == null)
        {
            return null;
        }

        var result = new ObjectData
        {
            Id = GetId(),
            ModelId = actualDomainType.Id,
            Display = GetDisplay()
        };

        if (actualTarget == null) { return result; }
        if (!eager && actualDomainType.Locatable) { return result; }

        if (currentDepth > actualDomainType.MaxFetchDepth)
        {
            throw new MaxFetchDepthExceededException(actualDomainType.MaxFetchDepth, actualTarget);
        }

        foreach (var data in viewDomainType.Datas)
        {
            result.Data.Add(data.Name, data.CreateData(viewTarget, currentDepth + 1));
        }

        return result;
    }

    public VariableData GetData(string dataName)
    {
        if (!viewDomainType.Data.TryGetValue(dataName, out var data))
        {
            throw new DataDoesNotExistException(viewDomainType.Id, dataName);
        }

        return data.CreateData(viewTarget, true);
    }

    public VariableData Perform(string operationName, Dictionary<string, ParameterValueData> parameterValues) => Operation(operationName).Perform(viewTarget, parameterValues);
    public async Task<VariableData> PerformAsync(string operationName, Dictionary<string, ParameterValueData> parameterValues) => await Operation(operationName).PerformAsync(viewTarget, parameterValues);

    private DomainOperation Operation(string name)
    {
        if (!viewDomainType.Operation.TryGetValue(name, out var operation))
        {
            throw new OperationDoesNotExistException(viewDomainType.Id, name);
        }

        return operation;
    }
}
