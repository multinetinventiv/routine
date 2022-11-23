namespace Routine.Engine.Reflection;

public class ReflectedParameterInfo : ParameterInfo
{
    internal ReflectedParameterInfo(System.Reflection.ParameterInfo parameterInfo)
        : base(parameterInfo) { }

    protected override ParameterInfo Load() => this;

    public override MemberInfo Member => MemberInfo.Reflected(_parameterInfo.Member);
    public override string Name => _parameterInfo.Name;
    public override TypeInfo ParameterType => TypeInfo.Get(_parameterInfo.ParameterType);
    public override bool IsOptional => _parameterInfo.IsOptional;
    public override bool HasDefaultValue => _parameterInfo.HasDefaultValue;
    public override object DefaultValue => _parameterInfo.DefaultValue;
    public override int Position => _parameterInfo.Position;
    public override object[] GetCustomAttributes() => _parameterInfo.GetCustomAttributes(true);
}
