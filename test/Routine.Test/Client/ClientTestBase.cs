using Routine.Client.Context;
using Routine.Client;
using Routine.Core;
using Routine.Test.Core;

namespace Routine.Test.Client;

public abstract class ClientTestBase : CoreTestBase
{
    protected Mock<IObjectService> _mockObjectService;

    protected IClientContext _ctx;
    protected Rapplication _testingRapplication;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        _mockObjectService = new Mock<IObjectService>();

        var clientContext = new DefaultClientContext(_mockObjectService.Object, new Rapplication(_mockObjectService.Object));

        _ctx = clientContext;
        _testingRapplication = clientContext.Application;

        _mockObjectService.Setup(o => o.ApplicationModel)
            .Returns(GetApplicationModel);
        _mockObjectService.Setup(o => o.Get(It.IsAny<ReferenceData>()))
            .Returns((ReferenceData ord) => ObjectData(ord));
        _mockObjectService.Setup(o => o.GetAsync(It.IsAny<ReferenceData>()))
            .ReturnsAsync((ReferenceData ord) => ObjectData(ord));
        _mockObjectService.Setup(o => o.Do(It.IsAny<ReferenceData>(), It.IsAny<string>(), It.IsAny<Dictionary<string, ParameterValueData>>()))
            .Returns(Void());
        _mockObjectService.Setup(o => o.DoAsync(It.IsAny<ReferenceData>(), It.IsAny<string>(), It.IsAny<Dictionary<string, ParameterValueData>>()))
            .ReturnsAsync(Void());

        ModelsAre(Model());
    }

    private ObjectData ObjectData(ReferenceData ord) => _objectDictionary[ord];

    protected Rtype Rtyp(string id) => _testingRapplication[id];

    protected Robject RobjNull() => _testingRapplication.NullObject();
    protected Robject Robj(string id) => Robj(id, DefaultObjectModelId);
    protected Robject Robj(string id, string modelId) => Robj(id, modelId, modelId);
    protected Robject Robj(string id, string actualModelId, string viewModelId) =>
        _testingRapplication.Get(id, actualModelId, viewModelId);

    protected Robject Robj(string modelId, params Rvariable[] initializationParameters) =>
        _testingRapplication.Init(modelId, initializationParameters);

    protected Rvariable Rvar(string name, Robject value) => new(name, value);
    protected Rvariable Rvarlist(string name, IEnumerable<Robject> values) => new(name, values);
}
