using Routine.Core.Rest;
using Routine.Core;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System;

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

        private ApplicationModel FetchApplicationModel() => new((IDictionary<string, object>)Result(Get(Url("ApplicationModel"))));
        public ApplicationModel ApplicationModel => applicationModel.Value;

        public ObjectData Get(ReferenceData reference)
        {
            if (reference == null) { return null; }

            var response = Get(Url(reference));
            var result = Result(response);

            return Compressor(reference.ViewModelId).DecompressObjectData(result);
        }

        public VariableData Do(ReferenceData target, string operation, Dictionary<string, ParameterValueData> parameters)
        {
            if (target == null) { return new VariableData(); }

            var (body, resultViewModelId) = GetBodyAndResultViewModelId(target, operation, parameters);

            var response = Post(Url(target, operation), body);
            var result = Result(response);

            return Compressor(resultViewModelId).DecompressVariableData(result);
        }

        public async Task<VariableData> DoAsync(ReferenceData target, string operation, Dictionary<string, ParameterValueData> parameters)
        {
            if (target == null) { return new VariableData(); }

            var (body, resultViewModelId) = GetBodyAndResultViewModelId(target, operation, parameters);

            var response = await PostAsync(Url(target, operation), body);
            var result = Result(response);

            return Compressor(resultViewModelId).DecompressVariableData(result);
        }

        private Tuple<string, string> GetBodyAndResultViewModelId(ReferenceData target, string operation, Dictionary<string, ParameterValueData> parameters)
        {
            var operationModel = GetOperationModel(target, operation);

            var body = serializer.Serialize(parameters.ToDictionary(
                kvp => kvp.Key,
                kvp => Compressor(operationModel.Parameter[kvp.Key].ViewModelId).Compress(kvp.Value)
            ));

            return new Tuple<string, string>(body, operationModel.Result.ViewModelId);
        }

        private DataCompressor Compressor(string resultViewModelId) => new(ApplicationModel, resultViewModelId);

        private OperationModel GetOperationModel(ReferenceData target, string operation)
        {
            var operationModel = GetObjectModel(target).GetOperation(operation);

            if (operationModel == null) { throw OperationNotFound(target.ViewModelId, operation); }

            return operationModel;
        }

        private ObjectModel GetObjectModel(ReferenceData target)
        {
            if (!ApplicationModel.Model.TryGetValue(target.ViewModelId, out var objectModel))
            {
                throw TypeNotFound(target.ViewModelId);
            }

            return objectModel;
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

        private string Url(ReferenceData referenceData, string operation) => $"{Url(referenceData)}/{operation}";
        private string Url(ReferenceData referenceData) =>
            Url(referenceData.ModelId == referenceData.ViewModelId
                ? $"{referenceData.ModelId}/{referenceData.Id}"
                : $"{referenceData.ModelId}/{referenceData.Id}/{referenceData.ViewModelId}"
            );
        private string Url(string action) => $"{serviceClientConfiguration.GetServiceUrlBase()}/{action}";

        private RestResponse Get(string url)
        {
            try
            {
                return restClient.Get(url, BuildRequest(string.Empty));
            }
            catch (WebException ex)
            {
                if (ex.Response is not HttpWebResponse res)
                {
                    throw;
                }

                return Wrap(res);
            }
        }

        private RestResponse Post(string url, string body)
        {
            try
            {
                return restClient.Post(url, BuildRequest(body));
            }
            catch (WebException ex)
            {
                if (ex.Response is not HttpWebResponse res)
                {
                    throw;
                }

                return Wrap(res);
            }
        }

        private async Task<RestResponse> PostAsync(string url, string body)
        {
            try
            {
                return await restClient.PostAsync(url, BuildRequest(body));
            }
            catch (WebException ex)
            {
                if (ex.Response is not HttpWebResponse res)
                {
                    throw;
                }

                return Wrap(res);
            }
        }

        private RestRequest BuildRequest(string body) =>
            new RestRequest(body)
                .WithHeaders(
                    serviceClientConfiguration.GetRequestHeaders()
                        .ToDictionary(h => h, h => serviceClientConfiguration.GetRequestHeaderValue(h))
                );

        #region Exceptions

        private RestResponse Wrap(HttpWebResponse res) => new(
            serializer.Serialize(new ExceptionResult($"Http.{res.StatusCode}", res.StatusDescription, false))
        );

        private Exception OperationNotFound(string modelId, string operation) =>
            serviceClientConfiguration.GetException(new ExceptionResult(nameof(OperationNotFound),
                $"Given operation ({operation}) was not found in given model ({modelId}). Make sure you are connecting to the correct endpoint.",
                false
            ));

        private Exception TypeNotFound(string modelId) =>
            serviceClientConfiguration.GetException(new ExceptionResult(nameof(TypeNotFound),
                $"Given model id ({modelId}) was not found in current application model. Make sure you are connecting to the correct endpoint.",
                false
            ));

        #endregion
    }
}
