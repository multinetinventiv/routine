using Routine.Interception;

namespace Routine.Test.Interception;

[TestFixture]
public class InterceptionContextTest
{
    [Test]
    public void Can_contain_extra_data()
    {
        var testing = new InterceptionContext("test")
        {
            ["key"] = "value"
        };

        Assert.That(testing["key"], Is.EqualTo("value"));
    }

    [Test]
    public void Returns_null_when_given_key_does_not_exist()
    {
        var testing = new InterceptionContext("test");

        Assert.That(testing["key"], Is.Null);
    }
}
