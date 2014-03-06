using System.Collections.Generic;

namespace Routine.Core.Service
{
	public interface IObjectService
	{
		ApplicationModel GetApplicationModel();

		ObjectModel GetObjectModel(string objectModelId);

		List<SingleValueData> GetAvailableObjects(string objectModelId);

		string GetValue(ObjectReferenceData reference);
		ObjectData Get(ObjectReferenceData reference);

		MemberData GetMember(ObjectReferenceData reference, string memberModelId);
		//TODO memberdata direkt objectdata icerir, eager degilse bos gelir...
		//TODO MemberData GetMemberEager(ObjectReferenceData reference, string memberModelId);

		OperationData GetOperation(ObjectReferenceData reference, string operationModelId);

		ResultData PerformOperation(ObjectReferenceData targetReference, string operationModelId, List<ParameterValueData> parameterValues);
		//TODO resultdata direkt objectdata icerir, eager degilse bos gelir...
		//TODO ResultData PerformOperationEager(ObjectReferenceData targetReference, string operationModelId, List<ParameterValueData> parameterValues);
	}
}