using Microsoft.AspNetCore.Http;
using Routine.Core.Rest;
using System.Threading.Tasks;

namespace Routine.Service.RequestHandlers
{
    public class ConfigurationRequestHandler : RequestHandlerBase
	{
		public ConfigurationRequestHandler(IServiceContext serviceContext, IJsonSerializer jsonSerializer, IHttpContextAccessor httpContextAccessor)
			: base(serviceContext, jsonSerializer, httpContextAccessor) { }

		public override async Task WriteResponse()
		{
			await WriteJsonResponse(new
			{
				url = UrlBase,
				requestHeaders = ServiceContext.ServiceConfiguration.GetRequestHeaders(),
				responseHeaders = ServiceContext.ServiceConfiguration.GetResponseHeaders()
			});
		}
	}
}