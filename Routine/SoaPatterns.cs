using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Routine.Core.Builder;
using Routine.Soa;
using Routine.Soa.Configuration;

namespace Routine
{
	public static class SoaPatterns
	{
		public static GenericSoaConfiguration FromEmpty(this PatternBuilder<GenericSoaConfiguration> source) { return new GenericSoaConfiguration(false); }

		public static GenericSoaConfiguration ExceptionsWrappedAsUnhandledPattern(this PatternBuilder<GenericSoaConfiguration> source)
		{
			return source.FromEmpty()
				.ExceptionResult.Done(e => e.ByConverting(ex => new SoaExceptionResult(ex.GetType().FullName, ex.Message, false)));
		}
	}
}
