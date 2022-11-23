using Routine.Core.Reflection;

namespace Routine.Engine.Reflection;

public class PreloadedConstructorInfo : ConstructorInfo
{
    private bool _isPublic;
    private TypeInfo _declaringType;
    private TypeInfo _reflectedType;

    private ParameterInfo[] _parameters;
    private object[] _customAttributes;

    private IMethodInvoker _invoker;

    internal PreloadedConstructorInfo(System.Reflection.ConstructorInfo constructorInfo)
        : base(constructorInfo) { }

    protected override ConstructorInfo Load()
    {
        _isPublic = _constructorInfo.IsPublic;
        _declaringType = TypeInfo.Get(_constructorInfo.DeclaringType);
        _reflectedType = TypeInfo.Get(_constructorInfo.ReflectedType);

        _parameters = _constructorInfo.GetParameters().Select(p => ParameterInfo.Preloaded(this, p)).ToArray();
        _customAttributes = _constructorInfo.GetCustomAttributes(true);

        _invoker = _constructorInfo.CreateInvoker();

        return this;
    }

    public override bool IsPublic => _isPublic;
    public override TypeInfo DeclaringType => _declaringType;
    public override TypeInfo ReflectedType => _reflectedType;

    public override ParameterInfo[] GetParameters() => _parameters;
    public override object[] GetCustomAttributes() => _customAttributes;
    public override object Invoke(params object[] parameters) => _invoker.Invoke(null, parameters);
}
