using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;
using System.Web.Script.Serialization;
using Routine.Core;
using Routine.Core.Rest;

namespace Routine.Service
{
    public class ServiceHttpHandler : IHttpHandler, IRouteHandler
    {
        private readonly IServiceContext serviceContext;
        private readonly IJsonSerializer jsonSerializer;
        private const int DEFAULT_RECURSION_LIMIT = 100;
        private readonly Dictionary<string, List<ObjectModel>> modelIndex;
        private string UrlBase => "/Handler";

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

        #region Interface Implemntations
        public bool IsReusable => false;

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return this;
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

            else if (string.Equals(action, nameof(Index), StringComparison.InvariantCultureIgnoreCase))
            {
                Index(context);
            }

            else if (string.Equals(action, nameof(File), StringComparison.InvariantCultureIgnoreCase))
            {
                var path = context.Request.QueryString["path"];
                File(context, path);
            }

            else if (string.Equals(action, nameof(Configuration), StringComparison.InvariantCultureIgnoreCase))
            {
                Configuration(context);
            }

            else if (string.Equals(action, nameof(Fonts), StringComparison.InvariantCultureIgnoreCase))
            {
                var fileName = context.Request.QueryString["fileName"];
                Fonts(context, fileName);
            }
        }
        #endregion

        private void ApplicationModel(HttpContext context)
        {
            IndexApplicationModelIfNecessary();

            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.Write(jsonSerializer.Serialize(serviceContext.ObjectService.ApplicationModel));
        }

        private void Index(HttpContext context)
        {
            File(context, "app/application/index.html");
        }

        private void File(HttpContext context, string path)
        {
            var stream = GetStream(path);

            var sr = new StreamReader(stream);

            var fileContent = sr.ReadToEnd();
            sr.Close();
            stream.Close();

            fileContent = fileContent.Replace("$urlbase$", UrlBase);

            var file = Encoding.UTF8.GetBytes(fileContent);
            context.Response.ContentType = MimeTypeMap.GetMimeType(path.AfterLast("."));
            context.Response.BinaryWrite(file);
        }

        private void Configuration(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.Write(jsonSerializer.Serialize(new
            {
                url = UrlBase,
                requestHeaders = serviceContext.ServiceConfiguration.GetRequestHeaders(),
                responseHeaders = serviceContext.ServiceConfiguration.GetResponseHeaders()
            }));
        }

        private void Fonts(HttpContext context, string fileName)
        {
            var stream = GetStream("assets/fonts/" + fileName);

            var sr = new StreamReader(stream);

            var fileContent = sr.ReadToEnd();
            sr.Close();
            stream.Close();

            fileContent = fileContent.Replace("$urlbase$", UrlBase);

            var file = Encoding.UTF8.GetBytes(fileContent);

            context.Response.ContentType = MimeTypeMap.GetMimeType(fileName);
            context.Response.BinaryWrite(file);
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
    }
}
