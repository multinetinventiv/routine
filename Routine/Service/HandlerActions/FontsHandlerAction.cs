using System.Web;
using Routine.Core.Rest;

namespace Routine.Service.HandlerActions
{
	public class FontsHandlerAction : HandlerActionBase
	{
		public FontsHandlerAction(IServiceContext serviceContext, IJsonSerializer jsonSerializer, HttpContextBase httpContext)
			: base(serviceContext, jsonSerializer, httpContext) { }

		public override void WriteResponse()
		{
			WriteFontResponse($"{RouteData.Values["fileName"]}");
		}
	}
}