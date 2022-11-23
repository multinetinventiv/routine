using Microsoft.AspNetCore.Http;
using Routine.Core.Rest;
using Routine.Service.RequestHandlers.Helper;

namespace Routine.Service.RequestHandlers;

public class GetRequestHandler : ObjectServiceRequestHandlerBase
{
    private readonly Resolution _resolution;

    public GetRequestHandler(IServiceContext serviceContext, IJsonSerializer jsonSerializer, IHttpContextAccessor httpContextAccessor, Resolution resolution)
        : base(serviceContext, jsonSerializer, httpContextAccessor)
    {
        _resolution = resolution;
    }

    protected override bool AllowGet => true;

    protected override async Task<object> Process()
    {
        var objectData = await ServiceContext.ObjectService.GetAsync(_resolution.Reference);
        var compressor = new DataCompressor(ApplicationModel, _resolution.Reference.ViewModelId);

        return compressor.Compress(objectData);
    }
}
