namespace Routine.Engine.Reflection;

public abstract class PreloadedTypeInfo : BaseTypeInfo
{
    private string _name;
    private string _fullName;
    private string @_namespace;
    private TypeInfo _baseType;
    private TypeInfo[] _genericArguments;
    private TypeInfo[] _interfaces;
    private TypeInfo[] _assignableTypes;
    private object[] _customAttributes;

    public override string Name => _name;
    public override string FullName => _fullName;
    public override string Namespace => @_namespace;
    public override TypeInfo BaseType => _baseType;

    protected PreloadedTypeInfo(Type type)
        : base(type) { }

    protected internal override void Load()
    {
        _name = _type.Name;
        _fullName = _type.FullName;
        @_namespace = _type.Namespace;
        _baseType = Get(_type.BaseType);

        _genericArguments = _type.GetGenericArguments().Select(Get).ToArray();
        _interfaces = _type.GetInterfaces().Select(Get).ToArray();
        _assignableTypes = base.GetAssignableTypes();

        _customAttributes = _type.GetCustomAttributes(true);
    }

    protected internal override TypeInfo[] GetAssignableTypes() => _assignableTypes;

    protected internal override TypeInfo[] GetGenericArguments() => _genericArguments;
    protected internal override TypeInfo[] GetInterfaces() => _interfaces;
    public override bool CanBe(TypeInfo other) => _assignableTypes.Any(t => t == other);
    protected internal override TypeInfo GetElementType() => null;
    public override ConstructorInfo[] GetAllConstructors() => Array.Empty<ConstructorInfo>();
    public override PropertyInfo[] GetAllProperties() => Array.Empty<PropertyInfo>();
    public override PropertyInfo[] GetAllStaticProperties() => Array.Empty<PropertyInfo>();
    public override MethodInfo[] GetAllMethods() => Array.Empty<MethodInfo>();
    public override MethodInfo[] GetAllStaticMethods() => Array.Empty<MethodInfo>();
    public override object[] GetCustomAttributes() => _customAttributes;

    protected internal override MethodInfo GetParseMethod() => null;

    public override object CreateInstance() => Activator.CreateInstance(_type);
    public override IList CreateListInstance(int length) => (IList)Activator.CreateInstance(_type, length);
}
