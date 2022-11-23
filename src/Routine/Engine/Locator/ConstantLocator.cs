namespace Routine.Engine.Locator;

public class ConstantLocator : LocatorBase<ConstantLocator>
{
    private object _constant;

    public ConstantLocator(object constant)
    {
        _constant = constant;
    }

    protected override Task<List<object>> LocateAsync(IType type, List<string> ids) =>
        Task.FromResult(ids.Select(_ => _constant).ToList());
}
