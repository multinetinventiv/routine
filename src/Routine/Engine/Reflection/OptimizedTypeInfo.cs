using Routine.Core.Reflection;

namespace Routine.Engine.Reflection;

internal class OptimizedTypeInfo : PreloadedTypeInfo
{
    private IMethodInvoker _defaultConstructorInvoker;
    private IMethodInvoker _listConstructorInvoker;

    private ConstructorInfo[] _allConstructors;
    private PropertyInfo[] _allProperties;
    private PropertyInfo[] _allStaticProperties;
    private MethodInfo[] _allMethods;
    private MethodInfo[] _allStaticMethods;
    private MethodInfo _parseMethod;

    private MemberIndex<string, PropertyInfo> _allPropertiesNameIndex;
    private MemberIndex<string, PropertyInfo> _allStaticPropertiesNameIndex;
    private MemberIndex<string, MethodInfo> _allMethodsNameIndex;
    private MemberIndex<string, MethodInfo> _allStaticMethodsNameIndex;

    internal OptimizedTypeInfo(Type type)
        : base(type) { }

    protected internal override void Load()
    {
        base.Load();

        if (!_type.IsAbstract)
        {
            var defaultConstructor = _type.GetConstructor(Type.EmptyTypes);
            if (defaultConstructor != null)
            {
                _defaultConstructorInvoker = defaultConstructor.CreateInvoker();
            }

            var listConstructor = _type.GetConstructor(new[] { typeof(int) });
            if (listConstructor != null)
            {
                _listConstructorInvoker = listConstructor.CreateInvoker();
            }
        }

        _allConstructors = _type.GetConstructors(ALL_INSTANCE).Select(ConstructorInfo.Preloaded).ToArray();

        _allProperties = _type.GetProperties(ALL_INSTANCE).Select(PropertyInfo.Preloaded).ToArray();
        _allPropertiesNameIndex = MemberIndex.Build(_allProperties, p => p.Name);

        _allStaticProperties = _type.GetProperties(ALL_STATIC).Select(PropertyInfo.Preloaded).ToArray();
        _allStaticPropertiesNameIndex = MemberIndex.Build(_allStaticProperties, p => p.Name);

        _allMethods = _type.GetMethods(ALL_INSTANCE).Where(m => !m.IsSpecialName).Select(MethodInfo.Preloaded).ToArray();
        _allMethodsNameIndex = MemberIndex.Build(_allMethods, m => m.Name);

        _allStaticMethods = _type.GetMethods(ALL_STATIC).Where(m => !m.IsSpecialName).Select(MethodInfo.Preloaded).ToArray();
        _allStaticMethodsNameIndex = MemberIndex.Build(_allStaticMethods, m => m.Name);

        _parseMethod = _allStaticMethods.SingleOrDefault(m => m.HasParameters<string>() && m.Returns(this, "Parse"));
    }

    public override ConstructorInfo[] GetAllConstructors() => _allConstructors;
    public override PropertyInfo[] GetAllProperties() => _allProperties;
    public override PropertyInfo[] GetAllStaticProperties() => _allStaticProperties;
    public override MethodInfo[] GetAllMethods() => _allMethods;
    public override MethodInfo[] GetAllStaticMethods() => _allStaticMethods;
    protected internal override MethodInfo GetParseMethod() => _parseMethod;

    public override PropertyInfo GetProperty(string name) => _allPropertiesNameIndex.GetFirstOrDefault(name);
    public override List<PropertyInfo> GetProperties(string name) => _allPropertiesNameIndex.GetAll(name);
    public override PropertyInfo GetStaticProperty(string name) => _allStaticPropertiesNameIndex.GetFirstOrDefault(name);
    public override List<PropertyInfo> GetStaticProperties(string name) => _allStaticPropertiesNameIndex.GetAll(name);
    public override MethodInfo GetMethod(string name) => _allMethodsNameIndex.GetFirstOrDefault(name);
    public override List<MethodInfo> GetMethods(string name) => _allMethodsNameIndex.GetAll(name);
    public override MethodInfo GetStaticMethod(string name) => _allStaticMethodsNameIndex.GetFirstOrDefault(name);
    public override List<MethodInfo> GetStaticMethods(string name) => _allStaticMethodsNameIndex.GetAll(name);

    public override object CreateInstance()
    {
        if (_defaultConstructorInvoker == null)
        {
            throw new MissingMethodException("Default constructor not found!");
        }

        return _defaultConstructorInvoker.Invoke(null);
    }

    public override IList CreateListInstance(int length)
    {
        if (_listConstructorInvoker == null)
        {
            throw new MissingMethodException("List constructor not found!");
        }

        return (IList)_listConstructorInvoker.Invoke(null, length);
    }
}
