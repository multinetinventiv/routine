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

        Assert.AreEqual("type", testing.Type.Name);
        Assert.AreEqual("virtual", testing.Id);
    }

    [Test]
    public void To_string_uses_type_to_get_string_value()
    {
        var testing = new VirtualObject("virtual", VirtualType().ToStringMethod.Set(o => o.Id));

        Assert.AreEqual("virtual", testing.ToString());
    }

    [Test]
    public void Implements_equality_members_on_id_and_type()
    {
        var type = VirtualType();

        var testing1 = new VirtualObject("virtual", type);
        var testing2 = new VirtualObject("virtual", type);

        Assert.AreEqual(testing1, testing2);
        Assert.AreEqual(testing1.GetHashCode(), testing2.GetHashCode());
    }
}
