using System.Collections.Generic;
using Routine.Core;
using Routine.Soa.Context;

namespace Routine.Soa
{
	public interface ISoaContext
	{
		ISoaConfiguration SoaConfiguration { get; }
		IObjectService ObjectService { get; }

		ObjectReferenceData GetObjectReference(object @object);
		object GetObject(ObjectReferenceData reference);
	}
}
