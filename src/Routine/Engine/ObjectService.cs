using Routine.Core;
using Routine.Core.Cache;
using Routine.Core.Runtime;

using static Routine.Constants;

namespace Routine.Engine;

public class ObjectService : IObjectService
{
    private readonly ICoreContext ctx;
    private readonly ICache cache;

    public ObjectService(ICoreContext ctx, ICache cache)
    {
        this.cache = cache;
        this.ctx = ctx;
    }

    public ApplicationModel ApplicationModel
    {
        get
        {
            var result = (ApplicationModel)cache[CACHE_APPLICATION_MODEL];
            if (result == null)
            {
                lock (cache)
                {
                    if ((result = (ApplicationModel)cache[CACHE_APPLICATION_MODEL]) == null)
                    {
                        ctx.BuildDomainTypes();

                        result = new() { Models = ctx.DomainTypes.Select(dt => dt.GetModel()).ToList() };

                        cache.Add(CACHE_APPLICATION_MODEL, result);
                    }
                }
            }

            return result;
        }
    }

    public ObjectData Get(ReferenceData reference) => ctx.GetDomainObjectAsync(reference).WaitAndGetResult().GetObjectData(true);
    public async Task<ObjectData> GetAsync(ReferenceData reference) => (await ctx.GetDomainObjectAsync(reference)).GetObjectData(true);

    public VariableData Do(ReferenceData target, string operation, Dictionary<string, ParameterValueData> parameters) => ctx.GetDomainObjectAsync(target).WaitAndGetResult().Perform(operation, parameters);
    public async Task<VariableData> DoAsync(ReferenceData target, string operation, Dictionary<string, ParameterValueData> parameters) => await (await ctx.GetDomainObjectAsync(target)).PerformAsync(operation, parameters);
}
