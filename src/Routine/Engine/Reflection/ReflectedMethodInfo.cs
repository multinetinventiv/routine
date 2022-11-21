using Routine.Core.Reflection;

namespace Routine.Engine.Reflection;

public class ReflectedMethodInfo : MethodInfo
{
    internal ReflectedMethodInfo(System.Reflection.MethodInfo methodInfo)
        : base(methodInfo) { }

    protected override MethodInfo Load() => this;

    public override ParameterInfo[] GetParameters() => _methodInfo.GetParameters().Select(ParameterInfo.Reflected).ToArray();
    public override object[] GetCustomAttributes() => _methodInfo.GetCustomAttributes(true);
    public override object[] GetReturnTypeCustomAttributes() => _methodInfo.ReturnTypeCustomAttributes.GetCustomAttributes(true);

    public override TypeInfo GetFirstDeclaringType() => SearchFirstDeclaringType();

    public override object Invoke(object target, params object[] parameters) => new ReflectionMethodInvoker(_methodInfo).Invoke(target, parameters);
    public override async Task<object> InvokeAsync(object target, params object[] parameters) => await new ReflectionMethodInvoker(_methodInfo).InvokeAsync(target, parameters);
    public override object InvokeStatic(params object[] parameters) => new ReflectionMethodInvoker(_methodInfo).Invoke(null, parameters);
    public override async Task<object> InvokeStaticAsync(params object[] parameters) => await new ReflectionMethodInvoker(_methodInfo).InvokeAsync(null, parameters);

    public override string Name => _methodInfo.Name;
    public override bool IsPublic => _methodInfo.IsPublic;
    public override bool IsStatic => _methodInfo.IsStatic;

    public override TypeInfo DeclaringType => TypeInfo.Get(_methodInfo.DeclaringType);
    public override TypeInfo ReflectedType => TypeInfo.Get(_methodInfo.ReflectedType);
    public override TypeInfo ReturnType => TypeInfo.Get(IgnoreTask(_methodInfo.ReturnType));
}
