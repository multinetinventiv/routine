
using Microsoft.AspNetCore.Http;
using Routine.Core.Rest;

namespace Routine.Service.RequestHandlers
{
	public class ApplicationModelRequestHandler : RequestHandlerBase
	{
		public ApplicationModelRequestHandler(IServiceContext serviceContext, IJsonSerializer jsonSerializer, IHttpContextAccessor httpContextAccessor)
			: base(serviceContext, jsonSerializer, httpContextAccessor) { }

		public override void WriteResponse()
		{
			WriteJsonResponse(ServiceContext.ObjectService.ApplicationModel);
		}
	}
}