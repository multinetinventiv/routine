using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Routine.Core;
using Routine.Core.Rest;

namespace Routine.Service
{
	public class ServiceHttpHandler : IHttpHandler
	{
		private readonly IServiceContext serviceContext;
		private readonly IJsonSerializer jsonSerializer;
		private const int DEFAULT_RECURSION_LIMIT = 100;
		private readonly Dictionary<string, List<ObjectModel>> modelIndex;

		private static IJsonSerializer CreateDefaultSerializer(int maxResultLength)
		{
			return new JavaScriptSerializerAdapter(new JavaScriptSerializer
			{
				MaxJsonLength = maxResultLength,
				RecursionLimit = DEFAULT_RECURSION_LIMIT
			});
		}

		public ServiceHttpHandler(IServiceContext serviceContext) : this(serviceContext, CreateDefaultSerializer(serviceContext.ServiceConfiguration.GetMaxResultLength()))
		{
			this.serviceContext = serviceContext;
		}


		public ServiceHttpHandler(IServiceContext serviceContext, IJsonSerializer jsonSerializer)
		{
			this.serviceContext = serviceContext;
			this.jsonSerializer = jsonSerializer;
			modelIndex = new Dictionary<string, List<ObjectModel>>();
		}

		public void ProcessRequest(HttpContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			var routeData = context.Request.RequestContext.RouteData;

			var action = routeData.Values["action"].ToString();

			if (string.Equals(action, nameof(ApplicationModel), StringComparison.InvariantCultureIgnoreCase))
			{
				ApplicationModel(context);
			}
		}


		private void ApplicationModel(HttpContext context)
		{
			IndexApplicationModelIfNecessary();

			context.Response.ContentType = "application/json";
			context.Response.ContentEncoding = Encoding.UTF8;
			context.Response.Write(jsonSerializer.Serialize(serviceContext.ObjectService.ApplicationModel));
		}

		public bool IsReusable => false;

		private void IndexApplicationModelIfNecessary()
		{
			if (modelIndex.Count > 0) { return; }

			var appModel = serviceContext.ObjectService.ApplicationModel;

			foreach (var key in appModel.Model.Keys)
			{
				var shortModelId = key.AfterLast(".");
				if (!modelIndex.ContainsKey(shortModelId))
				{
					modelIndex.Add(shortModelId, new List<ObjectModel>());
				}

				modelIndex[shortModelId].Add(appModel.Model[key]);
			}
		}
	}
}
