namespace Routine.Engine.Virtual;

public class PropertyAsMethod : IMethod
{
    private readonly IProperty _property;
    private readonly string _namePrefix;

    public PropertyAsMethod(IProperty property) : this(property, Constants.PROPERTY_AS_METHOD_DEFAULT_PREFIX) { }
    public PropertyAsMethod(IProperty property, string namePrefix)
    {
        _property = property ?? throw new ArgumentNullException(nameof(property));
        _namePrefix = namePrefix ?? string.Empty;
    }

    public string Name => _property.Name.Prepend(_namePrefix);
    public object[] GetCustomAttributes() => _property.GetCustomAttributes();

    public IType ParentType => _property.ParentType;
    public IType ReturnType => _property.ReturnType;
    public object[] GetReturnTypeCustomAttributes() => _property.GetReturnTypeCustomAttributes();

    public List<IParameter> Parameters => new();
    public bool IsPublic => _property.IsPublic;
    public IType GetDeclaringType(bool firstDeclaringType) => _property.GetDeclaringType(firstDeclaringType);

    public object PerformOn(object target, params object[] parameters) => _property.FetchFrom(target);
    public Task<object> PerformOnAsync(object target, params object[] parameters) => Task.FromResult(_property.FetchFrom(target));
}
