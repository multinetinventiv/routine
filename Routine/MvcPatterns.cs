using Routine.Core.Configuration;
using Routine.Ui.Configuration;

namespace Routine
{
	public static class MvcPatterns
	{
		public static ConventionalMvcConfiguration FromEmpty(this PatternBuilder<ConventionalMvcConfiguration> source) { return new ConventionalMvcConfiguration(); }
	}
}

