using System.Collections;
using System;

namespace Routine.Engine.Reflection;

internal class ArrayTypeInfo : PreloadedTypeInfo
{
    private TypeInfo elementType;

    internal ArrayTypeInfo(Type type) : base(type)
    {
        IsArray = true;
    }

    protected override void Load()
    {
        base.Load();

        elementType = Get(type.GetElementType());
    }

    protected override TypeInfo GetElementType() => elementType;
    public override object CreateInstance() => CreateListInstance(0);
    public override IList CreateListInstance(int length) => Array.CreateInstance(elementType.GetActualType(), length);
}
