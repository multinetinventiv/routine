using System.Web;
using Routine.Core.Rest;

namespace Routine.Service.RequestHandlers
{
	public class ApplicationModelRequestHandler : RequestHandlerBase
	{
		public ApplicationModelRequestHandler(IServiceContext serviceContext, IJsonSerializer jsonSerializer, HttpContextBase httpContext)
			: base(serviceContext, jsonSerializer, httpContext) { }

		public override void WriteResponse()
		{
			WriteJsonResponse(ServiceContext.ObjectService.ApplicationModel);
		}
	}
}