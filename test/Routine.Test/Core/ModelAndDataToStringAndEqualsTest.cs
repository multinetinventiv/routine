using Routine.Core;

namespace Routine.Test.Core;

[TestFixture]
public class ModelAndDataToStringAndEqualsTest : CoreTestBase
{
    #region Helpers

    private class TestDataPrototype<T>
    {
        private readonly Func<T> _defaultFunction;

        public TestDataPrototype(Func<T> defaultFunction)
        {
            _defaultFunction = defaultFunction;
        }

        public T Create() { return Create(_ => { }); }
        public T Create(Action<T> manipulation)
        {
            var result = _defaultFunction();

            manipulation(result);

            return result;
        }
    }

    private void AssertEqualsAndHasSameHashCode<T>(T left, T right)
    {
        Assert.That(left.Equals(right), Is.True, $"{left} and {right} should be equal");
        Assert.That(left.GetHashCode(), Is.EqualTo(right.GetHashCode()));
    }

    private void AssertDoesNotEqualAndHasDifferentHashCode<T>(T left, T right)
    {
        Assert.That(left.Equals(right), Is.True, $"{left} and {right} should not be equal");
        Assert.That(left.GetHashCode(), Is.Not.EqualTo(right.GetHashCode()));
    }

    private void AssertToStringHasTypeName<T>(T unitUnderTest) =>
        Assert.That(unitUnderTest.ToString()?.Contains(typeof(T).Name), Is.True, $"{WrongToString(unitUnderTest.ToString())}, Expected: {typeof(T).Name}");

    private void AssertToStringHasProperty<T>(T unitUnderTest, string propertyName) =>
        Assert.That(unitUnderTest.ToString()?.Contains($"{propertyName}: "), Is.True, $"{WrongToString(unitUnderTest.ToString())}, Expected: {propertyName}: ");

    private void AssertToStringHasPropertyAndItsValue<T>(T unitUnderTest, string propertyName, object propertyValue) =>
        Assert.That(unitUnderTest.ToString()?.Contains($"{propertyName}: {propertyValue}"), Is.True, $"{WrongToString(unitUnderTest.ToString())}, Expected: {propertyName}: {propertyValue}");

    private string WrongToString(string toString) => $"Wrong ToString: {toString}";

    #endregion

    [Test]
    public void DataModel()
    {
        var prototype = new TestDataPrototype<DataModel>(() => new()
        {
            Marks = new() { "mark1", "mark2" },
            IsList = false,
            Name = "name",
            ViewModelId = "view_model_id"
        });

        var testing = prototype.Create();

        //ToString tests
        AssertToStringHasTypeName(testing);

        AssertToStringHasPropertyAndItsValue(testing, "Marks", testing.Marks.ToItemString());
        AssertToStringHasPropertyAndItsValue(testing, "IsList", testing.IsList);
        AssertToStringHasPropertyAndItsValue(testing, "Name", testing.Name);
        AssertToStringHasPropertyAndItsValue(testing, "ViewModelId", testing.ViewModelId);

        //Equals & HashCode tests
        AssertEqualsAndHasSameHashCode(testing, prototype.Create());

        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Marks.Add("different")));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.IsList = true));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Name = "different"));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.ViewModelId = "different"));
    }

    [Test]
    public void ParameterModel()
    {
        var prototype = new TestDataPrototype<ParameterModel>(() => new()
        {
            Marks = new() { "mark1", "mark2" },
            Groups = new() { 1, 2 },
            IsList = true,
            Name = "name",
            ViewModelId = "view_model_id",
            IsOptional = true,
            DefaultValue = new()
            {
                IsList = true,
                Values = new() { new() { Id = "obj1" }, new() { Id = "obj2" } }
            }
        });

        var testing = prototype.Create();

        //ToString tests
        AssertToStringHasTypeName(testing);

        AssertToStringHasPropertyAndItsValue(testing, "Marks", testing.Marks.ToItemString());
        AssertToStringHasPropertyAndItsValue(testing, "Groups", testing.Groups.ToItemString());
        AssertToStringHasPropertyAndItsValue(testing, "IsList", testing.IsList);
        AssertToStringHasPropertyAndItsValue(testing, "Name", testing.Name);
        AssertToStringHasPropertyAndItsValue(testing, "ViewModelId", testing.ViewModelId);
        AssertToStringHasPropertyAndItsValue(testing, "IsOptional", testing.IsOptional);
        AssertToStringHasPropertyAndItsValue(testing, "DefaultValue", testing.DefaultValue.ToString());

        //Equals & HashCode tests
        AssertEqualsAndHasSameHashCode(testing, prototype.Create());

        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Marks.Add("different")));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Groups.Add(3)));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.IsList = false));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Name = "different"));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.ViewModelId = "different"));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.IsOptional = false));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.DefaultValue.Values[0].Id = "different"));
    }

    [Test]
    public void ResultModel()
    {
        var prototype = new TestDataPrototype<ResultModel>(() => new()
        {
            IsList = true,
            IsVoid = false,
            ViewModelId = "view_model_id"
        });

        var testing = prototype.Create();

        //ToString tests
        AssertToStringHasTypeName(testing);

        AssertToStringHasPropertyAndItsValue(testing, "IsList", testing.IsList);
        AssertToStringHasPropertyAndItsValue(testing, "IsVoid", testing.IsVoid);
        AssertToStringHasPropertyAndItsValue(testing, "ViewModelId", testing.ViewModelId);

        //Equals & HashCode tests
        AssertEqualsAndHasSameHashCode(testing, prototype.Create());

        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.IsList = false));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.IsVoid = true));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.ViewModelId = "different"));
    }

    [Test]
    public void OperationModel()
    {
        var prototype = new TestDataPrototype<OperationModel>(() => new()
        {
            Marks = new() { "mark1", "mark2" },
            Name = "Operation",
            GroupCount = 2,
            Parameters = new() { new() { Name = "parameter" } },
            Result = new() { ViewModelId = "view_model_id" }
        });

        var testing = prototype.Create();

        //ToString tests
        AssertToStringHasTypeName(testing);

        AssertToStringHasPropertyAndItsValue(testing, "Marks", testing.Marks.ToItemString());
        AssertToStringHasPropertyAndItsValue(testing, "GroupCount", testing.GroupCount);
        AssertToStringHasPropertyAndItsValue(testing, "Name", testing.Name);

        AssertToStringHasProperty(testing, "Parameters");
        AssertToStringHasPropertyAndItsValue(testing, "Name", testing.Parameters[0].Name);

        AssertToStringHasProperty(testing, "Result");
        AssertToStringHasPropertyAndItsValue(testing, "ViewModelId", testing.Result.ViewModelId);

        //Equals & HashCode tests
        AssertEqualsAndHasSameHashCode(testing, prototype.Create());

        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Marks.Add("different")));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.GroupCount = 3));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Name = "different"));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Parameters[0].Name = "different"));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Result.ViewModelId = "different"));
    }

    [Test]
    public void InitializerModel()
    {
        var prototype = new TestDataPrototype<InitializerModel>(() => new()
        {
            Marks = new() { "mark1", "mark2" },
            GroupCount = 2,
            Parameters = new() { new() { Name = "parameter" } }
        });

        var testing = prototype.Create();

        //ToString tests
        AssertToStringHasTypeName(testing);

        AssertToStringHasPropertyAndItsValue(testing, "Marks", testing.Marks.ToItemString());
        AssertToStringHasPropertyAndItsValue(testing, "GroupCount", testing.GroupCount);

        AssertToStringHasProperty(testing, "Parameters");
        AssertToStringHasPropertyAndItsValue(testing, "Name", testing.Parameters[0].Name);

        //Equals & HashCode tests
        AssertEqualsAndHasSameHashCode(testing, prototype.Create());

        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Marks.Add("different")));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.GroupCount = 3));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Parameters[0].Name = "different"));
    }

    [Test]
    public void ObjectModel()
    {
        var prototype = new TestDataPrototype<ObjectModel>(() => new()
        {
            Id = "object",
            Marks = new() { "mark1", "mark2" },
            Name = "object",
            Module = "system",
            IsValueModel = true,
            IsViewModel = false,
            ViewModelIds = new() { "interface" },
            ActualModelIds = new() { "implementation" },
            Initializer = new() { GroupCount = 2 },
            Datas = new() { new() { Name = "data" } },
            Operations = new() { new() { Name = "operation" } },
            StaticInstances = new() { new() { Display = "value1" }, new() { Display = "value2" } }
        });

        var testing = prototype.Create();

        //ToString tests
        AssertToStringHasTypeName(testing);

        AssertToStringHasPropertyAndItsValue(testing, "Id", testing.Id);
        AssertToStringHasPropertyAndItsValue(testing, "Marks", testing.Marks.ToItemString());
        AssertToStringHasPropertyAndItsValue(testing, "Name", testing.Name);
        AssertToStringHasPropertyAndItsValue(testing, "Module", testing.Module);
        AssertToStringHasPropertyAndItsValue(testing, "IsValueModel", testing.IsValueModel);
        AssertToStringHasPropertyAndItsValue(testing, "IsViewModel", testing.IsViewModel);
        AssertToStringHasPropertyAndItsValue(testing, "ViewModelIds", testing.ViewModelIds[0].SurroundWith("[", "]"));
        AssertToStringHasPropertyAndItsValue(testing, "ActualModelIds", testing.ActualModelIds[0].SurroundWith("[", "]"));

        AssertToStringHasProperty(testing, "Initializer");
        AssertToStringHasPropertyAndItsValue(testing, "GroupCount", testing.Initializer.GroupCount);

        AssertToStringHasProperty(testing, "Datas");
        AssertToStringHasPropertyAndItsValue(testing, "Name", testing.Datas[0].Name);

        AssertToStringHasProperty(testing, "Operations");
        AssertToStringHasPropertyAndItsValue(testing, "Name", testing.Operations[0].Name);

        AssertToStringHasProperty(testing, "StaticInstances");
        AssertToStringHasPropertyAndItsValue(testing, "Display", testing.StaticInstances[0].Display);

        //Equals & HashCode tests
        AssertEqualsAndHasSameHashCode(testing, prototype.Create());

        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Id = "different"));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Marks.Add("different")));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Name = "different"));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Module = "different"));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.IsValueModel = false));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.IsViewModel = true));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.ViewModelIds[0] = "different"));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.ActualModelIds[0] = "different"));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Initializer.GroupCount = 3));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Datas[0].Name = "different"));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Operations[0].Name = "different"));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.StaticInstances[0].Display = "different"));
    }

    [Test]
    public void ApplicationModel()
    {
        var prototype = new TestDataPrototype<ApplicationModel>(() => new()
        {
            Models = new() { new() { Id = "model" } }
        });

        var testing = prototype.Create();

        //ToString tests
        AssertToStringHasTypeName(testing);

        AssertToStringHasProperty(testing, "Models");
        AssertToStringHasPropertyAndItsValue(testing, "Id", testing.Models[0].Id);

        //Equals & HashCode tests
        AssertEqualsAndHasSameHashCode(testing, prototype.Create());

        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Models[0].Id = "different"));
    }

    [Test]
    public void OperationWithObjectModel()
    {
        var prototype = new TestDataPrototype<OperationWithObjectModel>(() => new()
        {
            ObjectModel = new() { Id = "model" },
            OperationModel = new() { Name = "operation" }
        });

        var testing = prototype.Create();

        //ToString tests
        AssertToStringHasTypeName(testing);

        AssertToStringHasProperty(testing, "ObjectModel");
        AssertToStringHasPropertyAndItsValue(testing, "Id", testing.ObjectModel.Id);
        AssertToStringHasProperty(testing, "OperationModel");
        AssertToStringHasPropertyAndItsValue(testing, "Name", testing.OperationModel.Name);

        //Equals & HashCode tests
        AssertEqualsAndHasSameHashCode(testing, prototype.Create());

        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.ObjectModel.Id = "different"));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.OperationModel.Name = "different"));
    }

    [Test]
    public void ReferenceData()
    {
        var prototype = new TestDataPrototype<ReferenceData>(() => new()
        {
            Id = "object",
            ModelId = "actual",
            ViewModelId = "view"
        });

        var testing = prototype.Create();

        //ToString tests
        AssertToStringHasTypeName(testing);

        AssertToStringHasPropertyAndItsValue(testing, "Id", testing.Id);
        AssertToStringHasPropertyAndItsValue(testing, "ModelId", testing.ModelId);
        AssertToStringHasPropertyAndItsValue(testing, "ViewModelId", testing.ViewModelId);

        //Equals & HashCode tests
        AssertEqualsAndHasSameHashCode(testing, prototype.Create());

        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.Id = "different"));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.ModelId = "different"));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.ViewModelId = "different"));
    }

    [Test]
    public void ParameterValueData()
    {
        var prototype = new TestDataPrototype<ParameterValueData>(() => new()
        {
            IsList = true,
            Values = new() { new() { Id = "id" } },
        });

        var testing = prototype.Create();

        //ToString tests
        AssertToStringHasTypeName(testing);

        AssertToStringHasPropertyAndItsValue(testing, "IsList", testing.IsList);
        AssertToStringHasProperty(testing, "Values");
        AssertToStringHasPropertyAndItsValue(testing, "Id", testing.Values[0].Id);

        //Equals & HashCode tests
        AssertEqualsAndHasSameHashCode(testing, prototype.Create());

        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.IsList = false));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.Values[0].Id = "different"));
    }

    [Test]
    public void ParameterData()
    {
        var prototype = new TestDataPrototype<ParameterData>(() => new()
        {
            Id = "id",
            ModelId = "modelid",
            InitializationParameters = new()
            {
                {
                    "param1",
                    new()
                    {
                        IsList = false,
                        Values = new()
                        {
                            new()
                            {
                                Id = "childid",
                            }
                        }
                    }
                }
            },
        });

        var testing = prototype.Create();

        //ToString tests
        AssertToStringHasTypeName(testing);

        AssertToStringHasPropertyAndItsValue(testing, "Id", testing.Id);
        AssertToStringHasPropertyAndItsValue(testing, "ModelId", testing.ModelId);

        AssertToStringHasProperty(testing, "InitializationParameters");
        AssertToStringHasPropertyAndItsValue(testing, "Id", testing.InitializationParameters["param1"].Values[0].Id);

        //Equals & HashCode tests
        AssertEqualsAndHasSameHashCode(testing, prototype.Create());

        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.Id = "different"));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.ModelId = "different"));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.InitializationParameters["param1"].Values[0].Id = "different"));
    }

    [Test]
    public void VariableData()
    {
        var prototype = new TestDataPrototype<VariableData>(() => new()
        {
            IsList = true,
            Values = new() { new() { Display = "value" } },
        });

        var testing = prototype.Create();

        //ToString tests
        AssertToStringHasTypeName(testing);

        AssertToStringHasPropertyAndItsValue(testing, "IsList", testing.IsList);
        AssertToStringHasProperty(testing, "Values");
        AssertToStringHasPropertyAndItsValue(testing, "Display", testing.Values[0].Display);

        //Equals & HashCode tests
        AssertEqualsAndHasSameHashCode(testing, prototype.Create());

        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.IsList = false));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.Values[0].Display = "different"));
    }

    [Test]
    public void ObjectData()
    {
        var prototype = new TestDataPrototype<ObjectData>(() => new()
        {
            Display = "value",
            Id = "id",
            Data = new()
            {
                {
                    "mmodel",
                    new()
                    {
                        Values = new()
                        {
                            new()
                            {
                                Id = "childid",
                                Display = "childvalue"
                            }
                        }
                    }
                }
            }
        });

        var testing = prototype.Create();

        //ToString tests
        AssertToStringHasTypeName(testing);

        AssertToStringHasPropertyAndItsValue(testing, "Display", testing.Display);
        AssertToStringHasPropertyAndItsValue(testing, "Id", testing.Id);

        AssertToStringHasProperty(testing, "Data");
        AssertToStringHasPropertyAndItsValue(testing, "Id", testing.Data["mmodel"].Values[0].Id);
        AssertToStringHasPropertyAndItsValue(testing, "Display", testing.Data["mmodel"].Values[0].Display);


        //Equals & HashCode tests
        AssertEqualsAndHasSameHashCode(testing, prototype.Create());

        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.Display = "different"));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.Id = "different"));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.Data["mmodel"].Values[0].Display = "different"));
        AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.Data["mmodel"].Values[0].Id = "different"));
    }
}
