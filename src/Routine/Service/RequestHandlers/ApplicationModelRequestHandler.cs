
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Routine.Core.Rest;

namespace Routine.Service.RequestHandlers
{
	public class ApplicationModelRequestHandler : RequestHandlerBase
	{
		public ApplicationModelRequestHandler(IServiceContext serviceContext, IJsonSerializer jsonSerializer, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache)
			: base(serviceContext, jsonSerializer, httpContextAccessor, memoryCache) { }

		public override void WriteResponse()
		{
			WriteJsonResponse(ServiceContext.ObjectService.ApplicationModel);
		}
	}
}