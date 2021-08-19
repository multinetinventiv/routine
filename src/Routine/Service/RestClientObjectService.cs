using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Routine.Core;
using Routine.Core.Rest;

namespace Routine.Service
{
    public class RestClientObjectService : IObjectService
    {
        private readonly IServiceClientConfiguration serviceClientConfiguration;
        private readonly IRestClient restClient;
        private readonly IJsonSerializer serializer;
        private readonly Lazy<ApplicationModel> applicationModel;

        public RestClientObjectService(IServiceClientConfiguration serviceClientConfiguration, IRestClient restClient, IJsonSerializer serializer)
        {
            this.serviceClientConfiguration = serviceClientConfiguration;
            this.restClient = restClient;
            this.serializer = serializer;

            applicationModel = new Lazy<ApplicationModel>(FetchApplicationModel);
        }

        private ApplicationModel FetchApplicationModel()
        {
            return new ApplicationModel((IDictionary<string, object>)Result(Get(Url("ApplicationModel"))));
        }

        public ApplicationModel ApplicationModel { get { return applicationModel.Value; } }

        public ObjectData Get(ReferenceData reference)
        {
            if (reference == null) { return null; }

            var helper = new DataCompressor(ApplicationModel, reference.ViewModelId);

            return helper.DecompressObjectData(Result(Get(Url(reference))));
        }

        public VariableData Do(ReferenceData target, string operation, Dictionary<string, ParameterValueData> parameters)
        {
            if (target == null) { return new VariableData(); }

            var operationModel = GetOperationModel(target, operation);

            var body = serializer.Serialize(parameters.ToDictionary(kvp => kvp.Key, kvp =>
                new DataCompressor(ApplicationModel, operationModel.Parameter[kvp.Key].ViewModelId)
                    .Compress(kvp.Value)
                ));

            var result = Result(Post(Url(target, operation), body));

            var helper = new DataCompressor(ApplicationModel, operationModel.Result.ViewModelId);

            return helper.DecompressVariableData(result);
        }

        private ObjectModel GetObjectModel(ReferenceData target)
        {
            ObjectModel objectModel;

            if (!ApplicationModel.Model.TryGetValue(target.ViewModelId, out objectModel))
            {
                throw TypeNotFound(target.ViewModelId);
            }

            return objectModel;
        }

        private OperationModel GetOperationModel(ReferenceData target, string operation)
        {
            OperationModel operationModel;

            if (!GetObjectModel(target).Operation.TryGetValue(operation, out operationModel))
            {
                throw OperationNotFound(target.ViewModelId, operation);
            }

            return operationModel;
        }

        private object Result(RestResponse response)
        {
            foreach (var processor in serviceClientConfiguration.GetResponseHeaderProcessors())
            {
                processor.Process(response.Headers);
            }

            var result = serializer.DeserializeObject(response.Body);

            if (result is IDictionary<string, object> resultDictionary && resultDictionary.ContainsKey("IsException"))
            {
                var exceptionResult = new ExceptionResult(serializer.Deserialize<ExceptionResultData>(response.Body));

                throw serviceClientConfiguration.GetException(exceptionResult);
            }

            return result;
        }

        private string Url(string action)
        {
            return string.Format("{0}/{1}",
                serviceClientConfiguration.GetServiceUrlBase(),
                action);
        }

        private string Url(ReferenceData referenceData)
        {
            if (referenceData.ModelId == referenceData.ViewModelId)
            {
                return Url(string.Format("{0}/{1}",
                    referenceData.ModelId,
                    referenceData.Id));
            }

            return Url(string.Format("{0}/{1}/{2}",
                referenceData.ModelId,
                referenceData.Id,
                referenceData.ViewModelId));
        }

        private string Url(ReferenceData referenceData, string operation)
        {
            return string.Format("{0}/{1}", Url(referenceData), operation);
        }

        private RestResponse Post(string url, string body)
        {
            try
            {
                return restClient.Post(url, BuildRequest(body));
            }
            catch (WebException ex)
            {
                var res = ex.Response as HttpWebResponse;
                if (res == null)
                {
                    throw;
                }

                return Wrap(res);
            }
        }

        private RestResponse Get(string url)
        {
            try
            {
                return restClient.Get(url, BuildRequest(string.Empty));
            }
            catch (WebException ex)
            {
                var res = ex.Response as HttpWebResponse;
                if (res == null)
                {
                    throw;
                }

                return Wrap(res);
            }
        }

        private RestRequest BuildRequest(string body)
        {
            return new RestRequest(body)
                .WithHeaders(
                    serviceClientConfiguration.GetRequestHeaders()
                        .ToDictionary(h => h, h => serviceClientConfiguration.GetRequestHeaderValue(h))
                );
        }

        #region Exceptions

        private RestResponse Wrap(HttpWebResponse res)
        {
            return new RestResponse(
                serializer.Serialize(new ExceptionResult(string.Format("Http.{0}", res.StatusCode), res.StatusDescription, false))
                );
        }

        private Exception OperationNotFound(string modelId, string operation)
        {
            return serviceClientConfiguration.GetException(new ExceptionResult("OperationNotFound",
                string.Format(
                    "Given operation ({0}) was not found in given model ({1}). Make sure you are connecting to the correct endpoint.",
                    operation, modelId), false));
        }

        private Exception TypeNotFound(string modelId)
        {
            return serviceClientConfiguration.GetException(new ExceptionResult("TypeNotFound",
                string.Format(
                    "Given model id ({0}) was not found in current application model. Make sure you are connecting to the correct endpoint.",
                    modelId), false));
        }

        #endregion
    }
}