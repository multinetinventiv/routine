using System.Collections.Generic;

namespace Routine.Core
{
	public interface IObjectService
	{
		ApplicationModel GetApplicationModel();

		ObjectModel GetObjectModel(string objectModelId);

		List<ObjectData> GetAvailableObjects(string objectModelId);

		string GetValue(ObjectReferenceData reference);
		ObjectData Get(ObjectReferenceData reference);

		ValueData GetMember(ObjectReferenceData reference, string memberModelId);
		ValueData PerformOperation(ObjectReferenceData targetReference, string operationModelId, Dictionary<string, ReferenceData> parameterValues);
	}
}