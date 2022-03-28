using Microsoft.AspNetCore.Http;
using Routine.Core.Rest;
using Routine.Service.RequestHandlers.Helper;
using System.Threading.Tasks;

namespace Routine.Service.RequestHandlers
{
    public class GetRequestHandler : ObjectServiceRequestHandlerBase
    {
        private readonly Resolution resolution;

        public GetRequestHandler(IServiceContext serviceContext, IJsonSerializer jsonSerializer, IHttpContextAccessor httpContextAccessor, Resolution resolution)
            : base(serviceContext, jsonSerializer, httpContextAccessor)
        {
            this.resolution = resolution;
        }

        protected override bool AllowGet => true;

        protected override Task<object> Process()
        {
            var objectData = ServiceContext.ObjectService.Get(resolution.Reference);
            var compressor = new DataCompressor(ApplicationModel, resolution.Reference.ViewModelId);

            return Task.FromResult(compressor.Compress(objectData));
        }
    }
}
