using System.Linq.Expressions;
using Routine.Client;
using Routine.Core;

using static Routine.Client.Robject;

namespace Routine.Test.Client.Stubs;

public class Sync : IStubber
{
    public void Load(Robject target) => target.LoadObject();
    public Rvariable Get(DataValue target) => target.Get();
    public Rvariable Perform(Robject target, string operationName, params Rvariable[] parameters) =>
        target.Perform(operationName, parameters);

    public void SetUp(Mock<IObjectService> mock,
        ReferenceData id,
        string operation,
        Expression<Func<Dictionary<string, ParameterValueData>, bool>> match,
        VariableData result
    ) => mock.Setup(os => os.Do(id, operation, It.Is(match))).Returns(result);

    public void VerifyGet(Mock<IObjectService> mock) => mock.Verify(o => o.Get(It.IsAny<ReferenceData>()));
    public void VerifyGet(Mock<IObjectService> mock, Times times) => mock.Verify(o => o.Get(It.IsAny<ReferenceData>()), times);
}
