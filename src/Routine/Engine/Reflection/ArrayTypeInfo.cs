namespace Routine.Engine.Reflection;

internal class ArrayTypeInfo : PreloadedTypeInfo
{
    private TypeInfo _elementType;

    internal ArrayTypeInfo(Type type) : base(type)
    {
        SetIsArray(true);
    }

    protected internal override void Load()
    {
        base.Load();

        _elementType = Get(_type.GetElementType());
    }

    protected internal override TypeInfo GetElementType() => _elementType;
    public override object CreateInstance() => CreateListInstance(0);
    public override IList CreateListInstance(int length) => Array.CreateInstance(_elementType.GetActualType(), length);
}
