using System;
using System.Collections.Generic;
using Routine.Core;
using Routine.Core.Rest;

namespace Routine.Soa
{
	public class RestClientObjectService : IObjectService
	{
		private readonly ISoaClientConfiguration soaClientConfiguration;
		private readonly IRestClient client;
		private readonly IRestSerializer serializer;

		public RestClientObjectService(ISoaClientConfiguration soaClientConfiguration, IRestClient client, IRestSerializer serializer)
		{
			this.soaClientConfiguration = soaClientConfiguration;
			this.client = client;
			this.serializer = serializer;
		}

		private T As<T>(string responseString)
		{
			try
			{
				if (typeof(T) == typeof(ObjectReferenceData)) 
				{ 
					object result = SerializationExtensions.DeserializeObjectReferenceData(serializer.Deserialize(responseString));
					return (T)result;
				}
				else if (typeof(T) == typeof(ObjectData))
				{
					object result = SerializationExtensions.DeserializeObjectData(serializer.Deserialize(responseString));
					return (T)result;
				}
				else if (typeof(T) == typeof(ValueData))
				{
					object result = SerializationExtensions.DeserializeValueData(serializer.Deserialize(responseString));
					return (T)result;
				}

				return serializer.Deserialize<T>(responseString);
			}
			catch (Exception ex)
			{
				SoaExceptionResult exceptionResult = null;

				try { exceptionResult = new SoaExceptionResult(serializer.Deserialize<SoaExceptionResultData>(responseString)); } //assume it is a soa exception
				catch (Exception) { throw ex; } //assumption was wrong, throw first exception

				throw soaClientConfiguration.ExceptionExtractor.Extract(exceptionResult);
			}
		}

		private string Url(string serviceName)
		{
			return soaClientConfiguration.ServiceUrlBase + "/" + serviceName;
		}

		private RestParameter Param(string name, string value)
		{
			return new RestParameter(name, value);
		}

		private RestParameter[] BuildParameters(RestParameter[] parameters)
		{
			var result = new List<RestParameter>();

			foreach (var parameter in soaClientConfiguration.DefaultParameters)
			{
				result.Add(Param(parameter, soaClientConfiguration.DefaultParameterValueExtractor.Extract(parameter)));
			}

			result.AddRange(parameters);

			return result.ToArray();
		}

		private string Post(string serviceName, params RestParameter[] parameters)
		{
			return client.Post(Url(serviceName), BuildParameters(parameters));
		}

		private string Get(string serviceName, params RestParameter[] parameters)
		{
			return client.Get(Url(serviceName), BuildParameters(parameters));
		}

		public ApplicationModel GetApplicationModel()
		{
			return As<ApplicationModel>(Get("GetApplicationModel"));
		}

		public ObjectModel GetObjectModel(string objectModelId)
		{
			return As<ObjectModel>(Get("GetObjectModel", 
				Param("objectModelId", objectModelId)));
		}

		public List<ObjectData> GetAvailableObjects(string objectModelId)
		{
			return As<ValueData>(Get("GetAvailableObjects", 
				Param("objectModelId", objectModelId))).Values;
		}

		public string GetValue(ObjectReferenceData reference)
		{
			return Get("GetValue", 
				Param("reference.Id", reference.Id), 
				Param("reference.ActualModelId", reference.ActualModelId), 
				Param("reference.ViewModelId", reference.ViewModelId), 
				Param("reference.IsNull", reference.IsNull.ToString()));
		}

		public ObjectData Get(ObjectReferenceData reference)
		{
			return As<ObjectData>(Get("Get", 
				Param("reference.Id", reference.Id), 
				Param("reference.ActualModelId", reference.ActualModelId), 
				Param("reference.ViewModelId", reference.ViewModelId), 
				Param("reference.IsNull", reference.IsNull.ToString())));
		}

		public ValueData GetMember(ObjectReferenceData reference, string memberModelId)
		{
			return As<ValueData>(Get("GetMember", 
				Param("reference.Id", reference.Id), 
				Param("reference.ActualModelId", reference.ActualModelId), 
				Param("reference.ViewModelId", reference.ViewModelId), 
				Param("reference.IsNull", reference.IsNull.ToString()),
				Param("memberModelId", memberModelId)));
		}

		public ValueData PerformOperation(ObjectReferenceData targetReference, string operationModelId, Dictionary<string, ReferenceData> parameterValues)
		{
			var paramList = new List<RestParameter>();
			
			paramList.Add(Param("targetReference.Id", targetReference.Id));
			paramList.Add(Param("targetReference.ActualModelId", targetReference.ActualModelId));
			paramList.Add(Param("targetReference.ViewModelId", targetReference.ViewModelId));
			paramList.Add(Param("targetReference.IsNull", targetReference.IsNull.ToString()));
			
			paramList.Add(Param("operationModelId", operationModelId));

			int i = 0;
			foreach (var key in parameterValues.Keys)
			{
				paramList.Add(Param("parameterValues[" + i + "].Key", key));
				paramList.Add(Param("parameterValues[" + i + "].Value.IsList", parameterValues[key].IsList.ToString()));

				for (int j = 0; j < parameterValues[key].References.Count; j++)
				{
					paramList.Add(Param("parameterValues[" + i + "].Value.References[" + j + "].Id", parameterValues[key].References[j].Id));
					paramList.Add(Param("parameterValues[" + i + "].Value.References[" + j + "].ActualModelId", parameterValues[key].References[j].ActualModelId));
					paramList.Add(Param("parameterValues[" + i + "].Value.References[" + j + "].ViewModelId", parameterValues[key].References[j].ViewModelId));
					paramList.Add(Param("parameterValues[" + i + "].Value.References[" + j + "].IsNull", parameterValues[key].References[j].IsNull.ToString()));
				}

				i++;
			}

			return As<ValueData>(Post("PerformOperation", paramList.ToArray()));
		}
	}
}