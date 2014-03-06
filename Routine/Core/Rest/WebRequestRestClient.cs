using System.Net;
using System.IO;
using System.Linq;

namespace Routine.Core.Rest
{
	public class WebRequestRestClient : IRestClient
	{
		public string Get(string url, params RestParameter[] parameters)
		{
			url += "?" + string.Join("&", parameters.Select(p => p.Name + "=" + p.Value));

			var req = WebRequest.Create(url);
			req.Method = "Get";
			var res = req.GetResponse();

			string result;
			using(var reader = new StreamReader(res.GetResponseStream()))
			{
				result = reader.ReadToEnd();
			}

			return result;
		}
	}
}

