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

    private ConventionBasedServiceClientConfiguration _config;
    private Mock<IRestClient> _mock;
    private IJsonSerializer _serializer;
    private RestClientObjectService _testing;
    private IRestClientStubber _stubber;
    private IObjectServiceInvoker _invoker;

    public override void SetUp()
    {
        base.SetUp();

        _serializer = new JsonSerializerAdapter();
        _config = BuildRoutine.ServiceClientConfig().FromBasic()
            .ServiceUrlBase.Set(URL_BASE)
        ;

        _mock = new();
        _testing = new(_config, _mock.Object, _serializer);
        _stubber = new TRestClientStubber();
        _invoker = new TObjectServiceInvoker();

        _mock.Setup(rc => rc.Get($"{URL_BASE}/ApplicationModel", It.IsAny<RestRequest>()))
            .Returns(() => new(_serializer.Serialize(GetApplicationModel())));
    }

    private class TestException : Exception { public TestException(string message) : base(message) { } }

    private static RestRequestException HttpNotFound(string message) => new(HttpStatusCode.NotFound, new(message));

    #endregion

    [Test]
    public void Gets_application_model_using_get_method()
    {
        var actual = _testing.ApplicationModel;

        _mock.Verify(rc => rc.Get(It.Is<string>(url => url.EndsWith("/ApplicationModel")), RestRequest.Empty), Times.Once());
        Assert.That(actual.Models.Count, Is.EqualTo(0));
    }

    [Test]
    public void Application_model_is_cached_so_that_it_does_not_fetch_model_each_time()
    {
        var expected = _testing.ApplicationModel;
        var actual = _testing.ApplicationModel;

        _mock.Verify(rc => rc.Get(It.Is<string>(url => url.EndsWith("/ApplicationModel")), It.IsAny<RestRequest>()), Times.Once());

        Assert.That(actual, Is.SameAs(expected));
    }

    [Test]
    public void Gets_object_data_using_get_method()
    {
        ModelsAre(Model("model"));

        _stubber.SetUpGet(_mock,
            url: $"{URL_BASE}/model/3",
            response: @"{""Id"":""3"",""Display"":""Test""}"
        );

        var actual = _invoker.InvokeGet(_testing, Id("3", "model"));

        Assert.That(actual.Display, Is.EqualTo("Test"));
    }

    [Test]
    public void When_given_reference_data_has_a_view_model_id__gets_object_data_using_view_model_id()
    {
        ModelsAre(
            Model("model").ViewModelIds("viewmodel"),
            Model("viewmodel").IsView("model")
        );

        _stubber.SetUpGet(_mock,
            url: $"{URL_BASE}/model/3/viewmodel",
            response: @"{""Id"":""3"",""Display"":""Test""}"
        );

        var actual = _invoker.InvokeGet(_testing, Id("3", "model", "viewmodel"));

        Assert.That(actual.Display, Is.EqualTo("Test"));
    }

    [Test]
    public void When_given_reference_data_is_null__returns_null_without_making_a_remote_call()
    {
        var actual = _invoker.InvokeGet(_testing, Null());

        Assert.That(actual, Is.Null);
        _mock.Verify(rc => rc.Get(It.IsAny<string>(), It.IsAny<RestRequest>()), Times.Never());
        _mock.Verify(rc => rc.GetAsync(It.IsAny<string>(), It.IsAny<RestRequest>()), Times.Never());
        _mock.Verify(rc => rc.Post(It.IsAny<string>(), It.IsAny<RestRequest>()), Times.Never());
        _mock.Verify(rc => rc.PostAsync(It.IsAny<string>(), It.IsAny<RestRequest>()), Times.Never());
    }

    [Test]
    public void Gets_request_headers_from_configuration_and_sends_them_for_every_get()
    {
        _config
            .RequestHeaders.Add("header1", "header2")
            .RequestHeaderValue.Set(c => c.By(h => h + "_value"))
        ;

        ModelsAre(Model("model"));

        _stubber.SetUpGet(_mock,
            url: $"{URL_BASE}/model/3",
            response: @"""3"""
        );

        _invoker.InvokeGet(_testing, Id("3", "model"));

        _stubber.VerifyGet(_mock,
            match: req =>
                req.Headers.ContainsKey("header1") &&
                req.Headers["header1"] == "header1_value" &&
                req.Headers.ContainsKey("header2") &&
                req.Headers["header2"] == "header2_value"
        );
    }

    [Test]
    public void Delegates_response_headers_to_response_header_processors_for_every_get()
    {
        var mockHeaderProcessor = new Mock<IHeaderProcessor>();

        _config.ResponseHeaderProcessors.Add(mockHeaderProcessor.Object);

        ModelsAre(Model("model"));

        _stubber.SetUpGet(_mock,
            url: $"{URL_BASE}/model/3",
            response: new RestResponse("null",
                new Dictionary<string, string>
                {
                    { "header1", "header1_value" },
                    { "header2", "header2_value" }
                }
            )
        );

        _invoker.InvokeGet(_testing, Id("3", "model"));

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

        _stubber.SetUpPost(_mock,
            url: $"{URL_BASE}/model/3/action",
            body: @"{""arg1"":""4""}",
            response: @"{""Id"":""5"",""Display"":""Test""}"
        );

        var actual = _invoker.InvokeDo(_testing, Id("3", "model"), "action",
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

        Assert.That(actual.Values[0].Id, Is.EqualTo("5"));
        Assert.That(actual.Values[0].Display, Is.EqualTo("Test"));
    }

    [Test]
    public void When_given_target_is_null__returns_empty_variable_data_without_making_a_remote_call()
    {
        var actual = _invoker.InvokeDo(_testing, Null(), "doesn't matter", new());

        Assert.That(actual, Is.EqualTo(new VariableData()));
        _mock.Verify(rc => rc.Get(It.IsAny<string>(), It.IsAny<RestRequest>()), Times.Never());
        _mock.Verify(rc => rc.GetAsync(It.IsAny<string>(), It.IsAny<RestRequest>()), Times.Never());
        _mock.Verify(rc => rc.Post(It.IsAny<string>(), It.IsAny<RestRequest>()), Times.Never());
        _mock.Verify(rc => rc.PostAsync(It.IsAny<string>(), It.IsAny<RestRequest>()), Times.Never());
    }

    [Test]
    public void Gets_request_headers_from_configuration_and_sends_them_for_every_do()
    {
        _config
            .RequestHeaders.Add("header1", "header2")
            .RequestHeaderValue.Set(c => c.By(h => h + "_value"))
        ;

        ModelsAre(Model("model").Operation("action", true));

        _stubber.SetUpPost(_mock,
            url: $"{URL_BASE}/model/3/action",
            response: "null"
        );

        _invoker.InvokeDo(_testing, Id("3", "model"), "action", new());

        _stubber.VerifyPost(_mock,
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

        _config.ResponseHeaderProcessors.Add(mockHeaderProcessor.Object);

        ModelsAre(Model("model").Operation("action", true));

        _stubber.SetUpPost(_mock,
            url: $"{URL_BASE}/model/3/action",
            response: new RestResponse("null",
                new Dictionary<string, string>
                {
                    {"header1", "header1_value"},
                    {"header2", "header2_value"}
                })
            );

        _invoker.InvokeDo(_testing, Id("3", "model"), "action", new());

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
        _config
            .Exception.Set(c => c.By(er => new TestException(er.Message)).When(er => er.Type == "type"))
        ;

        ModelsAre(Model("model").Operation("action"));

        _stubber.SetUpGet(_mock,
            url: $"{URL_BASE}/model/3",
            response: @"{""IsException"":""true"",""Type"":""type"",""Handled"":""true"",""Message"":""message""}"
        );

        Assert.That(() => _invoker.InvokeGet(_testing, Id("3", "model")),
            Throws.TypeOf<TestException>().With.Property("Message").EqualTo("message")
        );

        _stubber.SetUpPost(_mock,
            url: $"{URL_BASE}/model/3/action",
            response: new RestResponse(@"{""IsException"":""true"",""Type"":""type"",""Handled"":""true"",""Message"":""message""}")
        );

        Assert.That(() => _invoker.InvokeDo(_testing, Id("3", "model"), "action", new()),
            Throws.TypeOf<TestException>().With.Property("Message").EqualTo("message")
        );
    }

    [Test]
    public void Wraps_web_exceptions_as_exception_result_and_throws_the_exception_created_via_extractor()
    {
        _config
            .Exception.Set(c => c.By(er => new TestException(er.Message)).When(er => er.Type == "Http.NotFound"))
        ;

        _stubber.SetUpGet(_mock,
            url: $"{URL_BASE}/model/3",
            exception: HttpNotFound("server message")
        );

        ModelsAre(Model("model").Operation("action"));

        Assert.That(() => _invoker.InvokeGet(_testing, Id("3", "model")),
            Throws.TypeOf<TestException>().With.Property("Message").EqualTo("server message")
        );

        _stubber.SetUpPost(_mock,
            url: $"{URL_BASE}/model/3/action",
            exception: HttpNotFound("server message")
        );

        Assert.That(() => _invoker.InvokeDo(_testing, Id("3", "model"), "action", new()),
            Throws.TypeOf<TestException>().With.Property("Message").EqualTo("server message")
        );
    }

    [Test]
    public void When_given_model_or_operation_does_not_exist_in_application_model_throws_incompatible_model_exception()
    {
        _config
            .Exception.Set(c => c.By(_ => new TestException("operation")).When(er => er.Type == "OperationNotFound"))
            .Exception.Set(c => c.By(_ => new TestException("type")).When(er => er.Type == "TypeNotFound"))
        ;

        ModelsAre(Model("model"));

        Assert.That(() => _invoker.InvokeDo(_testing, Id("3", "model"), "nonexistingaction", new()),
            Throws.TypeOf<TestException>().With.Property("Message").EqualTo("operation")
        );

        Assert.That(() => _invoker.InvokeDo(_testing, Id("3", "nonexistingmodel"), "nonexistingaction", new()),
            Throws.TypeOf<TestException>().With.Property("Message").EqualTo("type")
        );
    }
}
