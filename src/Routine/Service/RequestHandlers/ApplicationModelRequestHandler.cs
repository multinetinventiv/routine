using Microsoft.AspNetCore.Http;
using Routine.Core.Rest;

namespace Routine.Service.RequestHandlers;

public class ApplicationModelRequestHandler : RequestHandlerBase
{
    public ApplicationModelRequestHandler(IServiceContext serviceContext, IJsonSerializer jsonSerializer, IHttpContextAccessor httpContextAccessor)
        : base(serviceContext, jsonSerializer, httpContextAccessor) { }

    public override async Task WriteResponse() => await WriteJsonResponse(ServiceContext.ObjectService.ApplicationModel);
}
