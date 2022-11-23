namespace Routine.Engine.Extractor;

public class PropertyValueExtractor : ExtractorBase
{
    private readonly IProperty _property;

    private Func<object, object, string> _converterDelegate;

    public PropertyValueExtractor(IProperty property)
    {
        _property = property ?? throw new ArgumentNullException(nameof(property));

        Return(result => result?.ToString());
    }

    public PropertyValueExtractor Return(Func<object, string> converterDelegate) => Return((o, _) => converterDelegate(o));
    public PropertyValueExtractor Return(Func<object, object, string> converterDelegate) { _converterDelegate = converterDelegate; return this; }

    protected override string Extract(object obj)
    {
        if (obj == null)
        {
            return null;
        }

        var result = _property.FetchFrom(obj);

        return _converterDelegate(result, obj);
    }
}
