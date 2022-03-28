using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System;

namespace Routine.Core.Reflection
{
    internal class ReflectionOptimizer
    {
        public static bool Enabled { get; private set; } = true;
        public static void Disable() => Enabled = false;
        public static void Enable() => Enabled = true;

        private static readonly object OPTIMIZE_LIST_LOCK = new();
        private static readonly HashSet<MethodBase> OPTIMIZE_LIST = new();

        private static readonly object INVOKERS_LOCK = new();
        private static readonly Dictionary<MethodBase, IMethodInvoker> INVOKERS = new();

        public static IMethodInvoker CreateInvoker(MethodBase method)
        {
            if (!Enabled) { return new ReflectionMethodInvoker(method); }

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

        private readonly List<MethodBase> methods;

        private ReflectionOptimizer(List<MethodBase> methods)
        {
            this.methods = methods;
        }

        public Dictionary<MethodBase, IMethodInvoker> Optimize()
        {
            var result = new Dictionary<MethodBase, IMethodInvoker>();

            var compiler = new CodeCompiler();

            compiler.AddReferenceFrom<IMethodInvoker>();
            compiler.AddReferenceFrom<Task>();

            var methodsByName = new Dictionary<string, MethodBase>();
            foreach (var method in methods)
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

                var template = new OptimizedMethodInvokerTemplate(method);

                methodsByName[template.InvokerTypeName] = method;

                compiler.AddCode(template.Render());

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
            }

            if (!compiler.HasCode) { return result; }

            var assembly = compiler.Compile();

            foreach (var invokerType in assembly.GetExportedTypes())
            {
                if (!methodsByName.TryGetValue(invokerType.Name, out var method)) { continue; }

                result.TryAdd(method, (IMethodInvoker)Activator.CreateInstance(invokerType));
            }

            return result;
        }

        private static bool SetIsInitOnly(MethodBase setMethod) =>
            setMethod is MethodInfo setMethodInfo &&
            setMethodInfo.ReturnParameter != null &&
            setMethodInfo.ReturnParameter.GetRequiredCustomModifiers().Contains(typeof(System.Runtime.CompilerServices.IsExternalInit));
    }
}
