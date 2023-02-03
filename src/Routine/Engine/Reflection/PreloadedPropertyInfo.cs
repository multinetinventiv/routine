namespace Routine.Engine.Reflection;

public class PreloadedPropertyInfo : PropertyInfo
{
    private string _name;
    private TypeInfo _declaringType;
    private TypeInfo _reflectedType;
    private TypeInfo _propertyType;

    private MethodInfo _getMethod;
    private MethodInfo _setMethod;
    private ParameterInfo[] _indexParameters;
    private TypeInfo _firstDeclaringType;

    private object[] _customAttributes;

    internal PreloadedPropertyInfo(System.Reflection.PropertyInfo propertyInfo)
        : base(propertyInfo) { }

    protected override PropertyInfo Load()
    {
        _name = _propertyInfo.Name;
        _declaringType = TypeInfo.Get(_propertyInfo.DeclaringType);
        _reflectedType = TypeInfo.Get(_propertyInfo.ReflectedType);
        _propertyType = TypeInfo.Get(_propertyInfo.PropertyType);

        _getMethod = _propertyInfo.GetGetMethod(true) == null ? null : MethodInfo.Preloaded(_propertyInfo.GetGetMethod(true));
        _setMethod = _propertyInfo.GetSetMethod(true) == null ? null : MethodInfo.Preloaded(_propertyInfo.GetSetMethod(true));
        _indexParameters = _propertyInfo.GetIndexParameters().Select(p => ParameterInfo.Preloaded(this, p)).ToArray();

        if (IsIndexer)
        {
            _firstDeclaringType = DeclaringType;
        }
        else if (_getMethod != null)
        {
            _firstDeclaringType = _getMethod.GetFirstDeclaringType();
        }
        else if (_setMethod != null)
        {
            _firstDeclaringType = _setMethod.GetFirstDeclaringType();
        }
        else
        {
            _firstDeclaringType = DeclaringType;
        }

        //propertyInfo.GetCustomAttributes(true) does not retrieve attributes from inherited properties.
        _customAttributes = Attribute.GetCustomAttributes(_propertyInfo, true).Cast<object>().ToArray();

        return this;
    }

    public override string Name => _name;
    public override TypeInfo DeclaringType => _declaringType;
    public override TypeInfo ReflectedType => _reflectedType;
    public override TypeInfo PropertyType => _propertyType;

    public override MethodInfo GetGetMethod() => _getMethod;
    public override MethodInfo GetSetMethod() => _setMethod;
    public override ParameterInfo[] GetIndexParameters() => _indexParameters;
    public override TypeInfo GetFirstDeclaringType() => _firstDeclaringType;
    public override object GetValue(object target, params object[] index) => _getMethod.Invoke(target, index);
    public override object GetStaticValue(params object[] index) => _getMethod.InvokeStatic(index);

    public override void SetValue(object target, object value, params object[] index)
    {
        var parameters = new object[index.Length + 1];
        parameters[0] = value;
        for (var i = 0; i < index.Length; i++)
        {
            parameters[i + 1] = index[i];
        }
        _setMethod.Invoke(target, parameters);
    }

    public override void SetStaticValue(object value, params object[] index)
    {
        var parameters = new object[index.Length + 1];
        parameters[0] = value;
        for (var i = 0; i < index.Length; i++)
        {
            parameters[i + 1] = index[i];
        }
        _setMethod.Invoke(null, parameters);
    }

    public override object[] GetCustomAttributes() => _customAttributes;
    public override object[] GetReturnTypeCustomAttributes() =>
        _getMethod == null
            ? Array.Empty<object>()
            : _getMethod.GetReturnTypeCustomAttributes();
}
