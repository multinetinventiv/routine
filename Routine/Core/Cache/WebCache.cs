using System.Web;

namespace Routine.Core.Cache
{
	public class WebCache : ICache
	{
		public bool Contains(string key)
		{
			return HttpContext.Current.Cache[key] != null;
		}

		public void Add(string key, object value)
		{
			HttpContext.Current.Cache.Insert(key, value);
		}

		public void Remove(string key)
		{
			HttpContext.Current.Cache.Remove(key);
		}

		public object this[string key]
		{
			get
			{
				return HttpContext.Current.Cache[key];
			}
		}
	}
}

