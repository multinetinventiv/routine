using System;
using System.Net;
using System.IO;
using System.Linq;
using System.Text;

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

		public string Get(string url, params RestParameter[] parameters)
		{
			url += "?" + string.Join("&", parameters.Select(p => p.Name + "=" + p.Value));

			var req = requestFactory(url);
			req.Method = "GET";
			var res = req.GetResponse();

			string result;
			using (var reader = new StreamReader(res.GetResponseStream()))
			{
				result = reader.ReadToEnd();
			}

			return result;
		}

		public string Post(string url, params RestParameter[] parameters)
		{
			var req = requestFactory(url);
			req.Method = "POST";

			string postData = string.Join("&", parameters.Select(p => p.Name + "=" + p.Value));
			byte[] byteArray = Encoding.UTF8.GetBytes(postData);
			req.ContentType = "application/x-www-form-urlencoded";
			req.ContentLength = byteArray.Length;
			using (var dataStream = req.GetRequestStream())
			{
				dataStream.Write(byteArray, 0, byteArray.Length);
			}

			var res = req.GetResponse();

			string result;
			using (var reader = new StreamReader(res.GetResponseStream()))
			{
				result = reader.ReadToEnd();
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

