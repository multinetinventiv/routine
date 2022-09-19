using Routine.Core;

using static Routine.Constants;

namespace Routine.Client;

public class Rtype
{
    public static readonly Rtype Void = new();

    private readonly ObjectModel model;

    public Rapplication Application { get; }

    public List<Rtype> ViewTypes { get; }
    public List<Rtype> ActualTypes { get; }
    public Rinitializer Initializer { get; private set; }
    public Dictionary<string, Rdata> Data { get; }
    public Dictionary<string, Roperation> Operation { get; }

    private Rtype() : this(null, new ObjectModel { Id = MODEL_ID_VOID, IsValueModel = true }) { }
    public Rtype(Rapplication application, ObjectModel model)
    {
        Application = application;
        this.model = model;

        ViewTypes = new List<Rtype>();
        ActualTypes = new List<Rtype>();
        Initializer = null;
        Data = new Dictionary<string, Rdata>();
        Operation = new Dictionary<string, Roperation>();
    }

    internal void Load()
    {
        foreach (var viewModelId in model.ViewModelIds)
        {
            ViewTypes.Add(Application[viewModelId]);
        }

        foreach (var actualModelId in model.ActualModelIds)
        {
            ActualTypes.Add(Application[actualModelId]);
        }

        if (model.Initializer.GroupCount > 0)
        {
            Initializer = new Rinitializer(model.Initializer, this);
        }

        foreach (var data in model.Datas)
        {
            Data.Add(data.Name, new Rdata(data, this));
        }

        foreach (var operation in model.Operations)
        {
            Operation.Add(operation.Name, new Roperation(operation, this));
        }
    }

    public string Id => model.Id;
    public string Name => model.Name;
    public string Module => model.Module;
    public bool IsValueType => model.IsValueModel;
    public bool IsViewType => model.IsViewModel;
    public bool IsVoid => Id == Constants.MODEL_ID_VOID;
    public bool Initializable => Initializer != null;
    public List<Rdata> Datas => Data.Values.ToList();
    public List<Roperation> Operations => Operation.Values.ToList();

    public HashSet<string> Marks => model.Marks;

    public bool MarkedAs(string mark) => model.Marks.Contains(mark);

    public bool CanBe(Rtype viewType)
    {
        if (Equals(this, viewType)) { return true; }

        return ViewTypes.Contains(viewType);
    }

    public List<Robject> StaticInstances =>
        model
            .StaticInstances
            .Select(od => new Robject(od, Application[od.ModelId], this))
            .ToList();

    public Robject Get(string id) => new(id, this);
    public Robject Get(string id, Rtype viewType) => new(id, this, viewType);

    public Robject Init(params Rvariable[] initializationParameters) => Init(initializationParameters.AsEnumerable());
    public Robject Init(IEnumerable<Rvariable> initializationParameters) => new(initializationParameters, this);

    public override string ToString() => model.Id;

    #region Equality & Hashcode

    protected bool Equals(Rtype other)
    {
        return Equals(model, other.model);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) { return false; }
        if (ReferenceEquals(this, obj)) { return true; }
        if (obj.GetType() != GetType()) { return false; }

        return Equals((Rtype)obj);
    }

    public override int GetHashCode()
    {
        return (model != null ? model.GetHashCode() : 0);
    }

    #endregion
}
