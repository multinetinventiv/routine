using System.Web;
using Routine.Core.Rest;
using Routine.Service.HandlerActions.Helper;

namespace Routine.Service.HandlerActions
{
	public class GetHandlerAction : ObjectServiceHandlerActionBase
	{
		private readonly Resolution resolution;

		public GetHandlerAction(IServiceContext serviceContext, IJsonSerializer jsonSerializer, HttpContextBase httpContext, Resolution resolution)
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