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

        public ReferenceData GetObjectReference(object @object) => CoreContext.CreateDomainObject(@object).GetReferenceData();
        public string GetModelId(IType type) => CoreContext.GetDomainType(type).Id;
        public IType GetType(string modelId) => CoreContext.GetDomainType(modelId).Type;
        public object GetObject(ReferenceData reference) => CoreContext.GetObject(reference);
        public object GetObject(IType type, string id) => CoreContext.GetDomainType(type).Locate(id);
    }
}
