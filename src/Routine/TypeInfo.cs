using Routine.Engine.Reflection;
using Routine.Engine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

namespace Routine;

public abstract class TypeInfo : IType
{
    #region Factory Methods

    protected const System.Reflection.BindingFlags ALL_STATIC = System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public;
    protected const System.Reflection.BindingFlags ALL_INSTANCE = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public;

    private static readonly Dictionary<Type, TypeInfo> TYPE_CACHE;
    private static readonly List<Type> OPTIMIZED_TYPES;

    private static Func<Type, bool> proxyMatcher;
    private static Func<Type, Type> actualTypeGetter;

    static TypeInfo()
    {
        TYPE_CACHE = new Dictionary<Type, TypeInfo>();
        OPTIMIZED_TYPES = new List<Type>();

        SetProxyMatcher(null, null);
    }

    public static void Clear()
    {
        TYPE_CACHE.Clear();
        OPTIMIZED_TYPES.Clear();

        SetProxyMatcher(null, null);
    }

    public static List<TypeInfo> GetOptimizedTypes() => OPTIMIZED_TYPES.Select(t => t.ToTypeInfo()).ToList();

    public static void Optimize(params Type[] newDomainTypes)
    {
        OPTIMIZED_TYPES.AddRange(newDomainTypes.Where(t => !OPTIMIZED_TYPES.Contains(t)));

        TYPE_CACHE.Clear();
    }

    public static void SetProxyMatcher(Func<Type, bool> proxyMatcher, Func<Type, Type> actualTypeGetter)
    {
        TypeInfo.proxyMatcher = proxyMatcher ?? (_ => false);
        TypeInfo.actualTypeGetter = actualTypeGetter ?? (t => t);
    }

    public static TypeInfo Void() => Get(typeof(void));
    public static TypeInfo Get<T>() => Get(typeof(T));
    public static TypeInfo Get(Type type)
    {
        if (type == null)
        {
            return null;
        }

        if (!TYPE_CACHE.TryGetValue(type, out var result))
        {
            lock (TYPE_CACHE)
            {
                if (!TYPE_CACHE.TryGetValue(type, out result))
                {
                    if (proxyMatcher(type))
                    {
                        var actualType = actualTypeGetter(type);

                        if (!TYPE_CACHE.TryGetValue(actualType, out result))
                        {
                            result = CreateTypeInfo(actualType);

                            TYPE_CACHE.Add(actualType, result);

                            result.Load();
                        }

                        TYPE_CACHE.Add(type, result);
                    }
                    else
                    {
                        result = CreateTypeInfo(type);

                        TYPE_CACHE.Add(type, result);

                        result.Load();
                    }
                }
            }
        }

        return result;
    }

    private static TypeInfo CreateTypeInfo(Type type)
    {
        TypeInfo result;

        if (type == typeof(void))
        {
            result = new VoidTypeInfo();
        }
        else if (type.GetMethod("Parse", new[] { typeof(string) }) != null && type.GetMethod("Parse", new[] { typeof(string) }).ReturnType == type)
        {
            result = new ParseableTypeInfo(type);
        }
        else if (type.IsArray)
        {
            result = new ArrayTypeInfo(type);
        }
        else if (type.IsEnum)
        {
            result = new EnumTypeInfo(type);
        }
        else if (type.ContainsGenericParameters)
        {
            result = new ReflectedTypeInfo(type);
        }
        else if (OPTIMIZED_TYPES.Contains(type))
        {
            result = new OptimizedTypeInfo(type);
        }
        else
        {
            result = new ReflectedTypeInfo(type);
        }

        return result;
    }

    #endregion

    protected readonly Type type;

    protected TypeInfo(Type type)
    {
        this.type = type;

        IsPublic = type.IsPublic;
        IsAbstract = type.IsAbstract;
        IsInterface = type.IsInterface;
        IsValueType = type.IsValueType;
        IsGenericType = type.IsGenericType;
        IsPrimitive = type.IsPrimitive;
    }

    public Type GetActualType() => type;

    public bool IsPublic { get; protected set; }
    public bool IsAbstract { get; protected set; }
    public bool IsInterface { get; protected set; }
    public bool IsValueType { get; protected set; }
    public bool IsGenericType { get; protected set; }
    public bool IsPrimitive { get; protected set; }

    public bool IsVoid { get; protected set; }
    public bool IsEnum { get; protected set; }
    public bool IsArray { get; protected set; }

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
    protected abstract TypeInfo[] GetGenericArguments();
    protected abstract TypeInfo GetElementType();
    protected abstract TypeInfo[] GetInterfaces();
    public abstract bool CanBe(TypeInfo other);
    public virtual List<string> GetEnumNames() { return new List<string>(); }
    public virtual List<object> GetEnumValues() { return new List<object>(); }
    protected virtual TypeInfo GetEnumUnderlyingType() { return null; }

    protected abstract MethodInfo GetParseMethod();
    protected abstract void Load();

    public abstract object CreateInstance();
    public abstract IList CreateListInstance(int length);

    protected virtual TypeInfo[] GetAssignableTypes()
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

    public virtual List<ConstructorInfo> GetPublicConstructors() => GetAllConstructors().Where(c => c.IsPublic).ToList();

    public virtual ConstructorInfo GetConstructor(params TypeInfo[] typeInfos)
    {
        if (typeInfos.Length > 0)
        {
            var first = typeInfos[0];
            var rest = Enumerable.Range(1, typeInfos.Length - 1).Select(i => (IType)typeInfos[i]).ToArray();

            return GetAllConstructors().SingleOrDefault(c => c.HasParameters(first, rest));
        }

        return GetAllConstructors().SingleOrDefault(c => c.HasNoParameters());
    }

    public virtual ICollection<PropertyInfo> GetPublicProperties() { return GetPublicProperties(false); }
    public virtual ICollection<PropertyInfo> GetPublicProperties(bool onlyPublicReadableAndWritables)
    {
        if (onlyPublicReadableAndWritables)
        {
            return GetAllProperties().Where(p => p.IsPubliclyReadable && p.IsPubliclyWritable).ToList();
        }

        return GetAllProperties().Where(p => p.IsPubliclyReadable).ToList();
    }

    public virtual ICollection<PropertyInfo> GetPublicStaticProperties() { return GetPublicStaticProperties(false); }
    public virtual ICollection<PropertyInfo> GetPublicStaticProperties(bool onlyPublicReadableAndWritables)
    {
        if (onlyPublicReadableAndWritables)
        {
            return GetAllStaticProperties().Where(p => p.IsPubliclyReadable && p.IsPubliclyWritable).ToList();
        }

        return GetAllStaticProperties().Where(p => p.IsPubliclyReadable).ToList();
    }

    public virtual PropertyInfo GetProperty(string name) => GetAllProperties().SingleOrDefault(p => p.Name == name);
    public virtual List<PropertyInfo> GetProperties(string name) => GetAllProperties().Where(p => p.Name == name).ToList();
    public virtual PropertyInfo GetStaticProperty(string name) => GetAllStaticProperties().SingleOrDefault(p => p.Name == name);
    public virtual List<PropertyInfo> GetStaticProperties(string name) => GetAllStaticProperties().Where(p => p.Name == name).ToList();
    public virtual ICollection<MethodInfo> GetPublicMethods() => GetAllMethods().Where(m => m.IsPublic).ToList();
    public virtual ICollection<MethodInfo> GetPublicStaticMethods() => GetAllStaticMethods().Where(m => m.IsPublic).ToList();
    public virtual MethodInfo GetMethod(string name) => GetAllMethods().SingleOrDefault(m => m.Name == name);
    public virtual List<MethodInfo> GetMethods(string name) => GetAllMethods().Where(m => m.Name == name).ToList();
    public virtual MethodInfo GetStaticMethod(string name) => GetAllStaticMethods().SingleOrDefault(m => m.Name == name);
    public virtual List<MethodInfo> GetStaticMethods(string name) => GetAllStaticMethods().Where(m => m.Name == name).ToList();

    public override string ToString() => type.ToString();

    public static bool operator ==(TypeInfo l, TypeInfo r) { return Equals(l, r); }
    public static bool operator !=(TypeInfo l, TypeInfo r) { return !(l == r); }
    public static bool operator ==(TypeInfo l, IType r) { return Equals(l, r); }
    public static bool operator !=(TypeInfo l, IType r) { return !(l == r); }
    public static bool operator ==(IType l, TypeInfo r) { return Equals(l, r); }
    public static bool operator !=(IType l, TypeInfo r) { return !(l == r); }

    public override bool Equals(object obj)
    {
        if (obj == null) { return false; }
        if (obj is Type typeObj) { return type == typeObj; }
        if (obj is not TypeInfo typeInfoObj) { return false; }

        return ReferenceEquals(this, typeInfoObj) || type == typeInfoObj.type;
    }

    public override int GetHashCode() => type.GetHashCode();

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
        var thisAsIType = this as IType;

        if (!thisAsIType.CanBe(otherType))
        {
            throw new InvalidCastException($"Cannot cast an object of type {this} to {otherType}");
        }

        return @object;
    }

    #endregion
}

// ReSharper disable InconsistentNaming
public static class type
{
    public static TypeInfo of<T>() => TypeInfo.Get<T>();
    public static TypeInfo ofvoid() => TypeInfo.Void();
}
// ReSharper restore InconsistentNaming

public static class TypeInfoObjectExtensions
{
    public static TypeInfo GetTypeInfo(this object source) =>
        source == null
            ? null
            : TypeInfo.Get(source.GetType());

    public static TypeInfo ToTypeInfo(this Type source) => TypeInfo.Get(source);
}
