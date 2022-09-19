namespace Routine.Engine.Locator;

public class ConstantLocator : LocatorBase<ConstantLocator>
{
    private object constant;

    public ConstantLocator(object constant)
    {
        this.constant = constant;
    }

    protected override Task<List<object>> LocateAsync(IType type, List<string> ids) =>
        Task.FromResult(ids.Select(_ => constant).ToList());
}
