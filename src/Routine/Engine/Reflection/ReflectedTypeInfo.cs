namespace Routine.Engine.Reflection;

internal class ReflectedTypeInfo : BaseTypeInfo
{
    internal ReflectedTypeInfo(Type type)
        : base(type) { }

    public override ConstructorInfo[] GetAllConstructors() => type.GetConstructors(ALL_INSTANCE).Select(ConstructorInfo.Reflected).ToArray();
    public override PropertyInfo[] GetAllProperties() => type.GetProperties(ALL_INSTANCE).Select(PropertyInfo.Reflected).ToArray();
    public override PropertyInfo[] GetAllStaticProperties() => type.GetProperties(ALL_STATIC).Select(PropertyInfo.Reflected).ToArray();
    public override MethodInfo[] GetAllMethods() => type.GetMethods(ALL_INSTANCE).Where(m => !m.IsSpecialName).Select(MethodInfo.Reflected).ToArray();
    public override MethodInfo[] GetAllStaticMethods() => type.GetMethods(ALL_STATIC).Where(m => !m.IsSpecialName).Select(MethodInfo.Reflected).ToArray();
    public override object[] GetCustomAttributes() => type.GetCustomAttributes(true);
    protected internal override TypeInfo[] GetGenericArguments() => type.GetGenericArguments().Select(Get).ToArray();
    protected internal override TypeInfo GetElementType() => Get(type.GetElementType());
    protected internal override TypeInfo[] GetInterfaces() => type.GetInterfaces().Select(Get).ToArray();
    public override bool CanBe(TypeInfo other) => other.GetActualType().IsAssignableFrom(type);
    protected internal override MethodInfo GetParseMethod() => null;

    protected internal override void Load() { }

    public override string Name => type.Name;
    public override string FullName => type.FullName;
    public override string Namespace => type.Namespace;
    public override TypeInfo BaseType => Get(type.BaseType);

    public override object CreateInstance() => Activator.CreateInstance(type);
    public override IList CreateListInstance(int length) => (IList)Activator.CreateInstance(type, length);
}
