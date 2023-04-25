using Routine.Core;

namespace Routine.Test.Core;

[TestFixture]
public class ModelBuildFromDictionaryWithFaultToleranceTest
{
    [Test]
    public void ApplicationModel()
    {
        var testing = new ApplicationModel();

        Assert.That(testing.Model, Is.Not.Null);

        testing = new ApplicationModel(new Dictionary<string, object>());

        Assert.That(testing.Model, Is.Not.Null);
    }

    [Test]
    public void ObjectModel()
    {
        var testing = new ObjectModel();

        Assert.That(testing.ActualModelIds, Is.Not.Null);
        Assert.That(testing.Datas, Is.Not.Null);
        Assert.That(testing.Initializer, Is.Not.Null);
        Assert.That(testing.Marks, Is.Not.Null);
        Assert.That(testing.Operations, Is.Not.Null);
        Assert.That(testing.StaticInstances, Is.Not.Null);
        Assert.That(testing.ViewModelIds, Is.Not.Null);

        testing = new ObjectModel(new Dictionary<string, object>());

        Assert.That(testing.ActualModelIds, Is.Not.Null);
        Assert.That(testing.Datas, Is.Not.Null);
        Assert.That(testing.Initializer, Is.Not.Null);
        Assert.That(testing.Marks, Is.Not.Null);
        Assert.That(testing.Operations, Is.Not.Null);
        Assert.That(testing.StaticInstances, Is.Not.Null);
        Assert.That(testing.ViewModelIds, Is.Not.Null);
    }

    [Test]
    public void InitializerModel()
    {
        var testing = new InitializerModel();

        Assert.That(testing.Marks, Is.Not.Null);
        Assert.That(testing.Parameters, Is.Not.Null);

        testing = new InitializerModel(new Dictionary<string, object>());

        Assert.That(testing.Marks, Is.Not.Null);
        Assert.That(testing.Parameters, Is.Not.Null);
    }

    [Test]
    public void DataModel()
    {
        var testing = new DataModel();

        Assert.That(testing.Marks, Is.Not.Null);

        testing = new DataModel(new Dictionary<string, object>());

        Assert.That(testing.Marks, Is.Not.Null);
    }

    [Test]
    public void OperationModel()
    {
        var testing = new OperationModel();

        Assert.That(testing.Marks, Is.Not.Null);
        Assert.That(testing.Parameters, Is.Not.Null);
        Assert.That(testing.Result, Is.Not.Null);

        testing = new OperationModel(new Dictionary<string, object>());

        Assert.That(testing.Marks, Is.Not.Null);
        Assert.That(testing.Parameters, Is.Not.Null);
        Assert.That(testing.Result, Is.Not.Null);
    }

    [Test]
    public void ParameterModel()
    {
        var testing = new ParameterModel();

        Assert.That(testing.DefaultValue, Is.Not.Null);
        Assert.That(testing.Marks, Is.Not.Null);
        Assert.That(testing.Groups, Is.Not.Null);

        testing = new ParameterModel(new Dictionary<string, object>());

        Assert.That(testing.DefaultValue, Is.Not.Null);
        Assert.That(testing.Marks, Is.Not.Null);
        Assert.That(testing.Groups, Is.Not.Null);
    }

    [Test]
    public void VariableData()
    {
        var testing = new VariableData();

        Assert.That(testing.Values, Is.Not.Null);

        testing = new VariableData(new Dictionary<string, object>());

        Assert.That(testing.Values, Is.Not.Null);
    }

    [Test]
    public void ObjectData()
    {
        var testing = new ObjectData();

        Assert.That(testing.Data, Is.Not.Null);

        testing = new ObjectData(new Dictionary<string, object>());

        Assert.That(testing.Data, Is.Not.Null);
    }

    [Test]
    public void ResultModel()
    {
        Assert.That(() => new ResultModel(), Throws.Nothing);
        Assert.That(() => new ResultModel(new Dictionary<string, object>()), Throws.Nothing);
    }
}
