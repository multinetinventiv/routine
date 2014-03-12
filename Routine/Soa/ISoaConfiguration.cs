using System;
using Routine.Core;

namespace Routine.Soa
{
	public interface ISoaConfiguration
	{
		IExtractor<Exception, SoaExceptionResult> ExceptionResultExtractor { get; }
	}
}

