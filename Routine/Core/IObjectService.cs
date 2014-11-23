using System.Collections.Generic;

namespace Routine.Core
{
	public interface IObjectService
	{
		ApplicationModel GetApplicationModel();
		ObjectModel GetObjectModel(string objectModelId);

		string GetValue(ObjectReferenceData reference);
		ObjectData Get(ObjectReferenceData reference);
		ValueData PerformOperation(ObjectReferenceData targetReference, string operationModelId, Dictionary<string, ParameterValueData> parameterValues);
	}
}