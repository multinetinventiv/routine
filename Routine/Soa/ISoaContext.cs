using System.Collections.Generic;
using Routine.Core;
using Routine.Soa.Context;

namespace Routine.Soa
{
	public interface ISoaContext
	{
		ISoaConfiguration SoaConfiguration { get; }
		IObjectService ObjectService { get; }

		InterceptionContext CreateInterceptionContext();
		ObjectModelInterceptionContext CreateObjectModelInterceptionContext(string objectModelId);
		ObjectReferenceInterceptionContext CreateObjectReferenceInterceptionContext(ObjectReferenceData targetReference);
		MemberInterceptionContext CreateMemberInterceptionContext(ObjectReferenceData targetReference, string memberModelId);
		OperationInterceptionContext CreateOperationInterceptionContext(ObjectReferenceData targetReference, string operationModelId);
		PerformOperationInterceptionContext CreatePerformOperationInterceptionContext(ObjectReferenceData targetReference, string operationModelId, Dictionary<string, ReferenceData> parameterValues);

		ObjectReferenceData GetObjectReference(object @object);
		object GetObject(ObjectReferenceData reference);
	}
}
