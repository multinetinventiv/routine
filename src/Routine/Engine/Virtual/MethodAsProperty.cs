namespace Routine.Engine.Virtual;

public class MethodAsProperty : IProperty
{
    private readonly object[] _parameters;
    private readonly IMethod _method;
    private readonly string _ignorePrefix;

    public MethodAsProperty(IMethod method, params object[] parameters) : this(method, string.Empty, parameters) { }
    public MethodAsProperty(IMethod method, string ignorePrefix, params object[] parameters)
    {
        if (method == null) { throw new ArgumentNullException(nameof(method)); }
        if (method.Parameters.Count != parameters.Length) { throw new ArgumentException("Given parameters and method parameters do not match"); }
        if (method.ReturnsVoid()) { throw new ArgumentException("Given method must have a return type"); }

        _method = method;
        _ignorePrefix = ignorePrefix ?? string.Empty;
        _parameters = parameters;
    }

    public string Name => _method.Name.After(_ignorePrefix);
    public object[] GetCustomAttributes() => _method.GetCustomAttributes();

    public IType ParentType => _method.ParentType;
    public IType ReturnType => _method.ReturnType;
    public object[] GetReturnTypeCustomAttributes() => _method.GetReturnTypeCustomAttributes();

    public bool IsPublic => _method.IsPublic;
    public IType GetDeclaringType(bool firstDeclaringType) => _method.GetDeclaringType(firstDeclaringType);

    public object FetchFrom(object target) => _method.PerformOn(target, _parameters);
}
