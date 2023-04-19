using Routine.Engine.Virtual;

namespace Routine.Test.Engine.Virtual;

[TestFixture]
public class VirtualObjectTest
{
    #region Helpers

    private VirtualType VirtualType(string name = "type", string @namespace = "namespace") =>
        BuildRoutine.VirtualType().FromBasic().Name.Set(name).Namespace.Set(@namespace);

    #endregion

    [Test]
    public void Has_id_and_type()
    {
        var testing = new VirtualObject("virtual", VirtualType("type"));

        Assert.That(testing.Type.Name, Is.EqualTo("type"));
        Assert.That(testing.Id, Is.EqualTo("virtual"));
    }

    [Test]
    public void To_string_uses_type_to_get_string_value()
    {
        var testing = new VirtualObject("virtual", VirtualType().ToStringMethod.Set(o => o.Id));

        Assert.That(testing.ToString(), Is.EqualTo("virtual"));
    }

    [Test]
    public void Implements_equality_members_on_id_and_type()
    {
        var type = VirtualType();

        var testing1 = new VirtualObject("virtual", type);
        var testing2 = new VirtualObject("virtual", type);

        Assert.That(testing2, Is.EqualTo(testing1));
        Assert.That(testing2.GetHashCode(), Is.EqualTo(testing1.GetHashCode()));
    }
}
