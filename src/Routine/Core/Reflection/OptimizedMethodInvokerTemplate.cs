using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Routine.Core.Reflection
{
    public class OptimizedMethodInvokerTemplate
    {
        private static string TypeName(Type type) => type.ToCSharpString().AfterLast(".").Replace("<", "_").Replace(">", "_");
        private static string RenderParameters(IEnumerable<ParameterInfo> parameters) => string.Join(",", parameters.Select(Parameter));

        public static string Render(MethodBase method) => new OptimizedMethodInvokerTemplate(method).Render();
        public static string GetInvokerTypeName(MethodBase current) =>
            $"{current.ReflectedType?.Namespace}.{TypeName(current.ReflectedType)}_{new OptimizedMethodInvokerTemplate(current).MethodName}_Invoker_{current.GetHashCode()}";

        private readonly MethodBase method;

        public OptimizedMethodInvokerTemplate(MethodBase method)
        {
            this.method = method;
        }

        public string Render() => $@"
namespace {method.ReflectedType?.Namespace}
{{
    public class {ReflectedType}_{MethodName}_Invoker_{method.GetHashCode()} : {typeof(IMethodInvoker).ToCSharpString()}
    {{
        public object Invoke(object target, params object[] args)
        {{
            {Invocation}
        }}

        public async {typeof(Task<object>).ToCSharpString()} InvokeAsync(object target, params object[] args)
        {{
            throw new System.NotImplementedException();
        }}
    }}
}}
";

        private string Invocation
        {
            get
            {
                if (method.IsConstructor)
                {
                    return method.GetParameters().Any(p => p.ParameterType.IsByRefLike) ? NotSupportedInvocation : NewInvocation;
                }

                var methodInfo = (MethodInfo)method;
                if (method.IsSpecialName)
                {
                    if (methodInfo.ReturnType.IsByRefLike || methodInfo.GetParameters().Any(p => p.ParameterType.IsByRefLike))
                    {
                        return NotSupportedInvocation;
                    }

                    if (methodInfo.Name.StartsWith("get_"))
                    {
                        return methodInfo.GetParameters().Any() ? IndexerPropertyGetInvocation : PropertyGetInvocation;
                    }

                    return methodInfo.GetParameters().Length > 1 ? IndexerPropertySetInvocation : PropertySetInvocation;
                }

                if (methodInfo.ReturnType.IsByRefLike || methodInfo.GetParameters().Any(p => p.ParameterType.IsByRefLike))
                {
                    return NotSupportedInvocation;
                }

                return methodInfo.ReturnType == typeof(void) ? VoidInvocation : NonVoidInvocation;
            }
        }

        private string NewInvocation => $"return new {method.ReflectedType.ToCSharpString()}({Parameters});";
        
        private string VoidInvocation => $"{Target}.{MethodName}({Parameters}); return null;";
        private string NonVoidInvocation => $"return {Target}.{MethodName}({Parameters});";
        
        private string PropertyGetInvocation => $"return {Target}.{MethodName};";
        private string PropertySetInvocation => $"{Target}.{MethodName} = {LastParameter}; return null;";

        private string IndexerPropertyGetInvocation => $"return {Target}[{Parameters}];";
        private string IndexerPropertySetInvocation => $"{Target}[{ParametersExceptLast}] = {LastParameter}; return null;";
        
        private string NotSupportedInvocation => $"throw new {typeof(NotSupportedException).ToCSharpString()}(\"Cannot optimize methods that use ref struct types such as Span<T>, Memory<T> etc.\");";

        private string ReflectedType => TypeName(method.ReflectedType);
        private string Target => method.IsStatic ? method.ReflectedType.ToCSharpString() : $"(({method.ReflectedType.ToCSharpString()})target)";
        private string MethodName => method.IsConstructor ? "Constructor" : method.IsSpecialName ? method.Name.After("_") : method.Name;

        private string Parameters => RenderParameters(method.GetParameters());
        private string ParametersExceptLast => RenderParameters(method.GetParameters().Where((_, i) => i < method.GetParameters().Length - 1).ToArray());
        private string LastParameter => Parameter(method.GetParameters().LastOrDefault());

        private static string Parameter(ParameterInfo parameterInfo)
        {
            if (parameterInfo == null) { return ""; }

            return $@"(({parameterInfo.ParameterType.ToCSharpString()})(args[{parameterInfo.Position}]??default({parameterInfo.ParameterType.ToCSharpString()})))";
        }
    }
}