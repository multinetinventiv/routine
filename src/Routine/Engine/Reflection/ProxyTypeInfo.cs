namespace Routine.Engine.Reflection;

internal class ProxyTypeInfo : TypeInfo
{
    private volatile TypeInfo real;

    internal TypeInfo Real
    {
        get => real;
        set => real = value ?? throw new ArgumentNullException(nameof(real));
    }

    internal ProxyTypeInfo(TypeInfo real)
    {
        Real = real;
    }

    public override bool IsPublic => real.IsPublic;
    public override bool IsAbstract => real.IsAbstract;
    public override bool IsInterface => real.IsInterface;
    public override bool IsValueType => real.IsValueType;
    public override bool IsGenericType => real.IsGenericType;
    public override bool IsPrimitive => real.IsPrimitive;
    public override bool IsVoid => real.IsVoid;
    public override bool IsEnum => real.IsEnum;
    public override bool IsArray => real.IsArray;
    public override string Name => real.Name;
    public override string FullName => real.FullName;
    public override string Namespace => real.Namespace;
    public override TypeInfo BaseType => real.BaseType;

    public override bool CanBe(TypeInfo other) => real.CanBe(other);
    public override object CreateInstance() => real.CreateInstance();
    public override IList CreateListInstance(int length) => real.CreateListInstance(length);
    public override Type GetActualType() => real.GetActualType();
    public override ConstructorInfo[] GetAllConstructors() => real.GetAllConstructors();
    public override MethodInfo[] GetAllMethods() => real.GetAllMethods();
    public override PropertyInfo[] GetAllProperties() => real.GetAllProperties();
    public override MethodInfo[] GetAllStaticMethods() => real.GetAllStaticMethods();
    public override PropertyInfo[] GetAllStaticProperties() => real.GetAllStaticProperties();
    public override ConstructorInfo GetConstructor(params TypeInfo[] typeInfos) => real.GetConstructor(typeInfos);
    public override object[] GetCustomAttributes() => real.GetCustomAttributes();
    public override MethodInfo GetMethod(string name) => real.GetMethod(name);
    public override List<MethodInfo> GetMethods(string name) => real.GetMethods(name);
    public override List<PropertyInfo> GetProperties(string name) => real.GetProperties(name);
    public override PropertyInfo GetProperty(string name) => real.GetProperty(name);
    public override List<ConstructorInfo> GetPublicConstructors() => real.GetPublicConstructors();
    public override ICollection<MethodInfo> GetPublicMethods() => real.GetPublicMethods();
    public override ICollection<PropertyInfo> GetPublicProperties(bool onlyPublicReadableAndWritables = false) => real.GetPublicProperties(onlyPublicReadableAndWritables);
    public override ICollection<MethodInfo> GetPublicStaticMethods() => real.GetPublicStaticMethods();
    public override ICollection<PropertyInfo> GetPublicStaticProperties(bool onlyPublicReadableAndWritables = false) => real.GetPublicStaticProperties(onlyPublicReadableAndWritables);
    public override MethodInfo GetStaticMethod(string name) => real.GetStaticMethod(name);
    public override List<MethodInfo> GetStaticMethods(string name) => real.GetStaticMethods(name);
    public override List<PropertyInfo> GetStaticProperties(string name) => real.GetStaticProperties(name);
    public override PropertyInfo GetStaticProperty(string name) => real.GetStaticProperty(name);
    protected internal override TypeInfo[] GetAssignableTypes() => real.GetAssignableTypes();
    protected internal override TypeInfo GetElementType() => real.GetElementType();
    public override List<string> GetEnumNames() => real.GetEnumNames();
    protected internal override TypeInfo GetEnumUnderlyingType() => real.GetEnumUnderlyingType();
    public override List<object> GetEnumValues() => real.GetEnumValues();
    protected internal override TypeInfo[] GetGenericArguments() => real.GetGenericArguments();
    protected internal override TypeInfo[] GetInterfaces() => real.GetInterfaces();
    protected internal override MethodInfo GetParseMethod() => real.GetParseMethod();
    protected internal override void Load() => real.Load();
}
