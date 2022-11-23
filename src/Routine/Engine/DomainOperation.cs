using Routine.Core;
using Routine.Core.Runtime;

namespace Routine.Engine;

public class DomainOperation : IDomainParametric<IMethod>
{
    private readonly ICoreContext _ctx;
    private readonly List<DomainParameter.Group<IMethod>> _groups;

    public string Name { get; }
    public Marks Marks { get; }
    public bool ResultIsVoid { get; }
    public bool ResultIsList { get; }
    public DomainType ResultType { get; }
    public Dictionary<string, DomainParameter> Parameter { get; }

    public ICollection<DomainParameter> Parameters => Parameter.Values;

    ICoreContext IDomainParametric<IMethod>.Ctx => _ctx;
    int IDomainParametric<IMethod>.NextGroupIndex => _groups.Count;
    void IDomainParametric<IMethod>.AddGroup(IMethod parametric, IEnumerable<DomainParameter> parameters, int groupIndex) => _groups.Add(new(parametric, parameters, groupIndex));

    public DomainOperation(ICoreContext ctx, IMethod method)
    {
        _ctx = ctx;

        _groups = new();

        Name = ctx.CodingStyle.GetName(method);
        Marks = new();
        ResultIsVoid = method.ReturnType.IsVoid;
        ResultIsList = method.ReturnType.CanBeCollection();
        Parameter = new();

        var returnType = ResultIsList ? method.ReturnType.GetItemType() : method.ReturnType;

        if (!ctx.CodingStyle.ContainsType(returnType))
        {
            throw new TypeNotConfiguredException(returnType);
        }

        ResultType = ctx.GetDomainType(returnType);

        AddGroup(method);
    }

    public void AddGroup(IMethod method)
    {
        if (_groups.Any() &&
            !method.ReturnType.Equals(_groups.Last().Parametric.ReturnType))
        {
            throw new ReturnTypesDoNotMatchException(method, _groups.Last().Parametric.ReturnType, method.ReturnType);
        }

        if (_groups.Any(g => g.ContainsSameParameters(method)))
        {
            throw new IdenticalSignatureAlreadyAddedException(method);
        }

        DomainParameter.AddGroupToTarget(method, this);

        Marks.Join(_ctx.CodingStyle.GetMarks(method));
    }

    public bool MarkedAs(string mark) => Marks.Has(mark);

    public OperationModel GetModel() =>
        new()
        {
            Name = Name,
            Marks = Marks.Set,
            GroupCount = _groups.Count,
            Parameters = Parameters.Select(p => p.GetModel()).ToList(),
            Result = new()
            {
                IsList = ResultIsList,
                IsVoid = ResultIsVoid,
                ViewModelId = ResultType.Id
            }
        };

    public VariableData Perform(object target, Dictionary<string, ParameterValueData> parameterValues)
    {
        var (method, parameters) = Resolver(parameterValues).ResolveAsync().WaitAndGetResult();
        var result = method.PerformOn(target, parameters);

        return ResultData(result);
    }

    public async Task<VariableData> PerformAsync(object target, Dictionary<string, ParameterValueData> parameterValues)
    {
        var (method, parameters) = await Resolver(parameterValues).ResolveAsync();
        var result = await method.PerformOnAsync(target, parameters);

        return ResultData(result);
    }

    private DomainParameterResolver<IMethod> Resolver(Dictionary<string, ParameterValueData> parameterValues) => new(_groups, parameterValues);

    private VariableData ResultData(object result) =>
        ResultIsVoid
            ? new()
            : _ctx.CreateValueData(result, ResultIsList, ResultType, true);

    #region Formatting & Equality

    protected bool Equals(DomainOperation other)
    {
        return string.Equals(Name, other.Name);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((DomainOperation)obj);
    }

    public override int GetHashCode()
    {
        return (Name != null ? Name.GetHashCode() : 0);
    }

    public override string ToString()
    {
        return string.Format("{1} {0}(...)", Name, ResultType);
    }

    #endregion
}
