namespace Routine.Engine.Reflection;

internal class ProxyTypeInfo : TypeInfo
{
    private volatile TypeInfo _real;

    internal TypeInfo Real
    {
        get => _real;
        set => _real = value ?? throw new ArgumentNullException(nameof(_real));
    }

    internal ProxyTypeInfo(TypeInfo real)
    {
        Real = real;
    }

    public override bool IsPublic => _real.IsPublic;
    public override bool IsAbstract => _real.IsAbstract;
    public override bool IsInterface => _real.IsInterface;
    public override bool IsValueType => _real.IsValueType;
    public override bool IsGenericType => _real.IsGenericType;
    public override bool IsPrimitive => _real.IsPrimitive;
    public override bool IsVoid => _real.IsVoid;
    public override bool IsEnum => _real.IsEnum;
    public override bool IsArray => _real.IsArray;
    public override string Name => _real.Name;
    public override string FullName => _real.FullName;
    public override string Namespace => _real.Namespace;
    public override TypeInfo BaseType => _real.BaseType;

    public override bool CanBe(TypeInfo other) => _real.CanBe(other);
    public override object CreateInstance() => _real.CreateInstance();
    public override IList CreateListInstance(int length) => _real.CreateListInstance(length);
    public override Type GetActualType() => _real.GetActualType();
    public override ConstructorInfo[] GetAllConstructors() => _real.GetAllConstructors();
    public override MethodInfo[] GetAllMethods() => _real.GetAllMethods();
    public override PropertyInfo[] GetAllProperties() => _real.GetAllProperties();
    public override MethodInfo[] GetAllStaticMethods() => _real.GetAllStaticMethods();
    public override PropertyInfo[] GetAllStaticProperties() => _real.GetAllStaticProperties();
    public override ConstructorInfo GetConstructor(params TypeInfo[] typeInfos) => _real.GetConstructor(typeInfos);
    public override object[] GetCustomAttributes() => _real.GetCustomAttributes();
    public override MethodInfo GetMethod(string name) => _real.GetMethod(name);
    public override List<MethodInfo> GetMethods(string name) => _real.GetMethods(name);
    public override List<PropertyInfo> GetProperties(string name) => _real.GetProperties(name);
    public override PropertyInfo GetProperty(string name) => _real.GetProperty(name);
    public override List<ConstructorInfo> GetPublicConstructors() => _real.GetPublicConstructors();
    public override ICollection<MethodInfo> GetPublicMethods() => _real.GetPublicMethods();
    public override ICollection<PropertyInfo> GetPublicProperties(bool onlyPublicReadableAndWritables = false) => _real.GetPublicProperties(onlyPublicReadableAndWritables);
    public override ICollection<MethodInfo> GetPublicStaticMethods() => _real.GetPublicStaticMethods();
    public override ICollection<PropertyInfo> GetPublicStaticProperties(bool onlyPublicReadableAndWritables = false) => _real.GetPublicStaticProperties(onlyPublicReadableAndWritables);
    public override MethodInfo GetStaticMethod(string name) => _real.GetStaticMethod(name);
    public override List<MethodInfo> GetStaticMethods(string name) => _real.GetStaticMethods(name);
    public override List<PropertyInfo> GetStaticProperties(string name) => _real.GetStaticProperties(name);
    public override PropertyInfo GetStaticProperty(string name) => _real.GetStaticProperty(name);
    protected internal override TypeInfo[] GetAssignableTypes() => _real.GetAssignableTypes();
    protected internal override TypeInfo GetElementType() => _real.GetElementType();
    public override List<string> GetEnumNames() => _real.GetEnumNames();
    protected internal override TypeInfo GetEnumUnderlyingType() => _real.GetEnumUnderlyingType();
    public override List<object> GetEnumValues() => _real.GetEnumValues();
    protected internal override TypeInfo[] GetGenericArguments() => _real.GetGenericArguments();
    protected internal override TypeInfo[] GetInterfaces() => _real.GetInterfaces();
    protected internal override MethodInfo GetParseMethod() => _real.GetParseMethod();
    protected internal override void Load() => _real.Load();
}
