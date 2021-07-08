using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using Moq;
using Moq.Language.Flow;
using NUnit.Framework;
using Routine.Api.Configuration;
using Routine.Core;
using Routine.Core.Rest;
using Routine.Service;
using Routine.Service.Configuration;
using Routine.Test.Core;

namespace Routine.Test.Service
{
    [TestFixture]
    public class RestClientObjectServiceTest : CoreTestBase
    {
        #region SetUp & Helpers

        private ConventionBasedServiceClientConfiguration config;
        private Mock<IRestClient> mockRestClient;
        private IJsonSerializer serializer;
        private RestClientObjectService testing;

        private const string URL_BASE = "http://api.test.com/service";

        public override void SetUp()
        {
            base.SetUp();

            serializer = new JsonSerializerAdapter();
            config = BuildRoutine.ServiceClientConfig().FromBasic()
                .ServiceUrlBase.Set(URL_BASE);

            mockRestClient = new Mock<IRestClient>();

            testing = new RestClientObjectService(config, mockRestClient.Object, serializer);

            SetUpGet("ApplicationModel").Returns(() => new RestResponse(serializer.Serialize(GetApplicationModel())));
        }

        private ISetup<IRestClient, RestResponse> SetUpGet(string action) { return SetUpGet(action, req => true); }
        private ISetup<IRestClient, RestResponse> SetUpGet(string action, RestRequest restRequest) { return SetUpGet(action, req => Equals(req, restRequest)); }
        private ISetup<IRestClient, RestResponse> SetUpGet(string action, Expression<Func<RestRequest, bool>> restRequestMatcher)
        {
            return mockRestClient.Setup(rc => rc.Get(string.Format("{0}/{1}", URL_BASE, action), It.Is(restRequestMatcher)));
        }

        private ISetup<IRestClient, RestResponse> SetUpPost(string action) { return SetUpPost(action, req => true); }
        private ISetup<IRestClient, RestResponse> SetUpPost(string action, string body) { return SetUpPost(action, req => req.Body == body); }
        private ISetup<IRestClient, RestResponse> SetUpPost(string action, RestRequest restRequest) { return SetUpPost(action, req => Equals(req, restRequest)); }
        private ISetup<IRestClient, RestResponse> SetUpPost(string action, Expression<Func<RestRequest, bool>> restRequestMatcher)
        {
            return mockRestClient.Setup(rc => rc.Post(string.Format("{0}/{1}", URL_BASE, action), It.Is(restRequestMatcher)));
        }

        private class TestException : Exception { public TestException(string message) : base(message) { } }

        private static WebException HttpNotFound(string message)
        {
            var mock = new Mock<HttpWebResponse>();
            mock.Setup(wr => wr.StatusCode).Returns(HttpStatusCode.NotFound);
            mock.Setup(wr => wr.StatusDescription).Returns(message);

            return new WebException(message, null, WebExceptionStatus.Success, mock.Object);
        }

        #endregion

        [Test]
        public void Gets_application_model_using_get_method()
        {
            var actual = testing.ApplicationModel;

            mockRestClient.Verify(rc => rc.Get(It.Is<string>(url => url.EndsWith("/ApplicationModel")), RestRequest.Empty), Times.Once());
            Assert.AreEqual(0, actual.Models.Count);
        }

        [Test]
        public void Application_model_is_cached_so_that_it_does_not_fetch_model_each_time()
        {
            var expected = testing.ApplicationModel;
            var actual = testing.ApplicationModel;

            mockRestClient.Verify(rc => rc.Get(It.Is<string>(url => url.EndsWith("/ApplicationModel")), It.IsAny<RestRequest>()), Times.Once());

            Assert.AreSame(expected, actual);
        }

        [Test]
        public void Gets_object_data_using_get_method()
        {
            ModelsAre(Model("model"));

            SetUpGet("model/3").Returns(new RestResponse(
                "{" +
                "\"Id\":\"3\"," +
                "\"Display\":\"Test\"" +
                "}"
            ));

            var actual = testing.Get(Id("3", "model"));

            Assert.AreEqual("Test", actual.Display);
        }

        [Test]
        public void When_given_reference_data_has_a_view_model_id__gets_object_data_using_view_model_id()
        {
            ModelsAre(
                Model("model").ViewModelIds("viewmodel"),
                Model("viewmodel").IsView("model")
            );

            SetUpGet("model/3/viewmodel").Returns(new RestResponse(
                "{" +
                "\"Id\":\"3\"," +
                "\"Display\":\"Test\"" +
                "}"
            ));

            var actual = testing.Get(Id("3", "model", "viewmodel"));

            Assert.AreEqual("Test", actual.Display);
        }

        [Test]
        public void When_given_reference_data_is_null__returns_null_without_making_a_remote_call()
        {
            var actual = testing.Get(Null());

            Assert.AreEqual(null, actual);
            mockRestClient.Verify(rc => rc.Get(It.IsAny<string>(), It.IsAny<RestRequest>()), Times.Never());
            mockRestClient.Verify(rc => rc.Post(It.IsAny<string>(), It.IsAny<RestRequest>()), Times.Never());
        }

        [Test]
        public void Gets_request_headers_from_configuration_and_sends_them_for_every_get()
        {
            config
                .RequestHeaders.Add("header1", "header2")
                .RequestHeaderValue.Set(c => c.By(h => h + "_value"))
            ;

            ModelsAre(Model("model"));

            SetUpGet("model/3").Returns(new RestResponse("\"3\""));

            testing.Get(Id("3", "model"));

            mockRestClient.Verify(rc => rc.Get(It.IsAny<string>(), It.Is<RestRequest>(req =>
                req.Headers.ContainsKey("header1") &&
                req.Headers["header1"] == "header1_value" &&
                req.Headers.ContainsKey("header2") &&
                req.Headers["header2"] == "header2_value")));
        }

        [Test]
        public void Delegates_response_headers_to_response_header_processors_for_every_get()
        {
            var mockHeaderProcessor = new Mock<IHeaderProcessor>();

            config.ResponseHeaderProcessors.Add(mockHeaderProcessor.Object);

            ModelsAre(Model("model"));

            SetUpGet("model/3")
                .Returns(new RestResponse("null",
                    new Dictionary<string, string>
                    {
                        {"header1", "header1_value"},
                        {"header2", "header2_value"}
                    })
                );

            testing.Get(Id("3", "model"));

            mockHeaderProcessor.Verify(hp => hp.Process(It.Is<IDictionary<string, string>>(h =>
                h.ContainsKey("header1") &&
                h["header1"] == "header1_value" &&
                h.ContainsKey("header2") &&
                h["header2"] == "header2_value"
            )));
        }

        [Test]
        public void Posts_given_parameters_as_json_to_operation_action()
        {
            ModelsAre(
                Model("model")
                    .Operation("action", "model", PModel("arg1", "model"))
                );

            SetUpPost("model/3/action",
                "{" +
                    "\"arg1\":\"4\"" +
                "}").Returns(new RestResponse(
                    "{" +
                        "\"Id\":\"5\"," +
                        "\"Display\":\"Test\"" +
                    "}"));

            var actual = testing.Do(Id("3", "model"), "action",
                new Dictionary<string, ParameterValueData>
                {
                    {
                        "arg1",
                        new ParameterValueData {Values = new List<ParameterData>
                        {
                            new ParameterData {Id = "4", ModelId = "model"}
                        }}
                    }
                });

            Assert.AreEqual("5", actual.Values[0].Id);
            Assert.AreEqual("Test", actual.Values[0].Display);
        }

        [Test]
        public void When_given_target_is_null__returns_empty_variable_data_without_making_a_remote_call()
        {
            var actual = testing.Do(Null(), "doesn't matter", new Dictionary<string, ParameterValueData>());

            Assert.AreEqual(new VariableData(), actual);
            mockRestClient.Verify(rc => rc.Get(It.IsAny<string>(), It.IsAny<RestRequest>()), Times.Never());
            mockRestClient.Verify(rc => rc.Post(It.IsAny<string>(), It.IsAny<RestRequest>()), Times.Never());
        }

        [Test]
        public void Gets_request_headers_from_configuration_and_sends_them_for_every_do()
        {
            config
                .RequestHeaders.Add("header1", "header2")
                .RequestHeaderValue.Set(c => c.By(h => h + "_value"))
            ;

            ModelsAre(Model("model").Operation("action", true));

            SetUpPost("model/3/action").Returns(new RestResponse("null"));

            testing.Do(Id("3", "model"), "action", new Dictionary<string, ParameterValueData>());

            mockRestClient.Verify(rc => rc.Post(It.IsAny<string>(), It.Is<RestRequest>(req =>
                req.Headers.ContainsKey("header1") &&
                req.Headers["header1"] == "header1_value" &&
                req.Headers.ContainsKey("header2") &&
                req.Headers["header2"] == "header2_value")));
        }

        [Test]
        public void Delegates_response_headers_to_response_header_processors_for_every_do()
        {
            var mockHeaderProcessor = new Mock<IHeaderProcessor>();

            config.ResponseHeaderProcessors.Add(mockHeaderProcessor.Object);

            ModelsAre(Model("model").Operation("action", true));

            SetUpPost("model/3/action")
                .Returns(new RestResponse("null",
                    new Dictionary<string, string>
                    {
                        {"header1", "header1_value"},
                        {"header2", "header2_value"}
                    })
                );

            testing.Do(Id("3", "model"), "action", new Dictionary<string, ParameterValueData>());

            mockHeaderProcessor.Verify(hp => hp.Process(It.Is<IDictionary<string, string>>(h =>
                h.ContainsKey("header1") &&
                h["header1"] == "header1_value" &&
                h.ContainsKey("header2") &&
                h["header2"] == "header2_value"
            )));
        }

        [Test]
        public void Creates_exception_using_extractor_when_rest_service_result_is_exception()
        {
            config
                .Exception.Set(c => c.By(er => new TestException(er.Message)).When(er => er.Type == "type"))
            ;

            ModelsAre(Model("model").Operation("action"));

            SetUpGet("model/3").Returns(new RestResponse(
                "{" +
                    "\"IsException\":\"true\"," +
                    "\"Type\":\"type\"," +
                    "\"Handled\":\"true\"," +
                    "\"Message\":\"message\"" +
                "}"));

            try
            {
                testing.Get(Id("3", "model"));
                Assert.Fail("exception not thrown");
            }
            catch (TestException ex)
            {
                Assert.AreEqual("message", ex.Message);
            }

            SetUpPost("model/3/action").Returns(new RestResponse(
                "{" +
                    "\"IsException\":\"true\"," +
                    "\"Type\":\"type\"," +
                    "\"Handled\":\"true\"," +
                    "\"Message\":\"message\"" +
                "}"));

            try
            {
                testing.Do(Id("3", "model"), "action", new Dictionary<string, ParameterValueData>());
                Assert.Fail("exception not thrown");
            }
            catch (TestException ex)
            {
                Assert.AreEqual("message", ex.Message);
            }
        }

        [Test]
        public void Wraps_web_exceptions_as_exception_result_and_throws_the_exception_created_via_extractor()
        {
            config
                .Exception.Set(c => c.By(er => new TestException(er.Message)).When(er => er.Type == "Http.NotFound"))
            ;

            SetUpGet("model/3")
                .Throws(HttpNotFound("server message"));

            ModelsAre(Model("model").Operation("action"));

            try
            {
                testing.Get(Id("3", "model"));
                Assert.Fail("exception not thrown");
            }
            catch (TestException ex)
            {
                Assert.AreEqual("server message", ex.Message);
            }

            SetUpPost("model/3/action")
                .Throws(HttpNotFound("server message"));

            try
            {
                testing.Do(Id("3", "model"), "action", new Dictionary<string, ParameterValueData>());
                Assert.Fail("exception not thrown");
            }
            catch (TestException ex)
            {
                Assert.AreEqual("server message", ex.Message);
            }
        }

        [Test]
        public void When_given_model_or_operation_does_not_exist_in_application_model_throws_incompatible_model_exception()
        {
            config
                .Exception.Set(c => c.By(er => new TestException("operation")).When(er => er.Type == "OperationNotFound"))
                .Exception.Set(c => c.By(er => new TestException("type")).When(er => er.Type == "TypeNotFound"))
            ;

            ModelsAre(Model("model"));

            try
            {
                testing.Do(Id("3", "model"), "nonexistingaction", new Dictionary<string, ParameterValueData>());
                Assert.Fail("exception not thrown");
            }
            catch (TestException ex)
            {
                Assert.AreEqual("operation", ex.Message);
            }

            try
            {
                testing.Do(Id("3", "nonexistingmodel"), "nonexistingaction", new Dictionary<string, ParameterValueData>());
                Assert.Fail("exception not thrown");
            }
            catch (TestException ex)
            {
                Assert.AreEqual("type", ex.Message);
            }
        }

        [Test]
        [Ignore("")]
        public void Invalidates_cache_when_server_version_has_changed()
        {
            Assert.Fail("not implemented");
        }
    }
}
