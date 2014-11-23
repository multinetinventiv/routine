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

		public ApplicationModel GetApplicationModel()
		{
			if (!cache.Contains(Constants.APPLICATION_MODEL_CACHE_KEY))
			{
				lock (cache)
				{
					if (!cache.Contains(Constants.APPLICATION_MODEL_CACHE_KEY))
					{
						var applicationModel = new ApplicationModel();

						foreach (var type in ctx.CodingStyle.GetTypes())
						{
							if (applicationModel.Models.Any(m => m.Id == ctx.CodingStyle.GetTypeId(type))) { continue; }

							applicationModel.Models.Add(ctx.GetDomainType(type).GetModel());
						}

						foreach (var domainType in ctx.GetDomainTypes())
						{
							if (applicationModel.Models.Any(m => m.Id == domainType.Id)) { continue; }

							applicationModel.Models.Add(domainType.GetModel());
						}

						cache.Add(Constants.APPLICATION_MODEL_CACHE_KEY, applicationModel);
					}
				}
			}

			return cache[Constants.APPLICATION_MODEL_CACHE_KEY] as ApplicationModel;
		}

		public ObjectModel GetObjectModel(string objectModelId)
		{
			return ctx.GetDomainType(objectModelId).GetModel();
		}

		public string GetValue(ObjectReferenceData reference)
		{
			return ctx.CreateDomainObject(reference)
					  .GetValue();
		}

		public ObjectData Get(ObjectReferenceData reference)
		{
			return ctx.CreateDomainObject(reference)
					  .GetObjectData(true);
		}

		public ValueData PerformOperation(ObjectReferenceData targetReference, string operationModelId, Dictionary<string, ParameterValueData> parameters)
		{
			return ctx.CreateDomainObject(targetReference)
					  .Perform(operationModelId, parameters);
		}
	}

	public class MemberDoesNotExistException : Exception
	{
		public MemberDoesNotExistException(string objectModelId, string memberModelId)
			: base("Member '" + memberModelId + "' does not exist on Object '" + objectModelId + "'") { }
	}

	public class OperationDoesNotExistException : Exception
	{
		public OperationDoesNotExistException(string objectModelId, string operationModelId)
			: base("Operation '" + operationModelId + "' does not exist on Object '" + objectModelId + "'") { }
	}

	public class MissingParameterException : Exception
	{
		public MissingParameterException(string objectModelId, string operationModelId, string parameterModelId)
			: base("Parameter '" + parameterModelId + "' was not given for Operation '" + operationModelId + " on Object '" + objectModelId + "'") { }
	}
}

