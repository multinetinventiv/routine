using Microsoft.AspNetCore.Http;
using System.Web;

namespace Routine.Service.RequestHandlers.Helper
{
    public static class HttpResponseExtensions
    {
        public static void SetStatus(this HttpResponse source, int statusCode, string statusDescription)
        {
            source.StatusCode = statusCode;
            source.Headers["X-Status-Description"] = HttpUtility.UrlEncode(statusDescription);
        }
    }
}
