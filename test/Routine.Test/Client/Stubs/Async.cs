using System.Linq.Expressions;
using Routine.Client;
using Routine.Core;
using Routine.Core.Runtime;

using static Routine.Client.Robject;

namespace Routine.Test.Client.Stubs;

public class Async : IStubber
{
    public void Load(Robject target) => target.LoadObjectAsync().WaitAndGetResult();
    public Rvariable Get(DataValue target) => target.GetAsync().WaitAndGetResult();
    public Rvariable Perform(Robject target, string operationName, params Rvariable[] parameters) =>
        target.PerformAsync(operationName, parameters).WaitAndGetResult();

    public void SetUp(Mock<IObjectService> mock,
        ReferenceData id,
        string operation,
        Expression<Func<Dictionary<string, ParameterValueData>, bool>> match,
        VariableData result
    ) => mock.Setup(os => os.DoAsync(id, operation, It.Is(match))).ReturnsAsync(result);

    public void VerifyGet(Mock<IObjectService> mock) => mock.Verify(o => o.GetAsync(It.IsAny<ReferenceData>()));
    public void VerifyGet(Mock<IObjectService> mock, Times times) => mock.Verify(o => o.GetAsync(It.IsAny<ReferenceData>()), times);

}
