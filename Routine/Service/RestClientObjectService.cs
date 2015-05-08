using System;
using System.Collections.Generic;
using Routine.Core;
using Routine.Core.Rest;

namespace Routine.Service
{
	public class RestClientObjectService : IObjectService
	{
		private readonly IServiceClientConfiguration serviceClientConfiguration;
		private readonly IRestClient restClient;
		private readonly SerializerHelper serializer;

		public RestClientObjectService(IServiceClientConfiguration serviceClientConfiguration, IRestClient restClient, IJsonSerializer serializer)
		{
			this.serviceClientConfiguration = serviceClientConfiguration;
			this.restClient = restClient;
			this.serializer = new SerializerHelper(serializer);
		}

		private T Result<T>(RestResponse response)
		{
			try
			{
				return serializer.Deserialize<T>(response.Body);
			}
			catch (Exception)
			{
				ExceptionResult exceptionResult = null;

				var isExceptionResult = true;
				try
				{
					//assume it is an exception
					exceptionResult = new ExceptionResult(serializer.Deserialize<ExceptionResultData>(response.Body));
				} 
				catch (Exception) { isExceptionResult = false; }
				
				if (!isExceptionResult) { throw; } //assumption was wrong, throw first exception

				throw serviceClientConfiguration.GetException(exceptionResult);
			}
		}

		private string Url(string serviceName)
		{
			return serviceClientConfiguration.GetServiceUrlBase() + "/" + serviceName;
		}

		private RestParameter Param(string name, string value)
		{
			return new RestParameter(name, value);
		}

		private RestParameter[] BuildParameters(RestParameter[] parameters)
		{
			var result = new List<RestParameter>();

			foreach (var parameter in serviceClientConfiguration.GetRequestHeaders())
			{
				result.Add(Param(parameter, serviceClientConfiguration.GetRequestHeaderValue(parameter)));
			}

			result.AddRange(parameters);

			return result.ToArray();
		}

		private RestResponse Post(string serviceName, params RestParameter[] parameters)
		{
			return restClient.Post(Url(serviceName), BuildParameters(parameters));
		}

		private RestResponse Get(string serviceName, params RestParameter[] parameters)
		{
			return restClient.Get(Url(serviceName), BuildParameters(parameters));
		}

		public ApplicationModel GetApplicationModel()
		{
			return Result<ApplicationModel>(Get("GetApplicationModel"));
		}

		public ObjectModel GetObjectModel(string objectModelId)
		{
			return Result<ObjectModel>(Get("GetObjectModel",
				Param("objectModelId", objectModelId)));
		}

		public string GetValue(ObjectReferenceData reference)
		{
			return Result<string>(Get("GetValue",
				Param("reference.Id", reference.Id),
				Param("reference.ActualModelId", reference.ActualModelId),
				Param("reference.ViewModelId", reference.ViewModelId),
				Param("reference.IsNull", reference.IsNull.ToString())));
		}

		public ObjectData Get(ObjectReferenceData reference)
		{
			return Result<ObjectData>(Get("Get",
				Param("reference.Id", reference.Id),
				Param("reference.ActualModelId", reference.ActualModelId),
				Param("reference.ViewModelId", reference.ViewModelId),
				Param("reference.IsNull", reference.IsNull.ToString())));
		}

		public ValueData PerformOperation(ObjectReferenceData targetReference, string operationModelId, Dictionary<string, ParameterValueData> parameterValues)
		{
			var paramList = new List<RestParameter>();

			paramList.Add(Param("targetReference.Id", targetReference.Id));
			paramList.Add(Param("targetReference.ActualModelId", targetReference.ActualModelId));
			paramList.Add(Param("targetReference.ViewModelId", targetReference.ViewModelId));
			paramList.Add(Param("targetReference.IsNull", targetReference.IsNull.ToString()));

			paramList.Add(Param("operationModelId", operationModelId));

			paramList.Add(Param("parameters", serializer.Serialize(parameterValues)));

			var response = Post("Perform", paramList.ToArray());
			var result = Result<ValueData>(response);

			foreach (var processor in serviceClientConfiguration.GetResponseHeaderProcessors())
			{
				processor.Process(response.Headers);
			}

			return result;
		}
	}
}