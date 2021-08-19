using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace Routine.Core.Rest
{
	public class WebRequestRestClient : IRestClient
	{
		private readonly Func<string, WebRequest> requestFactory;

		public WebRequestRestClient() : this(_ => { }) { }
		public WebRequestRestClient(Action<WebRequest> configurer) 
			: this(url =>
				{
					var result = WebRequest.Create(url);

					configurer(result);

					return result;
				}) {}
		public WebRequestRestClient(Func<string, WebRequest> requestFactory)
		{
			this.requestFactory = requestFactory;
		}

		public RestResponse Get(string url, RestRequest request)
		{
			var response = CreateRestRequest(url, request, "GET").GetResponse();

			return CreateRestResponse(response);
		}

		public RestResponse Post(string url, RestRequest request)
		{
			var req = CreateRestRequest(url, request, "POST");

			req.ContentType = "application/json";

			var byteArray = Encoding.UTF8.GetBytes(request.Body);
			req.ContentLength = byteArray.Length;
			using (var dataStream = req.GetRequestStream())
			{
				dataStream.Write(byteArray, 0, byteArray.Length);
			}

			return CreateRestResponse(req.GetResponse());
		}

		private WebRequest CreateRestRequest(string url, RestRequest request, string method)
		{
			if (request.UrlParameters.Count > 0)
			{
				url += "?" + request.BuildUrlParameters();
			}

			var req = requestFactory(url);

			req.Method = method;

			foreach (var (key, value) in request.Headers)
			{
				req.Headers.Add(key, HttpUtility.UrlEncode(value));
			}

			return req;
		}

		private static RestResponse CreateRestResponse(WebResponse webResponse)
		{
			var rs = webResponse.GetResponseStream();

			if (rs == null)
			{
				return RestResponse.Empty;
			}

			string body;
			using (var reader = new StreamReader(rs))
			{
				body = reader.ReadToEnd();
			}

			var result = new RestResponse(body);
			foreach (var headerKey in webResponse.Headers.AllKeys)
			{
				result.Headers.Add(headerKey, HttpUtility.UrlDecode(webResponse.Headers[headerKey]));
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

