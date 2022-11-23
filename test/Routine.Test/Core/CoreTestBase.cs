using Routine.Core;
using System.Text.Json;

namespace Routine.Test.Core;

public abstract class CoreTestBase
{
    protected Dictionary<string, ObjectModel> _objectModelDictionary;
    protected Dictionary<ReferenceData, ObjectData> _objectDictionary;

    protected virtual string DefaultObjectModelId => "DefaultModel";

    [SetUp]
    public virtual void SetUp()
    {
        TypeInfo.SetProxyMatcher(t => t.Name.Contains("Proxy"), t => t.BaseType);

        _objectModelDictionary = new();
        _objectDictionary = new();
    }

    [TearDown]
    public virtual void TearDown() { }

    protected T Throw<T>(Exception ex) => throw ex;

    protected void AssertJsonEquals(string expected, string actual)
    {
        Assert.AreEqual(
            JsonSerializer.Serialize(JsonSerializer.Deserialize<object>(expected)),
            JsonSerializer.Serialize(JsonSerializer.Deserialize<object>(actual))
        );
    }

    #region Model Builders

    protected ApplicationModel GetApplicationModel() => new() { Models = _objectModelDictionary.Select(o => o.Value).ToList() };

    protected void ModelsAre(params ObjectModelBuilder[] objectModels) => ModelsAre(false, objectModels);
    protected void ModelsAre(bool clearPreviousModels, params ObjectModelBuilder[] objectModels)
    {
        if (clearPreviousModels)
        {
            _objectModelDictionary.Clear();
            var defaultModel = Model().Build();
            _objectModelDictionary.Add(defaultModel.Id, defaultModel);
        }

        foreach (var objectModel in objectModels.Select(o => o.Build()))
        {
            try
            {
                _objectModelDictionary.Add(objectModel.Id, objectModel);
            }
            catch (ArgumentException ex)
            {
                throw new Exception(objectModel.Id + " was already registered", ex);
            }
        }
    }

    protected ObjectModelBuilder Model() => Model(DefaultObjectModelId).IsValue();
    protected ObjectModelBuilder Model(string id) => new ObjectModelBuilder(DefaultObjectModelId).Id(id);

    protected class ObjectModelBuilder
    {
        private readonly string _defaultObjectModelId;

        private readonly ObjectModel _result;

        public ObjectModelBuilder(string defaultObjectModelId)
        {
            _defaultObjectModelId = defaultObjectModelId;
            _result = new();
        }

        public ObjectModelBuilder Id(string id) { _result.Id = id; return this; }

        public ObjectModelBuilder ViewModelIds(params string[] viewModelIds)
        {
            _result.ViewModelIds.AddRange(viewModelIds);
            return this;
        }

        public ObjectModelBuilder Mark(params string[] marks) { AddMarks(marks, _result.Marks); return this; }
        public ObjectModelBuilder MarkInitializer(params string[] marks) { AddMarks(marks, _result.Initializer.Marks); return this; }
        public ObjectModelBuilder MarkData(string dataName, params string[] marks) { AddMarks(marks, _result.Datas.Single(m => m.Name == dataName).Marks); return this; }
        public ObjectModelBuilder MarkOperation(string operationName, params string[] marks) { AddMarks(marks, _result.Operations.Single(o => o.Name == operationName).Marks); return this; }
        public ObjectModelBuilder MarkParameter(string operationName, string parameterName, params string[] marks) { AddMarks(marks, _result.Operations.Single(o => o.Name == operationName).Parameters.Single(p => p.Name == parameterName).Marks); return this; }

        private void AddMarks(IEnumerable<string> marks, HashSet<string> target)
        {
            foreach (var mark in marks)
            {
                target.Add(mark);
            }
        }

        public ObjectModelBuilder IsValue() { _result.IsValueModel = true; return this; }

        public ObjectModelBuilder IsView(string firstActualModelId, params string[] restOfTheActualModelIds)
        {
            if (string.IsNullOrEmpty(firstActualModelId)) { throw new ArgumentException("firstActualModelId cannot be null or empty. A view model should have at least one actual model id", nameof(firstActualModelId)); }

            _result.IsViewModel = true;
            _result.ActualModelIds.Add(firstActualModelId);
            _result.ActualModelIds.AddRange(restOfTheActualModelIds);

            return this;
        }

        public ObjectModelBuilder Name(string name) { _result.Name = name; return this; }
        public ObjectModelBuilder Module(string module) { _result.Module = module; return this; }

        public ObjectModelBuilder Initializer(params ParameterModel[] parameters) { return Initializer(parameters.Any() ? parameters.Max(p => p.Groups.Max()) + 1 : 1, parameters); }
        public ObjectModelBuilder Initializer(int groupCount, params ParameterModel[] parameters)
        {
            _result.Initializer = new InitializerModel
            {
                Parameters = parameters.ToList(),
                GroupCount = groupCount
            };

            return this;
        }

        public ObjectModelBuilder Data(string dataName) { return Data(dataName, _defaultObjectModelId); }
        public ObjectModelBuilder Data(string dataName, string viewModelId) { return Data(dataName, viewModelId, false); }
        public ObjectModelBuilder Data(string dataName, string viewModelId, bool isList)
        {
            _result.Data.Add(dataName, new DataModel
            {
                Name = dataName,
                ViewModelId = viewModelId,
                IsList = isList,
            });

            return this;
        }

        public ObjectModelBuilder Operation(string operationName, params ParameterModel[] parameters) { return Operation(operationName, true, parameters); }
        public ObjectModelBuilder Operation(string operationName, bool isVoid, params ParameterModel[] parameters) { return Operation(operationName, isVoid ? null : _defaultObjectModelId, parameters); }
        public ObjectModelBuilder Operation(string operationName, string resultViewModelId, params ParameterModel[] parameters) { return Operation(operationName, resultViewModelId, false, parameters); }
        public ObjectModelBuilder Operation(string operationName, string resultViewModelId, bool isList, params ParameterModel[] parameters)
        {
            var operationModel = new OperationModel
            {
                Name = operationName,
                Parameters = parameters.ToList(),
                GroupCount = parameters.Any() ? parameters.Max(p => p.Groups.Max()) + 1 : 1,
                Result =
                {
                    IsVoid = resultViewModelId == null,
                    ViewModelId = resultViewModelId,
                    IsList = isList
                }
            };

            return Operation(operationModel);
        }

        public ObjectModelBuilder Operation(OperationModel operationModel)
        {
            _result.Operation.Add(operationModel.Name, operationModel);

            return this;
        }

        public ObjectModelBuilder StaticInstanceIds(params string[] ids)
        {
            foreach (var id in ids)
            {
                StaticInstanceId(id, _result.Id);
            }

            return this;
        }

        public ObjectModelBuilder StaticInstanceId(string id, string modelId)
        {
            _result
                .StaticInstances
                .Add(
                    new ObjectData
                    {
                        Id = id,
                        ModelId = modelId,
                    }
                );

            return this;
        }

        public ObjectModel Build() => _result;
    }

    protected ParameterModel PModel(string name, params int[] groups) => PModel(name, false, groups);
    protected ParameterModel PModel(string name, string viewModelId, params int[] groups) => PModel(name, viewModelId, false, groups);
    protected ParameterModel PModel(string name, bool isList, params int[] groups) => PModel(name, DefaultObjectModelId, isList, groups);
    protected ParameterModel PModel(string name, string viewModelId, bool isList, params int[] groups) =>
        PModel(name, viewModelId: viewModelId, isList: isList, defaultValue: null, groups: groups);

    protected ParameterModel PModel(string name,
        string viewModelId = null,
        bool isList = false,
        string defaultValue = null,
        int[] groups = null
    ) => new()
    {
        Name = name,
        IsList = isList,
        ViewModelId = viewModelId ?? DefaultObjectModelId,
        Groups = (groups ?? new[] { 0 }).ToList(),
        DefaultValue = defaultValue == null
            ? null
            : new()
            {
                IsList = isList,
                Values = new()
                {
                    new()
                    {
                        Id = defaultValue,
                        ModelId = viewModelId ?? DefaultObjectModelId
                    }
                }
            },
        IsOptional = defaultValue != null
    };

    #endregion

    #region Data Builders

    protected void EnsureModels(params string[] modelIds)
    {
        foreach (var modelId in modelIds)
        {
            if (modelId != null && !_objectModelDictionary.ContainsKey(modelId))
            {
                ModelsAre(Model(modelId).Name(modelId));
            }
        }
    }

    protected ReferenceData Null() => Null(null);
    protected ReferenceData Null(string modelId) => Id("null", modelId, modelId, true);
    protected ReferenceData Id(string id) => Id(id, DefaultObjectModelId);
    protected ReferenceData Id(string id, string modelId) => Id(id, modelId, modelId);
    protected ReferenceData Id(string id, string modelId, string viewModelId) => Id(id, modelId, viewModelId, false);
    protected ReferenceData Id(string id, string modelId, string viewModelId, bool isNull)
    {
        if (isNull) { return null; }
        EnsureModels(modelId, viewModelId);

        return new ReferenceData
        {
            Id = id,
            ModelId = modelId,
            ViewModelId = viewModelId,
        };
    }

    protected void ObjectsAre(params ObjectBuilder[] objects)
    {
        foreach (var @object in objects.Select(o => o.Build()))
        {
            _objectDictionary.Add(new ReferenceData { Id = @object.Item2.Id, ModelId = @object.Item2.ModelId, ViewModelId = @object.Item1 }, @object.Item2);
        }
    }

    protected ObjectBuilder Object(ReferenceData reference) => new ObjectBuilder(_objectModelDictionary, _objectDictionary).Reference(reference);
    protected class ObjectBuilder
    {
        private readonly Dictionary<string, ObjectModel> _objectModels;
        private readonly Dictionary<ReferenceData, ObjectData> _objects;

        private string _viewModelId;
        private readonly ObjectData _result;

        public ObjectBuilder(Dictionary<string, ObjectModel> objectModels, Dictionary<ReferenceData, ObjectData> objects)
        {
            _objectModels = objectModels;
            _objects = objects;
            _result = new();
        }

        public ObjectBuilder Reference(ReferenceData reference)
        {
            _result.Id = reference.Id;
            _result.ModelId = reference.ModelId;
            _viewModelId = reference.ViewModelId;
            if (!_objectModels.ContainsKey(reference.ViewModelId))
            {
                return this;
            }

            var model = _objectModels[reference.ViewModelId];

            foreach (var dataModel in model.Datas)
            {
                _result.Data.Add(dataModel.Name, new VariableData { IsList = dataModel.IsList });
            }

            return this;
        }

        public ObjectBuilder Display(string value)
        {
            _result.Display = value;

            return this;
        }

        public ObjectBuilder Data(string dataName, params ReferenceData[] dataReference)
        {
            _result.Data[dataName]
                .Values.AddRange(dataReference.Select(m => m == null ? null : new ObjectData { Id = m.Id, ModelId = m.ModelId }));

            return this;
        }

        public Tuple<string, ObjectData> Build()
        {
            foreach (var data in _result.Data)
            {
                foreach (var val in data.Value.Values)
                {
                    if (val == null) { continue; }
                    if (_objectModels.ContainsKey(val.ModelId) && _objectModels[val.ModelId].IsValueModel)
                    {
                        val.Display = val.Id;
                        continue;
                    }

                    val.Display = _objects[new ReferenceData { Id = val.Id, ModelId = val.ModelId }].Display;
                }
            }
            return new Tuple<string, ObjectData>(_viewModelId, _result);
        }
    }

    private string DisplayOf(ReferenceData rd)
    {
        if (rd == null) { return ""; }
        if (_objectModelDictionary.ContainsKey(rd.ModelId) && _objectModelDictionary[rd.ModelId].IsValueModel)
        {
            return rd.Id;
        }

        if (_objectDictionary.ContainsKey(rd))
        {
            return _objectDictionary[rd].Display;
        }

        return _objectDictionary[new ReferenceData { Id = rd.Id, ModelId = rd.ModelId }].Display;
    }

    protected Dictionary<string, ParameterValueData> Params(params KeyValuePair<string, ParameterValueData>[] parameters) =>
        parameters.ToDictionary(parameter => parameter.Key, parameter => parameter.Value);

    protected ParameterData Init(string objectModelId, Dictionary<string, ParameterValueData> initializationParameters) =>
        new()
        {
            ModelId = objectModelId,
            InitializationParameters = initializationParameters
        };

    protected KeyValuePair<string, ParameterValueData> Param(string modelId, params ParameterData[] values) => Param(modelId, values.Length > 1, values);
    protected KeyValuePair<string, ParameterValueData> Param(string modelId, bool isList, params ParameterData[] values) =>
        new(modelId,
            new ParameterValueData
            {
                IsList = isList,
                Values = values.ToList()
            }
        );

    protected KeyValuePair<string, ParameterValueData> Param(string modelId, params ReferenceData[] references) => Param(modelId, references.Length > 1, references);
    protected KeyValuePair<string, ParameterValueData> Param(string modelId, bool isList, params ReferenceData[] references) =>
        new(modelId, new ParameterValueData
        {
            IsList = isList,
            Values = references.Select(PD).ToList()
        });

    protected ParameterData PD(ReferenceData reference) =>
        reference == null
            ? null
            : new ParameterData { ModelId = reference.ModelId, Id = reference.Id };

    protected VariableData Result(params ReferenceData[] rds)
    {
        var result = new VariableData();

        result.Values.AddRange(
            rds.Select(rd => rd == null ? null : new ObjectData
            {
                Id = rd.Id,
                ModelId = rd.ModelId,
                Display = DisplayOf(rd)
            }).ToList());

        return result;
    }

    protected VariableData Void() => new();

    #endregion
}
