using Routine.Core;

namespace Routine.Test.Engine.Stubs.DoInvokers;

public class Sync : IDoInvoker
{
    public VariableData InvokeDo(IObjectService testing, ReferenceData target, string operation, Dictionary<string, ParameterValueData> parameters)
        => testing.Do(target, operation, parameters);
}
