using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Moq.Language.Flow;
using Routine.Core;
using Routine.Core.Rest;
using Routine.Service;
using Routine.Service.Configuration;
using Routine.Service.RequestHandlers;
using Routine.Test.Core;
using System.Linq.Expressions;
using System.Net;
using System.Text;

namespace Routine.Test.Service;

[TestFixture]
public class HandleRequestHandlerTest : CoreTestBase
{
    #region SetUp & Helpers

    private Mock<IHttpContextAccessor> _httpContextAccessor;
    private Mock<IServiceContext> _serviceContext;
    private Mock<IObjectService> _objectService;
    private Mock<HttpRequest> _request;
    private Mock<HttpResponse> _response;
    private IHeaderDictionary _requestHeaders;
    private IHeaderDictionary _responseHeaders;
    private QueryString _requestQueryString;
    private IQueryCollection _requestQuery;
    private HandleRequestHandler _testing;
    private ConventionBasedServiceConfiguration _config;
    private Mock<IFeatureCollection> _featureCollection;
    private Mock<IHttpResponseFeature> _httpResponseFeature;
    private IJsonSerializer _serializer;

    public override void SetUp()
    {
        base.SetUp();

        _httpContextAccessor = new();
        var httpContext = new Mock<HttpContext>();
        _serviceContext = new();
        _objectService = new();
        _request = new();
        _response = new();
        _featureCollection = new();
        _httpResponseFeature = new();
        _requestHeaders = new HeaderDictionary();
        _responseHeaders = new HeaderDictionary();
        _requestQueryString = new();
        _requestQuery = new QueryCollection();
        _serializer = new JsonSerializerAdapter();

        _serviceContext.Setup(sc => sc.ObjectService).Returns(_objectService.Object);
        _config = BuildRoutine.ServiceConfig().FromBasic();
        _serviceContext.Setup(sc => sc.ServiceConfiguration).Returns(_config);
        _objectService.Setup(os => os.ApplicationModel).Returns(() => GetApplicationModel());
        _objectService.Setup(os => os.GetAsync(It.IsAny<ReferenceData>())).ReturnsAsync((ReferenceData referenceData) =>
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

            return _objectDictionary[referenceData];
        });
        _request.Setup(r => r.Headers).Returns(_requestHeaders);
        _request.Setup(r => r.QueryString).Returns(_requestQueryString);
        _request.Setup(r => r.Query).Returns(_requestQuery);
        _request.Setup(r => r.Method).Returns("POST");
        _request.Setup(r => r.Body).Returns(new MemoryStream()).Verifiable();

        // https://stackoverflow.com/questions/34677203/testing-the-result-of-httpresponse-statuscode/34677864#34677864
        _response.SetupAllProperties();
        _response.Setup(r => r.Body).Returns(new MemoryStream()).Verifiable();
        _response.Setup(r => r.Headers).Returns(_responseHeaders);
        _httpContextAccessor.Setup(hca => hca.HttpContext).Returns(httpContext.Object);
        _httpContextAccessor.Setup(hca => hca.HttpContext.Request).Returns(_request.Object);
        _httpContextAccessor.Setup(hca => hca.HttpContext.Response).Returns(_response.Object);
        _httpContextAccessor.Setup(hca => hca.HttpContext.Features).Returns(_featureCollection.Object);
        _httpContextAccessor.Setup(hca => hca.HttpContext.Response.HttpContext.Features.Get<IHttpResponseFeature>()).Returns(_httpResponseFeature.Object);
        _httpContextAccessor.Setup(hca => hca.HttpContext.Items).Returns(new Dictionary<object, object>());

        RequestHandlerBase.ClearModelIndex();
        _testing = new HandleRequestHandler(_serviceContext.Object, _serializer, _httpContextAccessor.Object,
            actionFactory: resolution => resolution.HasOperation
                ? new DoRequestHandler(_serviceContext.Object, _serializer, _httpContextAccessor.Object, resolution)
                : new GetRequestHandler(_serviceContext.Object, _serializer, _httpContextAccessor.Object, resolution)
        );
    }

    protected ObjectStubber When(ReferenceData referenceData) => new(_objectService, referenceData);

    protected class ObjectStubber
    {
        private readonly Mock<IObjectService> _objectService;
        private readonly ReferenceData _referenceData;

        public ObjectStubber(Mock<IObjectService> objectService, ReferenceData referenceData)
        {
            _objectService = objectService;
            _referenceData = referenceData;
        }

        public ISetup<IObjectService, Task<VariableData>> Performs(string operationName) => Performs(operationName, p => true);
        public ISetup<IObjectService, Task<VariableData>> Performs(string operationName, Expression<Func<Dictionary<string, ParameterValueData>, bool>> parameterMatcher) =>
            _objectService.Setup(o => o
                .DoAsync(
                    _referenceData,
                    operationName,
                    It.Is(parameterMatcher)
                )
            );
    }

    private void SetUpRequestBody(string body)
    {
        var sw = new StreamWriter(_request.Object.Body);
        sw.Write(body);
        sw.Flush();
    }

    #endregion

    [Test]
    public async Task When_operation_is_not_given__interprets_the_call_as_a_get_request()
    {
        ModelsAre(Model("model"));
        ObjectsAre(Object(Id("3", "model")));

        await _testing.Handle("model", "3", null, null);

        _objectService.Verify(os => os.GetAsync(Id("3", "model")));
    }

    [Test]
    public async Task When_operation_is_given__interprets_the_call_as_a_do_request()
    {
        ModelsAre(
            Model("model").Operation("action", true)
        );

        await _testing.Handle("model", "3", "action", null);

        _objectService.Verify(os => os.DoAsync(Id("3", "model"), "action", It.Is<Dictionary<string, ParameterValueData>>(p => p.Count == 0)));
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
        await _testing.Handle("model", null, null, null);
        _objectService.Verify(os => os.GetAsync(Id(null, "model")));

        // /modelId-short
        await _testing.Handle("model2", null, null, null);
        _objectService.Verify(os => os.GetAsync(Id(null, "prefix.model2")));

        // /modelId/id
        await _testing.Handle("model", "instance", null, null);
        _objectService.Verify(os => os.GetAsync(Id("instance", "model")));

        // /modelId-short/id
        await _testing.Handle("model2", "instance", null, null);
        _objectService.Verify(os => os.GetAsync(Id("instance", "prefix.model2")));

        // /modelId/viewModelId
        await _testing.Handle("model", "viewmodel", null, null);
        _objectService.Verify(os => os.GetAsync(Id(null, "model", "viewmodel")));

        // /modelId-short/viewModelId
        await _testing.Handle("model2", "viewmodel", null, null);
        _objectService.Verify(os => os.GetAsync(Id(null, "prefix.model2", "viewmodel")));

        // /modelId/viewModelId-short
        await _testing.Handle("model", "viewmodel2", null, null);
        _objectService.Verify(os => os.GetAsync(Id(null, "model", "prefix.viewmodel2")));

        // /modelId-short/viewModelId-short
        await _testing.Handle("model2", "viewmodel2", null, null);
        _objectService.Verify(os => os.GetAsync(Id(null, "prefix.model2", "prefix.viewmodel2")));

        // /modelId/operation
        await _testing.Handle("model", "action", null, null);
        _objectService.Verify(os => os.DoAsync(Id(null, "model"), "action", It.Is<Dictionary<string, ParameterValueData>>(pvd => pvd.Count == 0)));

        // /modelId-short/operation
        await _testing.Handle("model2", "action", null, null);
        _objectService.Verify(os => os.DoAsync(Id(null, "prefix.model2"), "action", It.Is<Dictionary<string, ParameterValueData>>(pvd => pvd.Count == 0)));

        // /modelId/id/viewModelId
        await _testing.Handle("model", "instance", "viewmodel", null);
        _objectService.Verify(os => os.GetAsync(Id("instance", "model", "viewmodel")));

        // /modelId-short/id/viewModelId
        await _testing.Handle("model2", "instance", "viewmodel", null);
        _objectService.Verify(os => os.GetAsync(Id("instance", "prefix.model2", "viewmodel")));

        // /modelId/id/viewModelId-short
        await _testing.Handle("model", "instance", "viewmodel2", null);
        _objectService.Verify(os => os.GetAsync(Id("instance", "model", "prefix.viewmodel2")));

        // /modelId-short/id/viewModelId-short
        await _testing.Handle("model2", "instance", "viewmodel2", null);
        _objectService.Verify(os => os.GetAsync(Id("instance", "prefix.model2", "prefix.viewmodel2")));

        // /modelId/id/operation
        await _testing.Handle("model", "instance", "action", null);
        _objectService.Verify(os => os.DoAsync(Id("instance", "model"), "action", It.Is<Dictionary<string, ParameterValueData>>(pvd => pvd.Count == 0)));

        // /modelId-short/id/operation
        await _testing.Handle("model2", "instance", "action", null);
        _objectService.Verify(os => os.DoAsync(Id("instance", "prefix.model2"), "action", It.Is<Dictionary<string, ParameterValueData>>(pvd => pvd.Count == 0)));

        // /modelId/id/viewModelId/operation
        await _testing.Handle("model", "instance", "viewmodel", "action");
        _objectService.Verify(os => os.DoAsync(Id("instance", "model", "viewmodel"), "action", It.Is<Dictionary<string, ParameterValueData>>(pvd => pvd.Count == 0)));

        // /modelId-short/id/viewModelId/operation
        await _testing.Handle("model2", "instance", "viewmodel", "action");
        _objectService.Verify(os => os.DoAsync(Id("instance", "prefix.model2", "viewmodel"), "action", It.Is<Dictionary<string, ParameterValueData>>(pvd => pvd.Count == 0)));

        // /modelId/id/viewModelId-short/operation
        await _testing.Handle("model", "instance", "viewmodel2", "action");
        _objectService.Verify(os => os.DoAsync(Id("instance", "model", "prefix.viewmodel2"), "action", It.Is<Dictionary<string, ParameterValueData>>(pvd => pvd.Count == 0)));

        // /modelId-short/id/viewModelId-short/operation
        await _testing.Handle("model2", "instance", "viewmodel2", "action");
        _objectService.Verify(os => os.DoAsync(Id("instance", "prefix.model2", "prefix.viewmodel2"), "action", It.Is<Dictionary<string, ParameterValueData>>(pvd => pvd.Count == 0)));
    }

    [Test]
    public async Task GET_method_is_available_for_get_actions()
    {
        ModelsAre(Model("model"));
        ObjectsAre(Object(Id("3", "model")));

        _request.Setup(r => r.Method).Returns("GET");

        await _testing.Handle("model", "3", null, null);

        _objectService.Verify(os => os.GetAsync(Id("3", "model")));
    }

    [Test]
    public async Task GET_method_is_available_for_only_configured_do_actions()
    {
        _request.Setup(r => r.Method).Returns("GET");

        _config.AllowGet.Set(true, m => m.OperationModel.Name == "get");
        _config.AllowGet.Set(false, m => m.OperationModel.Name == "post");

        ModelsAre(
            Model("model")
            .Operation("post")
            .Operation("get")
        );
        ObjectsAre(Object(Id("3", "model")));

        await _testing.Handle("model", "3", "get", null);

        _objectService.Verify(os => os.DoAsync(It.IsAny<ReferenceData>(), "get", It.IsAny<Dictionary<string, ParameterValueData>>()));

        await _testing.Handle("model", "3", "post", null);

        _objectService.Verify(os => os.DoAsync(It.IsAny<ReferenceData>(), "post", It.IsAny<Dictionary<string, ParameterValueData>>()), Times.Never());
        Assert.That(_httpContextAccessor.Object.HttpContext?.Response, Is.Not.Null);
        Assert.That((HttpStatusCode)_httpContextAccessor.Object.HttpContext.Response.StatusCode, Is.EqualTo(HttpStatusCode.MethodNotAllowed));
    }

    [Test]
    public async Task Only_GET_and_POST_methods_are_supported()
    {
        ModelsAre(
            Model("model").Operation("action")
        );

        _request.Setup(r => r.Method).Returns("DELETE");
        await _testing.Handle("model", "action", null, null);
        Assert.That(_httpContextAccessor.Object.HttpContext?.Response, Is.Not.Null);
        Assert.That((HttpStatusCode)_httpContextAccessor.Object.HttpContext.Response.StatusCode, Is.EqualTo(HttpStatusCode.MethodNotAllowed));

        _request.Setup(r => r.Method).Returns("PUT");
        await _testing.Handle("model", "action", null, null);
        Assert.That(_httpContextAccessor.Object.HttpContext.Response, Is.Not.Null);
        Assert.That((HttpStatusCode)_httpContextAccessor.Object.HttpContext.Response.StatusCode, Is.EqualTo(HttpStatusCode.MethodNotAllowed));

        _objectService.Verify(os => os.DoAsync(It.IsAny<ReferenceData>(), It.IsAny<string>(), It.IsAny<Dictionary<string, ParameterValueData>>()), Times.Never());
        _objectService.Verify(os => os.GetAsync(It.IsAny<ReferenceData>()), Times.Never());
    }

    [Test]
    public async Task When_given_model_id_does_not_exist__returns_404()
    {
        await _testing.Handle("nonexistingmodel", null, null, null);

        Assert.That(_httpContextAccessor.Object.HttpContext?.Response, Is.Not.Null);
        Assert.That((HttpStatusCode)_httpContextAccessor.Object.HttpContext.Response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        Assert.That(
            _httpContextAccessor.Object.HttpContext.Response.Headers["X-Status-Description"].ToString(),
            Contains.Substring("nonexistingmodel"),
            "StatusDescription should contain given model id"
        );
    }

    [Test]
    public async Task When_given_model_id_is_not_full_id_and_there_are_more_than_one_model_with_similar_name__returns_404()
    {
        ModelsAre(Model("prefix1.model"), Model("prefix2.model"));

        await _testing.Handle("model", null, null, null);

        var statusDescription = _httpContextAccessor.Object.HttpContext?.Response.Headers["X-Status-Description"].ToString();

        Assert.That(_httpContextAccessor.Object.HttpContext?.Response, Is.Not.Null);
        Assert.That((HttpStatusCode)_httpContextAccessor.Object.HttpContext.Response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        Assert.That(statusDescription, Contains.Substring("prefix1.model"), "Status description should contain available model ids: prefix1.model");
        Assert.That(statusDescription, Contains.Substring("prefix2.model"), "Status description should contain available model ids: prefix2.model");
    }

    [Test]
    public async Task When_given_parameters_contains_a_non_existing_model_id__returns_404()
    {
        ModelsAre(
            Model("model").Operation("action", PModel("arg", "model"))
        );

        SetUpRequestBody("{\"arg\":{\"ModelId\":\"nonexistingmodel\",\"Data\":{\"arg\":null}}}");

        await _testing.Handle("model", "action", null, null);

        Assert.That(_httpContextAccessor.Object.HttpContext?.Response, Is.Not.Null);
        Assert.That((HttpStatusCode)_httpContextAccessor.Object.HttpContext.Response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        Assert.That(
            _httpContextAccessor.Object.HttpContext.Response.Headers["X-Status-Description"].ToString(),
            Contains.Substring("nonexistingmodel"),
            "Status description should contain given model id"
        );
    }

    [Test]
    public async Task When_body_cannot_be_deserialized__returns_BadRequest()
    {
        ModelsAre(
            Model("model").Operation("action")
        );

        SetUpRequestBody("{");

        await _testing.Handle("model", "action", null, null);

        Assert.That(_httpContextAccessor.Object.HttpContext?.Response, Is.Not.Null);
        Assert.That((HttpStatusCode)_httpContextAccessor.Object.HttpContext.Response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Request_headers_are_processed_for_each_get()
    {
        var header = new Mock<IHeaderProcessor>();

        ModelsAre(Model("model"));
        ObjectsAre(Object(Id("3", "model")));

        _requestHeaders.Append("test", "value");

        _config.RequestHeaders.Add("test");
        _config.RequestHeaderProcessors.Add(header.Object);

        await _testing.Handle("model", "3", null, null);

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

        _requestHeaders.Append("test", "value");

        _config.RequestHeaders.Add("test");
        _config.RequestHeaderProcessors.Add(header.Object);

        await _testing.Handle("model", "3", "action", null);

        header.Verify(h => h.Process(It.Is<Dictionary<string, string>>(d =>
            d.ContainsKey("test") && d["test"] == "value"
        )));
    }

    [Test]
    public async Task Request_headers_are_not_processed_before_validation_for_do_requests()
    {
        var header = new Mock<IHeaderProcessor>();

        _requestHeaders.Append("test", "value");

        _config.RequestHeaders.Add("test");
        _config.RequestHeaderProcessors.Add(header.Object);

        await _testing.Handle("model", "3", "action", null);

        header.Verify(h => h.Process(It.IsAny<Dictionary<string, string>>()), Times.Never());
    }

    [Test]
    public async Task Configured_response_headers_are_put_into_response_headers()
    {
        _config.ResponseHeaders.Add("test");
        _config.ResponseHeaderValue.Set("value", "test");

        ModelsAre(Model("model"));
        ObjectsAre(Object(Id("3", "model")));

        await _testing.Handle("model", "3", null, null);

        Assert.That(_responseHeaders["test"], Is.EqualTo("value"));
    }

    [Test]
    public async Task Compress_for_do_result_viewmodel()
    {
        ModelsAre(
            Model("model").Operation("action", "viewmodel").ViewModelIds("viewmodel"),
            Model("viewmodel").IsView("model").Data("data", "string"),
            Model("string").IsValue()
        );

        When(Id("2", "model")).Performs("action").ReturnsAsync(new VariableData
        {
            Values = new()
            {
                Object(Id("2", "model", "viewmodel"))
                .Display("model 2")
                .Data("data", Id("text", "string"))
                .Build().Item2
            }
        });

        await _testing.Handle("model", "2", "action", null);
        var body = "{" +
                   "\"Id\":\"2\"," +
                   "\"Display\":\"model 2\"," +
                   "\"ModelId\":\"model\"," +
                   "\"Data\":" +
                   "{" +
                   "\"data\":\"text\"" +
                   "}" +
                   "}";

        var actual = Encoding.UTF8.GetString(((MemoryStream)_httpContextAccessor.Object.HttpContext?.Response.Body)?.ToArray());

        Assert.That(actual, Is.EqualTo(body));
    }

    [Test]
    public async Task Compress_for_do_result_model()
    {
        ModelsAre(
            Model("model").Operation("action", "model").Data("data", "string"),
            Model("string").IsValue()
        );

        When(Id("2", "model")).Performs("action").ReturnsAsync(new VariableData
        {
            Values = new()
            {
                Object(Id("2", "model"))
                .Display("model 2")
                .Data("data", Id("text", "string"))
                .Build().Item2
            }
        });

        await _testing.Handle("model", "2", "action", null);
        var body = "{" +
                   "\"Id\":\"2\"," +
                   "\"Display\":\"model 2\"," +
                   "\"Data\":" +
                   "{" +
                   "\"data\":\"text\"" +
                   "}" +
                   "}";

        var actual = Encoding.UTF8.GetString(((MemoryStream)_httpContextAccessor.Object.HttpContext?.Response.Body)?.ToArray());

        Assert.That(actual, Is.EqualTo(body));
    }

    [Test]
    public async Task Decompress_for_parameterviewmodel()
    {
        ModelsAre(
            Model("model").Operation("action", PModel("arg", "model"))
        );

        SetUpRequestBody("{\"arg\":\"3\"}");

        await _testing.Handle("model", "2", "action", null);

        _objectService.Verify(os => os.DoAsync(Id("2", "model"), "action", It.Is<Dictionary<string, ParameterValueData>>(pvd =>
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

        await _testing.Handle("model", "3", null, null);
        var body = "{" +
                   "\"Id\":\"3\"," +
                   "\"Display\":\"display\"," +
                   "\"Data\":" +
                   "{" +
                   "\"data\":\"text\"" +
                   "}" +
                   "}";

        var actual = Encoding.UTF8.GetString(((MemoryStream)_httpContextAccessor.Object.HttpContext?.Response.Body)?.ToArray());

        Assert.That(actual, Is.EqualTo(body));
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

        await _testing.Handle("model", "3", "viewmodel", null);
        var body = "{" +
                   "\"Id\":\"3\"," +
                   "\"Display\":\"display\"," +
                   "\"ModelId\":\"model\"," +
                   "\"Data\":" +
                   "{" +
                   "\"data\":\"text\"" +
                   "}" +
                   "}";

        var actual = Encoding.UTF8.GetString(((MemoryStream)_httpContextAccessor.Object.HttpContext?.Response.Body)?.ToArray());

        Assert.That(actual, Is.EqualTo(body));
    }

    [Test]
    public async Task Ignore_nonexisting_parameter()
    {
        ModelsAre(
            Model("model").Operation("action")
        );

        SetUpRequestBody("{\"arg\":\"3\"}");

        await _testing.Handle("model", "2", "action", null);

        _objectService.Verify(os => os.DoAsync(Id("2", "model"), "action", It.Is<Dictionary<string, ParameterValueData>>(pvd =>
            pvd.Count == 0
        )));
    }
}
