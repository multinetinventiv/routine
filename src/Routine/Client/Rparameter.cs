using Routine.Core;

namespace Routine.Client;

public class Rparameter
{
    private readonly ParameterModel model;

    public Rparametric Owner { get; }
    public Rtype ParameterType { get; }

    public Rparameter(ParameterModel model, Rparametric owner)
    {
        this.model = model;

        Owner = owner;

        ParameterType = Application[model.ViewModelId];
    }

    public Rapplication Application => Owner.Type.Application;
    public Rtype Type => Owner.Type;
    public string Name => model.Name;
    public bool IsList => model.IsList;
    public List<int> Groups => model.Groups;
    public HashSet<string> Marks => model.Marks;

    public bool IsOptional => model.IsOptional;
    public Rvariable Default =>
        CreateVariable(model.DefaultValue.Values
            .Select(v => Application.Get(v.Id, v.ModelId))
            .ToList()
        );

    public bool MarkedAs(string mark) => model.Marks.Contains(mark);

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
        return Equals(Owner, other.Owner) && Equals(model, other.model);
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
            return ((Owner != null ? Owner.GetHashCode() : 0) * 397) ^ (model != null ? model.GetHashCode() : 0);
        }
    }

    #endregion
}
