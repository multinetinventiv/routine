using Routine.Engine;

namespace Routine.Test.Engine.Locator;

public class ConstantLocator
{
    [Test]
    public async Task Always_locates_given_constant()
    {
        var locator = BuildRoutine.Locator().Constant("constant") as ILocator;

        var actual = await locator.LocateAsync(type.of<string>(), new List<string> { string.Empty, string.Empty });

        Assert.That(actual[0], Is.EqualTo("constant"));
        Assert.That(actual[1], Is.EqualTo("constant"));
    }
}
