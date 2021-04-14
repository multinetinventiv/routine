using Microsoft.AspNetCore.Http;
using Routine.Core.Rest;

namespace Routine.Service.RequestHandlers
{
    public class IndexRequestHandler : RequestHandlerBase, IIndexRequestHandler
    {
        public IndexRequestHandler(IServiceContext serviceContext, IJsonSerializer jsonSerializer, IHttpContextAccessor httpContextAccessor)
            : base(serviceContext, jsonSerializer, httpContextAccessor) { }

        public override void WriteResponse()
        {
            WriteFileResponse("vue/index.html");
        }
    }
}