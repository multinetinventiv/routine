using Routine.Api.Configuration;
using Routine.Core.Configuration;

namespace Routine
{
	public static class ApiGenerationPatterns
	{
		public static ConventionalApiGenerationConfiguration FromEmpty(this PatternBuilder<ConventionalApiGenerationConfiguration> source) { return new ConventionalApiGenerationConfiguration(); }

		public static ConventionalApiGenerationConfiguration ShortModelIdPattern(this PatternBuilder<ConventionalApiGenerationConfiguration> source, string prefix, string shortPrefix)
		{
			return source
				.FromEmpty()
				.ReferencedType.Set(c => c
					.By(t => t.Id.NormalizeModelId(prefix, shortPrefix).ToTypeInfo(true))
					.When(t => t.Id.StartsWith(shortPrefix + "-")))
			;
		}

		public static ConventionalApiGenerationConfiguration ParseableValueTypePattern(this PatternBuilder<ConventionalApiGenerationConfiguration> source)
		{
			return source
				.FromEmpty()
				
				.ReferencedTypeIsValueType.Set(e => e.Constant(true).When(t => t.CanBe<string>()))
				.ReferencedTypeIsValueType.Set(e => e.Constant(true).When(t => t.CanParse()))

				.StringToValueCodeTemplate.Set(e => e.Constant("{valueString}").When(t => t.CanBe<string>()))
				.StringToValueCodeTemplate.Set(e => e.Constant("{type}.Parse({valueString})").When(t => t.CanParse()))

				.ValueToStringCodeTemplate.Set(e => e.Constant("{value}").When(t => t.CanBe<string>()))
				.ValueToStringCodeTemplate.Set(e => e.Constant("{value}.ToString()").When(t => t.CanParse()))
			;
		}
	}
}
