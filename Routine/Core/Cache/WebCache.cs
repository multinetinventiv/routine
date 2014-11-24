using System.Web;

namespace Routine.Core.Cache
{
	public class WebCache : ICache
	{
		public bool Contains(string key)
		{
			return HttpRuntime.Cache[key] != null;
		}

		public void Add(string key, object value)
		{
			HttpRuntime.Cache.Insert(key, value);
		}

		public void Remove(string key)
		{
			HttpRuntime.Cache.Remove(key);
		}

		public object this[string key]
		{
			get
			{
				return HttpRuntime.Cache[key];
			}
		}
	}
}

