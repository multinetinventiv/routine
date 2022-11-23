using Routine.Core;

namespace Routine.Client;

public class Robject
{
    #region Data Adapter Methods

    private static ReferenceData Rd(bool isNull) => Rd(null, null, null, isNull);
    private static ReferenceData Rd(string id, string mid, string vmid, bool isNull) => isNull ? null : new ReferenceData { Id = id, ModelId = mid, ViewModelId = vmid };
    private static ObjectData Od(ReferenceData rd) => Od(rd, null);
    private static ObjectData Od(ReferenceData rd, string display) => rd == null ? null : new ObjectData { Id = rd.Id, ModelId = rd.ModelId, Display = display };

    #endregion

    private readonly string _id;
    private readonly List<Rvariable> _initializationParameters;
    private readonly Dictionary<string, DataValue> _datas;

    private string display;

    public Rtype ActualType { get; }
    public Rtype ViewType { get; }
    public Rtype Type => ViewType ?? ActualType;

    public Robject() : this(Rd(true), null, null) { }
    public Robject(IEnumerable<Rvariable> initializationParameters, Rtype type) : this(initializationParameters.ToList(), Od(Rd(null, type.Id, type.Id, false)), type, type) { }
    public Robject(string id, Rtype type) : this(id, type, type) { }
    public Robject(string id, Rtype actualType, Rtype viewType) : this(Rd(id, actualType.Id, viewType.Id, id == null), actualType, viewType) { }
    internal Robject(ReferenceData referenceData, Rtype actualType, Rtype viewType) : this(Od(referenceData), actualType, viewType) { }
    internal Robject(ReferenceData referenceData, Rtype actualType, Rtype viewType, string value) : this(Od(referenceData, value), actualType, viewType) { }
    internal Robject(ObjectData objectData, Rtype actualType, Rtype viewType) : this(null, objectData, actualType, viewType) { }
    private Robject(List<Rvariable> initializationParameters, ObjectData objectData, Rtype actualType, Rtype viewType)
    {
        if (actualType != null && actualType.IsViewType)
        {
            throw new CannotCreateRobjectException($"Cannot create object with a view type '{actualType}' given as the actual type");
        }

        if (actualType != null && !actualType.CanBe(viewType))
        {
            throw new CannotCreateRobjectException($"{actualType} cannot be {viewType}");
        }

        _initializationParameters = initializationParameters;

        _id = objectData?.Id;

        ActualType = actualType;
        ViewType = viewType;

        _datas = new();

        if (!IsNull)
        {
            FillObject(objectData);
        }
    }

    private void FillObject(ObjectData objectData)
    {
        display = objectData.Display;

        if (objectData.Data.Count <= 0) { return; }

        LoadDataIfNecessary();

        foreach (var dataName in objectData.Data.Keys)
        {
            _datas[dataName].SetData(objectData.Data[dataName]);
        }
    }

    private void LoadDataIfNecessary()
    {
        if (IsNull) { return; }
        if (Type.IsValueType) { return; }
        if (_datas.Any()) { return; }

        foreach (var data in Type.Datas)
        {
            _datas.Add(data.Name, new DataValue(this, data));
        }
    }

    private void FetchValueIfNecessary()
    {
        if (display != null) { return; }

        if (IsNull)
        {
            display = "";

            return;
        }

        if (Type.IsValueType)
        {
            display = _id;

            return;
        }

        if (IsInitializedOnClient)
        {
            throw new RobjectIsInitializedOnClientException();
        }

        LoadObject();
    }

    public void LoadObject()
    {
        if (IsNull) { return; }
        if (Type.IsValueType) { return; }

        FillObject(Application.Service.Get(ReferenceData));
    }

    public async Task LoadObjectAsync()
    {
        if (IsNull) { return; }
        if (Type.IsValueType) { return; }

        FillObject(await Application.Service.GetAsync(ReferenceData));
    }

    internal ParameterData GetParameterData()
    {
        if (IsNull) { return null; }

        if (!IsInitializedOnClient)
        {
            return new ParameterData
            {
                ModelId = ActualType.Id,
                Id = _id
            };
        }

        var result = new ParameterData { ModelId = ActualType.Id };

        foreach (var initializationParameter in _initializationParameters)
        {
            result.InitializationParameters.Add(initializationParameter.Name, initializationParameter.GetParameterValueData());
        }

        return result;
    }

    public Rapplication Application => Type.Application;
    public string Id => _id;
    public bool IsNull => _id == null && !IsInitializedOnClient;
    public bool IsNaked => Equals(ActualType, ViewType);
    public bool IsInitializedOnClient => _initializationParameters != null;
    public string Display { get { FetchValueIfNecessary(); return display; } }
    internal ReferenceData ReferenceData => Rd(_id, ActualType.Id, ViewType.Id, IsNull);

    public DataValue this[string dataName] { get { LoadDataIfNecessary(); return _datas[dataName]; } }
    public List<DataValue> DataValues { get { LoadDataIfNecessary(); return _datas.Values.ToList(); } }

    public Robject As(Rtype viewType) => Type.Get(Id, viewType);

    public Rvariable Perform(string operationName, params Rvariable[] parameters) => Perform(operationName, parameters.ToList());
    public Rvariable Perform(string operationName, List<Rvariable> parameters)
    {
        if (IsNull) { return new Rvariable(); }

        return Type.Operation[operationName].Perform(this, parameters);
    }

    public async Task<Rvariable> PerformAsync(string operationName, params Rvariable[] parameters) => await PerformAsync(operationName, parameters.ToList());
    public async Task<Rvariable> PerformAsync(string operationName, List<Rvariable> parameters)
    {
        if (IsNull) { return new Rvariable(); }

        return await Type.Operation[operationName].PerformAsync(this, parameters);
    }

    public void Invalidate()
    {
        display = null;

        foreach (var data in _datas.Values)
        {
            data.Invalidate();
        }
    }

    public class DataValue
    {
        public Robject Object { get; }
        public Rdata Data { get; }

        internal DataValue(Robject @object, Rdata data)
        {
            Object = @object;
            Data = data;
        }

        private Rvariable _value;
        private VariableData _data;

        internal void SetData(VariableData data)
        {
            _data = data;

            _value = new(Object.Type.Application, data, Data.DataType.Id);
        }

        public Rvariable Get()
        {
            if (_data == null)
            {
                Object.LoadObject();
            }

            return _value;
        }

        public async Task<Rvariable> GetAsync()
        {
            if (_data == null)
            {
                await Object.LoadObjectAsync();
            }

            return _value;
        }

        internal void Invalidate()
        {
            _value = null;
            _data = null;
        }
    }

    public override string ToString() => $"{Id}({Type.Id})";

    #region Equality & Hashcode

    protected bool Equals(Robject other)
    {
        if (IsInitializedOnClient || other.IsInitializedOnClient)
        {
            return false;
        }

        return string.Equals(_id, other._id) && Equals(ActualType, other.ActualType) && Equals(ViewType, other.ViewType);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;

        return Equals((Robject)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = (_id != null ? _id.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (ActualType != null ? ActualType.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (ViewType != null ? ViewType.GetHashCode() : 0);
            return hashCode;
        }
    }

    #endregion
}
