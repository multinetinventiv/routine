using System.Text;
using System.Web;

namespace Routine.Core.Rest;

public class HttpClientRestClient : IRestClient
{
    private readonly Func<HttpClient> newClient;
    private readonly Func<HttpRequestMessage> newRequest;

    public HttpClientRestClient(Func<HttpClient> newClient = default, Func<HttpRequestMessage> newRequest = default)
    {
        this.newClient = newClient ?? (() => new());
        this.newRequest = newRequest ?? (() => new());
    }

    public RestResponse Get(string url, RestRequest request) => Make(url, request, HttpMethod.Get);
    public RestResponse Post(string url, RestRequest request) => Make(url, request, HttpMethod.Post);

    public async Task<RestResponse> GetAsync(string url, RestRequest request) => await MakeAsync(url, request, HttpMethod.Get);
    public async Task<RestResponse> PostAsync(string url, RestRequest request) => await MakeAsync(url, request, HttpMethod.Post);

    private RestResponse Make(string url, RestRequest request, HttpMethod method)
    {
        try
        {
            var req = BuildRequest(url, request, method);

            var res = newClient().Send(req).EnsureSuccessStatusCode();
            var rs = res.Content.ReadAsStream();

            if (rs == null) { return RestResponse.Empty; }

            string body;
            using (var reader = new StreamReader(rs))
            {
                body = reader.ReadToEnd();
            }

            return BuildResponse(res, body);
        }
        catch (HttpRequestException ex)
        {
            throw new RestRequestException(ex.StatusCode, ex);
        }
    }

    private async Task<RestResponse> MakeAsync(string url, RestRequest request, HttpMethod method)
    {
        try
        {
            var req = BuildRequest(url, request, method);

            var res = (await newClient().SendAsync(req)).EnsureSuccessStatusCode();
            var rs = await res.Content.ReadAsStreamAsync();

            if (rs == null) { return RestResponse.Empty; }

            string body;
            using (var reader = new StreamReader(rs))
            {
                body = await reader.ReadToEndAsync();
            }

            return BuildResponse(res, body);
        }
        catch (HttpRequestException ex)
        {
            throw new RestRequestException(ex.StatusCode, ex);
        }
    }

    private HttpRequestMessage BuildRequest(string url, RestRequest request, HttpMethod method)
    {
        if (request.UrlParameters.Count > 0)
        {
            url += "?" + request.BuildUrlParameters();
        }

        var result = newRequest();

        result.RequestUri = new(url);
        result.Method = method;

        foreach (var (key, value) in request.Headers)
        {
            result.Headers.TryAddWithoutValidation(key, HttpUtility.UrlEncode(value));
        }

        if (method != HttpMethod.Get)
        {
            result.Content = new StringContent(request.Body, Encoding.UTF8, "application/json");
        }

        return result;
    }

    private static RestResponse BuildResponse(HttpResponseMessage response, string body)
    {
        var result = new RestResponse(body);
        foreach (var header in response.Headers)
        {
            result.Headers.Add(header.Key, HttpUtility.UrlDecode(string.Join(",", header.Value)));
        }

        return result;
    }
}
