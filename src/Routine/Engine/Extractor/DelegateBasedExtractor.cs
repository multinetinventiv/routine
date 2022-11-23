namespace Routine.Engine.Extractor;

public class DelegateBasedExtractor : ExtractorBase
{
    private readonly Func<object, string> _extractorDelegate;

    public DelegateBasedExtractor(Func<object, string> extractorDelegate)
    {
        _extractorDelegate = extractorDelegate ?? throw new ArgumentNullException(nameof(extractorDelegate));
    }

    protected override string Extract(object obj) => _extractorDelegate(obj);
}
