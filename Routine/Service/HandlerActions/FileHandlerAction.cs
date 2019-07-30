using System.Web;
using Routine.Core.Rest;

namespace Routine.Service.HandlerActions
{
	public class FileHandlerAction : HandlerActionBase
	{
		public FileHandlerAction(IServiceContext serviceContext, IJsonSerializer jsonSerializer, HttpContextBase httpContext)
			: base(serviceContext, jsonSerializer, httpContext) { }

		public override void WriteResponse()
		{
			WriteFileResponse($"{QueryString["path"]}");
		}
	}
}