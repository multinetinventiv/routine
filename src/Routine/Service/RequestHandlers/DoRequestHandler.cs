using Microsoft.AspNetCore.Http;
using Routine.Core.Rest;
using Routine.Core;
using Routine.Engine.Context;
using Routine.Service.RequestHandlers.Exceptions;
using Routine.Service.RequestHandlers.Helper;

namespace Routine.Service.RequestHandlers;

public class DoRequestHandler : ObjectServiceRequestHandlerBase
{
    private readonly Resolution _resolution;

    public DoRequestHandler(IServiceContext serviceContext, IJsonSerializer jsonSerializer, IHttpContextAccessor httpContextAccessor, Resolution resolution)
        : base(serviceContext, jsonSerializer, httpContextAccessor)
    {
        _resolution = resolution;
    }

    protected override bool AllowGet => ServiceContext.ServiceConfiguration.GetAllowGet(_resolution.ViewModel, _resolution.OperationModel);
    protected override async Task<object> Process()
    {
        var appModel = ApplicationModel;

        Dictionary<string, ParameterValueData> parameterValues;
        try
        {
            parameterValues = (await GetParameterDictionary())
                .Where(kvp => _resolution.OperationModel.Parameter.ContainsKey(kvp.Key))
                .ToDictionary(kvp => kvp.Key, kvp =>
                    new DataCompressor(appModel, _resolution.OperationModel.Parameter[kvp.Key].ViewModelId)
                        .DecompressParameterValueData(kvp.Value)
                );
        }
        catch (TypeNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ex);
        }

        var variableData = await ServiceContext.ObjectService.DoAsync(_resolution.Reference, _resolution.OperationModel.Name, parameterValues);
        var compressor = new DataCompressor(appModel, _resolution.OperationModel.Result.ViewModelId);

        return compressor.Compress(variableData);
    }

    private async Task<IDictionary<string, object>> GetParameterDictionary()
    {
        if (IsGet)
        {
            return QueryString.Keys.ToDictionary(s => s, s => QueryString[s] as object);
        }

        HttpContext.Request.EnableBuffering();
        var req = HttpContext.Request.Body;
        req.Seek(0, SeekOrigin.Begin);
        var requestBody = await new StreamReader(req).ReadToEndAsync();

        return string.IsNullOrWhiteSpace(requestBody)
            ? new Dictionary<string, object>()
            : JsonSerializer.Deserialize<Dictionary<string, object>>(requestBody);
    }
}
