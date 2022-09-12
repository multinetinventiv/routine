using Routine.Core;
using System.Collections.Generic;

namespace Routine.Test.Engine.Stubs.DoInvokers;

public interface IDoInvoker
{
    VariableData InvokeDo(IObjectService testing, ReferenceData target, string operation, Dictionary<string, ParameterValueData> parameters);
}
