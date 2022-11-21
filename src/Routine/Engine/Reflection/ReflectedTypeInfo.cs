namespace Routine.Engine.Reflection;

internal class ReflectedTypeInfo : BaseTypeInfo
{
    internal ReflectedTypeInfo(Type type)
        : base(type) { }

    public override ConstructorInfo[] GetAllConstructors() => _type.GetConstructors(ALL_INSTANCE).Select(ConstructorInfo.Reflected).ToArray();
    public override PropertyInfo[] GetAllProperties() => _type.GetProperties(ALL_INSTANCE).Select(PropertyInfo.Reflected).ToArray();
    public override PropertyInfo[] GetAllStaticProperties() => _type.GetProperties(ALL_STATIC).Select(PropertyInfo.Reflected).ToArray();
    public override MethodInfo[] GetAllMethods() => _type.GetMethods(ALL_INSTANCE).Where(m => !m.IsSpecialName).Select(MethodInfo.Reflected).ToArray();
    public override MethodInfo[] GetAllStaticMethods() => _type.GetMethods(ALL_STATIC).Where(m => !m.IsSpecialName).Select(MethodInfo.Reflected).ToArray();
    public override object[] GetCustomAttributes() => _type.GetCustomAttributes(true);
    protected internal override TypeInfo[] GetGenericArguments() => _type.GetGenericArguments().Select(Get).ToArray();
    protected internal override TypeInfo GetElementType() => Get(_type.GetElementType());
    protected internal override TypeInfo[] GetInterfaces() => _type.GetInterfaces().Select(Get).ToArray();
    public override bool CanBe(TypeInfo other) => other.GetActualType().IsAssignableFrom(_type);
    protected internal override MethodInfo GetParseMethod() => null;

    protected internal override void Load() { }

    public override string Name => _type.Name;
    public override string FullName => _type.FullName;
    public override string Namespace => _type.Namespace;
    public override TypeInfo BaseType => Get(_type.BaseType);

    public override object CreateInstance() => Activator.CreateInstance(_type);
    public override IList CreateListInstance(int length) => (IList)Activator.CreateInstance(_type, length);
}
