using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Routine.Core.Rest;
using Routine.Core;
using Routine.Engine.Context;
using Routine.Service.RequestHandlers.Helper;
using System.Net;
using System.Text;

namespace Routine.Service.RequestHandlers;

public abstract class RequestHandlerBase : IRequestHandler
{
    #region Constants

    private const int CACHE_DURATION = 60;
    private const int BUFFER_SIZE = 0x1000;
    protected const string JSON_CONTENT_TYPE = "application/json";
    protected static readonly Encoding DEFAULT_CONTENT_ENCODING = Encoding.UTF8;
    private static readonly object MODEL_INDEX_LOCK = new();

    #endregion

    #region Construction

    protected IServiceContext ServiceContext { get; }
    protected IJsonSerializer JsonSerializer { get; }
    protected IHttpContextAccessor HttpContextAccessor { get; }

    protected RequestHandlerBase(IServiceContext serviceContext, IJsonSerializer jsonSerializer, IHttpContextAccessor httpContextAccessor)
    {
        ServiceContext = serviceContext;
        JsonSerializer = jsonSerializer;
        HttpContextAccessor = httpContextAccessor;
    }

    #endregion

    public abstract Task WriteResponse();

    protected HttpContext HttpContext => HttpContextAccessor.HttpContext;
    protected IQueryCollection QueryString => HttpContext.Request.Query;
    protected string UrlBase => ServiceContext.ServiceConfiguration.GetPath(string.Empty).BeforeLast('/');
    protected string TestApp => ServiceContext.ServiceConfiguration.GetTestAppPath();
    protected bool IsGet => "GET".Equals(HttpContext.Request.Method, StringComparison.InvariantCultureIgnoreCase);
    protected bool IsPost => "POST".Equals(HttpContext.Request.Method, StringComparison.InvariantCultureIgnoreCase);
    protected ApplicationModel ApplicationModel => ServiceContext.ObjectService.ApplicationModel;

    private static volatile Dictionary<string, List<ObjectModel>> _modelIndex;
    public static void ClearModelIndex()
    {
        lock (MODEL_INDEX_LOCK)
        {
            _modelIndex = null;
        }
    }

    protected Dictionary<string, List<ObjectModel>> ModelIndex
    {
        get
        {
            if (_modelIndex == null)
            {
                lock (MODEL_INDEX_LOCK)
                {
                    if (_modelIndex == null)
                    {
                        _modelIndex = BuildModelIndex();
                    }
                }
            }

            return _modelIndex;
        }
    }

    protected virtual void AddResponseCaching()
    {
        var headers = HttpContext.Response.GetTypedHeaders();
        headers.CacheControl = new CacheControlHeaderValue()
        {
            Public = true,
            MaxAge = new TimeSpan(0, CACHE_DURATION, 0)
        };
        headers.Expires = DateTime.Now.AddMinutes(CACHE_DURATION);
    }

    protected virtual void BadRequest(Exception ex)
    {
        HttpContext.Response.SetStatus(StatusCodes.Status400BadRequest,
            $"Cannot resolve parameters from request body. The exception is; {ex}"
        );
    }

    protected virtual void ModelNotFound(TypeNotFoundException ex)
    {
        HttpContext.Response.SetStatus(StatusCodes.Status404NotFound,
            $"Specified model ({ex.TypeId}) was not found in service model. The exception is; {ex}"
        );
    }

    protected virtual void MethodNotAllowed(bool allowGet)
    {
        HttpContext.Response.SetStatus(StatusCodes.Status405MethodNotAllowed,
            allowGet ? "Only GET, POST and OPTIONS are supported" : "Only POST and OPTIONS are supported"
        );
    }

    protected virtual async Task WriteFileResponse(string path)
    {
        var stream = GetStream(path);

        var sr = new StreamReader(stream);

        var fileContent = await sr.ReadToEndAsync();
        sr.Close();
        stream.Close();

        fileContent = fileContent.Replace("$urlbase$", $"/{UrlBase}");
        fileContent = fileContent.Replace("$testapp$", $"{TestApp}");

        AddResponseCaching();
        HttpContext.Response.ContentType = MimeTypeMap.GetMimeType(path.AfterLast("."));
        await HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(fileContent));
    }

    protected virtual async Task WriteFontResponse(string fileName)
    {
        var stream = GetStream($"assets/fonts/{fileName}");
        var outputStream = new MemoryStream { Position = 0 };

        await using (stream)
        {
            var buffer = new byte[BUFFER_SIZE];

            while (true)
            {
                var bytesRead = await stream.ReadAsync(buffer, 0, BUFFER_SIZE);
                if (bytesRead == 0)
                {
                    break;
                }

                outputStream.Write(buffer, 0, bytesRead);
            }
        }

        AddResponseCaching();
        HttpContext.Response.ContentType = MimeTypeMap.GetMimeType(fileName);

        await HttpContext.Response.Body.WriteAsync(outputStream.ToArray(), new CancellationTokenSource().Token);
        await HttpContext.Response.Body.FlushAsync();
    }

    protected virtual async Task WriteJsonResponse(object result, HttpStatusCode statusCode = HttpStatusCode.OK, bool clearError = false)
    {
        if (clearError)
        {
            HttpContext.Response.StatusCode = 200;
            HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = null;
        }

        HttpContext.Response.StatusCode = (int)statusCode;

        var mediaType = new MediaTypeHeaderValue(JSON_CONTENT_TYPE) { Encoding = DEFAULT_CONTENT_ENCODING };
        HttpContext.Response.ContentType = mediaType.ToString();

        if (result != null)
        {
            await HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(result)));
        }
    }

    private Dictionary<string, List<ObjectModel>> BuildModelIndex()
    {
        var result = new Dictionary<string, List<ObjectModel>>();
        var appModel = ServiceContext.ObjectService.ApplicationModel;

        foreach (var key in appModel.Model.Keys)
        {
            var shortModelId = key.AfterLast(".");
            if (!result.ContainsKey(shortModelId))
            {
                result.Add(shortModelId, new List<ObjectModel>());
            }

            result[shortModelId].Add(appModel.Model[key]);
        }

        return result;
    }

    private Stream GetStream(string path)
    {
        path = path.Replace("/", ".");
        var stream = GetType().Assembly.GetManifestResourceStream(
            GetType().Assembly.GetManifestResourceNames().Single(s => s.EndsWith(path))
        );

        if (stream == null)
        {
            throw new InvalidOperationException("Could not get manifest resource stream for test page");
        }

        return stream;
    }
}
