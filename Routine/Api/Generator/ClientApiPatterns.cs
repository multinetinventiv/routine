using System;

namespace Routine.Api.Generator
{
	public static class ClientApiPatterns
	{
		public static void UsingParseableValueTypes(this ClientApiGenerator source, string actualPrefix, string shortPrefix) { source.UsingParseableValueTypes(t => t.FullName.StartsWith(actualPrefix), t => t.FullName.ShortenModelId(actualPrefix, shortPrefix)); }
		public static void UsingParseableValueTypes(this ClientApiGenerator source, Func<TypeInfo, string> modelIdExtractor) { source.UsingParseableValueTypes(t => true, modelIdExtractor); }
		public static void UsingParseableValueTypes(this ClientApiGenerator source, Func<TypeInfo, bool> predicate, Func<TypeInfo, string> modelIdExtractor)
		{
			source.Using(t => t.CanBe<string>() && predicate(t), modelIdExtractor);
			source.Using(t => t.CanParse() && predicate(t), modelIdExtractor, "{value}.ToString()", "{type}.Parse({valueString})");
		}
	}
}

