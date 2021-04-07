using System;
using System.Linq;
using System.Web;
using Routine.Core.Rest;
using Routine.Engine.Context;
using Routine.Service.RequestHandlers.Exceptions;

namespace Routine.Service.RequestHandlers
{
	public abstract class ObjectServiceRequestHandlerBase : RequestHandlerBase
	{
		protected ObjectServiceRequestHandlerBase(IServiceContext serviceContext, IJsonSerializer jsonSerializer, HttpContextBase httpContext)
			: base(serviceContext, jsonSerializer, httpContext) { }

		protected abstract bool AllowGet { get; }
		protected abstract void Process();

		public sealed override void WriteResponse()
		{
			if (!IsPost && !IsGet) { MethodNotAllowed(AllowGet); return; }
			if (IsGet && !AllowGet) { MethodNotAllowed(false); return; }

			ProcessRequestHeaders();

			try
			{
				Process();
			}
			catch (TypeNotFoundException ex)
			{
				ModelNotFound(ex);

				return;
			}
			catch (BadRequestException ex)
			{
				BadRequest(ex.InnerException);

				return;
			}
			catch (Exception ex)
			{
				WriteJsonResponse(ServiceContext.ServiceConfiguration.GetExceptionResult(ex), clearError: true);

				return;
			}

			AddResponseHeaders();
		}

		private void ProcessRequestHeaders()
		{
			var requestHeaders = HttpContext.Request.Headers.AllKeys
				.ToDictionary(key => key, key => HttpUtility.HtmlDecode(HttpContext.Request.Headers[key]));

			foreach (var processor in ServiceContext.ServiceConfiguration.GetRequestHeaderProcessors())
			{
				processor.Process(requestHeaders);
			}
		}

		private void AddResponseHeaders()
		{
			foreach (var responseHeader in ServiceContext.ServiceConfiguration.GetResponseHeaders())
			{
				var responseHeaderValue = ServiceContext.ServiceConfiguration.GetResponseHeaderValue(responseHeader);
				if (!string.IsNullOrEmpty(responseHeaderValue))
				{
					HttpContext.Response.Headers.Add(responseHeader, HttpUtility.UrlEncode(responseHeaderValue));
				}
			}
		}
	}
}