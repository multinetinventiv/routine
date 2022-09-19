using System.Net;

namespace Routine.Core.Rest;

public class RestRequestException : Exception
{
    public HttpStatusCode? StatusCode { get; }

    public RestRequestException(HttpStatusCode? statusCode, Exception inner = default)
        : base(inner?.Message ?? $"{statusCode}", inner)
    {
        StatusCode = statusCode;
    }
}
