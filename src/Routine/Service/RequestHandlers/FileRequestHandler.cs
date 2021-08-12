using Microsoft.AspNetCore.Http;
using Routine.Core.Rest;
using System.Threading.Tasks;

namespace Routine.Service.RequestHandlers
{
    public class FileRequestHandler : RequestHandlerBase
	{
		public FileRequestHandler(IServiceContext serviceContext, IJsonSerializer jsonSerializer, IHttpContextAccessor httpContextAccessor)
			: base(serviceContext, jsonSerializer, httpContextAccessor) { }

		public override async Task WriteResponse()
		{
			await WriteFileResponse($"{QueryString["path"]}");
		}
	}
}