using Routine.Core;

namespace Routine.Soa
{
	public interface ISoaContext
	{
		ISoaConfiguration SoaConfiguration { get; }
		IObjectService ObjectService { get; }

		ObjectReferenceData GetObjectReference(object @object);
		object GetObject(ObjectReferenceData reference);
		object GetObject(TypeInfo type, string id);
	}
}
