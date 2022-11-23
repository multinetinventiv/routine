using Routine.Core.Configuration;

namespace Routine.Engine.Virtual;

public class VirtualParameter : IParameter
{
    private readonly IParametric _owner;

    public SingleConfiguration<VirtualParameter, string> Name { get; }
    public SingleConfiguration<VirtualParameter, IType> ParameterType { get; }
    public SingleConfiguration<VirtualParameter, int> Index { get; }

    public VirtualParameter(IParametric owner)
    {
        _owner = owner;

        Name = new(this, nameof(Name), true);
        ParameterType = new(this, nameof(ParameterType), true);
        Index = new(this, nameof(Index));
    }

    #region ITypeComponent implementation

    object[] ITypeComponent.GetCustomAttributes() => Array.Empty<object>();

    string ITypeComponent.Name => Name.Get();
    IType ITypeComponent.ParentType => _owner.ParentType;

    #endregion

    #region IParameter implementation

    IParametric IParameter.Owner => _owner;
    IType IParameter.ParameterType => ParameterType.Get();
    int IParameter.Index => Index.Get();
    bool IParameter.IsOptional => false;
    bool IParameter.HasDefaultValue => false;
    object IParameter.DefaultValue => null;

    #endregion

}
