using System;
using Routine.Core;
using Routine.Soa.Context;

namespace Routine.Soa
{
	public interface ISoaConfiguration
	{
		string ActionRouteName { get; }

		IExtractor<Exception, SoaExceptionResult> ExceptionResultExtractor { get; }

		IInterceptor<PerformOperationInterceptionContext> PerformOperationInterceptor { get; }
	}
}

