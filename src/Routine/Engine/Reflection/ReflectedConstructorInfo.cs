using Routine.Core.Reflection;

namespace Routine.Engine.Reflection;

public class ReflectedConstructorInfo : ConstructorInfo
{
    internal ReflectedConstructorInfo(System.Reflection.ConstructorInfo constructorInfo)
        : base(constructorInfo) { }

    protected override ConstructorInfo Load() => this;
    public override ParameterInfo[] GetParameters() => _constructorInfo.GetParameters().Select(ParameterInfo.Reflected).ToArray();
    public override object[] GetCustomAttributes() => _constructorInfo.GetCustomAttributes(true);

    public override object Invoke(params object[] parameters) => new ReflectionMethodInvoker(_constructorInfo).Invoke(null, parameters);

    public override bool IsPublic => _constructorInfo.IsPublic;
    public override TypeInfo DeclaringType => TypeInfo.Get(_constructorInfo.DeclaringType);
    public override TypeInfo ReflectedType => TypeInfo.Get(_constructorInfo.ReflectedType);
}
