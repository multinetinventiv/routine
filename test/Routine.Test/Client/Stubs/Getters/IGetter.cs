using Routine.Client;
using Routine.Core;

using static Routine.Client.Robject;

namespace Routine.Test.Client.Stubs.Getters;

public interface IGetter
{
    Rvariable Get(DataValue target);

    void VerifyGet(Mock<IObjectService> mock);
    void VerifyGet(Mock<IObjectService> mock, Times times);
}
