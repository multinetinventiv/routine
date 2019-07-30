using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Routing;
using Routine.Core;
using Routine.Core.Rest;
using Routine.Service.HandlerActions;
using Routine.Service.HandlerActions.Exceptions;
using Routine.Service.HandlerActions.Helper;

namespace Routine.Service
{
	public class ServiceRouteHandler : IRoutineRouteHandler
	{
		private readonly IServiceContext serviceContext;
		private readonly IJsonSerializer jsonSerializer;
		private readonly HandlerActionList handlerActions;

		public ServiceRouteHandler(IServiceContext serviceContext, IJsonSerializer jsonSerializer)
		{
			this.serviceContext = serviceContext;
			this.jsonSerializer = jsonSerializer;

			handlerActions = new HandlerActionList(this, serviceContext);
		}

		public virtual void RegisterRoutes()
		{
			handlerActions
				.Add(hcb => new IndexHandlerAction(serviceContext, jsonSerializer, hcb))
				.Add(hcb => new FileHandlerAction(serviceContext, jsonSerializer, hcb))
				.Add(path: "{action}/{fileName}/f", factory: hcb => new FontsHandlerAction(serviceContext, jsonSerializer, hcb))
				.Add(hcb => new ConfigurationHandlerAction(serviceContext, jsonSerializer, hcb))
				.Add(hcb => new ApplicationModelHandlerAction(serviceContext, jsonSerializer, hcb))
				.Add(
					action: "handle",
					path: "{modelId}/{idOrViewModelIdOrOperation}/{viewModelIdOrOperation}/{operation}",
					factory: hcb =>
					{
						var resolution = Resolve(hcb);

						if (resolution == null)
						{
							return new EmptyHandlerAction(serviceContext, jsonSerializer, hcb);
						}

						if (resolution.HasOperation)
						{
							return new DoHandlerAction(serviceContext, jsonSerializer, hcb, resolution);
						}

						return new GetHandlerAction(serviceContext, jsonSerializer, hcb, resolution);
					})
				;
		}

		public virtual ServiceHttpHandler GetHttpHandler() => new ServiceHttpHandler(handlerActions);

		IHttpHandler IRouteHandler.GetHttpHandler(RequestContext requestContext) => GetHttpHandler();

		protected virtual Resolution Resolve(HttpContextBase httpContext)
		{
			var routeData = httpContext.Request.RequestContext.RouteData;

			var modelId = $"{routeData.Values["modelId"]}";
			var idOrViewModelIdOrOperation = $"{routeData.Values["idOrViewModelIdOrOperation"]}";
			var viewModelIdOrOperation = $"{routeData.Values["viewModelIdOrOperation"]}";
			var operation = $"{routeData.Values["operation"]}";

			if (modelId == null) { throw new InvalidOperationException("Handle action does not handle request when modelId is null. Please check your route configuration, handle action should only be called when modelId is available."); }

			var appModel = serviceContext.ObjectService.ApplicationModel;

			ObjectModel model;

			try
			{
				model = FindModel(httpContext, modelId);
			}
			catch (AmbiguousModelException ex)
			{
				httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
				httpContext.Response.StatusDescription = $"More than one model found with given modelId ({modelId}). " +
														 $"Try sending full names. Available models are {string.Join(",", ex.AvailableModels.Select(om => om.Id))}.";
				return null;
			}
			catch (ModelNotFoundException)
			{
				httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
				httpContext.Response.StatusDescription =
					$"Could not resolve modelId or find an existing model from this modelId ({modelId}). " +
					"Make sure given modelId has a corresponding model and url is in one of the following format; " +
					"- serviceurlbase/modelId " + "- serviceurlbase/modelId/id " + "- serviceurlbase/modelId/operation " +
					"- serviceurlbase/modelId/viewModelId " + "- serviceurlbase/modelId/id/operation " +
					"- serviceurlbase/modelId/id/viewModelId " + "- serviceurlbase/modelId/id/viewModelId/operation";

				return null;
			}

			return CreateResolution(
				appModel,
				model,
				idOrViewModelIdOrOperation,
				viewModelIdOrOperation,
				operation
			);
		}

		private Resolution CreateResolution(ApplicationModel appModel, ObjectModel model, string idOrViewModelIdOrOperation, string viewModelIdOrOperation, string operation)
		{
			string id = null;
			string viewModelId = ViewModelId(model, viewModelIdOrOperation) ?? ViewModelId(model, idOrViewModelIdOrOperation) ?? model.Id; //view model is found by looking from right to left

			var viewModel = appModel.Model[viewModelId];

			if (!string.IsNullOrWhiteSpace(operation)) // if operation is not null then idOrViewModelIdOrOperation must be id, since all of four parameters are given
			{
				id = idOrViewModelIdOrOperation;
			}

			// if operation is null then it may be given in other parameters

			else if (!string.IsNullOrWhiteSpace(viewModelIdOrOperation))
			{
				if (viewModel.Operation.ContainsKey(viewModelIdOrOperation)) // if viewModel has an operation named exactly the same as viewModelIdOrOperation then it indicates an operation
				{
					operation = viewModelIdOrOperation;

					if (!viewModel.Id.EndsWith(idOrViewModelIdOrOperation)) // if idOrViewModelIdOrOperation is not like viewModelId then it indicates id
					{
						id = idOrViewModelIdOrOperation;
					}
				}
				else // if viewModelIdOrOperation is not an operation then it must be viewModelId so idOrViewModelIdOrOperation must be id
				{
					id = idOrViewModelIdOrOperation;
				}
			}
			else if (!string.IsNullOrWhiteSpace(idOrViewModelIdOrOperation)) // here we only have one parameter
			{
				if (viewModel.Operation.ContainsKey(idOrViewModelIdOrOperation))
				// if viewModel has an operation named exactly the as idOrViewModelIdOrOperation then it indicates an operation
				{
					operation = idOrViewModelIdOrOperation;
				}
				else if (!viewModel.Id.EndsWith(idOrViewModelIdOrOperation))
				// if idOrViewModelIdOrOperation is not like viewModelId then it indicates id
				{
					id = idOrViewModelIdOrOperation;
				}

				//otherwise idOrViewModelIdOrOperation must have been set as viewmodelid, so here we don't have id or operation name
			}

			return new Resolution(appModel, model.Id, id, viewModelId, operation);
		}

		private static string ViewModelId(ObjectModel model, string viewModelIdOrOperation)
		{
			if (string.IsNullOrWhiteSpace(viewModelIdOrOperation))
			{
				return null;
			}

			return
				model.Id == viewModelIdOrOperation ? model.Id :
					model.ViewModelIds.FirstOrDefault(vmid => vmid == viewModelIdOrOperation) ??
					model.ViewModelIds.FirstOrDefault(vmid => vmid.EndsWith(viewModelIdOrOperation));
		}

		private ObjectModel FindModel(HttpContextBase httpContext, string modelId)
		{
			var appModel = serviceContext.ObjectService.ApplicationModel;

			if (appModel.Model.TryGetValue(modelId, out var result))
			{
				return result;
			}

			GetModelIndex(httpContext).TryGetValue(modelId.AfterLast("."), out var availableModels);

			if (availableModels == null)
			{
				throw new ModelNotFoundException(modelId);
			}

			if (availableModels.Count > 1)
			{
				throw new AmbiguousModelException(availableModels);
			}

			if (availableModels.Count <= 0)
			{
				throw new ModelNotFoundException(modelId);
			}

			return availableModels[0];
		}
		protected virtual Dictionary<string, List<ObjectModel>> GetModelIndex(HttpContextBase httpContext)
		{
			var app = httpContext.Application;

			var result = (Dictionary<string, List<ObjectModel>>)app["Routine.EndPoint.ModelIndex"];

			if (result != null) { return result; }

			app.Lock();

			result = (Dictionary<string, List<ObjectModel>>)app["Routine.EndPoint.ModelIndex"]; ;

			if (result != null)
			{
				app.UnLock();

				return result;
			}

			result = BuildModelIndex();

			app["Routine.EndPoint.ModelIndex"] = result;

			app.UnLock();

			return result;
		}

		private Dictionary<string, List<ObjectModel>> BuildModelIndex()
		{
			var result = new Dictionary<string, List<ObjectModel>>();
			var appModel = serviceContext.ObjectService.ApplicationModel;

			foreach (var key in appModel.Model.Keys)
			{
				var shortModelId = key.AfterLast(".");
				if (!result.ContainsKey(shortModelId))
				{
					result.Add(shortModelId, new List<ObjectModel>());
				}

				result[shortModelId].Add(appModel.Model[key]);
			}

			return result;
		}
	}
}
