using Routine.Core.Reflection;
using Routine.Engine;
using Routine.Engine.Reflection;

namespace Routine;

public abstract class TypeInfo : IType
{
    #region Factory Methods

    protected const System.Reflection.BindingFlags ALL_STATIC = System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public;
    protected const System.Reflection.BindingFlags ALL_INSTANCE = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public;

    private static readonly Dictionary<string, TypeInfo> TYPE_CACHE = new();
    private static readonly object OPTIMIZE_LOCK = new();

    private static volatile Func<Type, bool> _proxyMatcher;
    private static volatile Func<Type, Type> _actualTypeGetter;

    static TypeInfo()
    {
        SetProxyMatcher(null, null);
    }

    public static void Clear()
    {
        TYPE_CACHE.Clear();
        ReflectionOptimizer.Clear();

        SetProxyMatcher(null, null);
    }

    public static void Optimize(params Type[] newDomainTypes)
    {
        lock (OPTIMIZE_LOCK)
        {
            foreach (var newDomainType in newDomainTypes)
            {
                GetOrCreate(newDomainType, true);
            }
        }
    }

    public static void SetProxyMatcher(Func<Type, bool> proxyMatcher, Func<Type, Type> actualTypeGetter)
    {
        TypeInfo._proxyMatcher = proxyMatcher ?? (_ => false);
        TypeInfo._actualTypeGetter = actualTypeGetter ?? (t => t);
    }

    private static string KeyOf(Type type) => type.FullName ?? string.Empty;

    public static TypeInfo Void() => Get(typeof(void));
    public static TypeInfo Get<T>() => Get(typeof(T));
    public static TypeInfo Get(Type type) => GetOrCreate(type, false);
    private static TypeInfo GetOrCreate(Type type, bool optimize)
    {
        if (type == null) { return null; }

        if (!TYPE_CACHE.TryGetValue(KeyOf(type), out var result))
        {
            lock (TYPE_CACHE)
            {
                if (!TYPE_CACHE.TryGetValue(KeyOf(type), out result))
                {
                    if (_proxyMatcher(type))
                    {
                        var actualType = _actualTypeGetter(type);
                        if (!TYPE_CACHE.TryGetValue(KeyOf(actualType), out result))
                        {
                            result = CreateTypeInfo(actualType);
                            TYPE_CACHE.Add(KeyOf(actualType), result);
                            result.Load();
                        }

                        TYPE_CACHE.Add(KeyOf(type), result);
                    }
                    else
                    {
                        result = CreateTypeInfo(type);
                        TYPE_CACHE.Add(KeyOf(type), result);
                        result.Load();
                    }
                }
            }
        }

        if (optimize && result is ProxyTypeInfo proxy)
        {
            if (_proxyMatcher(type))
            {
                var actualType = _actualTypeGetter(type);
                var optimized = new OptimizedTypeInfo(actualType);
                optimized.Load();
                proxy.Real = optimized;
            }
            else
            {
                var optimized = new OptimizedTypeInfo(type);
                optimized.Load();
                proxy.Real = optimized;
            }
        }

        return result;
    }

    private static TypeInfo CreateTypeInfo(Type type) => type switch
    {
        _ when type == typeof(void) => new VoidTypeInfo(),
        _ when GetParseMethod(type)?.ReturnType == type => new ParseableTypeInfo(type),
        { IsArray: true } => new ArrayTypeInfo(type),
        { IsEnum: true } => new EnumTypeInfo(type),
        { ContainsGenericParameters: true } => new ReflectedTypeInfo(type),
        _ => new ProxyTypeInfo(new ReflectedTypeInfo(type))
    };

    private static System.Reflection.MethodInfo GetParseMethod(Type type) => type.GetMethod("Parse", new[] { typeof(string) });

    #endregion

    public abstract Type GetActualType();

    public abstract bool IsPublic { get; }
    public abstract bool IsAbstract { get; }
    public abstract bool IsInterface { get; }
    public abstract bool IsValueType { get; }
    public abstract bool IsGenericType { get; }
    public abstract bool IsPrimitive { get; }

    public abstract bool IsVoid { get; }
    public abstract bool IsEnum { get; }
    public abstract bool IsArray { get; }

    public abstract string Name { get; }
    public abstract string FullName { get; }
    public abstract string Namespace { get; }
    public abstract TypeInfo BaseType { get; }

    public abstract ConstructorInfo[] GetAllConstructors();
    public abstract PropertyInfo[] GetAllProperties();
    public abstract PropertyInfo[] GetAllStaticProperties();
    public abstract MethodInfo[] GetAllMethods();
    public abstract MethodInfo[] GetAllStaticMethods();
    public abstract object[] GetCustomAttributes();
    protected internal abstract TypeInfo[] GetGenericArguments();
    protected internal abstract TypeInfo GetElementType();
    protected internal abstract TypeInfo[] GetInterfaces();
    public abstract bool CanBe(TypeInfo other);
    public abstract List<string> GetEnumNames();
    public abstract List<object> GetEnumValues();
    protected internal abstract TypeInfo GetEnumUnderlyingType();

    protected internal abstract MethodInfo GetParseMethod();
    protected internal abstract void Load();

    public abstract object CreateInstance();
    public abstract IList CreateListInstance(int length);

    protected internal abstract TypeInfo[] GetAssignableTypes();

    public abstract List<ConstructorInfo> GetPublicConstructors();
    public abstract ConstructorInfo GetConstructor(params TypeInfo[] typeInfos);

    public abstract ICollection<PropertyInfo> GetPublicProperties(bool onlyPublicReadableAndWritables = false);
    public abstract ICollection<PropertyInfo> GetPublicStaticProperties(bool onlyPublicReadableAndWritables = false);
    public abstract PropertyInfo GetProperty(string name);
    public abstract List<PropertyInfo> GetProperties(string name);
    public abstract PropertyInfo GetStaticProperty(string name);
    public abstract List<PropertyInfo> GetStaticProperties(string name);

    public abstract ICollection<MethodInfo> GetPublicMethods();
    public abstract ICollection<MethodInfo> GetPublicStaticMethods();
    public abstract MethodInfo GetMethod(string name);
    public abstract List<MethodInfo> GetMethods(string name);
    public abstract MethodInfo GetStaticMethod(string name);
    public abstract List<MethodInfo> GetStaticMethods(string name);

    public static bool operator ==(TypeInfo l, TypeInfo r) => Equals(l, r);
    public static bool operator !=(TypeInfo l, TypeInfo r) => !(l == r);
    public static bool operator ==(TypeInfo l, IType r) => Equals(l, r);
    public static bool operator !=(TypeInfo l, IType r) => !(l == r);
    public static bool operator ==(IType l, TypeInfo r) => Equals(l, r);
    public static bool operator !=(IType l, TypeInfo r) => !(l == r);

    public override string ToString() => GetActualType().ToString();

    public override bool Equals(object obj)
    {
        if (obj == null) { return false; }
        if (obj is Type typeObj) { return GetActualType() == typeObj; }
        if (obj is not TypeInfo typeInfoObj) { return false; }

        return ReferenceEquals(this, typeInfoObj) || GetActualType() == typeInfoObj.GetActualType();
    }

    public override int GetHashCode() => GetActualType().GetHashCode();

    #region ITypeComponent implementation

    IType ITypeComponent.ParentType => null;

    #endregion

    #region IType implementation

    IType IType.BaseType => BaseType;

    List<IType> IType.AssignableTypes => GetAssignableTypes().Cast<IType>().ToList();
    List<IConstructor> IType.Constructors => GetAllConstructors().Cast<IConstructor>().ToList();
    List<IProperty> IType.Properties => GetAllProperties().Where(p => !p.IsIndexer).Cast<IProperty>().ToList();
    List<IMethod> IType.Methods => GetAllMethods().Cast<IMethod>().ToList();

    List<IType> IType.GetGenericArguments() => GetGenericArguments().Cast<IType>().ToList();
    IType IType.GetElementType() => GetElementType();
    IMethod IType.GetParseMethod() => GetParseMethod();
    IType IType.GetEnumUnderlyingType() => GetEnumUnderlyingType();

    bool IType.CanBe(IType otherType) => otherType is TypeInfo otherTypeInfo && CanBe(otherTypeInfo);

    object IType.Cast(object @object, IType otherType)
    {
        IType thisAsIType = this;

        if (!thisAsIType.CanBe(otherType))
        {
            throw new InvalidCastException($"Cannot cast an object of type {this} to {otherType}");
        }

        return @object;
    }

    #endregion
}

public static class type
{
    public static TypeInfo of<T>() => TypeInfo.Get<T>();
    public static TypeInfo ofvoid() => TypeInfo.Void();
}

public static class TypeInfoExtensions
{
    public static TypeInfo GetTypeInfo(this object source) =>
        source == null
            ? null
            : TypeInfo.Get(source.GetType());

    public static TypeInfo ToTypeInfo(this Type source) => TypeInfo.Get(source);
}
