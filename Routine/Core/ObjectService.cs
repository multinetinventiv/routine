using System;
using System.Collections.Generic;
using System.Linq;
using Routine.Core;

namespace Routine.Core
{
	public class ObjectService : IObjectService
	{
		private const string APPLICATION_MODEL_KEY = "_application_model";

		private readonly ICoreContext ctx;
		private readonly ICache cache;

		public ObjectService(ICoreContext context, ICache cache)
		{
			this.cache = cache;
			this.ctx = context;
		}

		public ApplicationModel GetApplicationModel()
		{
			if(!cache.Contains(APPLICATION_MODEL_KEY))
			{
				lock(cache)
				{
					if(!cache.Contains(APPLICATION_MODEL_KEY))
					{
						var applicationModel = new ApplicationModel();

						foreach(var type in TypeInfo.GetAllDomainTypes())
						{
							try
							{
								var objectModelId = ctx.CodingStyle.ModelIdSerializer.Serialize(type);

								if(applicationModel.Models.Any(m => m.Id == objectModelId)){continue;}

								var objectModel = ctx.GetDomainType(objectModelId).GetModel();

								if(objectModel.IsValueModel){continue;}

								applicationModel.Models.Add(objectModel);
							}
							catch(CannotSerializeDeserializeException) { continue; }
						}

						cache.Add(APPLICATION_MODEL_KEY, applicationModel);
					}
				}
			}

			return cache[APPLICATION_MODEL_KEY] as ApplicationModel;
		}

		public ObjectModel GetObjectModel(string objectModelId)
		{
			return ctx.GetDomainType(objectModelId).GetModel();
		}

		public List<ObjectData> GetAvailableObjects(string objectModelId)
		{
			return ctx.GetDomainType(objectModelId)
					  .GetAvailableObjects()
					  .Select(o => o.GetSingleValue())
					  .ToList();
		}

		public string GetValue(ObjectReferenceData reference)
		{
			return ctx.GetDomainObject(reference)
					  .GetValue();
		}

		public ObjectData Get(ObjectReferenceData reference)
		{
			return ctx.GetDomainObject(reference)
					  .GetObject();
		}

		public ValueData GetMember(ObjectReferenceData reference, string memberModelId)
		{
			return ctx.GetDomainObject(reference)
					  .GetMember(memberModelId);
		}

		public ValueData PerformOperation(ObjectReferenceData targetReference, string operationModelId, Dictionary<string, ReferenceData> parameters)
		{
			return ctx.GetDomainObject(targetReference)
					  .Perform(operationModelId, parameters);
		}
	}

	public class MemberDoesNotExistException : Exception
	{
		public MemberDoesNotExistException(string objectModelId, string memberModelId) 
			: base("Member '" + memberModelId + "' does not exist on Object '" + objectModelId + "'") {}
	}

	public class OperationDoesNotExistException : Exception
	{
		public OperationDoesNotExistException(string objectModelId, string operationModelId) 
			: base("Operation '" + operationModelId + "' does not exist on Object '" + objectModelId + "'") {}
	}

	public class MissingParameterException : Exception
	{
		public MissingParameterException(string objectModelId, string operationModelId, string parameterModelId)
			: base("Parameter '" + parameterModelId + "' was not given for Operation '" + operationModelId + " on Object '" + objectModelId + "'") {}
	}
}

