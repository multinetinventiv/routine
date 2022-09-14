namespace Routine.Engine.Virtual;

public class PropertyAsMethod : IMethod
{
    private readonly IProperty property;
    private readonly string namePrefix;

    public PropertyAsMethod(IProperty property) : this(property, Constants.PROPERTY_AS_METHOD_DEFAULT_PREFIX) { }
    public PropertyAsMethod(IProperty property, string namePrefix)
    {
        this.property = property;
        this.namePrefix = namePrefix ?? throw new ArgumentNullException(nameof(namePrefix));
    }

    public string Name => property.Name.Prepend(namePrefix);
    public object[] GetCustomAttributes() => property.GetCustomAttributes();

    public IType ParentType => property.ParentType;
    public IType ReturnType => property.ReturnType;
    public object[] GetReturnTypeCustomAttributes() => property.GetReturnTypeCustomAttributes();

    public List<IParameter> Parameters => new();
    public bool IsPublic => property.IsPublic;
    public IType GetDeclaringType(bool firstDeclaringType) => property.GetDeclaringType(firstDeclaringType);
    public object PerformOn(object target, params object[] parameters) => property.FetchFrom(target);
    public Task<object> PerformOnAsync(object target, params object[] parameters) => Task.FromResult(property.FetchFrom(target));
}
