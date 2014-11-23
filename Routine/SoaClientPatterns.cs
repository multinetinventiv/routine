using System;
using Routine.Core.Configuration;
using Routine.Soa.Configuration;

namespace Routine
{
	public static class SoaClientPatterns
	{
		public static ConventionalSoaClientConfiguration FromEmpty(this PatternBuilder<ConventionalSoaClientConfiguration> source) { return new ConventionalSoaClientConfiguration(); }

		public static ConventionalSoaClientConfiguration FormattedExceptionPattern(this PatternBuilder<ConventionalSoaClientConfiguration> source, string formatWithMessageAndTypeAndIsHandled)
		{
			return source.FromEmpty()
				.Exception.Set(e => e.By(ex => new Exception(string.Format(formatWithMessageAndTypeAndIsHandled, ex.Message, ex.Type, ex.IsHandled))))
			;
		}
	}
}
