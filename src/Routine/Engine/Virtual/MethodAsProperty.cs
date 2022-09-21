namespace Routine.Engine.Virtual;

public class MethodAsProperty : IProperty
{
    private readonly object[] parameters;
    private readonly IMethod method;
    private readonly string ignorePrefix;

    public MethodAsProperty(IMethod method, params object[] parameters) : this(method, string.Empty, parameters) { }
    public MethodAsProperty(IMethod method, string ignorePrefix, params object[] parameters)
    {
        if (method == null) { throw new ArgumentNullException(nameof(method)); }
        if (method.Parameters.Count != parameters.Length) { throw new ArgumentException("Given parameters and method parameters do not match"); }
        if (method.ReturnsVoid()) { throw new ArgumentException("Given method must have a return type"); }

        this.method = method;
        this.ignorePrefix = ignorePrefix ?? string.Empty;
        this.parameters = parameters;
    }

    public string Name => method.Name.After(ignorePrefix);
    public object[] GetCustomAttributes() => method.GetCustomAttributes();
    public IType ParentType => method.ParentType;
    public IType ReturnType => method.ReturnType;
    public object[] GetReturnTypeCustomAttributes() => method.GetReturnTypeCustomAttributes();
    public bool IsPublic => method.IsPublic;
    public IType GetDeclaringType(bool firstDeclaringType) => method.GetDeclaringType(firstDeclaringType);
    public object FetchFrom(object target) => method.PerformOn(target, parameters);
}
