using Routine.Client;

namespace Routine.Test.Client;

[TestFixture]
public class EqualityAndFormattingTest : ClientTestBase
{
    [Test]
    public void Rapplication_implements_equality_members()
    {
        ModelsAre(Model("model"));

        var left = new Rapplication(_mockObjectService.Object);
        var right = new Rapplication(_mockObjectService.Object);

        Assert.That(right, Is.EqualTo(left));
        Assert.That(right, Is.Not.SameAs(left));

        Assert.That(right.GetHashCode(), Is.EqualTo(left.GetHashCode()));
    }

    [Test]
    public void Rtype_implements_formatting_and_equality_members()
    {
        ModelsAre(Model("model"), Model("model2"));

        var left = new Rapplication(_mockObjectService.Object)["model"];
        var right = new Rapplication(_mockObjectService.Object)["model"];
        var other = new Rapplication(_mockObjectService.Object)["model2"];

        Assert.That(right, Is.EqualTo(left));
        Assert.That(right, Is.Not.SameAs(left));
        Assert.That(left, Is.Not.EqualTo(other));

        Assert.That(right.GetHashCode(), Is.EqualTo(left.GetHashCode()));
        Assert.That(left.GetHashCode(), Is.Not.EqualTo(other.GetHashCode()));
    }

    [Test]
    public void Rinitializer_implements_formatting_and_equality_datas()
    {
        ModelsAre(Model("model").Initializer(), Model("model2").Initializer());

        var left = new Rapplication(_mockObjectService.Object)["model"].Initializer;
        var right = new Rapplication(_mockObjectService.Object)["model"].Initializer;
        var other = new Rapplication(_mockObjectService.Object)["model2"].Initializer;

        Assert.That(right, Is.EqualTo(left));
        Assert.That(right, Is.Not.SameAs(left));
        Assert.That(left, Is.Not.EqualTo(other));

        Assert.That(right.GetHashCode(), Is.EqualTo(left.GetHashCode()));
        Assert.That(left.GetHashCode(), Is.Not.EqualTo(other.GetHashCode()));
    }

    [Test]
    public void Rdata_implements_formatting_and_equality_members()
    {
        ModelsAre(Model("model").Data("data"), Model("model2").Data("data2"));

        var left = new Rapplication(_mockObjectService.Object)["model"].Data["data"];
        var right = new Rapplication(_mockObjectService.Object)["model"].Data["data"];
        var other = new Rapplication(_mockObjectService.Object)["model2"].Data["data2"];

        Assert.That(right, Is.EqualTo(left));
        Assert.That(right, Is.Not.SameAs(left));
        Assert.That(left, Is.Not.EqualTo(other));

        Assert.That(right.GetHashCode(), Is.EqualTo(left.GetHashCode()));
        Assert.That(left.GetHashCode(), Is.Not.EqualTo(other.GetHashCode()));
    }

    [Test]
    public void Roperation_implements_formatting_and_equality_members()
    {
        ModelsAre(Model("model").Operation("operation"), Model("model2").Operation("operation2"));

        var left = new Rapplication(_mockObjectService.Object)["model"].Operation["operation"];
        var right = new Rapplication(_mockObjectService.Object)["model"].Operation["operation"];
        var other = new Rapplication(_mockObjectService.Object)["model2"].Operation["operation2"];

        Assert.That(right, Is.EqualTo(left));
        Assert.That(right, Is.Not.SameAs(left));
        Assert.That(left, Is.Not.EqualTo(other));

        Assert.That(right.GetHashCode(), Is.EqualTo(left.GetHashCode()));
        Assert.That(left.GetHashCode(), Is.Not.EqualTo(other.GetHashCode()));
    }

    [Test]
    public void Rparameter_implements_formatting_and_equality_members()
    {
        ModelsAre(Model("model").Operation("operation", PModel("arg1")), Model("model2").Operation("operation2", PModel("arg1")));

        var left = new Rapplication(_mockObjectService.Object)["model"].Operation["operation"].Parameter["arg1"];
        var right = new Rapplication(_mockObjectService.Object)["model"].Operation["operation"].Parameter["arg1"];
        var other = new Rapplication(_mockObjectService.Object)["model2"].Operation["operation2"].Parameter["arg1"];

        Assert.That(right, Is.EqualTo(left));
        Assert.That(right, Is.Not.SameAs(left));
        Assert.That(left, Is.Not.EqualTo(other));

        Assert.That(right.GetHashCode(), Is.EqualTo(left.GetHashCode()));
        Assert.That(left.GetHashCode(), Is.Not.EqualTo(other.GetHashCode()));
    }

    [Test]
    public void Robjects_implements_formatting_and_equality_members()
    {
        ObjectsAre(Object(Id("value", "model")), Object(Id("value2", "model")));

        var left = Robj("value", "model");
        var right = Robj("value", "model");
        var other = Robj("value2", "model");

        Assert.That(right, Is.EqualTo(left));
        Assert.That(right, Is.Not.SameAs(left));
        Assert.That(left, Is.Not.EqualTo(other));

        Assert.That(right.GetHashCode(), Is.EqualTo(left.GetHashCode()));
        Assert.That(left.GetHashCode(), Is.Not.EqualTo(other.GetHashCode()));
    }
}
