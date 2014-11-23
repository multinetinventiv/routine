using Routine.Core.Configuration;
using Routine.Soa;
using Routine.Soa.Configuration;

namespace Routine
{
	public static class SoaPatterns
	{
		public static ConventionalSoaConfiguration FromEmpty(this PatternBuilder<ConventionalSoaConfiguration> source) { return new ConventionalSoaConfiguration(false); }

		public static ConventionalSoaConfiguration ExceptionsWrappedAsUnhandledPattern(this PatternBuilder<ConventionalSoaConfiguration> source)
		{
			return source.FromEmpty()
				.ExceptionResult.Set(e => e.By(ex => new SoaExceptionResult(ex.GetType().FullName, ex.Message, false)))
			;
		}
	}
}
