using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Moq;
using Moq.Language.Flow;
using NUnit.Framework;
using Routine.Core;
using Routine.Core.Rest;
using Routine.Service;
using Routine.Service.Configuration;
using Routine.Service.RequestHandlers;
using Routine.Test.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Routine.Test.Service
{
    [TestFixture]
    public class HandleRequestHandlerTest : CoreTestBase
    {
        #region SetUp & Helpers

        private Mock<IHttpContextAccessor> httpContextAccessor;
        private Mock<IServiceContext> serviceContext;
        private Mock<IObjectService> objectService;
        private Mock<HttpRequest> request;
        private Mock<HttpResponse> response;
        private IHeaderDictionary requestHeaders;
        private IHeaderDictionary responseHeaders;
        private QueryString requestQueryString;
        private IQueryCollection requestQuery;
        private HandleRequestHandler testing;
        private ConventionBasedServiceConfiguration config;
        private Mock<IFeatureCollection> featureCollection;
        private Mock<IHttpResponseFeature> httpResponseFeature;
        private IJsonSerializer serializer;

        public override void SetUp()
        {
            base.SetUp();

            httpContextAccessor = new Mock<IHttpContextAccessor>();
            var httpContext = new Mock<HttpContext>();
            serviceContext = new Mock<IServiceContext>();
            objectService = new Mock<IObjectService>();
            request = new Mock<HttpRequest>();
            response = new Mock<HttpResponse>();
            featureCollection = new Mock<IFeatureCollection>();
            httpResponseFeature = new Mock<IHttpResponseFeature>();
            requestHeaders = new HeaderDictionary();
            responseHeaders = new HeaderDictionary();
            requestQueryString = new QueryString();
            requestQuery = new QueryCollection();
            serializer = new JsonSerializerAdapter();

            serviceContext.Setup(sc => sc.ObjectService).Returns(objectService.Object);
            config = BuildRoutine.ServiceConfig().FromBasic();
            serviceContext.Setup(sc => sc.ServiceConfiguration).Returns(config);
            objectService.Setup(os => os.ApplicationModel).Returns(() => GetApplicationModel());
            objectService.Setup(os => os.Get(It.IsAny<ReferenceData>())).Returns((ReferenceData referenceData) =>
            {
                if (referenceData.Id == null)
                {
                    referenceData = new ReferenceData
                    {
                        Id = "instance",
                        ModelId = referenceData.ModelId,
                        ViewModelId = referenceData.ViewModelId
                    };
                }

                return objectDictionary[referenceData];
            });
            request.Setup(r => r.Headers).Returns(requestHeaders);
            request.Setup(r => r.QueryString).Returns(requestQueryString);
            request.Setup(r => r.Query).Returns(requestQuery);
            request.Setup(r => r.Method).Returns("POST");
            request.Setup(r => r.Body).Returns(new MemoryStream()).Verifiable();

            // https://stackoverflow.com/questions/34677203/testing-the-result-of-httpresponse-statuscode/34677864#34677864
            response.SetupAllProperties();
            response.Setup(r => r.Body).Returns(new MemoryStream()).Verifiable();
            response.Setup(r => r.Headers).Returns(responseHeaders);
            httpContextAccessor.Setup(hca => hca.HttpContext).Returns(httpContext.Object);
            httpContextAccessor.Setup(hca => hca.HttpContext.Request).Returns(request.Object);
            httpContextAccessor.Setup(hca => hca.HttpContext.Response).Returns(response.Object);
            httpContextAccessor.Setup(hca => hca.HttpContext.Features).Returns(featureCollection.Object);
            httpContextAccessor.Setup(hca => hca.HttpContext.Response.HttpContext.Features.Get<IHttpResponseFeature>()).Returns(httpResponseFeature.Object);
            httpContextAccessor.Setup(hca => hca.HttpContext.Items).Returns(new Dictionary<object, object>());

            RequestHandlerBase.ClearModelIndex();
            testing = new HandleRequestHandler(serviceContext.Object, serializer, httpContextAccessor.Object,
                actionFactory: resolution => resolution.HasOperation
                    ? new DoRequestHandler(serviceContext.Object, serializer, httpContextAccessor.Object, resolution)
                    : new GetRequestHandler(serviceContext.Object, serializer, httpContextAccessor.Object, resolution)
            );
        }

        protected ObjectStubber When(ReferenceData referenceData) => new(objectService, referenceData);

        protected class ObjectStubber
        {
            private readonly Mock<IObjectService> objectService;
            private readonly ReferenceData referenceData;

            public ObjectStubber(Mock<IObjectService> objectService, ReferenceData referenceData)
            {
                this.objectService = objectService;
                this.referenceData = referenceData;
            }

            public ISetup<IObjectService, VariableData> Performs(string operationName) => Performs(operationName, p => true);
            public ISetup<IObjectService, VariableData> Performs(string operationName, Expression<Func<Dictionary<string, ParameterValueData>, bool>> parameterMatcher) =>
                objectService
                    .Setup(o => o.Do(
                        referenceData,
                        operationName,
                        It.Is(parameterMatcher)));
        }

        private void SetUpRequestBody(string body)
        {
            var sw = new StreamWriter(request.Object.Body);
            sw.Write(body);
            sw.Flush();
        }

        #endregion

        [Test]
        public async Task When_operation_is_not_given__interprets_the_call_as_a_get_request()
        {
            ModelsAre(Model("model"));
            ObjectsAre(Object(Id("3", "model")));

            await testing.Handle("model", "3", null, null);

            objectService.Verify(os => os.Get(Id("3", "model")));
        }

        [Test]
        public async Task When_operation_is_given__interprets_the_call_as_a_do_request()
        {
            ModelsAre(
                Model("model").Operation("action", true)
            );

            await testing.Handle("model", "3", "action", null);

            objectService.Verify(os => os.Do(Id("3", "model"), "action", It.Is<Dictionary<string, ParameterValueData>>(p => p.Count == 0)));
        }

        [Test]
        public async Task All_resolution_cases()
        {
            ModelsAre(
                Model("model").ViewModelIds("viewmodel", "prefix.viewmodel2").Operation("action"),
                Model("prefix.model2").ViewModelIds("viewmodel", "prefix.viewmodel2").Operation("action"),
                Model("viewmodel").IsView("model", "prefix.model2").Operation("action"),
                Model("prefix.viewmodel2").IsView("model", "prefix.model2").Operation("action")
            );

            ObjectsAre(
                Object(Id("instance", "model")),
                Object(Id("instance", "model", "viewmodel")),
                Object(Id("instance", "model", "prefix.viewmodel2")),
                Object(Id("instance", "prefix.model2")),
                Object(Id("instance", "prefix.model2", "viewmodel")),
                Object(Id("instance", "prefix.model2", "prefix.viewmodel2"))
            );

            // /modelId
            await testing.Handle("model", null, null, null);
            objectService.Verify(os => os.Get(Id(null, "model")));

            // /modelId-short
            await testing.Handle("model2", null, null, null);
            objectService.Verify(os => os.Get(Id(null, "prefix.model2")));

            // /modelId/id
            await testing.Handle("model", "instance", null, null);
            objectService.Verify(os => os.Get(Id("instance", "model")));

            // /modelId-short/id
            await testing.Handle("model2", "instance", null, null);
            objectService.Verify(os => os.Get(Id("instance", "prefix.model2")));

            // /modelId/viewModelId
            await testing.Handle("model", "viewmodel", null, null);
            objectService.Verify(os => os.Get(Id(null, "model", "viewmodel")));

            // /modelId-short/viewModelId
            await testing.Handle("model2", "viewmodel", null, null);
            objectService.Verify(os => os.Get(Id(null, "prefix.model2", "viewmodel")));

            // /modelId/viewModelId-short
            await testing.Handle("model", "viewmodel2", null, null);
            objectService.Verify(os => os.Get(Id(null, "model", "prefix.viewmodel2")));

            // /modelId-short/viewModelId-short
            await testing.Handle("model2", "viewmodel2", null, null);
            objectService.Verify(os => os.Get(Id(null, "prefix.model2", "prefix.viewmodel2")));

            // /modelId/operation
            await testing.Handle("model", "action", null, null);
            objectService.Verify(os => os.Do(Id(null, "model"), "action", It.Is<Dictionary<string, ParameterValueData>>(pvd => pvd.Count == 0)));

            // /modelId-short/operation
            await testing.Handle("model2", "action", null, null);
            objectService.Verify(os => os.Do(Id(null, "prefix.model2"), "action", It.Is<Dictionary<string, ParameterValueData>>(pvd => pvd.Count == 0)));

            // /modelId/id/viewModelId
            await testing.Handle("model", "instance", "viewmodel", null);
            objectService.Verify(os => os.Get(Id("instance", "model", "viewmodel")));

            // /modelId-short/id/viewModelId
            await testing.Handle("model2", "instance", "viewmodel", null);
            objectService.Verify(os => os.Get(Id("instance", "prefix.model2", "viewmodel")));

            // /modelId/id/viewModelId-short
            await testing.Handle("model", "instance", "viewmodel2", null);
            objectService.Verify(os => os.Get(Id("instance", "model", "prefix.viewmodel2")));

            // /modelId-short/id/viewModelId-short
            await testing.Handle("model2", "instance", "viewmodel2", null);
            objectService.Verify(os => os.Get(Id("instance", "prefix.model2", "prefix.viewmodel2")));

            // /modelId/id/operation
            await testing.Handle("model", "instance", "action", null);
            objectService.Verify(os => os.Do(Id("instance", "model"), "action", It.Is<Dictionary<string, ParameterValueData>>(pvd => pvd.Count == 0)));

            // /modelId-short/id/operation
            await testing.Handle("model2", "instance", "action", null);
            objectService.Verify(os => os.Do(Id("instance", "prefix.model2"), "action", It.Is<Dictionary<string, ParameterValueData>>(pvd => pvd.Count == 0)));

            // /modelId/id/viewModelId/operation
            await testing.Handle("model", "instance", "viewmodel", "action");
            objectService.Verify(os => os.Do(Id("instance", "model", "viewmodel"), "action", It.Is<Dictionary<string, ParameterValueData>>(pvd => pvd.Count == 0)));

            // /modelId-short/id/viewModelId/operation
            await testing.Handle("model2", "instance", "viewmodel", "action");
            objectService.Verify(os => os.Do(Id("instance", "prefix.model2", "viewmodel"), "action", It.Is<Dictionary<string, ParameterValueData>>(pvd => pvd.Count == 0)));

            // /modelId/id/viewModelId-short/operation
            await testing.Handle("model", "instance", "viewmodel2", "action");
            objectService.Verify(os => os.Do(Id("instance", "model", "prefix.viewmodel2"), "action", It.Is<Dictionary<string, ParameterValueData>>(pvd => pvd.Count == 0)));

            // /modelId-short/id/viewModelId-short/operation
            await testing.Handle("model2", "instance", "viewmodel2", "action");
            objectService.Verify(os => os.Do(Id("instance", "prefix.model2", "prefix.viewmodel2"), "action", It.Is<Dictionary<string, ParameterValueData>>(pvd => pvd.Count == 0)));
        }

        [Test]
        public async Task GET_method_is_available_for_get_actions()
        {
            ModelsAre(Model("model"));
            ObjectsAre(Object(Id("3", "model")));

            request.Setup(r => r.Method).Returns("GET");

            await testing.Handle("model", "3", null, null);

            objectService.Verify(os => os.Get(Id("3", "model")));
        }

        [Test]
        public async Task GET_method_is_available_for_only_configured_do_actions()
        {
            request.Setup(r => r.Method).Returns("GET");

            config.AllowGet.Set(true, m => m.OperationModel.Name == "get");
            config.AllowGet.Set(false, m => m.OperationModel.Name == "post");

            ModelsAre(
                Model("model")
                .Operation("post")
                .Operation("get")
            );
            ObjectsAre(Object(Id("3", "model")));

            await testing.Handle("model", "3", "get", null);

            objectService.Verify(os => os.Do(It.IsAny<ReferenceData>(), "get", It.IsAny<Dictionary<string, ParameterValueData>>()));

            await testing.Handle("model", "3", "post", null);
            objectService.Verify(os => os.Do(It.IsAny<ReferenceData>(), "post", It.IsAny<Dictionary<string, ParameterValueData>>()), Times.Never());
            Assert.IsNotNull(httpContextAccessor.Object.HttpContext?.Response);
            Assert.AreEqual(HttpStatusCode.MethodNotAllowed, (HttpStatusCode)httpContextAccessor.Object.HttpContext.Response.StatusCode);
        }

        [Test]
        public async Task Only_GET_and_POST_methods_are_supported()
        {
            ModelsAre(
                Model("model").Operation("action")
            );

            request.Setup(r => r.Method).Returns("DELETE");
            await testing.Handle("model", "action", null, null);
            Assert.IsNotNull(httpContextAccessor.Object.HttpContext?.Response);
            Assert.AreEqual(HttpStatusCode.MethodNotAllowed, (HttpStatusCode)httpContextAccessor.Object.HttpContext.Response.StatusCode);

            request.Setup(r => r.Method).Returns("PUT");
            await testing.Handle("model", "action", null, null);
            Assert.IsNotNull(httpContextAccessor.Object.HttpContext.Response);
            Assert.AreEqual(HttpStatusCode.MethodNotAllowed, (HttpStatusCode)httpContextAccessor.Object.HttpContext.Response.StatusCode);

            objectService.Verify(os => os.Do(It.IsAny<ReferenceData>(), It.IsAny<string>(), It.IsAny<Dictionary<string, ParameterValueData>>()), Times.Never());
            objectService.Verify(os => os.Get(It.IsAny<ReferenceData>()), Times.Never());
        }

        [Test]
        public async Task When_given_model_id_does_not_exist__returns_404()
        {
            await testing.Handle("nonexistingmodel", null, null, null);

            Assert.IsNotNull(httpContextAccessor.Object.HttpContext?.Response);
            Assert.AreEqual(HttpStatusCode.NotFound, (HttpStatusCode)httpContextAccessor.Object.HttpContext.Response.StatusCode);
            Assert.IsTrue(httpContextAccessor.Object.HttpContext.Response.Headers["X-Status-Description"].ToString().Contains("nonexistingmodel"), "StatusDescription should contain given model id");
        }

        [Test]
        public async Task When_given_model_id_is_not_full_id_and_there_are_more_than_one_model_with_similar_name__returns_404()
        {
            ModelsAre(Model("prefix1.model"), Model("prefix2.model"));

            await testing.Handle("model", null, null, null);

            var statusDescription = httpContextAccessor.Object.HttpContext?.Response.Headers["X-Status-Description"].ToString();

            Assert.IsNotNull(httpContextAccessor.Object.HttpContext?.Response);
            Assert.AreEqual(HttpStatusCode.NotFound, (HttpStatusCode)httpContextAccessor.Object.HttpContext.Response.StatusCode);
            Assert.IsTrue(statusDescription.Contains("prefix1.model") && statusDescription.Contains("prefix2.model"),
                "Status description should contain available model ids");
        }

        [Test]
        public async Task When_given_parameters_contains_a_non_existing_model_id__returns_404()
        {
            ModelsAre(
                Model("model").Operation("action", PModel("arg", "model"))
            );

            SetUpRequestBody("{\"arg\":{\"ModelId\":\"nonexistingmodel\",\"Data\":{\"arg\":null}}}");

            await testing.Handle("model", "action", null, null);

            Assert.IsNotNull(httpContextAccessor.Object.HttpContext?.Response);
            Assert.AreEqual(HttpStatusCode.NotFound, (HttpStatusCode)httpContextAccessor.Object.HttpContext.Response.StatusCode);
            Assert.IsTrue(httpContextAccessor.Object.HttpContext.Response.Headers["X-Status-Description"].ToString().Contains("nonexistingmodel"),
                "Status description should contain given model id");
        }

        [Test]
        public async Task When_body_cannot_be_deserialized__returns_BadRequest()
        {
            ModelsAre(
                Model("model").Operation("action")
            );

            SetUpRequestBody("{");

            await testing.Handle("model", "action", null, null);

            Assert.IsNotNull(httpContextAccessor.Object.HttpContext?.Response);
            Assert.AreEqual(HttpStatusCode.BadRequest, (HttpStatusCode)httpContextAccessor.Object.HttpContext.Response.StatusCode);
        }

        [Test]
        public async Task Request_headers_are_processed_for_each_get()
        {
            var header = new Mock<IHeaderProcessor>();

            ModelsAre(Model("model"));
            ObjectsAre(Object(Id("3", "model")));

            requestHeaders.Add("test", "value");

            config.RequestHeaders.Add("test");
            config.RequestHeaderProcessors.Add(header.Object);

            await testing.Handle("model", "3", null, null);

            header.Verify(h => h.Process(It.Is<Dictionary<string, string>>(d =>
                d.ContainsKey("test") && d["test"] == "value"
            )));
        }

        [Test]
        public async Task Request_headers_are_processed_for_each_do()
        {
            var header = new Mock<IHeaderProcessor>();

            ModelsAre(Model("model").Operation("action"));
            ObjectsAre(Object(Id("3", "model")));

            requestHeaders.Add("test", "value");

            config.RequestHeaders.Add("test");
            config.RequestHeaderProcessors.Add(header.Object);

            await testing.Handle("model", "3", "action", null);

            header.Verify(h => h.Process(It.Is<Dictionary<string, string>>(d =>
                d.ContainsKey("test") && d["test"] == "value"
            )));
        }

        [Test]
        public async Task Request_headers_are_not_processed_before_validation_for_do_requests()
        {
            var header = new Mock<IHeaderProcessor>();

            requestHeaders.Add("test", "value");

            config.RequestHeaders.Add("test");
            config.RequestHeaderProcessors.Add(header.Object);

            await testing.Handle("model", "3", "action", null);

            header.Verify(h => h.Process(It.IsAny<Dictionary<string, string>>()), Times.Never());
        }

        [Test]
        public async Task Configured_response_headers_are_put_into_response_headers()
        {
            config.ResponseHeaders.Add("test");
            config.ResponseHeaderValue.Set("value", "test");

            ModelsAre(Model("model"));
            ObjectsAre(Object(Id("3", "model")));

            await testing.Handle("model", "3", null, null);

            Assert.AreEqual("value", responseHeaders["test"]);
        }

        [Test]
        public async Task Compress_for_do_result_viewmodel()
        {
            ModelsAre(
                Model("model").Operation("action", "viewmodel").ViewModelIds("viewmodel"),
                Model("viewmodel").IsView("model").Data("data", "string"),
                Model("string").IsValue()
            );

            When(Id("2", "model")).Performs("action").Returns(new VariableData
            {
                Values = new List<ObjectData>
                {
                    Object(Id("2", "model", "viewmodel"))
                    .Display("model 2")
                    .Data("data", Id("text", "string"))
                    .Build().Item2
                }
            });

            await testing.Handle("model", "2", "action", null);
            var body = "{" +
                       "\"Id\":\"2\"," +
                       "\"Display\":\"model 2\"," +
                       "\"ModelId\":\"model\"," +
                       "\"Data\":" +
                       "{" +
                       "\"data\":\"text\"" +
                       "}" +
                       "}";

            var actual = Encoding.UTF8.GetString(((MemoryStream)httpContextAccessor.Object.HttpContext?.Response.Body)?.ToArray());

            Assert.AreEqual(body, actual);
        }

        [Test]
        public async Task Compress_for_do_result_model()
        {
            ModelsAre(
                Model("model").Operation("action", "model").Data("data", "string"),
                Model("string").IsValue()
            );

            When(Id("2", "model")).Performs("action").Returns(new VariableData
            {
                Values = new List<ObjectData>
                {
                    Object(Id("2", "model"))
                    .Display("model 2")
                    .Data("data", Id("text", "string"))
                    .Build().Item2
                }
            });

            await testing.Handle("model", "2", "action", null);
            var body = "{" +
                       "\"Id\":\"2\"," +
                       "\"Display\":\"model 2\"," +
                       "\"Data\":" +
                       "{" +
                       "\"data\":\"text\"" +
                       "}" +
                       "}";

            var actual = Encoding.UTF8.GetString(((MemoryStream)httpContextAccessor.Object.HttpContext?.Response.Body)?.ToArray());

            Assert.AreEqual(body, actual);
        }

        [Test]
        public async Task Decompress_for_parameterviewmodel()
        {
            ModelsAre(
                Model("model").Operation("action", PModel("arg", "model"))
            );

            SetUpRequestBody("{\"arg\":\"3\"}");

            await testing.Handle("model", "2", "action", null);

            objectService.Verify(os => os.Do(Id("2", "model"), "action", It.Is<Dictionary<string, ParameterValueData>>(pvd =>
                pvd.ContainsKey("arg") &&
                pvd["arg"].IsList == false &&
                pvd["arg"].Values.Count == 1 &&
                pvd["arg"].Values[0].Id == "3" &&
                pvd["arg"].Values[0].ModelId == "model"
            )));
        }

        [Test]
        public async Task Compress_for_get_model()
        {
            ModelsAre(
                Model("model").Data("data", "string"),
                Model("string").IsValue()
            );

            ObjectsAre(
                Object(Id("3", "model"))
                .Display("display")
                .Data("data", Id("text", "string"))
            );

            await testing.Handle("model", "3", null, null);
            var body = "{" +
                       "\"Id\":\"3\"," +
                       "\"Display\":\"display\"," +
                       "\"Data\":" +
                       "{" +
                       "\"data\":\"text\"" +
                       "}" +
                       "}";

            var actual = Encoding.UTF8.GetString(((MemoryStream)httpContextAccessor.Object.HttpContext?.Response.Body)?.ToArray());

            Assert.AreEqual(body, actual);
        }

        [Test]
        public async Task Compress_for_get_viewmodel()
        {
            ModelsAre(
                Model("model").ViewModelIds("viewmodel"),
                Model("viewmodel").IsView("model").Data("data", "string"),
                Model("string").IsValue()
            );

            ObjectsAre(
                Object(Id("3", "model", "viewmodel"))
                .Display("display")
                .Data("data", Id("text", "string"))
            );

            await testing.Handle("model", "3", "viewmodel", null);
            var body = "{" +
                       "\"Id\":\"3\"," +
                       "\"Display\":\"display\"," +
                       "\"ModelId\":\"model\"," +
                       "\"Data\":" +
                       "{" +
                       "\"data\":\"text\"" +
                       "}" +
                       "}";

            var actual = Encoding.UTF8.GetString(((MemoryStream)httpContextAccessor.Object.HttpContext?.Response.Body)?.ToArray());

            Assert.AreEqual(body, actual);
        }

        [Test]
        public async Task Ignore_nonexisting_parameter()
        {
            ModelsAre(
                Model("model").Operation("action")
            );

            SetUpRequestBody("{\"arg\":\"3\"}");

            await testing.Handle("model", "2", "action", null);

            objectService.Verify(os => os.Do(Id("2", "model"), "action", It.Is<Dictionary<string, ParameterValueData>>(pvd =>
                pvd.Count == 0
            )));
        }

        [Test]
        [Ignore("")]
        public void Handles_exception()
        {
            Assert.Fail("not implemented");
        }

        [Test]
        [Ignore("")]
        public void IgnoreCase_for_parameternames()
        {
            Assert.Fail();
        }

        [Test]
        [Ignore("")]
        public void Clients_should_be_able_to_know_application_model_version_so_that_they_can_invalidate_their_cache()
        {
            Assert.Fail("not implemented");
        }
    }
}

