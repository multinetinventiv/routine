namespace Routine.Engine.Locator;

public class ConstantLocator : LocatorBase<ConstantLocator>
{
    private object staticResult;

    public ConstantLocator(object staticResult)
    {
        this.staticResult = staticResult;
    }

    protected override Task<List<object>> LocateAsync(IType type, List<string> ids)
    {
        throw new NotImplementedException();
    }
}
