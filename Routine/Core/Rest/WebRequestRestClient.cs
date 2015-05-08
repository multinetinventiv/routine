using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Routine.Core.Rest
{
	public class WebRequestRestClient : IRestClient
	{
		private readonly Func<string, WebRequest> requestFactory;

		public WebRequestRestClient() : this(wr => { }) { }
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

		public RestResponse Get(string url, params RestParameter[] parameters)
		{
			url += "?" + string.Join("&", parameters.Select(p => p.Name + "=" + HttpUtility.UrlEncode(p.Value)));

			var req = requestFactory(url);
			req.Method = "GET";

			return CreateRestResponse(req.GetResponse());
		}

		public RestResponse Post(string url, params RestParameter[] parameters)
		{
			var req = requestFactory(url);
			req.Method = "POST";

			string postData = string.Join("&", parameters.Select(p => p.Name + "=" + HttpUtility.UrlEncode(p.Value)));
			byte[] byteArray = Encoding.UTF8.GetBytes(postData);
			req.ContentType = "application/x-www-form-urlencoded";
			req.ContentLength = byteArray.Length;
			using (var dataStream = req.GetRequestStream())
			{
				dataStream.Write(byteArray, 0, byteArray.Length);
			}

			return CreateRestResponse(req.GetResponse());
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
				if (headerKey.Contains(Constants.SERVICE_RESPONSE_HEADER_PREFIX))
				{
					result.Headers.Add(headerKey.After(Constants.SERVICE_RESPONSE_HEADER_PREFIX), webResponse.Headers[headerKey]);
				}
			}
			return result;
		}
	}

	public static class ContextBuilderWebRequestRestClientExtensions
	{
		public static ContextBuilder UsingRestClient(this ContextBuilder source, Action<WebRequest> configurer) { return source.UsingRestClient(new WebRequestRestClient(configurer)); }
		public static ContextBuilder UsingRestClient(this ContextBuilder source, Func<string, WebRequest> requestFactory) { return source.UsingRestClient(new WebRequestRestClient(requestFactory)); }
	}
}

