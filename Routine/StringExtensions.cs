using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Routine
{
	public static class StringExtensions
	{
		public static string ToUpperInitial(this string source)
		{
			if(source == null) { return null; }
			if(source.Length == 0) { return source; }
			if(source.Length == 1) { return source.ToUpperInvariant(); }

			return char.ToUpperInvariant(source[0]) + source.Substring(1);
		}

		public static string ToLowerInitial(this string source)
		{
			if (source == null) { return null; }
			if (source.Length == 0) { return source; }
			if (source.Length == 1) { return source.ToLowerInvariant(); }

			return char.ToLowerInvariant(source[0]) + source.Substring(1);
		}

		public static string SplitCamelCase(this string source) { return source.SplitCamelCase(' '); }
		public static string SplitCamelCase(this string source, char splitter)
		{
			var pattern = string.Format("{0}|{1}|{2}",
			                            "(?<=[A-Z])(?=[A-Z][a-z])",
			                            "(?<=[^A-Z])(?=[A-Z])",
			                            "(?<=[A-Za-z])(?=[^A-Za-z])");

			return Regex.Replace(source, pattern, splitter.ToString(CultureInfo.InvariantCulture));
		}

		public static string SnakeCaseToCamelCase(this string source) { return source.SnakeCaseToCamelCase('_'); }
		public static string SnakeCaseToCamelCase(this string source, char splitter)
		{
			if(source == null) {return null;}
			if (!source.Contains(splitter.ToString(CultureInfo.InvariantCulture))) { return source; }

			var words = source.Split(splitter);

			var result = words[0];
			for(int i = 1; i < words.Length; i++)
			{
				result += words[i].ToUpperInitial();
			}

			return result;
		}

		public static string Before(this string source, char searchChar) { return source.Before(searchChar.ToString(CultureInfo.InvariantCulture)); }
		public static string Before(this string source, string searchString) { return source.Before(searchString, true); }
		public static string BeforeLast(this string source, char searchChar) { return source.BeforeLast(searchChar.ToString(CultureInfo.InvariantCulture)); }
		public static string BeforeLast(this string source, string searchString) { return source.Before(searchString, false); }
		private static string Before(this string source, string searchString, bool first)
		{
			var ix = first ? source.IndexOf(searchString, StringComparison.Ordinal) : source.LastIndexOf(searchString, StringComparison.Ordinal);

			return ix < 0 ? source : source.Substring(0, ix);
		}
		
		public static string After(this string source, char searchChar) { return source.After(searchChar.ToString(CultureInfo.InvariantCulture)); }
		public static string After(this string source, string searchString) { return source.After(searchString, true); }
		public static string AfterLast(this string source, char searchChar) { return source.AfterLast(searchChar.ToString(CultureInfo.InvariantCulture));}
		public static string AfterLast(this string source, string searchString) {return source.After(searchString, false);}
		private static string After(this string source, string searchString, bool first)
		{
			var ix = first ? source.IndexOf(searchString, StringComparison.Ordinal) : source.LastIndexOf(searchString, StringComparison.Ordinal);
			
			if(ix < 0) { return source; }
			ix = ix + searchString.Length;
			return source.Substring(ix, source.Length - ix);
		}

		public static string SurroundWith(this string source, string prefixAndSuffix) { return source.SurroundWith(prefixAndSuffix, prefixAndSuffix); }
		public static string SurroundWith(this string source, string prefix, string suffix)
		{
			return source.Prepend(prefix).Append(suffix);
		}

		public static string Append(this string source, string suffix)
		{
			return new StringBuilder(source).Append(suffix).ToString();
		}

		public static string Prepend(this string source, string prefix)
		{
			return new StringBuilder(prefix).Append(source).ToString();
		}

		public static bool Matches(this string source, string regexPattern)
		{
			return Regex.IsMatch(source, "^" + regexPattern + "$");
		}
	}
}

