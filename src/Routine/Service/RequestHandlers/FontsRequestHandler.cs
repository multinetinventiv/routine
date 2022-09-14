using Microsoft.AspNetCore.Http;
using Routine.Core.Rest;

namespace Routine.Service.RequestHandlers;

public class FontsRequestHandler : RequestHandlerBase
{
    public FontsRequestHandler(IServiceContext serviceContext, IJsonSerializer jsonSerializer, IHttpContextAccessor httpContextAccessor)
        : base(serviceContext, jsonSerializer, httpContextAccessor) { }

    public override async Task WriteResponse() => await WriteFontResponse($"{HttpContext.Request.Path.Value.After("fonts/").BeforeLast("/f")}");
}
