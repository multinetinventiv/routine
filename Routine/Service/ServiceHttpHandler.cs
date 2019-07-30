using System.Web;
using Routine.Service.HandlerActions;

namespace Routine.Service
{
	public class ServiceHttpHandler : IHttpHandler
	{
		private readonly HandlerActionList handlerActions;

		public ServiceHttpHandler(HandlerActionList handlerActions)
		{
			this.handlerActions = handlerActions;
		}
		
		internal void ProcessRequest(HttpContextBase ctx)
		{
			handlerActions.Get(ctx).WriteResponse();
		}

		bool IHttpHandler.IsReusable => false;
		void IHttpHandler.ProcessRequest(HttpContext httpContext) { ProcessRequest(new HttpContextWrapper(httpContext)); }
	}
}
