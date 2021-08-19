using Routine.Core;
using Routine.Engine;

namespace Routine.Service.Context
{
	public class DefaultServiceContext : IServiceContext
	{
		public IServiceConfiguration ServiceConfiguration { get; }
		public IObjectService ObjectService { get; }
		public ICoreContext CoreContext { get; }

		public DefaultServiceContext(ICoreContext coreContext, IServiceConfiguration serviceConfiguration, IObjectService objectService)
		{
			ServiceConfiguration = serviceConfiguration;
			ObjectService = objectService;
			CoreContext = coreContext;
		}

		public ReferenceData GetObjectReference(object @object)
		{
			return CoreContext.CreateDomainObject(@object).GetReferenceData();
		}

		public string GetModelId(IType type)
		{
			return CoreContext.GetDomainType(type).Id;
		}

		public IType GetType(string modelId)
		{
			return CoreContext.GetDomainType(modelId).Type;
		}

		public object GetObject(ReferenceData reference)
		{
			return CoreContext.GetObject(reference);
		}

		public object GetObject(IType type, string id)
		{
			return CoreContext.GetDomainType(type).Locate(id);
		}
	}
}
