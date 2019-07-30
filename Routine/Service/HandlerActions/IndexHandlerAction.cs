using System.Web;
using Routine.Core.Rest;

namespace Routine.Service.HandlerActions
{
	public class IndexHandlerAction : HandlerActionBase, IIndexHandlerAction
	{
		public IndexHandlerAction(IServiceContext serviceContext, IJsonSerializer jsonSerializer, HttpContextBase httpContext)
			: base(serviceContext, jsonSerializer, httpContext) { }

		public override void WriteResponse()
		{
			WriteFileResponse("app/application/index.html");
		}
	}
}