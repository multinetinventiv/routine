using Routine.Core.Cache;
using Routine.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using static Routine.Constants;

namespace Routine.Engine
{
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
                if (!cache.Contains(CACHE_APPLICATION_MODEL))
                {
                    lock (cache)
                    {
                        if (!cache.Contains(CACHE_APPLICATION_MODEL))
                        {
                            cache.Add(CACHE_APPLICATION_MODEL, new ApplicationModel
                            {
                                Models = ctx.GetDomainTypes().Select(dt => dt.GetModel()).ToList()
                            });
                        }
                    }
                }

                return cache[CACHE_APPLICATION_MODEL] as ApplicationModel;
            }
        }

        public ObjectData Get(ReferenceData reference) => ctx.CreateDomainObject(reference).GetObjectData(true);
        public VariableData Do(ReferenceData target, string operation, Dictionary<string, ParameterValueData> parameters) => ctx.CreateDomainObject(target).Perform(operation, parameters);
        public async Task<VariableData> DoAsync(ReferenceData target, string operation, Dictionary<string, ParameterValueData> parameters) => await ctx.CreateDomainObject(target).PerformAsync(operation, parameters);
    }

    public class DataDoesNotExistException : Exception
    {
        public DataDoesNotExistException(string objectModelId, string dataName)
            : base("Data '" + dataName + "' does not exist on Object '" + objectModelId + "'") { }
    }

    public class OperationDoesNotExistException : Exception
    {
        public OperationDoesNotExistException(string objectModelId, string operationName)
            : base("Operation '" + operationName + "' does not exist on Object '" + objectModelId + "'") { }
    }

    public class MissingParameterException : Exception
    {
        public MissingParameterException(string objectModelId, string operationName, string parameterName)
            : base("Parameter '" + parameterName + "' was not given for Operation '" + operationName + " on Object '" + objectModelId + "'") { }
    }
}
