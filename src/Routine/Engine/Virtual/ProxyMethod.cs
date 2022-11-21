using Routine.Core.Configuration;

namespace Routine.Engine.Virtual;

public class ProxyMethod : IMethod
{
    private readonly IMethod _real;
    private readonly IType _parentType;
    private readonly int _parameterOffset;
    private readonly List<IParameter> _parameters;
    private readonly Func<object, object[], object> _targetDelegate;

    public SingleConfiguration<ProxyMethod, string> Name { get; }

    public ProxyMethod(IType parentType, IMethod real, params IParameter[] parameters) : this(parentType, real, (o, _) => o, parameters.AsEnumerable()) { }
    public ProxyMethod(IType parentType, IMethod real, Func<object, object[], object> targetDelegate, params IParameter[] parameters) : this(parentType, real, targetDelegate, parameters.AsEnumerable()) { }
    public ProxyMethod(IType parentType, IMethod real, Func<object, object[], object> targetDelegate, IEnumerable<IParameter> parameters)
    {
        if (parameters == null) { throw new ArgumentNullException(nameof(parameters)); }

        Name = new(this, nameof(Name), true);

        _real = real ?? throw new ArgumentNullException(nameof(real));
        _parentType = parentType ?? throw new ArgumentNullException(nameof(parentType));
        _targetDelegate = targetDelegate ?? throw new ArgumentNullException(nameof(targetDelegate));

        _parameters = parameters.Select((p, i) => new ProxyParameter(p, this, i) as IParameter).ToList();

        _parameterOffset = _parameters.Count;

        _parameters.AddRange(real.Parameters.Select((p, i) => new ProxyParameter(p, this, _parameterOffset + i) as IParameter));

        Name.Set(real.Name);
    }

    private object PerformOn(object target, object[] parameters) => _real.PerformOn(_targetDelegate(target, parameters), parameters.Skip(_parameterOffset).ToArray());
    public async Task<object> PerformOnAsync(object target, params object[] parameters) => await _real.PerformOnAsync(_targetDelegate(target, parameters), parameters.Skip(_parameterOffset).ToArray());

    #region ITypeComponent implementation

    IType ITypeComponent.ParentType => _parentType;
    string ITypeComponent.Name => Name.Get();
    object[] ITypeComponent.GetCustomAttributes() => _real.GetCustomAttributes();

    #endregion

    #region IParametric implementation

    List<IParameter> IParametric.Parameters => _parameters;

    #endregion

    #region IReturnable implementation

    IType IReturnable.ReturnType => _real.ReturnType;
    object[] IReturnable.GetReturnTypeCustomAttributes() => _real.GetReturnTypeCustomAttributes();

    #endregion

    #region IMethod

    object IMethod.PerformOn(object target, params object[] parameters) => PerformOn(target, parameters);
    bool IMethod.IsPublic => _real.IsPublic;
    IType IMethod.GetDeclaringType(bool firstDeclaringType) => _parentType;

    #endregion
}
