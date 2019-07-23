﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Routine.Core;
using Routine.Core.Rest;
using Routine.Engine.Context;

namespace Routine.Service
{
	public class ServiceHttpHandler : IHttpHandler
	{
		#region Properties & Variables

		private const int BUFFER_SIZE = 0x1000;
		private const string JSON_CONTENT_TYPE = "application/json";

		private readonly Dictionary<string, List<ObjectModel>> modelIndex;

		private string UrlBase => "/Handler";
		private bool IsGet(HttpContext httpContext) => "GET".Equals(httpContext.Request.HttpMethod, StringComparison.InvariantCultureIgnoreCase);
		private bool IsPost(HttpContext httpContext) => "POST".Equals(httpContext.Request.HttpMethod, StringComparison.InvariantCultureIgnoreCase);
		private Encoding DefaultContentEncoding => Encoding.UTF8;

		public static string IndexAction => "Index";
		public static string HandleAction => "Handle";
		public static string ApplicationModelAction => "ApplicationModel";
		public static string ConfigurationAction => "Configuration";
		public static string FileAction => "File";
		public static string FontsAction => "Fonts";

		#endregion

		#region Constructor

		private readonly IServiceContext serviceContext;
		private readonly IJsonSerializer jsonSerializer;

		public ServiceHttpHandler(IServiceContext serviceContext, IJsonSerializer jsonSerializer)
		{
			this.serviceContext = serviceContext;
			this.jsonSerializer = jsonSerializer;

			modelIndex = new Dictionary<string, List<ObjectModel>>();
		}

		#endregion

		#region Interface Implementations
		public bool IsReusable => false;

		public void ProcessRequest(HttpContext httpContext)
		{
			var routeData = httpContext.Request.RequestContext.RouteData;

			var action = $"{routeData.Values["action"]}";

			if (string.Equals(action, nameof(ApplicationModel), StringComparison.InvariantCultureIgnoreCase))
			{
				ApplicationModel(httpContext);
			}

			else if (string.Equals(action, nameof(Index), StringComparison.InvariantCultureIgnoreCase))
			{
				Index(httpContext);
			}

			else if (string.Equals(action, nameof(File), StringComparison.InvariantCultureIgnoreCase))
			{
				var path = $"{httpContext.Request.QueryString["path"]}";
				File(httpContext, path);
			}

			else if (string.Equals(action, nameof(Configuration), StringComparison.InvariantCultureIgnoreCase))
			{
				Configuration(httpContext);
			}

			else if (string.Equals(action, nameof(Fonts), StringComparison.InvariantCultureIgnoreCase))
			{
				var fileName = $"{routeData.Values["fileName"]}";
				Fonts(httpContext, fileName);
			}

			else if (string.Equals(action, nameof(Handle), StringComparison.InvariantCultureIgnoreCase))
			{
				var modelId = $"{routeData.Values["modelId"]}";
				var idOrViewModelIdOrOperation = $"{routeData.Values["idOrViewModelIdOrOperation"]}";
				var viewModelIdOrOperation = $"{routeData.Values["viewModelIdOrOperation"]}";
				var operation = $"{routeData.Values["operation"]}";

				Handle(httpContext, modelId, idOrViewModelIdOrOperation, viewModelIdOrOperation, operation);
			}
		}
		#endregion

		#region Actions

		private void ApplicationModel(HttpContext httpContext)
		{
			IndexApplicationModelIfNecessary();

			Response(httpContext, jsonSerializer.Serialize(serviceContext.ObjectService.ApplicationModel));
		}

		private void Index(HttpContext httpContext)
		{
			File(httpContext, "app/application/index.html");
		}

		private void File(HttpContext httpContext, string path)
		{
			var stream = GetStream(path);

			var sr = new StreamReader(stream);

			var fileContent = sr.ReadToEnd();
			sr.Close();
			stream.Close();

			fileContent = fileContent.Replace("$urlbase$", UrlBase);

			var file = Encoding.UTF8.GetBytes(fileContent);
			httpContext.Response.ContentType = MimeTypeMap.GetMimeType(path.AfterLast("."));
			httpContext.Response.BinaryWrite(file);
		}

		private void Configuration(HttpContext httpContext)
		{
			Response(httpContext, jsonSerializer.Serialize(new
			{
				url = UrlBase,
				requestHeaders = serviceContext.ServiceConfiguration.GetRequestHeaders(),
				responseHeaders = serviceContext.ServiceConfiguration.GetResponseHeaders()
			}));
		}

		private void Fonts(HttpContext context, string fileName)
		{
			var stream = GetStream("assets/fonts/" + fileName);

			var outputStream = context.Response.OutputStream;
			using (stream)
			{
				var buffer = new byte[BUFFER_SIZE];

				while (true)
				{
					var bytesRead = stream.Read(buffer, 0, BUFFER_SIZE);
					if (bytesRead == 0)
					{
						break;
					}

					outputStream.Write(buffer, 0, bytesRead);
				}
			}
			context.Response.ContentType = MimeTypeMap.GetMimeType(fileName);
			context.Response.Flush();
			context.Response.End();
		}

		private void Handle(HttpContext httpContext, string modelId, string idOrViewModelIdOrOperation, string viewModelIdOrOperation, string operation)
		{
			try
			{
				IndexApplicationModelIfNecessary();

				if (modelId == null) { throw new InvalidOperationException("Handle action does not handle request when modelId is null. Please check your route configuration, handle action should only be called when modelId is available."); }

				var appModel = serviceContext.ObjectService.ApplicationModel;

				ObjectModel model;

				try
				{
					model = FindModel(modelId);
				}
				catch (AmbiguousModelException ex)
				{
					AmbiguousModel(httpContext, modelId, ex.AvailableModels);
					return;
				}
				catch (ModelNotFoundException ex)
				{
					TargetModelNotFound(httpContext, ex.ModelId);
					return;
				}

				var resolution = Resolve(appModel, model, idOrViewModelIdOrOperation, viewModelIdOrOperation, operation);

				if (resolution.HasOperation)
				{
					var allowGet = serviceContext.ServiceConfiguration.GetAllowGet(resolution.ViewModel, resolution.OperationModel);

					if (!IsPost(httpContext) && !IsGet(httpContext)) { MethodNotAllowed(httpContext, allowGet); return; }
					if (IsGet(httpContext) && !allowGet) { MethodNotAllowed(httpContext, false); return; }

					Dictionary<string, ParameterValueData> parameterValues;
					try
					{
						parameterValues = GetParameterDictionary(httpContext)
							.Where(kvp => resolution.OperationModel.Parameter.ContainsKey(kvp.Key))
							.ToDictionary(kvp => kvp.Key, kvp =>
								new DataCompressor(appModel, resolution.OperationModel.Parameter[kvp.Key].ViewModelId)
									.DecompressParameterValueData(kvp.Value)
							);
					}
					catch (TypeNotFoundException ex)
					{
						ModelNotFound(httpContext, ex);
						return;
					}
					catch (Exception ex)
					{
						BadRequest(httpContext, ex);
						return;
					}

					ProcessRequestHeaders(httpContext);

					var variableData = serviceContext.ObjectService.Do(resolution.Reference, resolution.OperationModel.Name, parameterValues);
					var compressor = new DataCompressor(appModel, resolution.OperationModel.Result.ViewModelId);

					Response(httpContext, jsonSerializer.Serialize(compressor.Compress(variableData)));
				}
				else
				{
					if (!IsPost(httpContext) && !IsGet(httpContext)) { MethodNotAllowed(httpContext, true); }

					ProcessRequestHeaders(httpContext);

					var objectData = serviceContext.ObjectService.Get(resolution.Reference);
					var compressor = new DataCompressor(appModel, resolution.Reference.ViewModelId);

					Response(httpContext, jsonSerializer.Serialize(compressor.Compress(objectData)));
				}

				AddResponseHeaders(httpContext);
			}
			catch (Exception ex)
			{
				var exceptionResult = jsonSerializer.Serialize(serviceContext.ServiceConfiguration.GetExceptionResult(ex));
				httpContext.Server.ClearError();
				Response(httpContext, exceptionResult);
			}
		}

		#endregion
		private void Response(HttpContext httpContext, string result) { Response(httpContext, result, HttpStatusCode.OK); }

		private void Response(HttpContext httpContext, string result, HttpStatusCode statusCode)
		{
			httpContext.Response.StatusCode = (int)statusCode;
			httpContext.Response.ContentType = JSON_CONTENT_TYPE;
			httpContext.Response.ContentEncoding = DefaultContentEncoding;
			httpContext.Response.Write(result);
		}

		private IDictionary<string, object> GetParameterDictionary(HttpContext httpContext)
		{
			if (IsGet(httpContext))
			{
				return httpContext.Request.QueryString.AllKeys.ToDictionary(s => s, s => httpContext.Request.QueryString[s] as object);
			}

			var req = httpContext.Request.InputStream;
			req.Seek(0, SeekOrigin.Begin);
			var requestBody = new StreamReader(req).ReadToEnd();

			return (IDictionary<string, object>)jsonSerializer.DeserializeObject(requestBody) ?? new Dictionary<string, object>();
		}

		private void ProcessRequestHeaders(HttpContext context)
		{
			var requestHeaders = context.Request.Headers.AllKeys.ToDictionary(key => key, key => HttpUtility.HtmlDecode(context.Request.Headers[key]));

			foreach (var processor in serviceContext.ServiceConfiguration.GetRequestHeaderProcessors())
			{
				processor.Process(requestHeaders);
			}
		}

		private static void BadRequest(HttpContext context, Exception ex)
		{
			context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
			context.Response.StatusDescription = string.Format("Cannot resolve parameters from request body. The exception is; {0}", ex);
		}

		private static void ModelNotFound(HttpContext context, TypeNotFoundException ex)
		{
			context.Response.StatusCode = (int)HttpStatusCode.NotFound;
			context.Response.StatusDescription = string.Format("Specified model ({0}) was not found in service model. The exception is; {1}", ex.TypeId, ex);
		}

		private static void MethodNotAllowed(HttpContext context, bool allowGet)
		{
			context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
			if (allowGet)
			{
				context.Response.StatusDescription = "Only GET, POST and OPTIONS are supported";
			}
			context.Response.StatusDescription = "Only POST and OPTIONS are supported";
		}

		private static void AmbiguousModel(HttpContext context, string modelId, List<ObjectModel> availableModels)
		{
			context.Response.StatusCode = (int)HttpStatusCode.NotFound;
			context.Response.StatusDescription = string.Format(
				"More than one model found with given modelId ({0}). " +
				"Try sending full names. Available models are {1}.",
				modelId, string.Join(",", availableModels.Select(om => om.Id)));
		}

		private static void TargetModelNotFound(HttpContext context, string modelId)
		{
			context.Response.StatusCode = (int)HttpStatusCode.NotFound;
			context.Response.StatusDescription = string.Format(
				"Could not resolve modelId or find an existing model from this modelId ({0}). " +
				"Make sure given modelId has a corresponding model and url is in one of the following format; " +
				"- serviceurlbase/modelId " +
				"- serviceurlbase/modelId/id " +
				"- serviceurlbase/modelId/operation " +
				"- serviceurlbase/modelId/viewModelId " +
				"- serviceurlbase/modelId/id/operation " +
				"- serviceurlbase/modelId/id/viewModelId " +
				"- serviceurlbase/modelId/id/viewModelId/operation", modelId);
		}

		private ObjectModel FindModel(string modelId)
		{
			ObjectModel result;
			var appModel = serviceContext.ObjectService.ApplicationModel;

			if (appModel.Model.TryGetValue(modelId, out result))
			{
				return result;
			}

			List<ObjectModel> availableModels;

			modelIndex.TryGetValue(modelId.AfterLast("."), out availableModels);

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

		private Stream GetStream(string path)
		{
			path = path.Replace("/", ".");
			var stream = GetType().Assembly.GetManifestResourceStream(
				GetType().Assembly.GetManifestResourceNames().Single(s => s.EndsWith(path)));

			if (stream == null)
			{
				throw new InvalidOperationException("Could not get manifest resource stream for test page");
			}
			return stream;
		}

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

		private void AddResponseHeaders(HttpContext context)
		{
			foreach (var responseHeader in serviceContext.ServiceConfiguration.GetResponseHeaders())
			{
				var responseHeaderValue = serviceContext.ServiceConfiguration.GetResponseHeaderValue(responseHeader);
				if (!string.IsNullOrEmpty(responseHeaderValue))
				{
					context.Response.Headers.Add(responseHeader, HttpUtility.UrlEncode(responseHeaderValue));
				}
			}
		}

		#region Http status code responses
		private class ModelNotFoundException : Exception
		{
			public string ModelId { get; private set; }

			public ModelNotFoundException(string modelId)
			{
				ModelId = modelId;
			}
		}

		private class AmbiguousModelException : Exception
		{
			public List<ObjectModel> AvailableModels { get; private set; }

			public AmbiguousModelException(List<ObjectModel> availableModels)
			{
				AvailableModels = availableModels;
			}
		}
		#endregion

		#region Id, ViewModelId, Operation Resolution

		private class Resolution
		{
			private readonly ApplicationModel appModel;
			private readonly string modelId;
			private readonly string id;
			private readonly string viewModelId;
			private readonly string operation;

			public Resolution(ApplicationModel appModel, string modelId, string id, string viewModelId, string operation)
			{
				this.appModel = appModel;
				this.modelId = modelId;
				this.id = id;
				this.viewModelId = viewModelId;
				this.operation = operation;
			}

			public ReferenceData Reference { get { return new ReferenceData { Id = id, ModelId = modelId, ViewModelId = viewModelId }; } }
			public ObjectModel Model { get { return appModel.Model[Reference.ModelId]; } }
			public ObjectModel ViewModel { get { return appModel.Model[Reference.ViewModelId]; } }
			public OperationModel OperationModel { get { return ViewModel.Operation[operation]; } }

			public bool HasOperation { get { return !string.IsNullOrWhiteSpace(operation); } }
		}

		private Resolution Resolve(ApplicationModel appModel, ObjectModel model, string idOrViewModelIdOrOperation, string viewModelIdOrOperation, string operation)
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

		#endregion
	}
}
