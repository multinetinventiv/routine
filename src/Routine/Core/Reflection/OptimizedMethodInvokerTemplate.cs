using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System;

namespace Routine.Core.Reflection;

public class OptimizedMethodInvokerTemplate
{
    private readonly MethodBase method;
    private readonly InvocationType invocationType;

    public OptimizedMethodInvokerTemplate(MethodBase method)
    {
        this.method = method;

        invocationType = ResolveInvocationType();
    }

    public string InvokerTypeName => Name;
    public string Render() => $@"
namespace {Namespace}
{{
    public class {Name} : {NameOf<IMethodInvoker>()}
    {{
        public object Invoke(object target, params object[] args)
        {{
            {Invocation}
        }}

        public async {NameOf<Task<object>>()} InvokeAsync(object target, params object[] args)
        {{
            {AsyncInvocation}
        }}
    }}
}}
";

    private string Namespace => method.ReflectedType?.Namespace;
    private string Name => $"{Fix(NameOf(method.ReflectedType))}_{MethodName}_Invoker_{method.GetHashCode()}";

    private string Invocation => invocationType switch
    {
        InvocationType.NotSupported => $"throw new {NameOf<NotSupportedException>()}(\"Cannot optimize methods that use ref struct types such as Span<T>, Memory<T> etc.\");",
        InvocationType.Constructor => $"return new {NameOf(method.ReflectedType)}({Parameters});",
        InvocationType.Get => $"return {Target}.{MethodName};",
        InvocationType.Set => $@"
            {Target}.{MethodName} = {LastParameter};
            return null;
",
        InvocationType.IndexerGet => $"return {Target}[{Parameters}];",
        InvocationType.IndexerSet => $@"
            {Target}[{ParametersExceptLast}] = {LastParameter};
            return null;
",
        InvocationType.ReturnsVoid => $@"
            {Target}.{MethodName}({Parameters});
            return null;
",
        InvocationType.ReturnsVoidAsync => $@"
            var task = {Target}.{MethodName}({Parameters});
            try
            {{
                {NameOf<Task>()}.WaitAll(task);
            }}
            catch({NameOf<AggregateException>()} ex)
            {{
                if(ex.InnerException != null)
                {{
                    throw ex.InnerException;
                }}

                throw;
            }}

            return null;
",
        InvocationType.HasReturnType => $"return {Target}.{MethodName}({Parameters});",
        InvocationType.HasReturnTypeAsync => $@"
            var task = {Target}.{MethodName}({Parameters});
            try
            {{
                {NameOf<Task>()}.WaitAll(task);
            }}
            catch({NameOf<AggregateException>()} ex)
            {{
                if(ex.InnerException != null)
                {{
                    throw ex.InnerException;
                }}

                throw;
            }}

            return task.Result;
",
        _ => throw new NotSupportedException($"Cannot render an optimized method invoker for method: {method}")
    };

    private string AsyncInvocation => invocationType switch
    {
        InvocationType.ReturnsVoidAsync => $@"
            await {Target}.{MethodName}({Parameters});
            return null;
",
        InvocationType.HasReturnTypeAsync => $@"return await {Target}.{MethodName}({Parameters});",
        _ => Invocation
    };

    private enum InvocationType
    {
        NotSupported,
        Constructor,
        Get,
        Set,
        IndexerGet,
        IndexerSet,
        ReturnsVoid,
        ReturnsVoidAsync,
        HasReturnType,
        HasReturnTypeAsync
    }

    private InvocationType ResolveInvocationType()
    {
        if (method.GetParameters().Any(p => p.ParameterType.IsByRefLike) ||
            method is MethodInfo mi && mi.ReturnType.IsByRefLike)
        {
            return InvocationType.NotSupported;
        }

        if (method.IsConstructor) { return InvocationType.Constructor; }

        if (method.IsSpecialName)
        {
            if (method.Name.StartsWith("get_"))
            {
                return method.GetParameters().Any() ? InvocationType.IndexerGet : InvocationType.Get;
            }

            if (method.Name.StartsWith("set_"))
            {
                return method.GetParameters().Length > 1 ? InvocationType.IndexerSet : InvocationType.Set;
            }
        }

        if (method is not MethodInfo methodInfo) { return InvocationType.NotSupported; }

        if (methodInfo.ReturnType == typeof(void))
        {
            return InvocationType.ReturnsVoid;
        }

        if (methodInfo.ReturnType == typeof(Task))
        {
            return InvocationType.ReturnsVoidAsync;
        }

        if (methodInfo.ReturnType.IsGenericType &&
            methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
        {
            return InvocationType.HasReturnTypeAsync;
        }

        return InvocationType.HasReturnType;

    }

    private string Target => method.IsStatic ? NameOf(method.ReflectedType) : $"(({NameOf(method.ReflectedType)})target)";
    private string MethodName => method.IsConstructor ? "Constructor" : method.IsSpecialName ? method.Name.After("_") : method.Name;

    private string Parameters => RenderParameters(method.GetParameters());
    private string ParametersExceptLast => RenderParameters(method.GetParameters().Where((_, i) => i < method.GetParameters().Length - 1).ToArray());
    private string LastParameter => RenderParameter(method.GetParameters().LastOrDefault());

    private static string RenderParameters(IEnumerable<ParameterInfo> parameters) => string.Join(",", parameters.Select(RenderParameter));
    private static string RenderParameter(ParameterInfo parameterInfo) =>
        parameterInfo == null
        ? ""
        : $"(({NameOf(parameterInfo.ParameterType)})(args[{parameterInfo.Position}]??default({NameOf(parameterInfo.ParameterType)})))";

    private static string Fix(string typeName) => typeName.AfterLast(".").Replace("<", "_").Replace(">", "_");
    private static string NameOf<T>() => NameOf(typeof(T));
    private static string NameOf(Type type) => type.ToCSharpString();
}
