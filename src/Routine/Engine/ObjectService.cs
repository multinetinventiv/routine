using Routine.Core;
using Routine.Core.Cache;
using Routine.Core.Runtime;

using static Routine.Constants;

namespace Routine.Engine;

public class ObjectService : IObjectService
{
    private readonly ICoreContext _ctx;
    private readonly ICache _cache;

    public ObjectService(ICoreContext ctx, ICache cache)
    {
        _cache = cache;
        _ctx = ctx;
    }

    public ApplicationModel ApplicationModel
    {
        get
        {
            var result = (ApplicationModel)_cache[CACHE_APPLICATION_MODEL];
            if (result == null)
            {
                lock (_cache)
                {
                    if ((result = (ApplicationModel)_cache[CACHE_APPLICATION_MODEL]) == null)
                    {
                        _ctx.BuildDomainTypes();

                        result = new() { Models = _ctx.DomainTypes.Select(dt => dt.GetModel()).ToList() };

                        _cache.Add(CACHE_APPLICATION_MODEL, result);
                    }
                }
            }

            return result;
        }
    }

    public ObjectData Get(ReferenceData reference) => _ctx.GetDomainObjectAsync(reference).WaitAndGetResult().GetObjectData(true);
    public async Task<ObjectData> GetAsync(ReferenceData reference) => (await _ctx.GetDomainObjectAsync(reference)).GetObjectData(true);

    public VariableData Do(ReferenceData target, string operation, Dictionary<string, ParameterValueData> parameters) => _ctx.GetDomainObjectAsync(target).WaitAndGetResult().Perform(operation, parameters);
    public async Task<VariableData> DoAsync(ReferenceData target, string operation, Dictionary<string, ParameterValueData> parameters) => await (await _ctx.GetDomainObjectAsync(target)).PerformAsync(operation, parameters);
}
