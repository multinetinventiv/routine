using Routine.Core;

namespace Routine.Engine;

public class DomainData
{
    private readonly ICoreContext _ctx;
    private readonly IProperty _property;

    public string Name { get; }
    public Marks Marks { get; }
    public bool IsList { get; }
    public bool FetchedEagerly { get; }
    public DomainType DataType { get; }

    public DomainData(ICoreContext ctx, IProperty property)
    {
        _ctx = ctx;
        _property = property;

        Name = ctx.CodingStyle.GetName(property);
        Marks = new(ctx.CodingStyle.GetMarks(property));
        IsList = property.ReturnType.CanBeCollection();
        FetchedEagerly = ctx.CodingStyle.IsFetchedEagerly(property);

        var returnType = IsList ? property.ReturnType.GetItemType() : property.ReturnType;

        if (!ctx.CodingStyle.ContainsType(returnType))
        {
            throw new TypeNotConfiguredException(returnType);
        }

        DataType = ctx.GetDomainType(returnType);
    }

    public bool MarkedAs(string mark) => Marks.Has(mark);

    public DataModel GetModel() =>
        new()
        {
            Name = Name,
            Marks = Marks.Set,
            IsList = IsList,
            ViewModelId = DataType.Id
        };

    public VariableData CreateData(object target) => CreateData(target, FetchedEagerly);
    public VariableData CreateData(object target, bool eager) => CreateData(target, Constants.FIRST_DEPTH, eager);
    internal VariableData CreateData(object target, int currentDepth) => CreateData(target, currentDepth, FetchedEagerly);
    internal VariableData CreateData(object target, int currentDepth, bool eager) => _ctx.CreateValueData(_property.FetchFrom(target), IsList, DataType, currentDepth, eager);

    #region Formatting & Equality

    protected bool Equals(DomainData other)
    {
        return string.Equals(Name, other.Name);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((DomainData)obj);
    }

    public override int GetHashCode()
    {
        return (Name != null ? Name.GetHashCode() : 0);
    }

    public override string ToString()
    {
        return string.Format("{1} {0}", Name, DataType);
    }

    #endregion
}
