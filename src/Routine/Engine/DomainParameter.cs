using Routine.Core;

namespace Routine.Engine;

public class DomainParameter
{
    #region internal class DomainParameter.Group

    internal class Group<T> where T : class, IParametric
    {
        public T Parametric { get; }
        public List<DomainParameter> Parameters { get; }
        public int GroupIndex { get; }

        public Group(T parametric, IEnumerable<DomainParameter> parameters, int groupIndex)
        {
            Parametric = parametric;
            Parameters = parameters.OrderBy(p => parametric.Parameters.Single(p2 => p2.Name == p.Name).Index).ToList();
            GroupIndex = groupIndex;
        }

        public bool ContainsSameParameters(T parametric) =>
            Parametric.Parameters.Count == parametric.Parameters.Count &&
            Parametric.Parameters.All(p1 => parametric.Parameters.Any(p2 => p1.Name == p2.Name));
    }

    #endregion

    #region internal static void AddGroupToTarget<T>(T group, IDomainParametric<T> target)

    internal static void AddGroupToTarget<T>(T group, IDomainParametric<T> target)
        where T : class, IParametric
    {
        Validate(group, target);

        foreach (var parameter in group.Parameters)
        {
            if (target.Parameter.TryGetValue(parameter.Name, out var domainParameter))
            {
                domainParameter.AddGroup(parameter, target.NextGroupIndex);
            }
            else
            {
                target.Parameter.Add(parameter.Name, new DomainParameter(target.Ctx, parameter, target.NextGroupIndex));
            }
        }

        target.AddGroup(group, target.Parameter.Values.Where(p => p.Groups.Contains(target.NextGroupIndex)), target.NextGroupIndex);
    }

    private static void Validate<T>(T group, IDomainParametric<T> target)
        where T : class, IParametric
    {
        foreach (var parameter in group.Parameters)
        {
            if (target.Parameter.TryGetValue(parameter.Name, out var domainParameter))
            {
                if (domainParameter.Groups.Contains(target.NextGroupIndex))
                {
                    throw new InvalidOperationException(
                        $"{parameter.Owner.ParentType.Name}.{parameter.Owner.Name}(...,{parameter.Name},...): Given groupIndex ({target.NextGroupIndex}) was already added!");
                }

                if (!domainParameter.parameter.ParameterType.Equals(parameter.ParameterType))
                {
                    throw new ParameterTypesDoNotMatchException(
                        parameter,
                        domainParameter.ParameterType.Type,
                        parameter.ParameterType
                    );
                }
            }
            else if (!target.Ctx.CodingStyle.ContainsType(Fix(parameter.ParameterType)))
            {
                throw new TypeNotConfiguredException(Fix(parameter.ParameterType));
            }
        }
    }

    private static IType Fix(IType type) => type.CanBeCollection() ? type.GetItemType() : type;

    #endregion

    private readonly ICoreContext ctx;
    private readonly IParameter parameter;

    public string Name { get; }
    public DomainType ParameterType { get; }
    public Marks Marks { get; }
    public List<int> Groups { get; }
    public bool IsList { get; }
    public bool IsOptional { get; }

    private readonly object defaultValue;

    private DomainParameter(ICoreContext ctx, IParameter parameter, int initialGroupIndex)
    {
        this.ctx = ctx;
        this.parameter = parameter;

        Name = ctx.CodingStyle.GetName(parameter);
        ParameterType = ctx.GetDomainType(Fix(parameter.ParameterType));
        Marks = new(ctx.CodingStyle.GetMarks(parameter));
        Groups = new() { initialGroupIndex };
        IsList = parameter.ParameterType.CanBeCollection();
        IsOptional = ctx.CodingStyle.IsOptional(parameter);

        defaultValue = ctx.CodingStyle.GetDefaultValue(parameter);
    }

    private void AddGroup(IParameter parameter, int groupIndex)
    {
        Groups.Add(groupIndex);

        Marks.Join(ctx.CodingStyle.GetMarks(parameter));
    }

    public bool MarkedAs(string mark) => Marks.Has(mark);

    public ParameterModel GetModel() =>
        new()
        {
            Name = Name,
            ViewModelId = ParameterType.Id,
            Marks = Marks.List,
            Groups = Groups,
            IsList = IsList,
            IsOptional = IsOptional,
            DefaultValue = ctx.CreateValueData(defaultValue, IsList, ParameterType, false)
        };

    internal async Task<object> LocateAsync(ParameterValueData parameterValueData)
    {
        if (!IsList) { return await GetObjectAsync(parameterValueData); }

        var result = parameter.ParameterType.CreateListInstance(parameterValueData.Values.Count);

        var objects = await GetObjectsAsync(parameterValueData);

        for (var i = 0; i < objects.Count; i++)
        {
            if (parameter.ParameterType.IsArray)
            {
                result[i] = objects[i];
            }
            else
            {
                result.Add(objects[i]);
            }
        }

        return result;
    }

    private async Task<object> GetObjectAsync(ParameterValueData parameterValueData)
    {
        if (!parameterValueData.Values.Any())
        {
            return null;
        }

        var parameterData = parameterValueData.Values.First();

        return await GetDomainType(parameterData).LocateAsync(parameterData);
    }

    private async Task<List<object>> GetObjectsAsync(ParameterValueData parameterValueData)
    {
        if (!parameterValueData.Values.Any()) { return new(); }

        var result = new List<object>();

        var domainTypes = parameterValueData.Values.Select(GetDomainType).ToList();
        if (domainTypes.Any(dt => !Equals(dt, ParameterType)))
        {
            for (var i = 0; i < parameterValueData.Values.Count; i++)
            {
                var parameterData = parameterValueData.Values[i];
                var domainType = domainTypes[i];

                result.Add(await domainType.LocateAsync(parameterData));
            }
        }
        else
        {
            result.AddRange(await ParameterType.LocateManyAsync(parameterValueData.Values));
        }

        return result;
    }

    private DomainType GetDomainType(ParameterData parameterData)
    {
        if (parameterData == null)
        {
            return ctx.GetDomainType((IType)null);
        }

        var domainType = ParameterType;

        if (parameterData.ModelId != domainType.Id && !string.IsNullOrEmpty(parameterData.ModelId))
        {
            domainType = ctx.GetDomainType(parameterData.ModelId);
        }

        return domainType;
    }

    #region Formatting & Equality

    protected bool Equals(DomainParameter other)
    {
        return string.Equals(Name, other.Name);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;

        return Equals((DomainParameter)obj);
    }

    public override int GetHashCode()
    {
        return (Name != null ? Name.GetHashCode() : 0);
    }

    public override string ToString()
    {
        return string.Format("{1} {0}", Name, ParameterType);
    }

    #endregion
}
