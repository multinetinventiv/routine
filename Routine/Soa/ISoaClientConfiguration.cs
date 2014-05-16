using System;
using System.Collections.Generic;
using Routine.Core;

namespace Routine.Soa
{
	public interface ISoaClientConfiguration
	{
		string ServiceUrlBase { get; }
		List<string> DefaultParameters { get; }

		IExtractor<SoaExceptionResult, Exception> ExceptionExtractor { get; }
		IExtractor<string, string> DefaultParameterValueExtractor { get; }
	}
}

