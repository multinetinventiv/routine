using System;
using Routine.Core;
using Routine.Soa.Context;

namespace Routine.Soa
{
	public interface ISoaConfiguration
	{
		string ActionRouteName { get; }

		IExtractor<Exception, SoaExceptionResult> ExceptionResultExtractor { get; }
		
		IInterceptor<InterceptionContext> GetApplicationModelInterceptor { get; }
		IInterceptor<ObjectModelInterceptionContext> GetObjectModelInterceptor { get; }
		IInterceptor<ObjectModelInterceptionContext> GetAvailableObjectsInterceptor { get; }
		IInterceptor<ObjectReferenceInterceptionContext> GetValueInterceptor { get; }
		IInterceptor<ObjectReferenceInterceptionContext> GetInterceptor { get; }
		IInterceptor<MemberInterceptionContext> GetMemberInterceptor { get; }
		IInterceptor<OperationInterceptionContext> GetOperationInterceptor { get; }
		IInterceptor<PerformOperationInterceptionContext> PerformOperationInterceptor { get; }
	}
}

