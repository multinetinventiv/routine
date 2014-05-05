using Routine.Api.Configuration;
using Routine.Core.Builder;

namespace Routine
{
	public static class ApiGenerationPatterns
	{
		public static GenericApiGenerationConfiguration FromEmpty(this PatternBuilder<GenericApiGenerationConfiguration> source) { return new GenericApiGenerationConfiguration(); }

		public static GenericApiGenerationConfiguration ShortModelIdPattern(this PatternBuilder<GenericApiGenerationConfiguration> source, string prefix, string shortPrefix)
		{
			return source
				.FromEmpty()
				.SerializeReferencedModelId.Done(s => s
					.SerializeBy(t => t.FullName.ShortenModelId(prefix, shortPrefix))
					.SerializeWhen(t => t.FullName.StartsWith(prefix + ".") && t.IsPublic)
					.DeserializeBy(str => str.NormalizeModelId(prefix, shortPrefix).ToType())
					.DeserializeWhen(str => str.StartsWith(shortPrefix + "-")))
			;
		}

		public static GenericApiGenerationConfiguration ParseableValueTypePattern(this PatternBuilder<GenericApiGenerationConfiguration> source)
		{
			return source
				.FromEmpty()
				.ExtractStringToValueCodeTemplate
					.Add(e => e.Always("{valueString}").When(t => t.CanBe<string>()))
					.Done(e => e.Always("{type}.Parse({valueString})").When(t => t.CanParse()))
				.ExtractValueToStringCodeTemplate
					.Add(e => e.Always("{value}").When(t => t.CanBe<string>()))
					.Done(e => e.Always("{value}.ToString()").When(t => t.CanParse()))
			;
		}
	}
}
