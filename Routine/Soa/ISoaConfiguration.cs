using System;
using Routine.Core;

namespace Routine.Soa
{
	public interface ISoaConfiguration
	{
		string ActionRouteName { get; }

		IExtractor<Exception, SoaExceptionResult> ExceptionResultExtractor { get; }
	}
}

