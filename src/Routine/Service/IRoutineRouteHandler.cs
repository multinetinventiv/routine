using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Routine.Service
{
	public interface IRoutineRouteHandler : IRouteHandler
	{
		void RegisterRoutes(IApplicationBuilder applicationBuilder);
	}
}