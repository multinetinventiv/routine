using Routine.Core.Configuration;
using Routine.Ui.Configuration;

namespace Routine
{
	public static class MvcPatterns
	{
		public static ConventionBasedMvcConfiguration FromEmpty(this PatternBuilder<ConventionBasedMvcConfiguration> source) { return new ConventionBasedMvcConfiguration(); }
	}
}

