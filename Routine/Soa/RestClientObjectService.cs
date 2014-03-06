using System;
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

		public RestClientObjectService(ISoaClientConfiguration soaClientConfiguration, IRestClient client)
		{
			this.soaClientConfiguration = soaClientConfiguration;
			this.client = client;
		}

		private T As<T>(string jsonString)
		{
			return new JavaScriptSerializer().Deserialize<T>(jsonString);
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
			throw new NotImplementedException();
		}
	}
}