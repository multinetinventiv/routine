using System.Web;
using Routine.Core.Rest;

namespace Routine.Service.HandlerActions
{
	public class ConfigurationHandlerAction : HandlerActionBase
	{
		public ConfigurationHandlerAction(IServiceContext serviceContext, IJsonSerializer jsonSerializer, HttpContextBase httpContext)
			: base(serviceContext, jsonSerializer, httpContext) { }

		public override void WriteResponse()
		{
			WriteJsonResponse(new
			{
				url = UrlBase,
				requestHeaders = ServiceContext.ServiceConfiguration.GetRequestHeaders(),
				responseHeaders = ServiceContext.ServiceConfiguration.GetResponseHeaders()
			});
		}
	}
}