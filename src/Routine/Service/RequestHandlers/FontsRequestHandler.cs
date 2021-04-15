using Microsoft.AspNetCore.Http;
using Routine.Core.Rest;

namespace Routine.Service.RequestHandlers
{
	public class FontsRequestHandler : RequestHandlerBase
	{
		private readonly string fileNameRouteKey;

		public FontsRequestHandler(IServiceContext serviceContext, IJsonSerializer jsonSerializer, IHttpContextAccessor httpContextAccessor, string fileNameRouteKey)
			: base(serviceContext, jsonSerializer, httpContextAccessor)
		{
			this.fileNameRouteKey = fileNameRouteKey;
		}

		public override void WriteResponse()
		{
			WriteFontResponse($"{RouteData.Values[fileNameRouteKey]}");
		}
	}
}