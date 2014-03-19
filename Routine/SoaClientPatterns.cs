using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Routine.Core.Builder;
using Routine.Soa;
using Routine.Soa.Configuration;

namespace Routine
{
	public static class SoaClientPatterns
	{
		public static GenericSoaClientConfiguration FromEmpty(this PatternBuilder<GenericSoaClientConfiguration> source) { return new GenericSoaClientConfiguration(); }

		public static GenericSoaClientConfiguration FormattedExceptionPattern(this PatternBuilder<GenericSoaClientConfiguration> source, string format)
		{
			return source.FromEmpty()
				.ExtractException.Done(e => e.ByConverting(ex => new Exception(string.Format(format,ex.Message, ex.Type, ex.IsHandled))))
			;
		}
	}
}
