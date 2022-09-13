using Routine.Core.Reflection;

namespace Routine.Engine.Reflection;

public class ReflectedPropertyInfo : PropertyInfo
{
    internal ReflectedPropertyInfo(System.Reflection.PropertyInfo propertyInfo)
        : base(propertyInfo) { }

    protected override PropertyInfo Load() => this;

    public override MethodInfo GetGetMethod() => MethodInfo.Reflected(propertyInfo.GetGetMethod(true));
    public override MethodInfo GetSetMethod() => MethodInfo.Reflected(propertyInfo.GetSetMethod(true));

    public override ParameterInfo[] GetIndexParameters() => propertyInfo.GetIndexParameters().Select(ParameterInfo.Reflected).ToArray();

    public override TypeInfo GetFirstDeclaringType()
    {
        if (IsIndexer)
        {
            return DeclaringType;
        }

        var getMethod = GetGetMethod();
        if (getMethod != null)
        {
            return getMethod.GetFirstDeclaringType();
        }

        var setMethod = GetSetMethod();
        if (setMethod != null)
        {
            return setMethod.GetFirstDeclaringType();
        }

        return DeclaringType;
    }

    public override object GetValue(object target, params object[] index) => new ReflectionMethodInvoker(propertyInfo.GetGetMethod()).Invoke(target, index);
    public override object GetStaticValue(params object[] index) => new ReflectionMethodInvoker(propertyInfo.GetGetMethod()).Invoke(null, index);

    public override void SetValue(object target, object value, params object[] index) => new ReflectionMethodInvoker(propertyInfo.GetSetMethod()).Invoke(target, Merge(value, index));
    public override void SetStaticValue(object value, params object[] index) => new ReflectionMethodInvoker(propertyInfo.GetSetMethod()).Invoke(null, Merge(value, index));

    private static object[] Merge(object value, object[] index)
    {
        index ??= Array.Empty<object>();

        var result = new object[index.Length + 1];

        result[0] = value;

        for (var i = 0; i < index.Length; i++)
        {
            result[i + 1] = index[i];
        }

        return result;
    }

    public override string Name => propertyInfo.Name;
    public override TypeInfo DeclaringType => TypeInfo.Get(propertyInfo.DeclaringType);
    public override TypeInfo ReflectedType => TypeInfo.Get(propertyInfo.ReflectedType);
    public override TypeInfo PropertyType => TypeInfo.Get(propertyInfo.PropertyType);

    public override object[] GetCustomAttributes() =>
        //propertyInfo.GetCustomAttributes(true) does not retrieve attributes from inherited properties.
        Attribute.GetCustomAttributes(propertyInfo, true).Cast<object>().ToArray();

    public override object[] GetReturnTypeCustomAttributes() => GetGetMethod().GetReturnTypeCustomAttributes();
}
