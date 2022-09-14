using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;

namespace Routine;

public static class StringExtensions
{
    public static string ToUpperInitial(this string source)
    {
        if (source == null) { return null; }
        if (source.Length == 0) { return source; }
        if (source.Length == 1) { return source.ToUpperInvariant(); }

        return char.ToUpperInvariant(source[0]) + source[1..];
    }

    public static string ToLowerInitial(this string source)
    {
        if (source == null) { return null; }
        if (source.Length == 0) { return source; }
        if (source.Length == 1) { return source.ToLowerInvariant(); }

        return char.ToLowerInvariant(source[0]) + source[1..];
    }

    public static string SplitCamelCase(this string source) { return source.SplitCamelCase(' '); }
    public static string SplitCamelCase(this string source, char splitter)
    {
        const string pattern = "(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])";

        return Regex.Replace(source, pattern, splitter.ToString(CultureInfo.InvariantCulture));
    }

    public static string SnakeCaseToCamelCase(this string source) { return source.SnakeCaseToCamelCase('_'); }
    public static string SnakeCaseToCamelCase(this string source, char splitter)
    {
        if (source == null) { return null; }
        if (!source.Contains(splitter.ToString(CultureInfo.InvariantCulture))) { return source; }

        var words = source.Split(splitter);

        var result = words[0];
        for (var i = 1; i < words.Length; i++)
        {
            result += words[i].ToUpperInitial();
        }

        return result;
    }

    public static string Before(this string source, char searchChar, StringComparison comparisonType = StringComparison.Ordinal) => source.Before(searchChar.ToString(CultureInfo.InvariantCulture), comparisonType);
    public static string Before(this string source, string searchString, StringComparison comparisonType = StringComparison.Ordinal) => source.Before(searchString, true, comparisonType);
    public static string BeforeLast(this string source, char searchChar, StringComparison comparisonType = StringComparison.Ordinal) => source.BeforeLast(searchChar.ToString(CultureInfo.InvariantCulture), comparisonType);
    public static string BeforeLast(this string source, string searchString, StringComparison comparisonType = StringComparison.Ordinal) => source.Before(searchString, false, comparisonType);
    private static string Before(this string source, string searchString, bool first, StringComparison comparisonType = StringComparison.Ordinal)
    {
        var ix = first ? source.IndexOf(searchString, comparisonType) : source.LastIndexOf(searchString, comparisonType);

        return ix < 0 ? source : source[..ix];
    }

    public static string After(this string source, char searchChar, StringComparison comparisonType = StringComparison.Ordinal) => source.After(searchChar.ToString(CultureInfo.InvariantCulture), comparisonType);
    public static string After(this string source, string searchString, StringComparison comparisonType = StringComparison.Ordinal) => source.After(searchString, true, comparisonType);
    public static string AfterLast(this string source, char searchChar, StringComparison comparisonType = StringComparison.Ordinal) => source.AfterLast(searchChar.ToString(CultureInfo.InvariantCulture), comparisonType);
    public static string AfterLast(this string source, string searchString, StringComparison comparisonType = StringComparison.Ordinal) => source.After(searchString, false, comparisonType);
    private static string After(this string source, string searchString, bool first, StringComparison comparisonType = StringComparison.Ordinal)
    {
        var ix = first ? source.IndexOf(searchString, comparisonType) : source.LastIndexOf(searchString, comparisonType);

        if (ix < 0) { return source; }
        ix += searchString.Length;
        return source.Substring(ix, source.Length - ix);
    }

    public static string SurroundWith(this string source, string prefixAndSuffix) => source.SurroundWith(prefixAndSuffix, prefixAndSuffix);
    public static string SurroundWith(this string source, string prefix, string suffix) =>
        source.Prepend(prefix).Append(suffix);

    public static string Append(this string source, string suffix) => new StringBuilder(source).Append(suffix).ToString();
    public static string Prepend(this string source, string prefix) => new StringBuilder(prefix).Append(source).ToString();
    public static bool Matches(this string source, string regexPattern) => Regex.IsMatch(source, "^" + regexPattern + "$");
}
