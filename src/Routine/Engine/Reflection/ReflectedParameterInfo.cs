namespace Routine.Engine.Reflection;

public class ReflectedParameterInfo : ParameterInfo
{
    internal ReflectedParameterInfo(System.Reflection.ParameterInfo parameterInfo)
        : base(parameterInfo) { }

    protected override ParameterInfo Load() => this;

    public override MemberInfo Member => MemberInfo.Reflected(parameterInfo.Member);
    public override string Name => parameterInfo.Name;
    public override TypeInfo ParameterType => TypeInfo.Get(parameterInfo.ParameterType);
    public override bool IsOptional => parameterInfo.IsOptional;
    public override bool HasDefaultValue => parameterInfo.HasDefaultValue;
    public override object DefaultValue => parameterInfo.DefaultValue;
    public override int Position => parameterInfo.Position;
    public override object[] GetCustomAttributes() => parameterInfo.GetCustomAttributes(true);
}
