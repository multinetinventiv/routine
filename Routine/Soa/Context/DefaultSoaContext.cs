using System.Collections.Generic;
using Routine.Core;

namespace Routine.Soa.Context
{
	public class DefaultSoaContext : ISoaContext
	{
		private readonly ICoreContext coreContext;

		public ISoaConfiguration SoaConfiguration { get; private set; }
		public IObjectService ObjectService { get; private set; }

		public DefaultSoaContext(ICoreContext coreContext, ISoaConfiguration soaConfiguration, IObjectService objectService)
		{
			this.coreContext = coreContext;

			SoaConfiguration = soaConfiguration;
			ObjectService = objectService;
		}

		public ObjectReferenceData GetObjectReference(object @object)
		{
			return coreContext.CreateReferenceData(@object);
		}

		public object GetObject(ObjectReferenceData reference)
		{
			return coreContext.Locate(reference);
		}
	}
}
