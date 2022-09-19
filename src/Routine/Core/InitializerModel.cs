namespace Routine.Core;

public class InitializerModel
{
    public HashSet<string> Marks { get; set; } = new();
    public int GroupCount { get; set; }

    internal Dictionary<string, ParameterModel> Parameter { get; private set; } = new();

    public InitializerModel() { }
    public InitializerModel(IDictionary<string, object> model)
    {
        if (model == null) return;

        if (model.TryGetValue("Marks", out var marks))
        {
            Marks = ((IEnumerable)marks).Cast<string>().ToHashSet();
        }

        if (model.TryGetValue("GroupCount", out var groupCount))
        {
            GroupCount = (int)groupCount;
        }

        if (model.TryGetValue("Parameters", out var parameters))
        {
            Parameters = ((IEnumerable)parameters).Cast<IDictionary<string, object>>().Select(p => new ParameterModel(p)).ToList();
        }
    }

    public List<ParameterModel> Parameters
    {
        get => Parameter.Values.ToList();
        set => Parameter = value.ToDictionary(kvp => kvp.Name, kvp => kvp);
    }

    public ParameterModel GetParameter(string name)
    {
        Parameter.TryGetValue(name, out var result);

        return result;
    }

    public void AddParameter(string name, ParameterModel model) => Parameter.Add(name, model);

    #region ToString & Equality

    public override string ToString()
    {
        return
            $"[InitializerModel: [Marks: {Marks.ToItemString()}, GroupCount: {GroupCount}, Parameters: {Parameters.ToItemString()}]]";
    }

    protected bool Equals(InitializerModel other)
    {
        return Marks.ItemEquals(other.Marks) && GroupCount == other.GroupCount && Parameters.ItemEquals(other.Parameters);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;

        return Equals((InitializerModel)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = (Marks != null ? Marks.GetItemHashCode() : 0);
            hashCode = (hashCode * 397) ^ GroupCount;
            hashCode = (hashCode * 397) ^ (Parameters != null ? Parameters.GetItemHashCode() : 0);
            return hashCode;
        }
    }

    #endregion
}
