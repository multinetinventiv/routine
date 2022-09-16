using Routine.Core;
using Routine.Engine;

namespace Routine.Service.Context;

public class DefaultServiceContext : IServiceContext
{
    public ICoreContext CoreContext { get; }
    public IServiceConfiguration ServiceConfiguration { get; }
    public IObjectService ObjectService { get; }

    public DefaultServiceContext(ICoreContext coreContext, IServiceConfiguration serviceConfiguration, IObjectService objectService)
    {
        CoreContext = coreContext;
        ServiceConfiguration = serviceConfiguration;
        ObjectService = objectService;
    }

    public string GetModelId(IType type) => CoreContext.GetDomainType(type).Id;
    public IType GetType(string modelId) => CoreContext.GetDomainType(modelId).Type;

    public ReferenceData GetObjectReference(object @object) => CoreContext.CreateDomainObject(@object).GetReferenceData();

    public async Task<object> GetObjectAsync(ReferenceData reference) => await CoreContext.GetObjectAsync(reference);
    public async Task<object> GetObjectAsync(IType type, string id) => await CoreContext.GetDomainType(type).LocateAsync(id);
}
