using System.Web;
using Routine.Core.Rest;

namespace Routine.Service.RequestHandlers
{
	public class FontsRequestHandler : RequestHandlerBase
	{
		private readonly string fileNameRouteKey;

		public FontsRequestHandler(IServiceContext serviceContext, IJsonSerializer jsonSerializer, HttpContextBase httpContext, string fileNameRouteKey)
			: base(serviceContext, jsonSerializer, httpContext)
		{
			this.fileNameRouteKey = fileNameRouteKey;
		}

		public override void WriteResponse()
		{
			WriteFontResponse($"{RouteData.Values[fileNameRouteKey]}");
		}
	}
}