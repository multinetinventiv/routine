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
				return serializer.Deserialize<T>(responseString);
			}
			catch (Exception)
			{
				SoaExceptionResult exceptionResult = null;

				var isExceptionResult = true;
				try
				{
					//assume it is a soa exception
					exceptionResult = new SoaExceptionResult(serializer.Deserialize<SoaExceptionResultData>(responseString));
				} 
				catch (Exception) { isExceptionResult = false; }
				
				if (!isExceptionResult) { throw; } //assumption was wrong, throw first exception

				throw soaClientConfiguration.GetException(exceptionResult);
			}
		}

		private string Url(string serviceName)
		{
			return soaClientConfiguration.GetServiceUrlBase() + "/" + serviceName;
		}

		private RestParameter Param(string name, string value)
		{
			return new RestParameter(name, value);
		}

		private RestParameter[] BuildParameters(RestParameter[] parameters)
		{
			var result = new List<RestParameter>();

			foreach (var parameter in soaClientConfiguration.GetHeaders())
			{
				result.Add(Param(parameter, soaClientConfiguration.GetHeaderValue(parameter)));
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

		public ValueData PerformOperation(ObjectReferenceData targetReference, string operationModelId, Dictionary<string, ParameterValueData> parameterValues)
		{
			var paramList = new List<RestParameter>();

			paramList.Add(Param("targetReference.Id", targetReference.Id));
			paramList.Add(Param("targetReference.ActualModelId", targetReference.ActualModelId));
			paramList.Add(Param("targetReference.ViewModelId", targetReference.ViewModelId));
			paramList.Add(Param("targetReference.IsNull", targetReference.IsNull.ToString()));

			paramList.Add(Param("operationModelId", operationModelId));

			paramList.Add(Param("parameters", serializer.Serialize(parameterValues)));

			return As<ValueData>(Post("Perform", paramList.ToArray()));
		}
	}
}