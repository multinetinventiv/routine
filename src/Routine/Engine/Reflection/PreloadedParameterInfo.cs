namespace Routine.Engine.Reflection;

public class PreloadedParameterInfo : ParameterInfo
{
    private readonly MemberInfo _member;

    private string _name;
    private TypeInfo _parameterType;
    private int _position;
    private bool _isOptional;
    private bool _hasDefaultValue;
    private object _defaultValue;
    private object[] _customAttributes;

    internal PreloadedParameterInfo(MemberInfo member, System.Reflection.ParameterInfo parameterInfo)
        : base(parameterInfo)
    {
        _member = member;
    }

    protected override ParameterInfo Load()
    {
        _name = _parameterInfo.Name;
        _parameterType = TypeInfo.Get(_parameterInfo.ParameterType);
        _position = _parameterInfo.Position;
        _isOptional = _parameterInfo.IsOptional;
        _hasDefaultValue = _parameterInfo.HasDefaultValue;
        _defaultValue = _parameterInfo.DefaultValue;
        _customAttributes = _parameterInfo.GetCustomAttributes(true);

        return this;
    }

    public override MemberInfo Member => _member;
    public override string Name => _name;
    public override TypeInfo ParameterType => _parameterType;
    public override int Position => _position;
    public override bool IsOptional => _isOptional;
    public override bool HasDefaultValue => _hasDefaultValue;
    public override object DefaultValue => _defaultValue;
    public override object[] GetCustomAttributes() => _customAttributes;
}
