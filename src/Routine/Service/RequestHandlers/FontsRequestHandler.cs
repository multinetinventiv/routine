using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Routine.Core.Rest;

namespace Routine.Service.RequestHandlers
{
	public class FontsRequestHandler : RequestHandlerBase
	{
		private readonly string fileNameRouteKey;

		public FontsRequestHandler(IServiceContext serviceContext, IJsonSerializer jsonSerializer, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, string fileNameRouteKey)
			: base(serviceContext, jsonSerializer, httpContextAccessor,memoryCache)
		{
			this.fileNameRouteKey = fileNameRouteKey;
		}

		public override void WriteResponse()
		{
			WriteFontResponse($"{RouteData.Values[fileNameRouteKey]}");
		}
	}
}