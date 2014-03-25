using Routine.Core;
using Routine.Core.Service;

namespace Routine.Soa.Context
{
	public class DefaultSoaContext : ISoaContext
	{
		private readonly ICoreContext coreContext;

		public ISoaConfiguration SoaConfiguration { get; private set; }
		public IObjectService ObjectService { get; private set; }

		public DefaultSoaContext(ICoreContext coreContext, ISoaConfiguration soaConfiguration, IObjectService objectService)
		{
			this.coreContext = coreContext;

			SoaConfiguration = soaConfiguration;
			ObjectService = objectService;
		}

		public PerformOperationInterceptionContext CreatePerformOperationInterceptionContext(ObjectReferenceData targetReference, string operationModelId, System.Collections.Generic.List<ParameterValueData> parameterValues)
		{
			return new PerformOperationInterceptionContext(coreContext, targetReference, operationModelId, parameterValues);
		}
	}
}
