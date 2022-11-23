using Routine.Core;

namespace Routine.Engine;

public class DomainObject
{
    private readonly DomainType _actualDomainType;
    private readonly DomainType _viewDomainType;
    private readonly object _actualTarget;
    private readonly object _viewTarget;

    public DomainObject(object target, DomainType actualDomainType, DomainType viewDomainType)
    {
        _actualDomainType = actualDomainType ?? throw new ArgumentNullException(nameof(actualDomainType));
        _viewDomainType = viewDomainType ?? throw new ArgumentNullException(nameof(viewDomainType));

        _actualTarget = target;
        _viewTarget = actualDomainType.Convert(target, viewDomainType);
    }

    public string GetId() => _actualDomainType.IdExtractor == null
        ? string.Empty
        : _actualDomainType.IdExtractor.GetId(_actualTarget);

    public string GetDisplay()
    {
        if (_viewDomainType.IsValueModel)
        {
            return GetId();
        }

        if (_actualTarget == null || _viewDomainType.ValueExtractor == null)
        {
            return _actualDomainType.ValueExtractor == null
                ? string.Empty
                : _actualDomainType.ValueExtractor.GetValue(_actualTarget);
        }

        return _viewDomainType.ValueExtractor.GetValue(_viewTarget);
    }

    public ReferenceData GetReferenceData()
    {
        if (_actualTarget == null)
        {
            return null;
        }

        var result = new ReferenceData
        {
            ModelId = _actualDomainType.Id,
            ViewModelId = _viewDomainType.Id,
            Id = GetId()
        };

        return result;
    }

    public ObjectData GetObjectData(bool eager) => GetObjectData(Constants.FIRST_DEPTH, eager);
    internal ObjectData GetObjectData(int currentDepth, bool eager)
    {
        if (_actualTarget == null)
        {
            return null;
        }

        var result = new ObjectData
        {
            Id = GetId(),
            ModelId = _actualDomainType.Id,
            Display = GetDisplay()
        };

        if (_actualTarget == null) { return result; }
        if (!eager && _actualDomainType.Locatable) { return result; }

        if (currentDepth > _actualDomainType.MaxFetchDepth)
        {
            throw new MaxFetchDepthExceededException(_actualDomainType.MaxFetchDepth, _actualTarget);
        }

        foreach (var data in _viewDomainType.Datas)
        {
            result.Data.Add(data.Name, data.CreateData(_viewTarget, currentDepth + 1));
        }

        return result;
    }

    public VariableData GetData(string dataName)
    {
        if (!_viewDomainType.Data.TryGetValue(dataName, out var data))
        {
            throw new DataDoesNotExistException(_viewDomainType.Id, dataName);
        }

        return data.CreateData(_viewTarget, true);
    }

    public VariableData Perform(string operationName, Dictionary<string, ParameterValueData> parameterValues) => Operation(operationName).Perform(_viewTarget, parameterValues);
    public async Task<VariableData> PerformAsync(string operationName, Dictionary<string, ParameterValueData> parameterValues) => await Operation(operationName).PerformAsync(_viewTarget, parameterValues);

    private DomainOperation Operation(string name)
    {
        if (!_viewDomainType.Operation.TryGetValue(name, out var operation))
        {
            throw new OperationDoesNotExistException(_viewDomainType.Id, name);
        }

        return operation;
    }
}
