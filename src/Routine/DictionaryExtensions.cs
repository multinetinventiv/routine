using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Routine
{
	public static class DictionaryExtensions
	{
		public static string ToKeyValueString(this IDictionary source)
		{
			var result = new List<string>();
			foreach (var key in source.Keys)
			{
				result.Add($"[{key}={source[key]}]");
			}

			return string.Join(",", result.ToArray()).SurroundWith("[", "]");
		}

		public static bool KeyValueEquals(this IDictionary source, IDictionary other)
		{
			if(source == null && other == null)
				return true;

			if(source == null || other == null)
				return false;

			if (source.Keys.Cast<object>().Any(k => !other.Contains(k)) ||
			    other.Keys.Cast<object>().Any(k => !source.Contains(k)) ||
			    source.Keys.Cast<object>().Any(k => !Equals(source[k], other[k])))
			{
				return false;
			}

			return true;
		}

		public static int GetKeyValueHashCode(this IDictionary source)
		{
			var result = 0;
			unchecked
			{
				foreach(var key in source.Keys)
				{
					result = (result * 397) ^ key.GetHashCode() ^ ((source[key] != null) ? source[key].GetHashCode() : 0);
				}
			}
			return result;
		}
	}
}

