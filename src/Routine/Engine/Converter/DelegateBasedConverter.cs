namespace Routine.Engine.Converter;

public class DelegateBasedConverter : ConverterBase<DelegateBasedConverter>
{
    private readonly Func<IEnumerable<IType>> _targetTypesDelegate;
    private readonly Func<object, IType, object> _converterDelegate;

    public DelegateBasedConverter(Func<IEnumerable<IType>> targetTypesDelegate, Func<object, IType, object> converterDelegate)
    {
        _targetTypesDelegate = targetTypesDelegate ?? throw new ArgumentNullException(nameof(targetTypesDelegate));
        _converterDelegate = converterDelegate ?? throw new ArgumentNullException(nameof(converterDelegate));
    }

    protected override List<IType> GetTargetTypes(IType type) => _targetTypesDelegate().ToList();
    protected override object Convert(object @object, IType from, IType to) => _converterDelegate(@object, to);
}
