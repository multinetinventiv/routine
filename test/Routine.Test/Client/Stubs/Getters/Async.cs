using Routine.Client;
using Routine.Core;
using Routine.Core.Runtime;

using static Routine.Client.Robject;

namespace Routine.Test.Client.Stubs.Getters;

public class Async : IGetter
{
    public Rvariable Get(DataValue target) => target.GetAsync().WaitAndGetResult();

    public void VerifyGet(Mock<IObjectService> mock) => mock.Verify(o => o.GetAsync(It.IsAny<ReferenceData>()));
    public void VerifyGet(Mock<IObjectService> mock, Times times) => mock.Verify(o => o.GetAsync(It.IsAny<ReferenceData>()), times);
}
