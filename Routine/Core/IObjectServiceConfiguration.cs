using Routine.Core.Context;

namespace Routine.Core
{
	public interface IInterceptionConfiguration
	{
		IInterceptor<InterceptionContext> GetApplicationModelInterceptor { get; }
		IInterceptor<ObjectModelInterceptionContext> GetObjectModelInterceptor { get; }
		IInterceptor<ObjectModelInterceptionContext> GetAvailableObjectsInterceptor { get; }
		IInterceptor<ObjectReferenceInterceptionContext> GetValueInterceptor { get; }
		IInterceptor<ObjectReferenceInterceptionContext> GetInterceptor { get; }
		IInterceptor<PerformOperationInterceptionContext> PerformOperationInterceptor { get; }
	}
}
