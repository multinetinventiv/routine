using System.Web;
using System.Web.Routing;

namespace Routine.Service
{
	public class ServiceRouteHandler: IRouteHandler
	{
		private readonly IServiceContext context;

		public ServiceRouteHandler(IServiceContext context)
		{
			this.context = context;
		}
		public IHttpHandler GetHttpHandler(RequestContext requestContext)
		{
			return new ServiceHttpHandler(context);
		}
	}
}
