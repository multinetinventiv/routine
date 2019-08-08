using System.Collections.Generic;

namespace Routine.Core
{
	public interface IObjectService
	{
		ApplicationModel ApplicationModel { get; }

		ObjectData Get(ReferenceData reference);
		VariableData Do(ReferenceData target, string operation, Dictionary<string, ParameterValueData> parameters);
	}
}