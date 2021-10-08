using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Routine.Core.Reflection
{
    public class OptimizedMethodInvokerTemplate
    {
        private const string NOT_SUPPORTED_INVOCATION = @"
throw new System.NotSupportedException(""Cannot optimize methods that use ref struct types such as Span<T>, Memory<T> etc."");
";

        private const string VOID_INVOCATION = @"
$Target$.$MethodName$($Parameters$);
return null;
";

        private const string NON_VOID_INVOCATION = @"
return $Target$.$MethodName$($Parameters$);
";

        private const string PROPERTY_GET_INVOCATION = @"
return $Target$.$MethodName$;
";

        private const string INDEXER_PROPERTY_GET_INVOCATION = @"
return $Target$[$Parameters$];
";

        private const string PROPERTY_SET_INVOCATION = @"
$Target$.$MethodName$ = $LastParameter$;
return null;
";

        private const string INDEXER_PROPERTY_SET_INVOCATION = @"
$Target$[$ParametersExceptLast$] = $LastParameter$;
return null;
";

        private const string NEW_INVOCATION = @"
return new $ReflectedType$($Parameters$);
";

        private const string PARAMETER = @"
(($ParameterType$)(args[$ParameterIndex$]??default($ParameterType$)))
";
        

        private static string Parameter(ParameterInfo parameterInfo)
        {
            if (parameterInfo == null) { return ""; }

            return PARAMETER
                    .Replace("$ParameterType$", parameterInfo.ParameterType.ToCSharpString())
                    .Replace("$ParameterIndex$", parameterInfo.Position.ToString());
        }

        private static string Parameters(ParameterInfo[] parameters) => string.Join(",", parameters.Select(Parameter));

        private static string Invocation(MethodBase method)
        {
            var parameters = method.GetParameters();
            var lastParameter = parameters.LastOrDefault();
            var parametersExceptLast = parameters.Where((_, i) => i < parameters.Length - 1).ToArray();

            string result;

            if (method.IsConstructor)
            {
                result = method.GetParameters().Any(p => p.ParameterType.IsByRefLike) ? NOT_SUPPORTED_INVOCATION : NEW_INVOCATION;
            }
            else if (method.IsSpecialName)
            {
                var methodInfo = (MethodInfo)method;
                if (methodInfo.ReturnType.IsByRefLike || methodInfo.GetParameters().Any(p => p.ParameterType.IsByRefLike)) { result = NOT_SUPPORTED_INVOCATION; }
                else if (methodInfo.Name.StartsWith("get_"))
                {
                    result = methodInfo.GetParameters().Any() ? INDEXER_PROPERTY_GET_INVOCATION : PROPERTY_GET_INVOCATION;
                }
                else
                {
                    result = methodInfo.GetParameters().Length > 1 ? INDEXER_PROPERTY_SET_INVOCATION : PROPERTY_SET_INVOCATION;
                }
            }
            else
            {
                var methodInfo = (MethodInfo)method;
                if (methodInfo.ReturnType.IsByRefLike || methodInfo.GetParameters().Any(p => p.ParameterType.IsByRefLike)) { result = NOT_SUPPORTED_INVOCATION; }
                else if (methodInfo.ReturnType == typeof(void)) { result = VOID_INVOCATION; }
                else { result = NON_VOID_INVOCATION; }
            }

            result = result
                .Replace("$Target$", method.IsStatic ? "$ReflectedType$" : "(($ReflectedType$)target)")
                .Replace("$ReflectedType$", method.ReflectedType.ToCSharpString())
                .Replace("$Parameters$", Parameters(parameters))
                .Replace("$LastParameter$", Parameter(lastParameter))
                .Replace("$ParametersExceptLast$", Parameters(parametersExceptLast))
                .Replace("$MethodName$", MethodName(method))
            ;

            return result;
        }

        private static string TypeName(Type type) => type.ToCSharpString().AfterLast(".").Replace("<", "_").Replace(">", "_");

        private static string MethodName(MethodBase method)
        {
            if (method.IsConstructor)
            {
                return "Constructor";
            }

            if (method.IsSpecialName)
            {
                return method.Name.After("_");
            }

            return method.Name;
        }

        public static string Render(MethodBase method) => @$"
namespace {method.ReflectedType?.Namespace}
{{
    public class {TypeName(method.ReflectedType)}_{MethodName(method)}_Invoker_{method.GetHashCode()} : {typeof(IMethodInvoker).ToCSharpString()}
    {{
        public object Invoke(object target, params object[] args)
        {{
            {Invocation(method)}
        }}

        public async {typeof(Task<object>).ToCSharpString()} InvokeAsync(object target, params object[] args)
        {{
            throw new System.NotImplementedException();
        }}
    }}
}}
";

        public static string GetInvokerTypeName(MethodBase current) =>
            $"{current.ReflectedType?.Namespace}.{TypeName(current.ReflectedType)}_{MethodName(current)}_Invoker_{current.GetHashCode()}";
    }
}