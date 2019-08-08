using System.Web;
using Routine.Core.Rest;

namespace Routine.Service.RequestHandlers
{
	public class FileRequestHandler : RequestHandlerBase
	{
		public FileRequestHandler(IServiceContext serviceContext, IJsonSerializer jsonSerializer, HttpContextBase httpContext)
			: base(serviceContext, jsonSerializer, httpContext) { }

		public override void WriteResponse()
		{
			WriteFileResponse($"{QueryString["path"]}");
		}
	}
}