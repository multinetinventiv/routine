using Routine.Core;

namespace Routine.Client;

public class Rdata
{
    private readonly DataModel _model;

    public Rtype Type { get; }
    public Rtype DataType { get; }

    public Rdata(DataModel model, Rtype type)
    {
        _model = model;

        Type = type;
        DataType = Application[model.ViewModelId];
    }

    public Rapplication Application => Type.Application;
    public string Name => _model.Name;
    public bool IsList => _model.IsList;
    public HashSet<string> Marks => _model.Marks;

    public bool MarkedAs(string mark) => Marks.Contains(mark);

    #region Equality & Hashcode

    protected bool Equals(Rdata other)
    {
        return Equals(Type, other.Type) && Equals(_model, other._model);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Rdata)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return ((Type != null ? Type.GetHashCode() : 0) * 397) ^ (_model != null ? _model.GetHashCode() : 0);
        }
    }

    #endregion
}
