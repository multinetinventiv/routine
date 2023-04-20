namespace Routine.Test;

[TestFixture]
public class EnumerableExtensionsTest
{
    [Test]
    public void Test_IEnumerable_ToItemString()
    {
        Assert.That(new[] { "a", "b" }.ToItemString(), Is.EqualTo("[a,b]"));
    }

    [Test]
    public void Test_IEnumerable_ItemEquals()
    {
        Assert.That(((IEnumerable)null).ItemEquals(null), Is.True);
        Assert.That(((IEnumerable)null).ItemEquals(Array.Empty<string>()), Is.False);
        Assert.That(Array.Empty<string>().ItemEquals(null), Is.False);

        Assert.That(Array.Empty<string>().ItemEquals(new List<string>()), Is.True);
        Assert.That(Array.Empty<object>().ItemEquals(new List<string>()), Is.True);

        Assert.That(new[] { "a" }.ItemEquals(new List<string> { "a" }), Is.True);

        Assert.That(new[] { "a" }.ItemEquals(new List<string> { "a", "b" }), Is.False);
        Assert.That(new[] { "a", "b" }.ItemEquals(new List<string> { "a" }), Is.False);
    }

    [Test]
    public void Test_IEnumerable_GetItemHashCode()
    {
        Assert.That(new List<string>().GetItemHashCode(), Is.EqualTo(Array.Empty<string>().GetItemHashCode()));

        Assert.That(new List<string> { "a" }.GetItemHashCode(), Is.EqualTo(new[] { "a" }.GetItemHashCode()));
        Assert.That(new List<string> { "a", "b" }.GetItemHashCode(), Is.Not.EqualTo(new[] { "a" }.GetItemHashCode()));
    }
}
