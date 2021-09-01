using Routine.Core.Configuration;
using Routine.Interception.Configuration;

namespace Routine
{
	public static class InterceptionPatterns
	{
		public static ConventionBasedInterceptionConfiguration FromEmpty(this PatternBuilder<ConventionBasedInterceptionConfiguration> source) => new();
    }
}
