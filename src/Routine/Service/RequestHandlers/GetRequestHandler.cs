using Microsoft.AspNetCore.Http;
using Routine.Core.Rest;
using Routine.Service.RequestHandlers.Helper;

namespace Routine.Service.RequestHandlers;

public class GetRequestHandler : ObjectServiceRequestHandlerBase
{
    private readonly Resolution resolution;

    public GetRequestHandler(IServiceContext serviceContext, IJsonSerializer jsonSerializer, IHttpContextAccessor httpContextAccessor, Resolution resolution)
        : base(serviceContext, jsonSerializer, httpContextAccessor)
    {
        this.resolution = resolution;
    }

    protected override bool AllowGet => true;

    protected override async Task<object> Process()
    {
        var objectData = await ServiceContext.ObjectService.GetAsync(resolution.Reference);
        var compressor = new DataCompressor(ApplicationModel, resolution.Reference.ViewModelId);

        return compressor.Compress(objectData);
    }
}
