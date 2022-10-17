namespace Routine.Engine.Reflection;

internal class EnumTypeInfo : PreloadedTypeInfo
{
    private readonly List<string> enumNames;
    private readonly List<object> enumValues;
    private readonly TypeInfo enumUnderlyingType;

    internal EnumTypeInfo(Type type)
        : base(type)
    {
        SetIsEnum(true);

        enumNames = type.GetEnumNames().ToList();
        enumValues = type.GetEnumValues().Cast<object>().ToList();
        enumUnderlyingType = Get(type.GetEnumUnderlyingType());
    }

    public override List<string> GetEnumNames() => enumNames;
    public override List<object> GetEnumValues() => enumValues;
    protected internal override TypeInfo GetEnumUnderlyingType() => enumUnderlyingType;
}
