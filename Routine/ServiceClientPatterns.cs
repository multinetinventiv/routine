using System;
using Routine.Core.Configuration;
using Routine.Service.Configuration;

namespace Routine
{
	public static class ServiceClientPatterns
	{
		public static ConventionalServiceClientConfiguration FromEmpty(this PatternBuilder<ConventionalServiceClientConfiguration> source) { return new ConventionalServiceClientConfiguration(); }

		public static ConventionalServiceClientConfiguration FormattedExceptionPattern(this PatternBuilder<ConventionalServiceClientConfiguration> source, string formatWithMessageAndTypeAndIsHandled)
		{
			return source.FromEmpty()
				.Exception.Set(e => e.By(ex => new Exception(string.Format(formatWithMessageAndTypeAndIsHandled, ex.Message, ex.Type, ex.IsHandled))))
			;
		}
	}
}
