using Routine.Core;

namespace Routine.Test.Engine.Stubs.DoInvokers;

public interface IDoInvoker
{
    VariableData InvokeDo(IObjectService testing, ReferenceData target, string operation, Dictionary<string, ParameterValueData> parameters);
}
