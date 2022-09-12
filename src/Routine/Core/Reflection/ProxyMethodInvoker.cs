using System.Threading.Tasks;

namespace Routine.Core.Reflection;

public static class SystemReflectionFacadeExtensions
{
    public static IMethodInvoker CreateInvoker(this System.Reflection.MethodBase source) => new ProxyMethodInvoker(source);
}

internal class ProxyMethodInvoker : IMethodInvoker
{
    private readonly System.Reflection.MethodBase method;

    public ProxyMethodInvoker(System.Reflection.MethodBase method)
    {
        this.method = method;

        ReflectionOptimizer.AddToOptimizeList(method);
    }

    private IMethodInvoker real;
    public IMethodInvoker Real => real ??= ReflectionOptimizer.CreateInvoker(method);

    public object Invoke(object target, params object[] args) => Real.Invoke(target, args);
    public async Task<object> InvokeAsync(object target, params object[] args) => await Real.InvokeAsync(target, args);
}
