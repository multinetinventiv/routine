using System.Collections.Generic;
using Routine.Core.Service;
using Routine.Soa.Context;

namespace Routine.Soa
{
	public interface ISoaContext
	{
		ISoaConfiguration SoaConfiguration { get; }
		IObjectService ObjectService { get; }

		PerformOperationInterceptionContext CreatePerformOperationInterceptionContext(ObjectReferenceData targetReference, string operationModelId, List<ParameterValueData> parameterValues);
	}
}
