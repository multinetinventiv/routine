using System.Collections;
using System.Linq;

namespace Routine
{
	public static class IEnumerableExtensions
	{
		public static string ToItemString(this IEnumerable source)
		{
			return string.Join(",", source.Cast<object>().ToArray()).SurroundWith("[", "]");
		}

		public static bool ItemEquals(this IEnumerable source, IEnumerable other)
		{
			if(source == null && other == null)
				return true;

			if(source == null || other == null)
				return false;

			var sourceGen = source.Cast<object>();
			var otherGen = other.Cast<object>();

			if(sourceGen.All(s => otherGen.Any(o => object.Equals(s, o))) &&
				otherGen.All(o => sourceGen.Any(s => object.Equals(o, s)))) {
				return true;
			}

			return false;
		}

		public static int GetItemHashCode(this IEnumerable source)
		{
			int result = 0;
			unchecked
			{
				foreach(var item in source)
				{
					result ^= (item != null)?item.GetHashCode():0;
				}
			}
			return result;
		}
	}
}

