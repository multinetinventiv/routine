namespace Routine.Core;

public class VariableData
{
    public bool IsList { get; set; }
    public List<ObjectData> Values { get; set; } = new();

    public VariableData() { }
    public VariableData(IDictionary<string, object> data)
    {
        if (data == null) return;

        if (data.TryGetValue(nameof(IsList), out var isList))
        {
            IsList = (bool)isList;
        }

        if (data.TryGetValue(nameof(Values), out var values))
        {
            Values = ((IEnumerable)values).Cast<IDictionary<string, object>>().Select(o => new ObjectData(o)).ToList();
        }
    }

    #region ToString & Equality

    public override string ToString()
    {
        return $"[VariableData: [IsList: {IsList}, Values: {Values.ToItemString()}]]";
    }

    protected bool Equals(VariableData other)
    {
        return IsList == other.IsList && Values.ItemEquals(other.Values);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;

        return Equals((VariableData)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (IsList.GetHashCode() * 397) ^ (Values != null ? Values.GetItemHashCode() : 0);
        }
    }

    #endregion
}
