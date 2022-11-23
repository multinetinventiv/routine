using Routine.Core.Rest;
using Routine.Core;

namespace Routine.Service;

public class RestClientObjectService : IObjectService
{
    private readonly IServiceClientConfiguration _serviceClientConfiguration;
    private readonly IRestClient _restClient;
    private readonly IJsonSerializer _serializer;

    public RestClientObjectService(IServiceClientConfiguration serviceClientConfiguration, IRestClient restClient, IJsonSerializer serializer)
    {
        _serviceClientConfiguration = serviceClientConfiguration;
        _restClient = restClient;
        _serializer = serializer;
    }

    private readonly object _applicationModelLock = new();
    private volatile ApplicationModel _applicationModel;
    public ApplicationModel ApplicationModel
    {
        get
        {
            if (_applicationModel != null) { return _applicationModel; }

            lock (_applicationModelLock)
            {
                if (_applicationModel != null) { return _applicationModel; }

                return _applicationModel = new((IDictionary<string, object>)Result(Get(Url("ApplicationModel"))));
            }
        }
    }

    public ObjectData Get(ReferenceData reference)
    {
        if (reference == null) { return null; }

        var response = Get(Url(reference));
        var result = Result(response);

        return Compressor(reference.ViewModelId).DecompressObjectData(result);
    }

    public async Task<ObjectData> GetAsync(ReferenceData reference)
    {
        if (reference == null) { return null; }

        var response = await GetAsync(Url(reference));
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

        var body = _serializer.Serialize(parameters.ToDictionary(
            kvp => kvp.Key,
            kvp => Compressor(operationModel.Parameter[kvp.Key].ViewModelId).Compress(kvp.Value)
        ));

        return new Tuple<string, string>(body, operationModel.Result.ViewModelId);
    }

    private DataCompressor Compressor(string resultViewModelId) => new(ApplicationModel, resultViewModelId);

    private OperationModel GetOperationModel(ReferenceData target, string operation) =>
        GetObjectModel(target).GetOperation(operation) ?? throw OperationNotFound(target.ViewModelId, operation);

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
        foreach (var processor in _serviceClientConfiguration.GetResponseHeaderProcessors())
        {
            processor.Process(response.Headers);
        }

        var result = _serializer.DeserializeObject(response.Body);

        if (result is IDictionary<string, object> resultDictionary && resultDictionary.ContainsKey("IsException"))
        {
            var exceptionResult = new ExceptionResult(_serializer.Deserialize<ExceptionResultData>(response.Body));

            throw _serviceClientConfiguration.GetException(exceptionResult);
        }

        return result;
    }

    private string Url(ReferenceData referenceData, string operation) => $"{Url(referenceData)}/{operation}";
    private string Url(ReferenceData referenceData) =>
        Url(referenceData.ModelId == referenceData.ViewModelId
            ? $"{referenceData.ModelId}/{referenceData.Id}"
            : $"{referenceData.ModelId}/{referenceData.Id}/{referenceData.ViewModelId}"
        );
    private string Url(string action) => $"{_serviceClientConfiguration.GetServiceUrlBase()}/{action}";

    private RestResponse Get(string url)
    {
        try { return _restClient.Get(url, BuildRequest(string.Empty)); }
        catch (RestRequestException ex) { return Wrap(ex); }
    }

    private async Task<RestResponse> GetAsync(string url)
    {
        try { return await _restClient.GetAsync(url, BuildRequest(string.Empty)); }
        catch (RestRequestException ex) { return Wrap(ex); }
    }

    private RestResponse Post(string url, string body)
    {
        try { return _restClient.Post(url, BuildRequest(body)); }
        catch (RestRequestException ex) { return Wrap(ex); }
    }

    private async Task<RestResponse> PostAsync(string url, string body)
    {
        try { return await _restClient.PostAsync(url, BuildRequest(body)); }
        catch (RestRequestException ex) { return Wrap(ex); }
    }

    private RestRequest BuildRequest(string body) =>
        new RestRequest(body)
            .WithHeaders(
                _serviceClientConfiguration.GetRequestHeaders()
                    .ToDictionary(h => h, h => _serviceClientConfiguration.GetRequestHeaderValue(h))
            );

    private RestResponse Wrap(RestRequestException ex) =>
        new(_serializer.Serialize(new ExceptionResult($"Http.{ex.StatusCode}", ex.Message, false)));

    private Exception OperationNotFound(string modelId, string operation) =>
        _serviceClientConfiguration.GetException(new(nameof(OperationNotFound),
            $"Given operation ({operation}) was not found in given model ({modelId}). Make sure you are connecting to the correct endpoint.",
            false
        ));

    private Exception TypeNotFound(string modelId) =>
        _serviceClientConfiguration.GetException(new(nameof(TypeNotFound),
            $"Given model id ({modelId}) was not found in current application model. Make sure you are connecting to the correct endpoint.",
            false
        ));
}
