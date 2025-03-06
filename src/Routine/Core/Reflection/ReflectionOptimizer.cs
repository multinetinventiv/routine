using System.Reflection;

namespace Routine.Core.Reflection;

internal class ReflectionOptimizer
{
    public static bool Enabled { get; private set; } = true;
    public static void Disable() => Enabled = false;
    public static void Enable() => Enabled = true;
    public static event EventHandler Optimized;

    private static readonly object OPTIMIZE_LIST_LOCK = new();
    private static readonly HashSet<MethodBase> OPTIMIZE_LIST = new();

    private static readonly object INVOKERS_LOCK = new();
    private static readonly Dictionary<MethodBase, IMethodInvoker> INVOKERS = new();

    public static IMethodInvoker CreateInvoker(MethodBase method)
    {
        if (method == null) { throw new ArgumentNullException(nameof(method)); }

        AddToOptimizeList(method);

        lock (OPTIMIZE_LIST_LOCK)
        {
            var localOptimizeList = new List<MethodBase>(OPTIMIZE_LIST);
            lock (INVOKERS_LOCK)
            {
                var invokers = new ReflectionOptimizer(localOptimizeList).Optimize();
                foreach (var (key, value) in invokers)
                {
                    INVOKERS[key] = value;
                }
            }

            foreach (var current in localOptimizeList)
            {
                OPTIMIZE_LIST.Remove(current);
            }
        }

        if (!INVOKERS.TryGetValue(method, out var result))
        {
            throw new InvalidOperationException($"Cannot optimize {method.ReflectedType} {method}");
        }

        return result;
    }

    public static void AddToOptimizeList(MethodBase method)
    {
        if (method == null) { throw new ArgumentNullException(nameof(method)); }
        if (INVOKERS.ContainsKey(method)) { return; }

        lock (OPTIMIZE_LIST_LOCK)
        {
            OPTIMIZE_LIST.Add(method);
        }
    }

    public static void Clear()
    {
        lock (OPTIMIZE_LIST_LOCK)
        {
            lock (INVOKERS_LOCK)
            {

                OPTIMIZE_LIST.Clear();
                INVOKERS.Clear();
            }
        }
    }

    private readonly List<MethodBase> _methods;

    private ReflectionOptimizer(List<MethodBase> methods)
    {
        _methods = methods;
    }

    public Dictionary<MethodBase, IMethodInvoker> Optimize()
    {
        var result = new Dictionary<MethodBase, IMethodInvoker>();

        if (_methods.Count == 0)
        {
            Optimized?.Invoke(null, EventArgs.Empty);
            return result;
        }

        var compiler = new CodeCompiler();

        compiler.AddReferenceFrom<IMethodInvoker>();
        compiler.AddReferenceFrom<Task>();

        var methodsByName = new Dictionary<string, MethodBase>();
        foreach (var method in _methods)
        {
            if (method.ContainsGenericParameters || method.ReflectedType == null) { continue; }

            if (!method.IsPublic ||
                !method.ReflectedType.IsPublic && !method.ReflectedType.IsNestedPublic ||
                method.GetParameters().Any(pi => pi.IsIn || pi.IsOut || pi.ParameterType.IsPointer || pi.ParameterType.IsByRef) ||
                method.ReflectedType.IsValueType && method.IsSpecialName ||
                method.Name == "<Clone>$" ||
                method.Name.StartsWith("set_") && SetIsInitOnly(method))
            {
                result.TryAdd(method, new ReflectionMethodInvoker(method));

                continue;
            }

            using (var template = new OptimizedMethodInvokerTemplate(method))
            {
                methodsByName[template.InvokerTypeName] = method;
                compiler.AddCode(template.Render());
            }

            compiler.AddReferenceFrom(method.ReflectedType);
            compiler.AddReferenceFrom(method.DeclaringType);

            foreach (var parameter in method.GetParameters())
            {
                compiler.AddReferenceFrom(parameter.ParameterType);
            }

            if (method is MethodInfo methodInfo)
            {
                compiler.AddReferenceFrom(methodInfo.ReturnType);
            }

            result.TryAdd(method, new SwitchableMethodInvoker(new ReflectionMethodInvoker(method)));
        }

        if (!compiler.HasCode) { return result; }

        Task.Run(() =>
        {
            var assembly = compiler.Compile();
            foreach (var invokerType in assembly.GetExportedTypes())
            {
                if (!methodsByName.TryGetValue(invokerType.Name, out var method)) { continue; }
                if (!result.TryGetValue(method, out var invoker)) { continue; }
                if (invoker is not SwitchableMethodInvoker switchable) { continue; }

                switchable.Switch((IMethodInvoker)Activator.CreateInstance(invokerType));
            }

            Optimized?.Invoke(null, EventArgs.Empty);
        });

        return result;
    }

    private static bool SetIsInitOnly(MethodBase setMethod) =>
        setMethod is MethodInfo setMethodInfo &&
        setMethodInfo.ReturnParameter != null &&
        setMethodInfo.ReturnParameter.GetRequiredCustomModifiers().Contains(typeof(System.Runtime.CompilerServices.IsExternalInit));
}
