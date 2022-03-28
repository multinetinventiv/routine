using Routine.Core;
using Routine.Interception.Context;

namespace Routine.Interception
{
    public interface IInterceptionConfiguration
    {
        IInterceptor<InterceptionContext> GetInterceptor(InterceptionTarget target);
        IInterceptor<ServiceInterceptionContext> GetServiceInterceptor(ObjectModel objectModel, OperationModel operationModel);
    }

    public enum InterceptionTarget
    {
        ApplicationModel,
        Get,
        Do
    }
}
