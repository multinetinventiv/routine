using Routine.Core.Configuration;
using Routine.Service;
using Routine.Service.Configuration;

namespace Routine
{
	public static class ServicePatterns
	{
		public static ConventionBasedServiceConfiguration FromEmpty(this PatternBuilder<ConventionBasedServiceConfiguration> source) { return new ConventionBasedServiceConfiguration(); }

		public static ConventionBasedServiceConfiguration ExceptionsWrappedAsUnhandledPattern(this PatternBuilder<ConventionBasedServiceConfiguration> source)
		{
			return source.FromEmpty()
				.ExceptionResult.Set(e => e.By(ex => new ExceptionResult(ex.GetType().FullName, ex.Message, false)))
			;
		}
	}
}
