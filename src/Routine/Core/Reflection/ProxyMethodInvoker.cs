namespace Routine.Core.Reflection;

internal class ProxyMethodInvoker : IMethodInvoker
{
    private readonly System.Reflection.MethodBase _method;

    public ProxyMethodInvoker(System.Reflection.MethodBase method)
    {
        _method = method;

        ReflectionOptimizer.AddToOptimizeList(method);
    }

    private IMethodInvoker _real;
    public IMethodInvoker Real
    {
        get
        {
            if (!ReflectionOptimizer.Enabled) { return new ReflectionMethodInvoker(_method); }

            return _real ??= ReflectionOptimizer.CreateInvoker(_method);
        }
    }

    public object Invoke(object target, params object[] args) => Real.Invoke(target, args);
    public async Task<object> InvokeAsync(object target, params object[] args) => await Real.InvokeAsync(target, args);
}
