using System;
using System.Collections.Generic;
using Routine.Core;

namespace Routine.Soa
{
	public interface ISoaConfiguration
	{
		string ActionRouteName { get; }
		List<string> DefaultParameters { get; }
		int MaxResultLength { get; }

		IExtractor<Exception, SoaExceptionResult> ExceptionResultExtractor { get; }
	}
}

