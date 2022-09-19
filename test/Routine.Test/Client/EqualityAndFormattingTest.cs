using Routine.Client;

namespace Routine.Test.Client;

[TestFixture]
public class EqualityAndFormattingTest : ClientTestBase
{
    [Test]
    public void Rapplication_implements_equality_members()
    {
        ModelsAre(Model("model"));

        var left = new Rapplication(mockObjectService.Object);
        var right = new Rapplication(mockObjectService.Object);

        Assert.AreEqual(left, right);
        Assert.AreNotSame(left, right);

        Assert.AreEqual(left.GetHashCode(), right.GetHashCode());
    }

    [Test]
    public void Rtype_implements_formatting_and_equality_members()
    {
        ModelsAre(Model("model"), Model("model2"));

        var left = new Rapplication(mockObjectService.Object)["model"];
        var right = new Rapplication(mockObjectService.Object)["model"];
        var other = new Rapplication(mockObjectService.Object)["model2"];

        Assert.AreEqual(left, right);
        Assert.AreNotSame(left, right);
        Assert.AreNotEqual(left, other);

        Assert.AreEqual(left.GetHashCode(), right.GetHashCode());
        Assert.AreNotEqual(left.GetHashCode(), other.GetHashCode());
    }

    [Test]
    public void Rinitializer_implements_formatting_and_equality_datas()
    {
        ModelsAre(Model("model").Initializer(), Model("model2").Initializer());

        var left = new Rapplication(mockObjectService.Object)["model"].Initializer;
        var right = new Rapplication(mockObjectService.Object)["model"].Initializer;
        var other = new Rapplication(mockObjectService.Object)["model2"].Initializer;

        Assert.AreEqual(left, right);
        Assert.AreNotSame(left, right);
        Assert.AreNotEqual(left, other);

        Assert.AreEqual(left.GetHashCode(), right.GetHashCode());
        Assert.AreNotEqual(left.GetHashCode(), other.GetHashCode());
    }

    [Test]
    public void Rdata_implements_formatting_and_equality_members()
    {
        ModelsAre(Model("model").Data("data"), Model("model2").Data("data2"));

        var left = new Rapplication(mockObjectService.Object)["model"].Data["data"];
        var right = new Rapplication(mockObjectService.Object)["model"].Data["data"];
        var other = new Rapplication(mockObjectService.Object)["model2"].Data["data2"];

        Assert.AreEqual(left, right);
        Assert.AreNotSame(left, right);
        Assert.AreNotEqual(left, other);

        Assert.AreEqual(left.GetHashCode(), right.GetHashCode());
        Assert.AreNotEqual(left.GetHashCode(), other.GetHashCode());
    }

    [Test]
    public void Roperation_implements_formatting_and_equality_members()
    {
        ModelsAre(Model("model").Operation("operation"), Model("model2").Operation("operation2"));

        var left = new Rapplication(mockObjectService.Object)["model"].Operation["operation"];
        var right = new Rapplication(mockObjectService.Object)["model"].Operation["operation"];
        var other = new Rapplication(mockObjectService.Object)["model2"].Operation["operation2"];

        Assert.AreEqual(left, right);
        Assert.AreNotSame(left, right);
        Assert.AreNotEqual(left, other);

        Assert.AreEqual(left.GetHashCode(), right.GetHashCode());
        Assert.AreNotEqual(left.GetHashCode(), other.GetHashCode());
    }

    [Test]
    public void Rparameter_implements_formatting_and_equality_members()
    {
        ModelsAre(Model("model").Operation("operation", PModel("arg1")), Model("model2").Operation("operation2", PModel("arg1")));

        var left = new Rapplication(mockObjectService.Object)["model"].Operation["operation"].Parameter["arg1"];
        var right = new Rapplication(mockObjectService.Object)["model"].Operation["operation"].Parameter["arg1"];
        var other = new Rapplication(mockObjectService.Object)["model2"].Operation["operation2"].Parameter["arg1"];

        Assert.AreEqual(left, right);
        Assert.AreNotSame(left, right);
        Assert.AreNotEqual(left, other);

        Assert.AreEqual(left.GetHashCode(), right.GetHashCode());
        Assert.AreNotEqual(left.GetHashCode(), other.GetHashCode());
    }

    [Test]
    public void Robjects_implements_formatting_and_equality_members()
    {
        ObjectsAre(Object(Id("value", "model")), Object(Id("value2", "model")));

        var left = Robj("value", "model");
        var right = Robj("value", "model");
        var other = Robj("value2", "model");

        Assert.AreEqual(left, right);
        Assert.AreNotSame(left, right);
        Assert.AreNotEqual(left, other);

        Assert.AreEqual(left.GetHashCode(), right.GetHashCode());
        Assert.AreNotEqual(left.GetHashCode(), other.GetHashCode());
    }
}
