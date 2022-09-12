using System;

namespace Routine.Engine.Extractor;

public class PropertyValueExtractor : ExtractorBase
{
    private readonly IProperty property;

    private Func<object, object, string> converterDelegate;

    public PropertyValueExtractor(IProperty property)
    {
        this.property = property ?? throw new ArgumentNullException(nameof(property));

        Return(result => result?.ToString());
    }

    public PropertyValueExtractor Return(Func<object, string> converterDelegate) => Return((o, _) => converterDelegate(o));
    public PropertyValueExtractor Return(Func<object, object, string> converterDelegate) { this.converterDelegate = converterDelegate; return this; }

    protected override string Extract(object obj)
    {
        if (obj == null)
        {
            return null;
        }

        var result = property.FetchFrom(obj);

        return converterDelegate(result, obj);
    }
}
