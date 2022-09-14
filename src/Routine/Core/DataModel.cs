namespace Routine.Core;

public class DataModel
{
    public List<string> Marks { get; set; } = new List<string>();

    public string Name { get; set; }
    public string ViewModelId { get; set; }
    public bool IsList { get; set; }

    public DataModel() { }
    public DataModel(IDictionary<string, object> model)
    {
        if (model == null) return;

        if (model.TryGetValue("Marks", out var marks))
        {
            Marks = ((IEnumerable)marks).Cast<string>().ToList();
        }

        if (model.TryGetValue("Name", out var name))
        {
            Name = (string)name;
        }

        if (model.TryGetValue("ViewModelId", out var viewModelId))
        {
            ViewModelId = (string)viewModelId;
        }

        if (model.TryGetValue("IsList", out var isList))
        {
            IsList = (bool)isList;
        }
    }

    #region ToString & Equality

    public override string ToString()
    {
        return
            $"[DataModel: [Marks: {Marks.ToItemString()}, Name: {Name}, ViewModelId: {ViewModelId}, IsList: {IsList}]]";
    }

    protected bool Equals(DataModel other)
    {
        return Marks.ItemEquals(other.Marks) && string.Equals(Name, other.Name) && string.Equals(ViewModelId, other.ViewModelId) && IsList == other.IsList;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;

        return Equals((DataModel)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = (Marks != null ? Marks.GetItemHashCode() : 0);
            hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (ViewModelId != null ? ViewModelId.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ IsList.GetHashCode();
            return hashCode;
        }
    }

    #endregion
}
