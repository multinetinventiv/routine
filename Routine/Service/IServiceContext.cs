using Routine.Core;
using Routine.Engine;

namespace Routine.Service
{
	public interface IServiceContext
	{
		IServiceConfiguration ServiceConfiguration { get; }
		IObjectService ObjectService { get; }
		ReferenceData GetObjectReference(object @object);
		object GetObject(ReferenceData reference);
		object GetObject(IType type, string id);
		string GetModelId(IType type);
		IType GetType(string modelId);
	}
}
