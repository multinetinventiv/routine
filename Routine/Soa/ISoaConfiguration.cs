using System;
using System.Collections.Generic;
using Routine.Core;
using Routine.Core.Interceptor;
using Routine.Core.Service;

namespace Routine.Soa
{
	public interface ISoaConfiguration
	{
		string ActionRouteName { get; }

		IExtractor<Exception, SoaExceptionResult> ExceptionResultExtractor { get; }

		//IInterceptor<Interception<PerformOperationContext, ResultData>> PerformOperationInterceptor { get; }
	}
}

