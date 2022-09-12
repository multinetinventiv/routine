using System.Collections.Generic;

namespace Routine.Engine;

public interface IDomainParametric<in T> where T : class, IParametric
{
    Dictionary<string, DomainParameter> Parameter { get; }
    ICoreContext Ctx { get; }
    int NextGroupIndex { get; }
    void AddGroup(T parametric, IEnumerable<DomainParameter> parameters, int groupIndex);
}
