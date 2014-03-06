namespace Routine.Api.Generator
{
	public static class ClientApiPatterns
	{
		public static void UsingParseableValueTypes(this ClientApiGenerator source, string valueTypePrefix)
		{
			source.Using(t => t.CanBe<string>(), t => t.FullName.Prepend(valueTypePrefix));
			source.Using(t => t.CanParse(), t => t.FullName.Prepend(valueTypePrefix), "{value}.ToString()", "{type}.Parse({valueString})");
		}

		public static void UsingClientClassesUnderCommonNamespace(this ClientApiGenerator source) {source.UsingClientClassesUnderCommonNamespace("-");}
		public static void UsingClientClassesUnderCommonNamespace(this ClientApiGenerator source, string separator)
		{
			source.Using(t => t.Namespace.StartsWith("Routine.Test.ApiGen.Client"), t => t.FullName.After("Routine.Test.ApiGen.Client").Replace(".", separator), true);
		}
	}
}

