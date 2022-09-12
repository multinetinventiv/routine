using System.Collections;
using System.Linq;

namespace Routine;

public static class EnumerableExtensions
{
    public static string ToItemString(this IEnumerable source)
    {
        return string.Join(",", source.Cast<object>().ToArray()).SurroundWith("[", "]");
    }

    public static bool ItemEquals(this IEnumerable source, IEnumerable other)
    {
        if (source == null && other == null) { return true; }
        if (source == null || other == null) { return false; }

        var sourceAsObject = source.Cast<object>().ToList();
        var otherAsObject = other.Cast<object>().ToList();

        if (sourceAsObject.Count != otherAsObject.Count) { return false; }

        for (var i = 0; i < sourceAsObject.Count; i++)
        {
            if (!Equals(sourceAsObject[i], otherAsObject[i]))
            {
                return false;
            }
        }

        return true;
    }

    public static int GetItemHashCode(this IEnumerable source)
    {
        var result = 0;
        unchecked
        {
            foreach (var item in source)
            {
                result = (result * 397) ^ (item?.GetHashCode() ?? 0);
            }
        }
        return result;
    }
}
