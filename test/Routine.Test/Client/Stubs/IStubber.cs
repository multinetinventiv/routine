using System.Linq.Expressions;
using Routine.Client;
using Routine.Core;

using static Routine.Client.Robject;

namespace Routine.Test.Client.Stubs;

public interface IStubber
{
    void Load(Robject target);
    Rvariable Get(DataValue target);
    Rvariable Perform(Robject target, string operationName, params Rvariable[] parameters);

    public void SetUp(Mock<IObjectService> mock, ReferenceData id, string operation, VariableData result) => SetUp(mock, id, operation, parameters => true, result);
    void SetUp(Mock<IObjectService> mock, ReferenceData id, string operation, Expression<Func<Dictionary<string, ParameterValueData>, bool>> match, VariableData result);

    void VerifyGet(Mock<IObjectService> mock);
    void VerifyGet(Mock<IObjectService> mock, Times times);
}
