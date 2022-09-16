using Routine.Core;
using Routine.Core.Rest;
using Routine.Service;
using Routine.Service.Configuration;
using Routine.Test.Core;
using Routine.Test.Engine.Stubs.ObjectServiceInvokers;
using Routine.Test.Service.Stubs;
using System.Net;

using AsyncInvoker = Routine.Test.Engine.Stubs.ObjectServiceInvokers.Async;
using AsyncStubber = Routine.Test.Service.Stubs.Async;
using SyncInvoker = Routine.Test.Engine.Stubs.ObjectServiceInvokers.Sync;
using SyncStubber = Routine.Test.Service.Stubs.Sync;

namespace Routine.Test.Service;

[TestFixture(typeof(SyncStubber), typeof(SyncInvoker))]
[TestFixture(typeof(AsyncStubber), typeof(AsyncInvoker))]
public class RestClientObjectServiceTest<TRestClientStubber, TObjectServiceInvoker> : CoreTestBase
    where TRestClientStubber : IRestClientStubber, new()
    where TObjectServiceInvoker : IObjectServiceInvoker, new()
{
    #region SetUp & Helpers

    private const string URL_BASE = "http://api.test.com/service";

    private ConventionBasedServiceClientConfiguration config;
    private Mock<IRestClient> mock;
    private IJsonSerializer serializer;
    private RestClientObjectService testing;
    private IRestClientStubber stubber;
    private IObjectServiceInvoker invoker;

    public override void SetUp()
    {
        base.SetUp();

        serializer = new JsonSerializerAdapter();
        config = BuildRoutine.ServiceClientConfig().FromBasic()
            .ServiceUrlBase.Set(URL_BASE)
        ;

        mock = new();
        testing = new(config, mock.Object, serializer);
        stubber = new TRestClientStubber();
        invoker = new TObjectServiceInvoker();

        mock.Setup(rc => rc.Get($"{URL_BASE}/ApplicationModel", It.IsAny<RestRequest>()))
            .Returns(() => new(serializer.Serialize(GetApplicationModel())));
    }

    private class TestException : Exception { public TestException(string message) : base(message) { } }

    private static WebException HttpNotFound(string message)
    {
        var mock = new Mock<HttpWebResponse>();
        mock.Setup(wr => wr.StatusCode).Returns(HttpStatusCode.NotFound);
        mock.Setup(wr => wr.StatusDescription).Returns(message);

        return new(message, null, WebExceptionStatus.Success, mock.Object);
    }

    #endregion

    [Test]
    public void Gets_application_model_using_get_method()
    {
        var actual = testing.ApplicationModel;

        mock.Verify(rc => rc.Get(It.Is<string>(url => url.EndsWith("/ApplicationModel")), RestRequest.Empty), Times.Once());
        Assert.AreEqual(0, actual.Models.Count);
    }

    [Test]
    public void Application_model_is_cached_so_that_it_does_not_fetch_model_each_time()
    {
        var expected = testing.ApplicationModel;
        var actual = testing.ApplicationModel;

        mock.Verify(rc => rc.Get(It.Is<string>(url => url.EndsWith("/ApplicationModel")), It.IsAny<RestRequest>()), Times.Once());

        Assert.AreSame(expected, actual);
    }

    [Test]
    public void Gets_object_data_using_get_method()
    {
        ModelsAre(Model("model"));

        stubber.SetUpGet(mock,
            url: $"{URL_BASE}/model/3",
            response: @"{""Id"":""3"",""Display"":""Test""}"
        );

        var actual = invoker.InvokeGet(testing, Id("3", "model"));

        Assert.AreEqual("Test", actual.Display);
    }

    [Test]
    public void When_given_reference_data_has_a_view_model_id__gets_object_data_using_view_model_id()
    {
        ModelsAre(
            Model("model").ViewModelIds("viewmodel"),
            Model("viewmodel").IsView("model")
        );

        stubber.SetUpGet(mock,
            url: $"{URL_BASE}/model/3/viewmodel",
            response: @"{""Id"":""3"",""Display"":""Test""}"
        );

        var actual = invoker.InvokeGet(testing, Id("3", "model", "viewmodel"));

        Assert.AreEqual("Test", actual.Display);
    }

    [Test]
    public void When_given_reference_data_is_null__returns_null_without_making_a_remote_call()
    {
        var actual = invoker.InvokeGet(testing, Null());

        Assert.AreEqual(null, actual);
        mock.Verify(rc => rc.Get(It.IsAny<string>(), It.IsAny<RestRequest>()), Times.Never());
        mock.Verify(rc => rc.Post(It.IsAny<string>(), It.IsAny<RestRequest>()), Times.Never());
        mock.Verify(rc => rc.PostAsync(It.IsAny<string>(), It.IsAny<RestRequest>()), Times.Never());
    }

    [Test]
    public void Gets_request_headers_from_configuration_and_sends_them_for_every_get()
    {
        config
            .RequestHeaders.Add("header1", "header2")
            .RequestHeaderValue.Set(c => c.By(h => h + "_value"))
        ;

        ModelsAre(Model("model"));

        stubber.SetUpGet(mock,
            url: $"{URL_BASE}/model/3",
            response: @"""3"""
        );

        invoker.InvokeGet(testing, Id("3", "model"));

        mock.Verify(rc => rc.Get(It.IsAny<string>(), It.Is<RestRequest>(req =>
            req.Headers.ContainsKey("header1") &&
            req.Headers["header1"] == "header1_value" &&
            req.Headers.ContainsKey("header2") &&
            req.Headers["header2"] == "header2_value"
        )));
    }

    [Test]
    public void Delegates_response_headers_to_response_header_processors_for_every_get()
    {
        var mockHeaderProcessor = new Mock<IHeaderProcessor>();

        config.ResponseHeaderProcessors.Add(mockHeaderProcessor.Object);

        ModelsAre(Model("model"));

        stubber.SetUpGet(mock,
            url: $"{URL_BASE}/model/3",
            response: new RestResponse("null",
                new Dictionary<string, string>
                {
                    { "header1", "header1_value" },
                    { "header2", "header2_value" }
                }
            )
        );

        invoker.InvokeGet(testing, Id("3", "model"));

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

        stubber.SetUpPost(mock,
            url: $"{URL_BASE}/model/3/action",
            body: @"{""arg1"":""4""}",
            response: @"{""Id"":""5"",""Display"":""Test""}"
        );

        var actual = invoker.InvokeDo(testing, Id("3", "model"), "action",
            new()
            {
                {
                    "arg1",
                    new()
                    {
                        Values = new()
                        {
                            new() { Id = "4", ModelId = "model" }
                        }
                    }
                }
            });

        Assert.AreEqual("5", actual.Values[0].Id);
        Assert.AreEqual("Test", actual.Values[0].Display);
    }

    [Test]
    public void When_given_target_is_null__returns_empty_variable_data_without_making_a_remote_call()
    {
        var actual = invoker.InvokeDo(testing, Null(), "doesn't matter", new());

        Assert.AreEqual(new VariableData(), actual);
        mock.Verify(rc => rc.Get(It.IsAny<string>(), It.IsAny<RestRequest>()), Times.Never());
        mock.Verify(rc => rc.Post(It.IsAny<string>(), It.IsAny<RestRequest>()), Times.Never());
        mock.Verify(rc => rc.PostAsync(It.IsAny<string>(), It.IsAny<RestRequest>()), Times.Never());
    }

    [Test]
    public void Gets_request_headers_from_configuration_and_sends_them_for_every_do()
    {
        config
            .RequestHeaders.Add("header1", "header2")
            .RequestHeaderValue.Set(c => c.By(h => h + "_value"))
        ;

        ModelsAre(Model("model").Operation("action", true));

        stubber.SetUpPost(mock,
            url: $"{URL_BASE}/model/3/action",
            response: "null"
        );

        invoker.InvokeDo(testing, Id("3", "model"), "action", new());

        stubber.VerifyPost(mock,
            match: req =>
                req.Headers.ContainsKey("header1") &&
                req.Headers["header1"] == "header1_value" &&
                req.Headers.ContainsKey("header2") &&
                req.Headers["header2"] == "header2_value"
        );
    }

    [Test]
    public void Delegates_response_headers_to_response_header_processors_for_every_do()
    {
        var mockHeaderProcessor = new Mock<IHeaderProcessor>();

        config.ResponseHeaderProcessors.Add(mockHeaderProcessor.Object);

        ModelsAre(Model("model").Operation("action", true));

        stubber.SetUpPost(mock,
            url: $"{URL_BASE}/model/3/action",
            response: new RestResponse("null",
                new Dictionary<string, string>
                {
                    {"header1", "header1_value"},
                    {"header2", "header2_value"}
                })
            );

        invoker.InvokeDo(testing, Id("3", "model"), "action", new());

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

        stubber.SetUpGet(mock,
            url: $"{URL_BASE}/model/3",
            response: @"{""IsException"":""true"",""Type"":""type"",""Handled"":""true"",""Message"":""message""}"
        );

        try
        {
            invoker.InvokeGet(testing, Id("3", "model"));
            Assert.Fail("exception not thrown");
        }
        catch (TestException ex)
        {
            Assert.AreEqual("message", ex.Message);
        }

        stubber.SetUpPost(mock,
            url: $"{URL_BASE}/model/3/action",
            response: new RestResponse(@"{""IsException"":""true"",""Type"":""type"",""Handled"":""true"",""Message"":""message""}")
        );

        try
        {
            invoker.InvokeDo(testing, Id("3", "model"), "action", new());
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

        stubber.SetUpGet(mock,
            url: $"{URL_BASE}/model/3",
            exception: HttpNotFound("server message")
        );

        ModelsAre(Model("model").Operation("action"));

        try
        {
            invoker.InvokeGet(testing, Id("3", "model"));
            Assert.Fail("exception not thrown");
        }
        catch (TestException ex)
        {
            Assert.AreEqual("server message", ex.Message);
        }

        stubber.SetUpPost(mock,
            url: $"{URL_BASE}/model/3/action",
            exception: HttpNotFound("server message")
        );

        try
        {
            invoker.InvokeDo(testing, Id("3", "model"), "action", new());
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
            .Exception.Set(c => c.By(_ => new TestException("operation")).When(er => er.Type == "OperationNotFound"))
            .Exception.Set(c => c.By(_ => new TestException("type")).When(er => er.Type == "TypeNotFound"))
        ;

        ModelsAre(Model("model"));

        try
        {
            invoker.InvokeDo(testing, Id("3", "model"), "nonexistingaction", new());
            Assert.Fail("exception not thrown");
        }
        catch (TestException ex)
        {
            Assert.AreEqual("operation", ex.Message);
        }

        try
        {
            invoker.InvokeDo(testing, Id("3", "nonexistingmodel"), "nonexistingaction", new());
            Assert.Fail("exception not thrown");
        }
        catch (TestException ex)
        {
            Assert.AreEqual("type", ex.Message);
        }
    }
}
