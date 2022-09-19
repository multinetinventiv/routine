using Routine.Core;

namespace Routine.Test.Engine.Stubs.ObjectServiceInvokers;

public interface IObjectServiceInvoker
{
    VariableData InvokeDo(IObjectService testing, ReferenceData target, string operation, Dictionary<string, ParameterValueData> parameters);
    ObjectData InvokeGet(IObjectService testing, ReferenceData target);
}
