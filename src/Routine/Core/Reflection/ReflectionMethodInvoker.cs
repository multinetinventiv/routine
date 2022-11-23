using Routine.Core.Runtime;
using System.Reflection;

namespace Routine.Core.Reflection;

public class ReflectionMethodInvoker : IMethodInvoker
{
    private readonly MethodBase _method;

    public ReflectionMethodInvoker(MethodBase method)
    {
        _method = method;
    }

    public object Invoke(object target, params object[] args)
    {
        var result = InvokeInner(target, args);

        if (result is not Task task) { return result; }

        return task.WaitAndGetResult();
    }

    public async Task<object> InvokeAsync(object target, params object[] args)
    {
        var result = InvokeInner(target, args);

        if (result is not Task task) { return result; }

        await task;

        return task.GetResult();
    }

    private object InvokeInner(object target, object[] args)
    {
        try
        {
            if (_method.IsConstructor)
            {
                var ctor = (ConstructorInfo)_method;

                return ctor.Invoke(args);
            }

            if (!_method.IsStatic && target == null) { throw new NullReferenceException(); }

            return _method.Invoke(target, args);
        }
        catch (TargetInvocationException ex)
        {
            throw ex.GetInnerException();
        }
    }
}
