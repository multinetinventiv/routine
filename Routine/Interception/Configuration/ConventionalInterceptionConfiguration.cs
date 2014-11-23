using Routine.Core;
using Routine.Core.Configuration;
using Routine.Interception.Context;

namespace Routine.Interception.Configuration
{
	public class ConventionalInterceptionConfiguration : IInterceptionConfiguration
	{
		public ConventionalListConfiguration<ConventionalInterceptionConfiguration, InterceptionTarget, IInterceptor<InterceptionContext>> Interceptors { get; private set; }
		public ConventionalListConfiguration<ConventionalInterceptionConfiguration, InterceptedOperationModel, IInterceptor<ServiceInterceptionContext>> ServiceInterceptors { get; private set; }

		internal ConventionalInterceptionConfiguration()
		{
			Interceptors = new ConventionalListConfiguration<ConventionalInterceptionConfiguration, InterceptionTarget, IInterceptor<InterceptionContext>>(this, "Interceptors");
			ServiceInterceptors = new ConventionalListConfiguration<ConventionalInterceptionConfiguration, InterceptedOperationModel, IInterceptor<ServiceInterceptionContext>>(this, "ServiceInterceptors");
		}

		public ConventionalInterceptionConfiguration Merge(ConventionalInterceptionConfiguration other)
		{
			Interceptors.Merge(other.Interceptors);
			ServiceInterceptors.Merge(other.ServiceInterceptors);

			return this;
		}

		#region IInterceptionConfiguration implementation

		IInterceptor<InterceptionContext> IInterceptionConfiguration.GetInterceptor(InterceptionTarget target) { return new ChainInterceptor<InterceptionContext>(Interceptors.Get(target)); }
		IInterceptor<ServiceInterceptionContext> IInterceptionConfiguration.GetServiceInterceptor(ObjectModel objectModel, OperationModel operationModel) { return new ChainInterceptor<ServiceInterceptionContext>(ServiceInterceptors.Get(new InterceptedOperationModel(objectModel, operationModel))); }

		#endregion

		public class InterceptedOperationModel
		{
			public ObjectModel ObjectModel { get; private set; }
			public OperationModel OperationModel { get; private set; }

			public InterceptedOperationModel(ObjectModel objectModel, OperationModel operationModel)
			{
				ObjectModel = objectModel;
				OperationModel = operationModel;
			}
		}
	}
}
