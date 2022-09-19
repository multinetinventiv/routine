using Routine.Core.Runtime;
using Routine.Core;

namespace Routine.Test.Engine.Stubs.ObjectServiceInvokers;

public class Async : IObjectServiceInvoker
{
    public VariableData InvokeDo(IObjectService testing, ReferenceData target, string operation, Dictionary<string, ParameterValueData> parameters)
        => testing.DoAsync(target, operation, parameters).WaitAndGetResult();

    public ObjectData InvokeGet(IObjectService testing, ReferenceData target)
        => testing.GetAsync(target).WaitAndGetResult();
}
