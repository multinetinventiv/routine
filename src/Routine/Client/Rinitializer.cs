using Routine.Core;

namespace Routine.Client;

public class Rinitializer : Rparametric
{
    private readonly InitializerModel _model;

    public Rinitializer(InitializerModel model, Rtype type)
        : base(Constants.INITIALIZER_ID, model.GroupCount, model.Parameters, model.Marks, type)
    {
        _model = model;
    }

    #region Equality & Hashcode

    protected bool Equals(Rinitializer other)
    {
        return Equals(Type, other.Type) && Equals(_model, other._model);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;

        return Equals((Rinitializer)obj);
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
