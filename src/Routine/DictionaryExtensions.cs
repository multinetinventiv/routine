using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Routine;

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
        if (source == null && other == null) { return true; }
        if (source == null || other == null) { return false; }

        return source.Keys.Cast<object>().All(other.Contains) &&
               other.Keys.Cast<object>().All(source.Contains) &&
               source.Keys.Cast<object>().All(k => Equals(source[k], other[k]));
    }

    public static int GetKeyValueHashCode(this IDictionary source)
    {
        var result = 0;
        unchecked
        {
            foreach (var key in source.Keys)
            {
                result = (result * 397) ^ key.GetHashCode() ^ (source[key] != null ? source[key].GetHashCode() : 0);
            }
        }
        return result;
    }
}
