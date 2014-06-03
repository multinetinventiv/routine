using Routine.Core.Context;
using Routine.Core.Interceptor;

namespace Routine.Core.Configuration
{
	public class GenericInterceptionConfiguration : IInterceptionConfiguration
	{
		public ChainInterceptor<GenericInterceptionConfiguration, InterceptionContext> InterceptGetApplicationModel { get; private set; }
		public ChainInterceptor<GenericInterceptionConfiguration, ObjectModelInterceptionContext> InterceptGetObjectModel { get; private set; }
		public ChainInterceptor<GenericInterceptionConfiguration, ObjectModelInterceptionContext> InterceptGetAvailableObjects { get; private set; }
		public ChainInterceptor<GenericInterceptionConfiguration, ObjectReferenceInterceptionContext> InterceptGetValue { get; private set; }
		public ChainInterceptor<GenericInterceptionConfiguration, ObjectReferenceInterceptionContext> InterceptGet { get; private set; }
		public ChainInterceptor<GenericInterceptionConfiguration, PerformOperationInterceptionContext> InterceptPerformOperation { get; private set; }

		internal GenericInterceptionConfiguration()
		{
			InterceptGetApplicationModel = new ChainInterceptor<GenericInterceptionConfiguration, InterceptionContext>(this);
			InterceptGetObjectModel = new ChainInterceptor<GenericInterceptionConfiguration, ObjectModelInterceptionContext>(this);
			InterceptGetAvailableObjects = new ChainInterceptor<GenericInterceptionConfiguration, ObjectModelInterceptionContext>(this);
			InterceptGetValue = new ChainInterceptor<GenericInterceptionConfiguration, ObjectReferenceInterceptionContext>(this);
			InterceptGet = new ChainInterceptor<GenericInterceptionConfiguration, ObjectReferenceInterceptionContext>(this);
			InterceptPerformOperation = new ChainInterceptor<GenericInterceptionConfiguration, PerformOperationInterceptionContext>(this);
		}

		public GenericInterceptionConfiguration Merge(GenericInterceptionConfiguration other)
		{
			InterceptGetApplicationModel.Merge(other.InterceptGetApplicationModel);
			InterceptGetObjectModel.Merge(other.InterceptGetObjectModel);
			InterceptGetAvailableObjects.Merge(other.InterceptGetAvailableObjects);
			InterceptGetValue.Merge(other.InterceptGetValue);
			InterceptGet.Merge(other.InterceptGet);
			InterceptPerformOperation.Merge(other.InterceptPerformOperation);

			return this;
		}

		#region IInterceptionConfiguration implementation

		IInterceptor<InterceptionContext> IInterceptionConfiguration.GetApplicationModelInterceptor { get { return InterceptGetApplicationModel; } }
		IInterceptor<ObjectModelInterceptionContext> IInterceptionConfiguration.GetObjectModelInterceptor { get { return InterceptGetObjectModel; } }
		IInterceptor<ObjectModelInterceptionContext> IInterceptionConfiguration.GetAvailableObjectsInterceptor { get { return InterceptGetAvailableObjects; } }
		IInterceptor<ObjectReferenceInterceptionContext> IInterceptionConfiguration.GetValueInterceptor { get { return InterceptGetValue; } }
		IInterceptor<ObjectReferenceInterceptionContext> IInterceptionConfiguration.GetInterceptor { get { return InterceptGet; } }
		IInterceptor<PerformOperationInterceptionContext> IInterceptionConfiguration.PerformOperationInterceptor { get { return InterceptPerformOperation; } }

		#endregion	
	}
}
