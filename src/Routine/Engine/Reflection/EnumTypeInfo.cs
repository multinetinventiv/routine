namespace Routine.Engine.Reflection;

internal class EnumTypeInfo : PreloadedTypeInfo
{
    private readonly List<string> _enumNames;
    private readonly List<object> _enumValues;
    private readonly TypeInfo _enumUnderlyingType;

    internal EnumTypeInfo(Type type)
        : base(type)
    {
        SetIsEnum(true);

        _enumNames = type.GetEnumNames().ToList();
        _enumValues = type.GetEnumValues().Cast<object>().ToList();
        _enumUnderlyingType = Get(type.GetEnumUnderlyingType());
    }

    public override List<string> GetEnumNames() => _enumNames;
    public override List<object> GetEnumValues() => _enumValues;
    protected internal override TypeInfo GetEnumUnderlyingType() => _enumUnderlyingType;
}
