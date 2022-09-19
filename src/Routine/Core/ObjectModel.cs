namespace Routine.Core;

public class ObjectModel
{
    public string Id { get; set; }
    public HashSet<string> Marks { get; set; } = new HashSet<string>();

    public string Name { get; set; }
    public string Module { get; set; }
    public bool IsValueModel { get; set; }
    public bool IsViewModel { get; set; }

    public List<string> ViewModelIds { get; set; } = new List<string>();
    public List<string> ActualModelIds { get; set; } = new List<string>();
    public InitializerModel Initializer { get; set; } = new InitializerModel();
    internal Dictionary<string, DataModel> Data { get; private set; } = new Dictionary<string, DataModel>();
    internal Dictionary<string, OperationModel> Operation { get; private set; } = new Dictionary<string, OperationModel>();
    public List<ObjectData> StaticInstances { get; set; } = new List<ObjectData>();

    public ObjectModel() { }
    public ObjectModel(IDictionary<string, object> model)
    {
        if (model == null) return;

        if (model.TryGetValue("Id", out var id))
        {
            Id = (string)id;
        }

        if (model.TryGetValue("Marks", out var marks))
        {
            Marks = ((IEnumerable)marks).Cast<string>().ToHashSet();
        }

        if (model.TryGetValue("Name", out var name))
        {
            Name = (string)name;
        }

        if (model.TryGetValue("Module", out var module))
        {
            Module = (string)module;
        }

        if (model.TryGetValue("IsValueModel", out var isValueModel))
        {
            IsValueModel = (bool)isValueModel;
        }

        if (model.TryGetValue("IsViewModel", out var isViewModel))
        {
            IsViewModel = (bool)isViewModel;
        }

        if (model.TryGetValue("ViewModelIds", out var viewModelIds))
        {
            ViewModelIds = ((IEnumerable)viewModelIds).Cast<string>().ToList();
        }

        if (model.TryGetValue("ActualModelIds", out var actualModelIds))
        {
            ActualModelIds = ((IEnumerable)actualModelIds).Cast<string>().ToList();
        }

        if (model.TryGetValue("Initializer", out var initializer))
        {
            Initializer = new InitializerModel((IDictionary<string, object>)initializer);
        }

        if (model.TryGetValue("Datas", out var datas))
        {
            Datas = ((IEnumerable)datas).Cast<IDictionary<string, object>>().Select(o => new DataModel(o)).ToList();
        }

        if (model.TryGetValue("Operations", out var operations))
        {
            Operations = ((IEnumerable)operations).Cast<IDictionary<string, object>>().Select(o => new OperationModel(o)).ToList();
        }

        if (model.TryGetValue("StaticInstances", out var staticInstances))
        {
            StaticInstances = ((IEnumerable)staticInstances).Cast<IDictionary<string, object>>().Select(o => new ObjectData(o)).ToList();
        }
    }

    public List<DataModel> Datas
    {
        get => Data.Values.ToList();
        set => Data = value.ToDictionary(o => o.Name, o => o);
    }

    public List<OperationModel> Operations
    {
        get => Operation.Values.ToList();
        set => Operation = value.ToDictionary(o => o.Name, o => o);
    }

    public DataModel GetData(string name)
    {
        Data.TryGetValue(name, out var result);

        return result;
    }

    public OperationModel GetOperation(string name)
    {
        Operation.TryGetValue(name, out var result);

        return result;
    }

    public void AddData(string name, DataModel model) => Data.Add(name, model);
    public void AddOperation(string name, OperationModel model) => Operation.Add(name, model);

    #region ToString & Equality

    public override string ToString()
    {
        return string.Format("[ObjectModel: [Id: {0}, Marks: {1}, Name: {2}, Module: {3}, IsValueModel: {4}, IsViewModel: {5}, " +
                             "ViewModelIds: {6}, ActualModelIds: {7}, Initializer: {8}, " +
                             "Datas: {9}, Operations: {10}, StaticInstances: {11}]]",
                             Id, Marks.ToItemString(), Name, Module, IsValueModel, IsViewModel,
                             ViewModelIds.ToItemString(), ActualModelIds.ToItemString(), Initializer,
                             Datas.ToItemString(), Operations.ToItemString(), StaticInstances.ToItemString());
    }

    protected bool Equals(ObjectModel other)
    {
        return
            string.Equals(Id, other.Id) && Marks.ItemEquals(other.Marks) && string.Equals(Name, other.Name) &&
            string.Equals(Module, other.Module) && IsValueModel == other.IsValueModel && IsViewModel == other.IsViewModel &&
            ViewModelIds.ItemEquals(other.ViewModelIds) && ActualModelIds.ItemEquals(other.ActualModelIds) &&
            Equals(Initializer, other.Initializer) && Datas.ItemEquals(other.Datas) &&
            Operations.ItemEquals(other.Operations) && StaticInstances.ItemEquals(other.StaticInstances);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;

        return Equals((ObjectModel)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = (Id != null ? Id.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (Marks != null ? Marks.GetItemHashCode() : 0);
            hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (Module != null ? Module.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ IsValueModel.GetHashCode();
            hashCode = (hashCode * 397) ^ IsViewModel.GetHashCode();
            hashCode = (hashCode * 397) ^ (ViewModelIds != null ? ViewModelIds.GetItemHashCode() : 0);
            hashCode = (hashCode * 397) ^ (ActualModelIds != null ? ActualModelIds.GetItemHashCode() : 0);
            hashCode = (hashCode * 397) ^ (Initializer != null ? Initializer.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (Datas != null ? Datas.GetItemHashCode() : 0);
            hashCode = (hashCode * 397) ^ (Operations != null ? Operations.GetItemHashCode() : 0);
            hashCode = (hashCode * 397) ^ (StaticInstances != null ? StaticInstances.GetItemHashCode() : 0);
            return hashCode;
        }
    }

    #endregion
}
