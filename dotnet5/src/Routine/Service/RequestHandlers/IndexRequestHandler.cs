using System.Web;
using Routine.Core.Rest;

namespace Routine.Service.RequestHandlers
{
    public class IndexRequestHandler : RequestHandlerBase, IIndexRequestHandler
    {
        public IndexRequestHandler(IServiceContext serviceContext, IJsonSerializer jsonSerializer, HttpContextBase httpContext)
            : base(serviceContext, jsonSerializer, httpContext) { }

        public override void WriteResponse()
        {
            WriteFileResponse("vue/index.html");
        }
    }
}