namespace Routine.Core.Reflection;

public interface IMethodInvoker
{
    object Invoke(object target, params object[] args);
    Task<object> InvokeAsync(object target, params object[] args);
}

public static class IMethodInvokerReflectionExtensions
{
    public static IMethodInvoker CreateInvoker(this System.Reflection.MethodBase source) => new ProxyMethodInvoker(source);
}
