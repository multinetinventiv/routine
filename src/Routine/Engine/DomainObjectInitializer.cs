using Routine.Core;

namespace Routine.Engine;

public class DomainObjectInitializer : IDomainParametric<IConstructor>
{
    private readonly ICoreContext _ctx;
    private readonly List<DomainParameter.Group<IConstructor>> _groups;

    public Dictionary<string, DomainParameter> Parameter { get; }
    public Marks Marks { get; }

    public ICollection<DomainParameter> Parameters => Parameter.Values;

    ICoreContext IDomainParametric<IConstructor>.Ctx => _ctx;
    int IDomainParametric<IConstructor>.NextGroupIndex => _groups.Count;
    void IDomainParametric<IConstructor>.AddGroup(IConstructor parametric, IEnumerable<DomainParameter> parameters, int groupIndex) => _groups.Add(new DomainParameter.Group<IConstructor>(parametric, parameters, groupIndex));

    public DomainObjectInitializer(ICoreContext ctx, IConstructor constructor)
    {
        _ctx = ctx;

        _groups = new();
        Parameter = new();

        Marks = new();

        AddGroup(constructor);
    }

    public void AddGroup(IConstructor constructor)
    {
        if (_groups.Any() &&
            !constructor.InitializedType.Equals(_groups.Last().Parametric.InitializedType))
        {
            throw new InitializedTypeDoNotMatchException(constructor, _groups.Last().Parametric.InitializedType, constructor.InitializedType);
        }

        if (_groups.Any(g => g.ContainsSameParameters(constructor)))
        {
            throw new IdenticalSignatureAlreadyAddedException(constructor);
        }

        DomainParameter.AddGroupToTarget(constructor, this);

        Marks.Join(_ctx.CodingStyle.GetMarks(constructor));
    }

    public async Task<object> InitializeAsync(Dictionary<string, ParameterValueData> parameterValues)
    {
        var resolution = await Resolver(parameterValues).ResolveAsync();

        return resolution.Result.Initialize(resolution.Parameters);
    }

    private DomainParameterResolver<IConstructor> Resolver(Dictionary<string, ParameterValueData> parameterValues) =>
        new(_groups, parameterValues);

    public InitializerModel GetModel() =>
        new()
        {
            Marks = Marks.Set,
            GroupCount = _groups.Count,
            Parameters = Parameters.Select(p => p.GetModel()).ToList()
        };
}
