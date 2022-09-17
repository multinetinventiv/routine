using Routine.Client;
using Routine.Core;

using static Routine.Client.Robject;

namespace Routine.Test.Client.Stubs.Getters;

public class Sync : IGetter
{
    public Rvariable Get(DataValue target) => target.Get();

    public void VerifyGet(Mock<IObjectService> mock) => mock.Verify(o => o.Get(It.IsAny<ReferenceData>()));
    public void VerifyGet(Mock<IObjectService> mock, Times times) => mock.Verify(o => o.Get(It.IsAny<ReferenceData>()), times);
}
