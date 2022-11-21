using Routine.Core.Configuration;
using Routine.Engine.Virtual;

namespace Routine.Engine.Configuration;

public class ProxyMethodBuilder<T> : LayeredBase<ProxyMethodBuilder<T>>
{
    private readonly IType _parentType;
    private readonly IEnumerable<IMethod> _methods;

    public ConventionBasedConfiguration<ProxyMethodBuilder<T>, IMethod, string> Name { get; }

    public ProxyMethodBuilder(IType parentType, IEnumerable<IMethod> methods)
    {
        _parentType = parentType;
        _methods = methods;

        Name = new(this, "Name");
    }

    public IType ParentType => _parentType;
    public IEnumerable<IMethod> Methods => _methods;

    public IEnumerable<IMethod> TargetBySelf() => TargetBy(o => (T)o);
    public IEnumerable<IMethod> Target(T target) => TargetBy(() => target);
    public IEnumerable<IMethod> TargetBy(Func<T> targetDelegate) => TargetBy(_ => targetDelegate());
    public IEnumerable<IMethod> TargetBy(Func<object, T> targetDelegate) => _methods.Select(o => Build(_parentType, o, (obj, _) => targetDelegate(obj)));

    public IEnumerable<IMethod> TargetByParameter() => TargetByParameter(typeof(T).Name.ToLowerInitial());
    public IEnumerable<IMethod> TargetByParameter(string parameterName) => TargetByParameter<T>(parameterName);
    public IEnumerable<IMethod> TargetByParameter<TConcrete>() where TConcrete : T => TargetByParameter<TConcrete>(typeof(TConcrete).Name.ToLowerInitial());
    public IEnumerable<IMethod> TargetByParameter<TConcrete>(string parameterName) where TConcrete : T =>
        _methods.Select(o =>
            Build(_parentType, o,
                (_, parameters) => parameters[0],
                BuildRoutine.Parameter(o).Virtual()
                    .ParameterType.Set(type.of<TConcrete>())
                    .Name.Set(parameterName)
            )
        );

    private ProxyMethod Build(IType parentType, IMethod real, Func<object, object[], object> targetDelegate, params IParameter[] parameters) =>
        new ProxyMethod(parentType, real, targetDelegate, parameters).Name.Set(Name.Get(real));
}
