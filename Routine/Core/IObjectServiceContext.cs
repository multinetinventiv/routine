using System.Collections.Generic;
using Routine.Core;
using Routine.Core.Context;

namespace Routine.Core
{
	public interface IInterceptionContext
	{
		IInterceptionConfiguration InterceptionConfiguration { get; }
		IObjectService ObjectService { get; }

		InterceptionContext CreateInterceptionContext();
		ObjectModelInterceptionContext CreateObjectModelInterceptionContext(string objectModelId);
		ObjectReferenceInterceptionContext CreateObjectReferenceInterceptionContext(ObjectReferenceData targetReference);
		MemberInterceptionContext CreateMemberInterceptionContext(ObjectReferenceData targetReference, string memberModelId);
		PerformOperationInterceptionContext CreatePerformOperationInterceptionContext(ObjectReferenceData targetReference, string operationModelId, Dictionary<string, ReferenceData> parameterValues);
	}
}
