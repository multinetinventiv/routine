using Routine.Core;

namespace Routine.Test.Core;

[TestFixture]
public class ModelBuildFromDictionaryWithFaultToleranceTest
{
    [Test]
    public void ApplicationModel()
    {
        var testing = new ApplicationModel();

        Assert.IsNotNull(testing.Model);

        testing = new ApplicationModel(new Dictionary<string, object>());

        Assert.IsNotNull(testing.Model);
    }

    [Test]
    public void ObjectModel()
    {
        var testing = new ObjectModel();

        Assert.IsNotNull(testing.ActualModelIds);
        Assert.IsNotNull(testing.Datas);
        Assert.IsNotNull(testing.Initializer);
        Assert.IsNotNull(testing.Marks);
        Assert.IsNotNull(testing.Operations);
        Assert.IsNotNull(testing.StaticInstances);
        Assert.IsNotNull(testing.ViewModelIds);

        testing = new ObjectModel(new Dictionary<string, object>());

        Assert.IsNotNull(testing.ActualModelIds);
        Assert.IsNotNull(testing.Datas);
        Assert.IsNotNull(testing.Initializer);
        Assert.IsNotNull(testing.Marks);
        Assert.IsNotNull(testing.Operations);
        Assert.IsNotNull(testing.StaticInstances);
        Assert.IsNotNull(testing.ViewModelIds);
    }

    [Test]
    public void InitializerModel()
    {
        var testing = new InitializerModel();

        Assert.IsNotNull(testing.Marks);
        Assert.IsNotNull(testing.Parameters);

        testing = new InitializerModel(new Dictionary<string, object>());

        Assert.IsNotNull(testing.Marks);
        Assert.IsNotNull(testing.Parameters);
    }

    [Test]
    public void DataModel()
    {
        var testing = new DataModel();

        Assert.IsNotNull(testing.Marks);

        testing = new DataModel(new Dictionary<string, object>());

        Assert.IsNotNull(testing.Marks);
    }

    [Test]
    public void OperationModel()
    {
        var testing = new OperationModel();

        Assert.IsNotNull(testing.Marks);
        Assert.IsNotNull(testing.Parameters);
        Assert.IsNotNull(testing.Result);

        testing = new OperationModel(new Dictionary<string, object>());

        Assert.IsNotNull(testing.Marks);
        Assert.IsNotNull(testing.Parameters);
        Assert.IsNotNull(testing.Result);
    }

    [Test]
    public void ParameterModel()
    {
        var testing = new ParameterModel();

        Assert.IsNotNull(testing.DefaultValue);
        Assert.IsNotNull(testing.Marks);
        Assert.IsNotNull(testing.Groups);

        testing = new ParameterModel(new Dictionary<string, object>());

        Assert.IsNotNull(testing.DefaultValue);
        Assert.IsNotNull(testing.Marks);
        Assert.IsNotNull(testing.Groups);
    }

    [Test]
    public void VariableData()
    {
        var testing = new VariableData();

        Assert.IsNotNull(testing.Values);

        testing = new VariableData(new Dictionary<string, object>());

        Assert.IsNotNull(testing.Values);
    }

    [Test]
    public void ObjectData()
    {
        var testing = new ObjectData();

        Assert.IsNotNull(testing.Data);

        testing = new ObjectData(new Dictionary<string, object>());

        Assert.IsNotNull(testing.Data);
    }

    [Test]
    public void ResultModel()
    {
        Assert.DoesNotThrow(() => new ResultModel());
        Assert.DoesNotThrow(() => new ResultModel(new Dictionary<string, object>()));
    }
}
