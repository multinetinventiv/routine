// using System.Web;

// namespace Routine.Service
// {
// 	public class ProxyHttpHandler : IHttpHandler
// 	{
// 		private readonly IRequestHandler requestHandler;

// 		public ProxyHttpHandler(IRequestHandler requestHandler)
// 		{
// 			this.requestHandler = requestHandler;
// 		}

// 		public bool IsReusable => false;

// 		public void ProcessRequest(HttpContext httpContext)
// 		{
// 			requestHandler.WriteResponse();
// 		}
// 	}
// }
