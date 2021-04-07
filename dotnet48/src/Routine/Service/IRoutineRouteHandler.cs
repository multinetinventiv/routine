using System.Web.Routing;

namespace Routine.Service
{
	public interface IRoutineRouteHandler : IRouteHandler
	{
		void RegisterRoutes();
	}
}