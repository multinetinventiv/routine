using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Routine.Core.Rest;

namespace Routine.Service.RequestHandlers
{
	public class ConfigurationRequestHandler : RequestHandlerBase
	{
		public ConfigurationRequestHandler(IServiceContext serviceContext, IJsonSerializer jsonSerializer, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache)
			: base(serviceContext, jsonSerializer, httpContextAccessor,memoryCache) { }

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