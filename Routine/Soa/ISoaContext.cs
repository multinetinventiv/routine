using Routine.Core;
using Routine.Engine;

namespace Routine.Soa
{
	public interface ISoaContext
	{
		ISoaConfiguration SoaConfiguration { get; }
		IObjectService ObjectService { get; }
		ObjectReferenceData GetObjectReference(object @object);
		object GetObject(ObjectReferenceData reference);
		object GetObject(IType type, string id);
	}
}
