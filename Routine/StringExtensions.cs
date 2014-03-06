using System;
using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;

namespace Routine
{
	public static class StringExtensions
	{
		public static string ToUpperInitial(this string source)
		{
			if(source == null) { return null; }
			if(source.Length == 0) { return source; }
			if(source.Length == 1) { return source.ToUpper(); }

			return char.ToUpper(source[0], new CultureInfo("en-US")) + source.Substring(1);
		}

		public static string SplitCamelCase(this string source)
		{
			var pattern = string.Format("{0}|{1}|{2}",
			                            "(?<=[A-Z])(?=[A-Z][a-z])",
			                            "(?<=[^A-Z])(?=[A-Z])",
			                            "(?<=[A-Za-z])(?=[^A-Za-z])");

			return Regex.Replace(source, pattern, " ");
		}

		public static string SnakeCaseToCamelCase(this string source)
		{
			if(source == null) {return null;}
			if(!source.Contains("_")) {return source;}

			var words = source.Split('_');

			var result = words[0];
			for(int i = 1; i < words.Length; i++)
			{
				result += words[i].ToUpperInitial();
			}

			return result;
		}

		public static string Before(this string source, char searchChar) { return source.Before(searchChar.ToString()); }
		public static string Before(this string source, string searchString) { return source.Before(searchString, true); }
		public static string BeforeLast(this string source, char searchChar) { return source.BeforeLast(searchChar.ToString()); }
		public static string BeforeLast(this string source, string searchString) { return source.Before(searchString, false); }
		private static string Before(this string source, char searchChar, bool first) { return source.Before(searchChar.ToString(), first); }
		private static string Before(this string source, string searchString, bool first)
		{
			int ix = first ? source.IndexOf(searchString) : source.LastIndexOf(searchString);
			if(ix < 0) { return source; }

			return source.Substring(0, ix);
		}
		
		public static string After(this string source, char searchChar) { return source.After(searchChar.ToString()); }
		public static string After(this string source, string searchString) { return source.After(searchString, true); }
		public static string AfterLast(this string source, char searchChar) { return source.AfterLast(searchChar.ToString());}
		public static string AfterLast(this string source, string searchString) {return source.After(searchString, false);}
		private static string After(this string source, char searchChar, bool first) {return source.After(searchChar.ToString(), first);}
		private static string After(this string source, string searchString, bool first)
		{
			int ix = first ? source.IndexOf(searchString) : source.LastIndexOf(searchString);
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

