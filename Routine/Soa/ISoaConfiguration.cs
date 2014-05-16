using System;
using System.Collections.Generic;
using Routine.Core;
using Routine.Soa.Context;

namespace Routine.Soa
{
	public interface ISoaConfiguration
	{
		string ActionRouteName { get; }
		List<string> DefaultParameters { get; }

		IExtractor<Exception, SoaExceptionResult> ExceptionResultExtractor { get; }
	}
}

