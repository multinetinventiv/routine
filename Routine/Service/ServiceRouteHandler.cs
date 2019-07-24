using System.Web;
using System.Web.Routing;
using Routine.Core.Rest;

namespace Routine.Service
{
	public class ServiceRouteHandler : IRouteHandler
	{
		private readonly IServiceContext serviceContext;
		private readonly IJsonSerializer jsonSerializer;

		public ServiceRouteHandler(IServiceContext serviceContext, IJsonSerializer jsonSerializer)
		{
			this.serviceContext = serviceContext;
			this.jsonSerializer = jsonSerializer;
		}

		public IHttpHandler GetHttpHandler(RequestContext requestContext)
		{
			return new ServiceHttpHandler(serviceContext, jsonSerializer);
		}
	}
}
