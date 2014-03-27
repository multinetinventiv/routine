using System.Collections.Generic;
using System.Web.Script.Serialization;
using Routine.Core.Rest;
using Routine.Core.Service;

namespace Routine.Soa
{
	public class RestClientObjectService : IObjectService
	{
		private readonly ISoaClientConfiguration soaClientConfiguration;
		private readonly IRestClient client;
		private readonly JavaScriptSerializer serializer;

		public RestClientObjectService(ISoaClientConfiguration soaClientConfiguration, IRestClient client)
		{
			this.soaClientConfiguration = soaClientConfiguration;
			this.client = client;
			this.serializer = new JavaScriptSerializer();
		}

		private T As<T>(string jsonString)
		{
			if (jsonString.Contains("\"IsException\":true"))
			{
				var exceptionResult = serializer.Deserialize<SoaExceptionResult>(jsonString);

				throw soaClientConfiguration.ExceptionExtractor.Extract(exceptionResult);
			}
			return serializer.Deserialize<T>(jsonString);
		}

		private string Url(string serviceName)
		{
			return soaClientConfiguration.ServiceUrlBase + "/" + serviceName;
		}

		private RestParameter Param(string name, string value)
		{
			return new RestParameter(name, value);
		}

		public ApplicationModel GetApplicationModel()
		{
			return As<ApplicationModel>(client.Get(Url("GetApplicationModel")));
		}

		public List<SingleValueData> GetRootObjects()
		{
			return As<List<SingleValueData>>(client.Get(Url("GetRootObjects")));
		}

		public ObjectModel GetObjectModel(string objectModelId)
		{
			return As<ObjectModel>(client.Get(Url("GetObjectModel"), 
				Param("objectModelId", objectModelId)));
		}

		public List<SingleValueData> GetAvailableObjects(string objectModelId)
		{
			return As<List<SingleValueData>>(client.Get(Url("GetAvailableObjects"), 
				Param("objectModelId", objectModelId)));
		}

		public string GetValue(ObjectReferenceData reference)
		{
			return client.Get(Url("GetValue"), 
				Param("reference.Id", reference.Id), 
				Param("reference.ActualModelId", reference.ActualModelId), 
				Param("reference.ViewModelId", reference.ViewModelId), 
				Param("reference.IsNull", reference.IsNull.ToString()));
		}

		public ObjectData Get(ObjectReferenceData reference)
		{
			return As<ObjectData>(client.Get(Url("Get"), 
				Param("reference.Id", reference.Id), 
				Param("reference.ActualModelId", reference.ActualModelId), 
				Param("reference.ViewModelId", reference.ViewModelId), 
				Param("reference.IsNull", reference.IsNull.ToString())));
		}

		public MemberData GetMember(ObjectReferenceData reference, string memberModelId)
		{
			return As<MemberData>(client.Get(Url("GetMember"), 
				Param("reference.Id", reference.Id), 
				Param("reference.ActualModelId", reference.ActualModelId), 
				Param("reference.ViewModelId", reference.ViewModelId), 
				Param("reference.IsNull", reference.IsNull.ToString()),
				Param("memberModelId", memberModelId)));
		}

		public OperationData GetOperation(ObjectReferenceData reference, string operationModelId)
		{
			return As<OperationData>(client.Get(Url("GetOperation"), 
				Param("reference.Id", reference.Id), 
				Param("reference.ActualModelId", reference.ActualModelId), 
				Param("reference.ViewModelId", reference.ViewModelId), 
				Param("reference.IsNull", reference.IsNull.ToString()),
				Param("operationModelId", operationModelId)));
		}

		public ResultData PerformOperation(ObjectReferenceData targetReference, string operationModelId, List<ParameterValueData> parameterValues)
		{
			var paramList = new List<RestParameter>();
			
			paramList.Add(Param("targetReference.Id", targetReference.Id));
			paramList.Add(Param("targetReference.ActualModelId", targetReference.ActualModelId));
			paramList.Add(Param("targetReference.ViewModelId", targetReference.ViewModelId));
			paramList.Add(Param("targetReference.IsNull", targetReference.IsNull.ToString()));
			
			paramList.Add(Param("operationModelId", operationModelId));

			for (int i = 0; i < parameterValues.Count; i++)
			{
				paramList.Add(Param("parameterValues[" + i + "].ParameterModelId", parameterValues[i].ParameterModelId));
				paramList.Add(Param("parameterValues[" + i + "].Value.IsList", parameterValues[i].Value.IsList.ToString()));

				for (int j = 0; j < parameterValues[i].Value.References.Count; j++)
				{
					paramList.Add(Param("parameterValues[" + i + "].Value.References[" + j + "].Id", parameterValues[i].Value.References[j].Id));
					paramList.Add(Param("parameterValues[" + i + "].Value.References[" + j + "].ActualModelId", parameterValues[i].Value.References[j].ActualModelId));
					paramList.Add(Param("parameterValues[" + i + "].Value.References[" + j + "].ViewModelId", parameterValues[i].Value.References[j].ViewModelId));
					paramList.Add(Param("parameterValues[" + i + "].Value.References[" + j + "].IsNull", parameterValues[i].Value.References[j].IsNull.ToString()));
				}
			}

			return As<ResultData>(client.Post(Url("PerformOperation"), paramList.ToArray()));
		}
	}
}