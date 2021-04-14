using Microsoft.AspNetCore.Http;
using Routine.Core.Rest;

namespace Routine.Service.RequestHandlers
{
	public class ConfigurationRequestHandler : RequestHandlerBase
	{
		public ConfigurationRequestHandler(IServiceContext serviceContext, IJsonSerializer jsonSerializer, IHttpContextAccessor httpContextAccessor)
			: base(serviceContext, jsonSerializer, httpContextAccessor) { }

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