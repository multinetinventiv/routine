using System.Web;
using Routine.Core.Rest;

namespace Routine.Service.HandlerActions
{
	public class EmptyHandlerAction : HandlerActionBase
	{
		public EmptyHandlerAction(IServiceContext serviceContext, IJsonSerializer jsonSerializer, HttpContextBase httpContext)
			: base(serviceContext, jsonSerializer, httpContext) { }

		public override void WriteResponse()
		{
		}
	}
}