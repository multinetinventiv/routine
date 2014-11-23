using Routine.Core;
using Routine.Engine;

namespace Routine.Soa
{
	public interface ISoaContext
	{
		ISoaConfiguration SoaConfiguration { get; }
		IObjectService ObjectService { get; }
		ICoreContext CoreContext { get; }
	}

	public static class SoaContextFacade
	{
		public static ObjectReferenceData GetObjectReference(this ISoaContext source, object @object)
		{
			return source.CoreContext.CreateDomainObject(@object).GetReferenceData();
		}

		public static object GetObject(this ISoaContext source, ObjectReferenceData reference)
		{
			return source.CoreContext.GetObject(reference);
		}

		public static object GetObject(this ISoaContext source, IType type, string id)
		{
			return source.CoreContext.GetDomainType(type).Locate(id);
		}
	}
}
