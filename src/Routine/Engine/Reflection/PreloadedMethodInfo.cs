using Routine.Core.Reflection;

namespace Routine.Engine.Reflection;

public class PreloadedMethodInfo : MethodInfo
{
    private string _name;
    private bool _isPublic;
    private bool _isStatic;
    private TypeInfo _declaringType;
    private TypeInfo _reflectedType;
    private TypeInfo _returnType;
    private ParameterInfo[] _parameters;
    private TypeInfo _firstDeclaringType;
    private object[] _customAttributes;
    private object[] _returnTypeCustomAttributes;

    private IMethodInvoker _invoker;

    internal PreloadedMethodInfo(System.Reflection.MethodInfo methodInfo)
        : base(methodInfo) { }

    protected override MethodInfo Load()
    {
        _name = _methodInfo.Name;
        _isPublic = _methodInfo.IsPublic;
        _isStatic = _methodInfo.IsStatic;
        _declaringType = TypeInfo.Get(_methodInfo.DeclaringType);
        _reflectedType = TypeInfo.Get(_methodInfo.ReflectedType);
        _returnType = TypeInfo.Get(IgnoreTask(_methodInfo.ReturnType));
        _parameters = _methodInfo.GetParameters().Select(p => ParameterInfo.Preloaded(this, p)).ToArray();
        _firstDeclaringType = SearchFirstDeclaringType();
        _customAttributes = _methodInfo.GetCustomAttributes(true);
        _returnTypeCustomAttributes = _methodInfo.ReturnTypeCustomAttributes.GetCustomAttributes(true);

        _invoker = _methodInfo.CreateInvoker();

        return this;
    }

    public override string Name => _name;
    public override bool IsPublic => _isPublic;
    public override bool IsStatic => _isStatic;
    public override TypeInfo DeclaringType => _declaringType;
    public override TypeInfo ReflectedType => _reflectedType;
    public override TypeInfo ReturnType => _returnType;

    public override ParameterInfo[] GetParameters() => _parameters;
    public override object[] GetCustomAttributes() => _customAttributes;
    public override object[] GetReturnTypeCustomAttributes() => _returnTypeCustomAttributes;

    public override object Invoke(object target, params object[] parameters) => _invoker.Invoke(target, parameters);
    public override async Task<object> InvokeAsync(object target, params object[] parameters) => await _invoker.InvokeAsync(target, parameters);

    public override object InvokeStatic(params object[] parameters) => _invoker.Invoke(null, parameters);
    public override async Task<object> InvokeStaticAsync(params object[] parameters) => await _invoker.InvokeAsync(null, parameters);

    public override TypeInfo GetFirstDeclaringType() => _firstDeclaringType;
}
