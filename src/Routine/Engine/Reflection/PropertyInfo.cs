namespace Routine.Engine.Reflection;

public abstract class PropertyInfo : MemberInfo, IProperty
{
    internal static PropertyInfo Reflected(System.Reflection.PropertyInfo propertyInfo) => new ReflectedPropertyInfo(propertyInfo).Load();
    internal static PropertyInfo Preloaded(System.Reflection.PropertyInfo propertyInfo) => new PreloadedPropertyInfo(propertyInfo).Load();

    protected readonly System.Reflection.PropertyInfo _propertyInfo;

    protected PropertyInfo(System.Reflection.PropertyInfo propertyInfo)
    {
        _propertyInfo = propertyInfo;
    }

    public System.Reflection.PropertyInfo GetActualProperty() => _propertyInfo;

    protected abstract PropertyInfo Load();

    public abstract TypeInfo PropertyType { get; }

    public abstract MethodInfo GetGetMethod();
    public abstract MethodInfo GetSetMethod();
    public abstract ParameterInfo[] GetIndexParameters();
    public abstract TypeInfo GetFirstDeclaringType();

    public abstract object GetValue(object target, params object[] index);
    public abstract object GetStaticValue(params object[] index);
    public abstract void SetValue(object target, object value, params object[] index);
    public abstract void SetStaticValue(object value, params object[] index);

    public abstract object[] GetCustomAttributes();
    public abstract object[] GetReturnTypeCustomAttributes();

    public bool IsPubliclyReadable => GetGetMethod() != null && GetGetMethod().IsPublic;
    public bool IsPubliclyWritable => GetSetMethod() != null && GetSetMethod().IsPublic;

    public bool IsIndexer => GetIndexParameters().Length > 0;

    #region ITypeComponent implementation

    IType ITypeComponent.ParentType => ReflectedType;

    #endregion

    #region IReturnable implementation

    IType IReturnable.ReturnType => PropertyType;

    #endregion

    #region IMember implementation

    object IProperty.FetchFrom(object target) => target == null ? GetStaticValue() : GetValue(target);
    IType IProperty.GetDeclaringType(bool firstDeclaringType) => firstDeclaringType ? GetFirstDeclaringType() : DeclaringType;
    bool IProperty.IsPublic => IsPubliclyReadable;

    #endregion
}
