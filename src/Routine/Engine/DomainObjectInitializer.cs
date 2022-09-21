using Routine.Core;

namespace Routine.Engine;

public class DomainObjectInitializer : IDomainParametric<IConstructor>
{
    private readonly ICoreContext ctx;
    private readonly List<DomainParameter.Group<IConstructor>> groups;

    public Dictionary<string, DomainParameter> Parameter { get; }
    public Marks Marks { get; }

    public ICollection<DomainParameter> Parameters => Parameter.Values;

    ICoreContext IDomainParametric<IConstructor>.Ctx => ctx;
    int IDomainParametric<IConstructor>.NextGroupIndex => groups.Count;
    void IDomainParametric<IConstructor>.AddGroup(IConstructor parametric, IEnumerable<DomainParameter> parameters, int groupIndex) => groups.Add(new DomainParameter.Group<IConstructor>(parametric, parameters, groupIndex));

    public DomainObjectInitializer(ICoreContext ctx, IConstructor constructor)
    {
        this.ctx = ctx;

        groups = new();
        Parameter = new();

        Marks = new();

        AddGroup(constructor);
    }

    public void AddGroup(IConstructor constructor)
    {
        if (groups.Any() &&
            !constructor.InitializedType.Equals(groups.Last().Parametric.InitializedType))
        {
            throw new InitializedTypeDoNotMatchException(constructor, groups.Last().Parametric.InitializedType, constructor.InitializedType);
        }

        if (groups.Any(g => g.ContainsSameParameters(constructor)))
        {
            throw new IdenticalSignatureAlreadyAddedException(constructor);
        }

        DomainParameter.AddGroupToTarget(constructor, this);

        Marks.Join(ctx.CodingStyle.GetMarks(constructor));
    }

    public async Task<object> InitializeAsync(Dictionary<string, ParameterValueData> parameterValues)
    {
        var resolution = await Resolver(parameterValues).ResolveAsync();

        return resolution.Result.Initialize(resolution.Parameters);
    }

    private DomainParameterResolver<IConstructor> Resolver(Dictionary<string, ParameterValueData> parameterValues) =>
        new(groups, parameterValues);

    public InitializerModel GetModel() =>
        new()
        {
            Marks = Marks.Set,
            GroupCount = groups.Count,
            Parameters = Parameters.Select(p => p.GetModel()).ToList()
        };
}
