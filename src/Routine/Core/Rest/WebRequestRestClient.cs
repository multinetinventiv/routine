using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Routine.Core.Rest
{
    public class WebRequestRestClient : IRestClient
    {
        private const string GET = "GET";
        private const string POST = "POST";

        private readonly Func<string, WebRequest> requestFactory;

        public WebRequestRestClient() : this(_ => { }) { }
        public WebRequestRestClient(Action<WebRequest> configurer)
            : this(url =>
                {
                    var result = WebRequest.Create(url);

                    configurer(result);

                    return result;
                })
        { }
        public WebRequestRestClient(Func<string, WebRequest> requestFactory)
        {
            this.requestFactory = requestFactory;
        }

        public RestResponse Get(string url, RestRequest request) => Make(url, request, GET);
        public RestResponse Post(string url, RestRequest request) => Make(url, request, POST);

        public async Task<RestResponse> GetAsync(string url, RestRequest request) => await MakeAsync(url, request, GET);
        public async Task<RestResponse> PostAsync(string url, RestRequest request) => await MakeAsync(url, request, POST);

        private RestResponse Make(string url, RestRequest request, string method)
        {
            var req = BuildRequest(url, request, method, out var byteArray);

            if (byteArray != null)
            {
                using (var dataStream = req.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }
            }

            var webResponse = req.GetResponse();
            var rs = webResponse.GetResponseStream();

            if (rs == null) { return RestResponse.Empty; }

            string body;
            using (var reader = new StreamReader(rs))
            {
                body = reader.ReadToEnd();
            }

            return BuildResponse(webResponse, body);
        }

        private async Task<RestResponse> MakeAsync(string url, RestRequest request, string method)
        {
            var req = BuildRequest(url, request, method, out var byteArray);

            if (byteArray != null)
            {
                await using (var dataStream = await req.GetRequestStreamAsync())
                {
                    await dataStream.WriteAsync(byteArray, 0, byteArray.Length);
                }
            }

            var webResponse = await req.GetResponseAsync();
            var rs = webResponse.GetResponseStream();

            if (rs == null) { return RestResponse.Empty; }

            string body;
            using (var reader = new StreamReader(rs))
            {
                body = await reader.ReadToEndAsync();
            }

            return BuildResponse(webResponse, body);
        }

        private WebRequest BuildRequest(string url, RestRequest request, string method, out byte[] byteArray)
        {
            if (request.UrlParameters.Count > 0)
            {
                url += "?" + request.BuildUrlParameters();
            }

            var result = requestFactory(url);

            result.Method = method;
            foreach (var (key, value) in request.Headers)
            {
                result.Headers.Add(key, HttpUtility.UrlEncode(value));
            }

            if (method == GET)
            {
                byteArray = null;
                return result;
            }

            byteArray = Encoding.UTF8.GetBytes(request.Body);
            result.ContentLength = byteArray.Length;
            result.ContentType = "application/json";

            return result;
        }

        private static RestResponse BuildResponse(WebResponse response, string body)
        {
            var result = new RestResponse(body);
            foreach (var headerKey in response.Headers.AllKeys)
            {
                result.Headers.Add(headerKey, HttpUtility.UrlDecode(response.Headers[headerKey]));
            }

            return result;
        }
    }

    public static class ContextBuilderWebRequestRestClientExtensions
    {
        public static ContextBuilder UsingRestClient(this ContextBuilder source, Action<WebRequest> configurer) => source.UsingRestClient(new WebRequestRestClient(configurer));
        public static ContextBuilder UsingRestClient(this ContextBuilder source, Func<string, WebRequest> requestFactory) => source.UsingRestClient(new WebRequestRestClient(requestFactory));
    }
}

