using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Routine.Core.Rest;
using Routine.Service.RequestHandlers.Helper;
using System.Threading.Tasks;

namespace Routine.Service.RequestHandlers
{
    public class GetRequestHandler : ObjectServiceRequestHandlerBase
	{
		private readonly Resolution resolution;

		public GetRequestHandler(IServiceContext serviceContext, IJsonSerializer jsonSerializer, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache, Resolution resolution)
			: base(serviceContext, jsonSerializer, httpContextAccessor, memoryCache)
		{
			this.resolution = resolution;
		}

		protected override bool AllowGet => true;

		protected override async Task Process()
		{
			var objectData = ServiceContext.ObjectService.Get(resolution.Reference);
			var compressor = new DataCompressor(ApplicationModel, resolution.Reference.ViewModelId);

			await WriteJsonResponse(compressor.Compress(objectData));
		}
	}
}