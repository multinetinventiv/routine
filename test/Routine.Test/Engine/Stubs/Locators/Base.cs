using Routine.Engine;
using Routine.Engine.Locator;

namespace Routine.Test.Engine.Stubs.Locators;

public class Base : LocatorBase<Base>
{
    private readonly bool provideDifferentNumberOfObjects;

    public Base(bool provideDifferentNumberOfObjects)
    {
        this.provideDifferentNumberOfObjects = provideDifferentNumberOfObjects;
    }

    protected override Task<List<object>> LocateAsync(IType type, List<string> ids)
    {
        var result = ids.Select(_ => (object)null).ToList();

        if (provideDifferentNumberOfObjects)
        {
            result.Add(null);
        }

        return Task.FromResult(result);
    }
}
