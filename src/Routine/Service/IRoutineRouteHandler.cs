using Microsoft.AspNetCore.Builder;

namespace Routine.Service
{
    public interface IRoutineRouteHandler
    {
        void RegisterRoutes(IApplicationBuilder applicationBuilder);
    }
}