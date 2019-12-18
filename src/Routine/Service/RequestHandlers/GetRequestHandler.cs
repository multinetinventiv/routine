using System.Web;
using Routine.Core.Rest;
using Routine.Service.RequestHandlers.Helper;

namespace Routine.Service.RequestHandlers
{
	public class GetRequestHandler : ObjectServiceRequestHandlerBase
	{
		private readonly Resolution resolution;

		public GetRequestHandler(IServiceContext serviceContext, IJsonSerializer jsonSerializer, HttpContextBase httpContext, Resolution resolution)
			: base(serviceContext, jsonSerializer, httpContext)
		{
			this.resolution = resolution;
		}

		protected override bool AllowGet => true;

		protected override void Process()
		{
			var objectData = ServiceContext.ObjectService.Get(resolution.Reference);
			var compressor = new DataCompressor(ApplicationModel, resolution.Reference.ViewModelId);

			WriteJsonResponse(compressor.Compress(objectData));
		}
	}
}