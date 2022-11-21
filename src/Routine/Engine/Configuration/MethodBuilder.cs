using Routine.Engine.Reflection;
using Routine.Engine.Virtual;

namespace Routine.Engine.Configuration;

public partial class MethodBuilder
{
    private readonly IType _parentType;

    public MethodBuilder(IType parentType)
    {
        _parentType = parentType;
    }

    public IType ParentType => _parentType;

    public IEnumerable<IMethod> Proxy<T>(T target) => Proxy<T>().Target(target);

    public ProxyMethodBuilder<T> Proxy<T>() => Proxy<T>(_ => true);
    public ProxyMethodBuilder<T> Proxy<T>(string targetMethodName) => Proxy<T>(m => m.Name == targetMethodName);
    public ProxyMethodBuilder<T> Proxy<T>(Func<MethodInfo, bool> targetMethodPredicate) =>
        new ProxyMethodBuilder<T>(_parentType, type.of<T>().GetAllMethods().Where(targetMethodPredicate))
            .Name.Set(c => c.By(o => o.Name))
            .NextLayer();

    private VirtualMethod Virtual() => new(_parentType);

    public VirtualMethod Virtual(string name) =>
        Virtual()
            .Name.Set(name)
            .ReturnType.Set(type.ofvoid());

    public VirtualMethod Virtual<T>(string name) =>
        Virtual()
            .Name.Set(name)
            .ReturnType.Set(type.of<T>());

    public VirtualMethod Virtual(string name, Action body) =>
        Virtual(name)
            .Body.Set((_, _) =>
            {
                body();
                return null;
            });

    public VirtualMethod Virtual<TReturn>(string name, Func<TReturn> body) =>
        Virtual()
            .Name.Set(name)
            .ReturnType.Set(type.of<TReturn>())
            .Body.Set((_, _) => body());
}
