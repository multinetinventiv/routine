using System;
using Routine.Core.Configuration;
using Routine.Service.Configuration;

namespace Routine
{
	public static class ServiceClientPatterns
	{
		public static ConventionalServiceClientConfiguration FromEmpty(this PatternBuilder<ConventionalServiceClientConfiguration> source) { return new ConventionalServiceClientConfiguration(); }

		public static ConventionalServiceClientConfiguration FormattedExceptionPattern(this PatternBuilder<ConventionalServiceClientConfiguration> source, string formatWithTypeAndMessageAndIsHandled)
		{
			return source.FromEmpty()
				.Exception.Set(e => e.By(ex => new Exception(string.Format(formatWithTypeAndMessageAndIsHandled, ex.Type, ex.Message, ex.IsHandled))))
			;
		}
	}
}
