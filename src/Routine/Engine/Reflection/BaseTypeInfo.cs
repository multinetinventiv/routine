namespace Routine.Engine.Reflection;

public abstract class BaseTypeInfo : TypeInfo
{
    protected readonly Type _type;

    private bool _isVoid;
    private bool _isEnum;
    private bool _isArray;

    protected BaseTypeInfo(Type type)
    {
        _type = type;

        IsPublic = type.IsPublic;
        IsAbstract = type.IsAbstract;
        IsInterface = type.IsInterface;
        IsValueType = type.IsValueType;
        IsGenericType = type.IsGenericType;
        IsPrimitive = type.IsPrimitive;
    }

    public override Type GetActualType() => _type;

    public override bool IsPublic { get; }
    public override bool IsAbstract { get; }
    public override bool IsInterface { get; }
    public override bool IsValueType { get; }
    public override bool IsGenericType { get; }
    public override bool IsPrimitive { get; }

    public override bool IsVoid => _isVoid;
    public override bool IsEnum => _isEnum;
    public override bool IsArray => _isArray;

    protected void SetIsVoid(bool isVoid) => _isVoid = isVoid;
    protected void SetIsEnum(bool isEnum) => _isEnum = isEnum;
    protected void SetIsArray(bool isArray) => _isArray = isArray;

    public override List<string> GetEnumNames() => new();
    public override List<object> GetEnumValues() => new();
    protected internal override TypeInfo GetEnumUnderlyingType() => null;

    protected internal override TypeInfo[] GetAssignableTypes()
    {
        var result = new List<TypeInfo> { Get<object>() };

        FillInheritance(this, result);

        foreach (var typeInfo in GetInterfaces())
        {
            FillInheritance(typeInfo, result);
        }

        return result.ToArray();
    }

    protected static void FillInheritance(TypeInfo root, List<TypeInfo> state)
    {
        var cur = root;
        while (cur != null)
        {
            if (!state.Contains(cur))
            {
                state.Add(cur);
            }

            cur = cur.BaseType;
        }
    }

    public override List<ConstructorInfo> GetPublicConstructors() => GetAllConstructors().Where(c => c.IsPublic).ToList();

    public override ConstructorInfo GetConstructor(params TypeInfo[] typeInfos)
    {
        if (typeInfos.Length > 0)
        {
            var first = typeInfos[0];
            var rest = Enumerable.Range(1, typeInfos.Length - 1).Select(i => (IType)typeInfos[i]).ToArray();

            return GetAllConstructors().SingleOrDefault(c => c.HasParameters(first, rest));
        }

        return GetAllConstructors().SingleOrDefault(c => c.HasNoParameters());
    }

    public override ICollection<PropertyInfo> GetPublicProperties(bool onlyPublicReadableAndWritables = false)
    {
        if (onlyPublicReadableAndWritables)
        {
            return GetAllProperties().Where(p => p.IsPubliclyReadable && p.IsPubliclyWritable).ToList();
        }

        return GetAllProperties().Where(p => p.IsPubliclyReadable).ToList();
    }

    public override ICollection<PropertyInfo> GetPublicStaticProperties(bool onlyPublicReadableAndWritables = false)
    {
        if (onlyPublicReadableAndWritables)
        {
            return GetAllStaticProperties().Where(p => p.IsPubliclyReadable && p.IsPubliclyWritable).ToList();
        }

        return GetAllStaticProperties().Where(p => p.IsPubliclyReadable).ToList();
    }

    public override PropertyInfo GetProperty(string name) => GetAllProperties().SingleOrDefault(p => p.Name == name);
    public override List<PropertyInfo> GetProperties(string name) => GetAllProperties().Where(p => p.Name == name).ToList();
    public override PropertyInfo GetStaticProperty(string name) => GetAllStaticProperties().SingleOrDefault(p => p.Name == name);
    public override List<PropertyInfo> GetStaticProperties(string name) => GetAllStaticProperties().Where(p => p.Name == name).ToList();
    public override ICollection<MethodInfo> GetPublicMethods() => GetAllMethods().Where(m => m.IsPublic).ToList();
    public override ICollection<MethodInfo> GetPublicStaticMethods() => GetAllStaticMethods().Where(m => m.IsPublic).ToList();
    public override MethodInfo GetMethod(string name) => GetAllMethods().SingleOrDefault(m => m.Name == name);
    public override List<MethodInfo> GetMethods(string name) => GetAllMethods().Where(m => m.Name == name).ToList();
    public override MethodInfo GetStaticMethod(string name) => GetAllStaticMethods().SingleOrDefault(m => m.Name == name);
    public override List<MethodInfo> GetStaticMethods(string name) => GetAllStaticMethods().Where(m => m.Name == name).ToList();
}
