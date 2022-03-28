using Routine.Core.Runtime;
using Routine.Core;
using System.Collections.Generic;

namespace Routine.Test.Engine.Stubs.DoInvokers
{
    public class Async : IDoInvoker
    {
        public VariableData InvokeDo(IObjectService testing, ReferenceData target, string operation, Dictionary<string, ParameterValueData> parameters)
            => testing.DoAsync(target, operation, parameters).WaitAndGetResult();
    }
}
