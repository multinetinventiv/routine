namespace Routine.Engine.Virtual;

public class ProxyParameter : IParameter
{
    private readonly IParameter _real;
    private readonly int _index;

    public IParametric Owner { get; }

    public ProxyParameter(IParameter real, IParametric owner) : this(real, owner, 0) { }
    public ProxyParameter(IParameter real, IParametric owner, int index)
    {
        if (index < 0) { throw new ArgumentOutOfRangeException(nameof(index), index, "'index' cannot be less than zero"); }

        _real = real ?? throw new ArgumentNullException(nameof(real));
        _index = index;

        Owner = owner ?? throw new ArgumentNullException(nameof(owner));
    }

    public string Name => _real.Name;
    public IType ParentType => Owner.ParentType;
    public int Index => _index;
    public IType ParameterType => _real.ParameterType;
    public bool IsOptional => _real.IsOptional;
    public bool HasDefaultValue => _real.HasDefaultValue;
    public object DefaultValue => _real.DefaultValue;

    public object[] GetCustomAttributes() => _real.GetCustomAttributes();
}
