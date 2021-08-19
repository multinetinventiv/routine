using Routine.Core;
using Routine.Core.Configuration;
using Routine.Interception.Context;

namespace Routine.Interception.Configuration
{
	public class ConventionBasedInterceptionConfiguration : LayeredBase<ConventionBasedInterceptionConfiguration>, IInterceptionConfiguration
	{
		public ConventionBasedListConfiguration<ConventionBasedInterceptionConfiguration, InterceptionTarget, IInterceptor<InterceptionContext>> Interceptors { get; }
		public ConventionBasedListConfiguration<ConventionBasedInterceptionConfiguration, OperationWithObjectModel, IInterceptor<ServiceInterceptionContext>> ServiceInterceptors { get; }

		internal ConventionBasedInterceptionConfiguration()
		{
			Interceptors = new ConventionBasedListConfiguration<ConventionBasedInterceptionConfiguration, InterceptionTarget, IInterceptor<InterceptionContext>>(this, "Interceptors");
			ServiceInterceptors = new ConventionBasedListConfiguration<ConventionBasedInterceptionConfiguration, OperationWithObjectModel, IInterceptor<ServiceInterceptionContext>>(this, "ServiceInterceptors");
		}

		public ConventionBasedInterceptionConfiguration Merge(ConventionBasedInterceptionConfiguration other)
		{
			Interceptors.Merge(other.Interceptors);
			ServiceInterceptors.Merge(other.ServiceInterceptors);

			return this;
		}

		#region IInterceptionConfiguration implementation

		IInterceptor<InterceptionContext> IInterceptionConfiguration.GetInterceptor(InterceptionTarget target) { return new ChainInterceptor<InterceptionContext>(Interceptors.Get(target)); }
		IInterceptor<ServiceInterceptionContext> IInterceptionConfiguration.GetServiceInterceptor(ObjectModel objectModel, OperationModel operationModel) { return new ChainInterceptor<ServiceInterceptionContext>(ServiceInterceptors.Get(new OperationWithObjectModel(objectModel, operationModel))); }

		#endregion
	}
}
