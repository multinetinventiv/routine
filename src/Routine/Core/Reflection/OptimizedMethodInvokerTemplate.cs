using System.Reflection;

namespace Routine.Core.Reflection;

public class OptimizedMethodInvokerTemplate
{
    private readonly MethodBase _method;
    private readonly InvocationType _invocationType;

    public OptimizedMethodInvokerTemplate(MethodBase method)
    {
        _method = method;

        _invocationType = ResolveInvocationType();
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

    private string Namespace => _method.ReflectedType?.Namespace;
    private string Name => $"{Fix(NameOf(_method.ReflectedType))}_{MethodName}_Invoker_{_method.GetHashCode()}";

    private string Invocation => _invocationType switch
    {
        InvocationType.NotSupported => $"throw new {NameOf<NotSupportedException>()}(\"Cannot optimize methods that use ref struct types such as Span<T>, Memory<T> etc.\");",
        InvocationType.Constructor => $"return new {NameOf(_method.ReflectedType)}({Parameters});",
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
        _ => throw new NotSupportedException($"Cannot render an optimized method invoker for method: {_method}")
    };

    private string AsyncInvocation => _invocationType switch
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
        if (_method.GetParameters().Any(p => p.ParameterType.IsByRefLike) ||
            _method is MethodInfo mi && mi.ReturnType.IsByRefLike)
        {
            return InvocationType.NotSupported;
        }

        if (_method.IsConstructor) { return InvocationType.Constructor; }

        if (_method.IsSpecialName)
        {
            if (_method.Name.StartsWith("get_"))
            {
                return _method.GetParameters().Any() ? InvocationType.IndexerGet : InvocationType.Get;
            }

            if (_method.Name.StartsWith("set_"))
            {
                return _method.GetParameters().Length > 1 ? InvocationType.IndexerSet : InvocationType.Set;
            }
        }

        if (_method is not MethodInfo methodInfo) { return InvocationType.NotSupported; }

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

    private string Target => _method.IsStatic ? NameOf(_method.ReflectedType) : $"(({NameOf(_method.ReflectedType)})target)";
    private string MethodName => _method.IsConstructor ? "Constructor" : _method.IsSpecialName ? _method.Name.After("_") : _method.Name;

    private string Parameters => RenderParameters(_method.GetParameters());
    private string ParametersExceptLast => RenderParameters(_method.GetParameters().Where((_, i) => i < _method.GetParameters().Length - 1).ToArray());
    private string LastParameter => RenderParameter(_method.GetParameters().LastOrDefault());

    private static string RenderParameters(IEnumerable<ParameterInfo> parameters) => string.Join(",", parameters.Select(RenderParameter));
    private static string RenderParameter(ParameterInfo parameterInfo) =>
        parameterInfo == null
        ? ""
        : $"(({NameOf(parameterInfo.ParameterType)})(args[{parameterInfo.Position}]??default({NameOf(parameterInfo.ParameterType)})))";

    private static string Fix(string typeName) => typeName.AfterLast(".").Replace("<", "_").Replace(">", "_");
    private static string NameOf<T>() => NameOf(typeof(T));
    private static string NameOf(Type type) => type.ToCSharpString();
}
