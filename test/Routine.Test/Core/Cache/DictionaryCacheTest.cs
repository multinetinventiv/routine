using Routine.Core.Cache;

namespace Routine.Test.Core.Cache;

[TestFixture]
public class DictionaryCacheTest
{
    private DictionaryCache _testing;

    [SetUp]
    public void SetUp()
    {
        _testing = new();
    }

    [Test]
    public void Indexer__returns_item_with_given_key()
    {
        var expected = new object();

        _testing.Add("test", expected);

        var actual = _testing["test"];

        Assert.That(actual, Is.SameAs(expected));
    }

    [Test]
    public void Indexer__returns_null_when_item_does_not_exist()
    {
        var actual = _testing["test"];

        Assert.That(actual, Is.Null);
    }

    [Test]
    public void ContainsKey__returns_true_when_item_exists__false_when_it_does_not()
    {
        _testing.Add("exists", new object());

        Assert.That(_testing.Contains("exists"), Is.True);
        Assert.That(_testing.Contains("doesn't exist"), Is.False);
    }

    [Test]
    public void Add__adds_given_item_to_key()
    {
        var expected = new object();

        _testing.Add("test", expected);

        Assert.That(_testing["test"], Is.EqualTo(expected));
    }

    [Test]
    public void Add__overrides_existing_item_with_the_new_one()
    {
        var old = new object();
        var @new = new object();

        _testing.Add("test", old);
        _testing.Add("test", @new);

        Assert.That(_testing["test"], Is.EqualTo(@new));
    }

    [Test]
    public void Remove__removes_item_at_key()
    {
        _testing.Add("test", new object());

        _testing.Remove("test");

        Assert.That(_testing.Contains("test"), Is.False);
    }

    [Test]
    public void Remove_does_not_do_anything_when_non_existing_key_is_removed()
    {
        Assert.That(() => _testing.Remove("non existing"), Throws.Nothing);
    }
}
