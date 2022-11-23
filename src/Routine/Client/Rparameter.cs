using Routine.Core;

namespace Routine.Client;

public class Rparameter
{
    private readonly ParameterModel _model;

    public Rparametric Owner { get; }
    public Rtype ParameterType { get; }

    public Rparameter(ParameterModel model, Rparametric owner)
    {
        _model = model;

        Owner = owner;

        ParameterType = Application[model.ViewModelId];
    }

    public Rapplication Application => Owner.Type.Application;
    public Rtype Type => Owner.Type;
    public string Name => _model.Name;
    public bool IsList => _model.IsList;
    public List<int> Groups => _model.Groups;
    public HashSet<string> Marks => _model.Marks;

    public bool IsOptional => _model.IsOptional;
    public Rvariable Default =>
        CreateVariable(_model.DefaultValue.Values
            .Select(v => Application.Get(v.Id, v.ModelId))
            .ToList()
        );

    public bool MarkedAs(string mark) => Marks.Contains(mark);

    public Rvariable CreateVariable(params Robject[] robjs) => CreateVariable(robjs.ToList());
    public Rvariable CreateVariable(List<Robject> robjs)
    {
        var result = new Rvariable(Name, robjs);

        return IsList ? result : result.ToSingle();
    }

    internal ParameterValueData CreateParameterValueData(params Robject[] robjs) => CreateParameterValueData(robjs.ToList());
    internal ParameterValueData CreateParameterValueData(List<Robject> robjs) =>
        new()
        {
            IsList = IsList,
            Values = robjs.Select(robj => robj.GetParameterData()).ToList()
        };

    #region Equality & Hashcode

    protected bool Equals(Rparameter other)
    {
        return Equals(Owner, other.Owner) && Equals(_model, other._model);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;

        return Equals((Rparameter)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return ((Owner != null ? Owner.GetHashCode() : 0) * 397) ^ (_model != null ? _model.GetHashCode() : 0);
        }
    }

    #endregion
}
