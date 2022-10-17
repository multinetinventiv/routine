namespace Routine.Engine.Reflection;

internal class ArrayTypeInfo : PreloadedTypeInfo
{
    private TypeInfo elementType;

    internal ArrayTypeInfo(Type type) : base(type)
    {
        SetIsArray(true);
    }

    protected internal override void Load()
    {
        base.Load();

        elementType = Get(type.GetElementType());
    }

    protected internal override TypeInfo GetElementType() => elementType;
    public override object CreateInstance() => CreateListInstance(0);
    public override IList CreateListInstance(int length) => Array.CreateInstance(elementType.GetActualType(), length);
}
