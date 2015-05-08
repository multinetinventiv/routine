using Routine.Core.Configuration;
using Routine.Service;
using Routine.Service.Configuration;

namespace Routine
{
	public static class ServicePatterns
	{
		public static ConventionalServiceConfiguration FromEmpty(this PatternBuilder<ConventionalServiceConfiguration> source) { return new ConventionalServiceConfiguration(); }

		public static ConventionalServiceConfiguration ExceptionsWrappedAsUnhandledPattern(this PatternBuilder<ConventionalServiceConfiguration> source)
		{
			return source.FromEmpty()
				.ExceptionResult.Set(e => e.By(ex => new ExceptionResult(ex.GetType().FullName, ex.Message, false)))
			;
		}
	}
}
