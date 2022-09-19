namespace Routine.Core;

public interface IObjectService
{
    ApplicationModel ApplicationModel { get; }

    ObjectData Get(ReferenceData reference);
    Task<ObjectData> GetAsync(ReferenceData reference);

    VariableData Do(ReferenceData target, string operation, Dictionary<string, ParameterValueData> parameters);
    Task<VariableData> DoAsync(ReferenceData target, string operation, Dictionary<string, ParameterValueData> parameters);
}
