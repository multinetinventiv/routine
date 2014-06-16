using System.Collections.Generic;

namespace Routine.Core.Context
{
	public class DefaultInterceptionContext : IInterceptionContext
	{
		public IInterceptionConfiguration InterceptionConfiguration { get; private set; }
		public IObjectService ObjectService { get; private set; }

		public DefaultInterceptionContext(IInterceptionConfiguration interceptionConfiguration, IObjectService objectService)
		{
			InterceptionConfiguration = interceptionConfiguration;
			ObjectService = objectService;
		}

		public InterceptionContext CreateInterceptionContext()
		{
			return new InterceptionContext();
		}

		public ObjectModelInterceptionContext CreateObjectModelInterceptionContext(string objectModelId)
		{
			return new ObjectModelInterceptionContext(ObjectService, objectModelId);
		}

		public ObjectReferenceInterceptionContext CreateObjectReferenceInterceptionContext(ObjectReferenceData targetReference)
		{
			return new ObjectReferenceInterceptionContext(ObjectService, targetReference);
		}

		public PerformOperationInterceptionContext CreatePerformOperationInterceptionContext(ObjectReferenceData targetReference, string operationModelId, Dictionary<string, ParameterValueData> parameterValues)
		{
			return new PerformOperationInterceptionContext(ObjectService, targetReference, operationModelId, parameterValues);
		}
	}
}
