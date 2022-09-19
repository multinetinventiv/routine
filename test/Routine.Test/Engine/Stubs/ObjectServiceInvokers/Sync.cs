using Routine.Core;

namespace Routine.Test.Engine.Stubs.ObjectServiceInvokers;

public class Sync : IObjectServiceInvoker
{
    public VariableData InvokeDo(IObjectService testing, ReferenceData target, string operation, Dictionary<string, ParameterValueData> parameters)
        => testing.Do(target, operation, parameters);

    public ObjectData InvokeGet(IObjectService testing, ReferenceData target)
        => testing.Get(target);
}
