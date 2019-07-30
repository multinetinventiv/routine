using System.Web;
using Routine.Core.Rest;

namespace Routine.Service.HandlerActions
{
	public class ApplicationModelHandlerAction : HandlerActionBase
	{
		public ApplicationModelHandlerAction(IServiceContext serviceContext, IJsonSerializer jsonSerializer, HttpContextBase httpContext)
			: base(serviceContext, jsonSerializer, httpContext) { }

		public override void WriteResponse()
		{
			WriteJsonResponse(ServiceContext.ObjectService.ApplicationModel);
		}
	}
}