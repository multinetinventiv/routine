using Microsoft.AspNetCore.Http;
using Routine.Core.Rest;
using Routine.Engine.Context;
using Routine.Service.RequestHandlers.Exceptions;
using System.Web;

namespace Routine.Service.RequestHandlers;

public abstract class ObjectServiceRequestHandlerBase : RequestHandlerBase
{
    protected ObjectServiceRequestHandlerBase(IServiceContext serviceContext, IJsonSerializer jsonSerializer, IHttpContextAccessor httpContextAccessor)
        : base(serviceContext, jsonSerializer, httpContextAccessor) { }

    protected abstract bool AllowGet { get; }
    protected abstract Task<object> Process();

    public sealed override async Task WriteResponse()
    {
        if (!IsPost && !IsGet) { MethodNotAllowed(AllowGet); return; }
        if (IsGet && !AllowGet) { MethodNotAllowed(false); return; }

        var requestHeaders = HttpContext.Request.Headers.Keys
            .ToDictionary(key => key, key => HttpUtility.HtmlDecode(HttpContext.Request.Headers[key]));

        foreach (var processor in ServiceContext.ServiceConfiguration.GetRequestHeaderProcessors())
        {
            processor.Process(requestHeaders);
        }

        try
        {
            var response = await Process();

            foreach (var responseHeader in ServiceContext.ServiceConfiguration.GetResponseHeaders())
            {
                var responseHeaderValue = ServiceContext.ServiceConfiguration.GetResponseHeaderValue(responseHeader);
                if (!string.IsNullOrEmpty(responseHeaderValue))
                {
                    HttpContext.Response.Headers.Add(responseHeader, HttpUtility.UrlEncode(responseHeaderValue));
                }
            }

            await WriteJsonResponse(response);
        }
        catch (TypeNotFoundException ex)
        {
            ModelNotFound(ex);
        }
        catch (BadRequestException ex)
        {
            BadRequest(ex.InnerException);
        }
        catch (Exception ex)
        {
            await WriteJsonResponse(ServiceContext.ServiceConfiguration.GetExceptionResult(ex), clearError: true);
        }
    }
}
