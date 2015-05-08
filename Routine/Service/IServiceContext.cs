using Routine.Core;
using Routine.Engine;

namespace Routine.Service
{
	public interface IServiceContext
	{
		IServiceConfiguration ServiceConfiguration { get; }
		IObjectService ObjectService { get; }
		ObjectReferenceData GetObjectReference(object @object);
		object GetObject(ObjectReferenceData reference);
		object GetObject(IType type, string id);
	}
}
