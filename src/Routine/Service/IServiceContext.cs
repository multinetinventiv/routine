using Routine.Core;
using Routine.Engine;

namespace Routine.Service;

public interface IServiceContext
{
    IServiceConfiguration ServiceConfiguration { get; }
    IObjectService ObjectService { get; }

    string GetModelId(IType type);
    IType GetType(string modelId);

    ReferenceData GetObjectReference(object @object);

    Task<object> GetObjectAsync(ReferenceData reference);
    Task<object> GetObjectAsync(IType type, string id);
}
