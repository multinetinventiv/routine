using System.Net;
using System.IO;
using System.Linq;
using System.Text;

namespace Routine.Core.Rest
{
	public class WebRequestRestClient : IRestClient
	{
		public string Get(string url, params RestParameter[] parameters)
		{
			url += "?" + string.Join("&", parameters.Select(p => p.Name + "=" + p.Value));

			var req = WebRequest.Create(url);
			req.Method = "GET";
			var res = req.GetResponse();

			string result;
			using(var reader = new StreamReader(res.GetResponseStream()))
			{
				result = reader.ReadToEnd();
			}

			return result;
		}

		public string Post(string url, params RestParameter[] parameters)
		{
			var req = WebRequest.Create(url);
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
}

