using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Routing;
using Routine.Core;
using Routine.Core.Rest;
using Routine.Engine.Context;

namespace Routine.Service.RequestHandlers
{
	public abstract class RequestHandlerBase : IRequestHandler
	{
		#region Constants

		private const int BUFFER_SIZE = 0x1000;
		protected const string JSON_CONTENT_TYPE = "application/json";
		protected readonly Encoding DEFAULT_CONTENT_ENCODING = Encoding.UTF8;

		#endregion

		#region Construction

		protected IServiceContext ServiceContext { get; }
		protected IJsonSerializer JsonSerializer { get; }
		protected HttpContextBase HttpContext { get; }

		protected RequestHandlerBase(IServiceContext serviceContext, IJsonSerializer jsonSerializer, HttpContextBase httpContext)
		{
			ServiceContext = serviceContext;
			JsonSerializer = jsonSerializer;
			HttpContext = httpContext;
		}

		#endregion

		public abstract void WriteResponse();

		protected HttpApplicationStateBase Application => HttpContext.Application;
		protected RouteData RouteData => HttpContext.Request.RequestContext.RouteData;
		protected NameValueCollection QueryString => HttpContext.Request.QueryString;
		protected string UrlBase => ServiceContext.ServiceConfiguration.GetPath(string.Empty).BeforeLast('/');
		protected bool IsGet => "GET".Equals(HttpContext.Request.HttpMethod, StringComparison.InvariantCultureIgnoreCase);
		protected bool IsPost => "POST".Equals(HttpContext.Request.HttpMethod, StringComparison.InvariantCultureIgnoreCase);
		protected ApplicationModel ApplicationModel => ServiceContext.ObjectService.ApplicationModel;
		protected virtual Dictionary<string, List<ObjectModel>> ModelIndex
		{
			get
			{
				var result = (Dictionary<string, List<ObjectModel>>)HttpContext.Application["Routine.RequestHandler.ModelIndex"];

				if (result != null) { return result; }

				HttpContext.Application.Lock();

				result = (Dictionary<string, List<ObjectModel>>)HttpContext.Application["Routine.RequestHandler.ModelIndex"]; ;

				if (result != null)
				{
					HttpContext.Application.UnLock();

					return result;
				}

				result = BuildModelIndex();

				HttpContext.Application["Routine.RequestHandler.ModelIndex"] = result;

				HttpContext.Application.UnLock();

				return result;
			}
		}

		protected virtual void BadRequest(Exception ex)
		{
			HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
			HttpContext.Response.StatusDescription =
				$"Cannot resolve parameters from request body. The exception is; {ex}";
		}

		protected virtual void ModelNotFound(TypeNotFoundException ex)
		{
			HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
			HttpContext.Response.StatusDescription =
				$"Specified model ({ex.TypeId}) was not found in service model. The exception is; {ex}";
		}

		protected virtual void MethodNotAllowed(bool allowGet)
		{
			HttpContext.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
			if (allowGet)
			{
				HttpContext.Response.StatusDescription = "Only GET, POST and OPTIONS are supported";
			}
			HttpContext.Response.StatusDescription = "Only POST and OPTIONS are supported";
		}

		protected virtual void WriteFileResponse(string path)
		{
			var stream = GetStream(path);

			var sr = new StreamReader(stream);

			var fileContent = sr.ReadToEnd();
			sr.Close();
			stream.Close();

			fileContent = fileContent.Replace("$urlbase$", UrlBase);

			var file = Encoding.UTF8.GetBytes(fileContent);
			HttpContext.Response.ContentType = MimeTypeMap.GetMimeType(path.AfterLast("."));
			HttpContext.Response.BinaryWrite(file);
		}

		protected virtual void WriteFontResponse(string fileName)
		{
			var stream = GetStream("assets/fonts/" + fileName);

			var outputStream = HttpContext.Response.OutputStream;
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
			HttpContext.Response.ContentType = MimeTypeMap.GetMimeType(fileName);
			HttpContext.Response.Flush();
			HttpContext.Response.End();
		}

		protected virtual void WriteJsonResponse(object result, HttpStatusCode statusCode = HttpStatusCode.OK, bool clearError = false)
		{
			if (clearError)
			{
				HttpContext.Server.ClearError();
			}

			HttpContext.Response.StatusCode = (int)statusCode;
			HttpContext.Response.ContentType = JSON_CONTENT_TYPE;
			HttpContext.Response.ContentEncoding = DEFAULT_CONTENT_ENCODING;
			HttpContext.Response.Write(JsonSerializer.Serialize(result));
		}

		private Dictionary<string, List<ObjectModel>> BuildModelIndex()
		{
			var result = new Dictionary<string, List<ObjectModel>>();
			var appModel = ServiceContext.ObjectService.ApplicationModel;

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

		private Stream GetStream(string path)
		{
			path = path.Replace("/", ".");
			var stream = GetType().Assembly.GetManifestResourceStream(
				GetType().Assembly.GetManifestResourceNames().Single(s => s.EndsWith(path))
			);

			if (stream == null)
			{
				throw new InvalidOperationException("Could not get manifest resource stream for test page");
			}

			return stream;
		}
	}
}