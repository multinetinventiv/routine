using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Core;
using Routine.Core.Cache;

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
                if (!cache.Contains(Constants.APPLICATION_MODEL_CACHE_KEY))
                {
                    lock (cache)
                    {
                        if (!cache.Contains(Constants.APPLICATION_MODEL_CACHE_KEY))
                        {
                            cache.Add(Constants.APPLICATION_MODEL_CACHE_KEY, new ApplicationModel
                            {
                                Models = ctx.GetDomainTypes().Select(dt => dt.GetModel()).ToList()
                            });
                        }
                    }
                }

                return cache[Constants.APPLICATION_MODEL_CACHE_KEY] as ApplicationModel;
            }
        }

        public ObjectData Get(ReferenceData reference) => ctx.CreateDomainObject(reference).GetObjectData(true);
        public VariableData Do(ReferenceData target, string operation, Dictionary<string, ParameterValueData> parameters) => ctx.CreateDomainObject(target).Perform(operation, parameters);
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

