using Routine.Core.Builder;
using Routine.Mvc.Configuration;

namespace Routine
{
	public static class MvcPatterns
	{
		public static GenericMvcConfiguration FromEmpty(this PatternBuilder<GenericMvcConfiguration> source) { return new GenericMvcConfiguration(false); }
	}
}

