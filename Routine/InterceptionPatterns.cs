using Routine.Core.Configuration;
using Routine.Interception.Configuration;

namespace Routine
{
	public static class InterceptionPatterns
	{
		public static ConventionalInterceptionConfiguration FromEmpty(this PatternBuilder<ConventionalInterceptionConfiguration> source) { return new ConventionalInterceptionConfiguration(); }
	}
}
