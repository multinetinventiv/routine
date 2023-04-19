using Routine.Client;
using System.Globalization;

namespace Routine.Test.Client;

[TestFixture]
public class FacadeTest : ClientTestBase
{
    [Test]
    public void Facade_Rvariable_As()
    {
        ModelsAre(Model("s-int-32").IsValue());

        int result = Rvar("value", Robj("10", "s-int-32")).As(robj => int.Parse(robj.Display));

        Assert.That(result, Is.EqualTo(10));
    }

    [Test]
    public void Facade_Rvariable_As_Returns_default_when_value_is_null()
    {
        int intResult = Rvar("value", RobjNull()).As(robj => int.Parse(robj.Display));

        Assert.That(intResult, Is.EqualTo(0));

        string stringResult = Rvar("value", RobjNull()).As(robj => robj.Display);

        Assert.That(stringResult, Is.Null);
    }

    [Test]
    public void Facade_Rvariable_AsList()
    {
        ModelsAre(Model("s-int-32").IsValue());

        List<int> result = Rvarlist("value", new[] { Robj("10", "s-int-32"), Robj("11", "s-int-32") }).AsList(robj => int.Parse(robj.Display));

        Assert.That(result[0], Is.EqualTo(10));
        Assert.That(result[1], Is.EqualTo(11));
    }

    [Test]
    public void Facade_Rvariable_AsList_Puts_default_value_when_an_item_is_null()
    {
        List<int> intResult = Rvarlist("value", new[] { RobjNull() }).AsList(robj => int.Parse(robj.Display));

        Assert.That(intResult[0], Is.EqualTo(0));

        List<string> stringResult = Rvarlist("value", new[] { RobjNull() }).AsList(robj => robj.Display);

        Assert.That(stringResult[0], Is.Null);
    }

    [Test]
    public void Facade_Rapplication_NewVar()
    {
        ModelsAre(Model("s-int-32").IsValue());

        var actual = _testingRapplication.NewVar("name", Robj("10", "s-int-32"));

        Assert.That(actual.Object.Id, Is.EqualTo("10"));
        Assert.That(actual.Name, Is.EqualTo("name"));

        actual = _testingRapplication.NewVar("name", 10, "s-int-32");

        Assert.That(actual.Object.Id, Is.EqualTo("10"));
        Assert.That(actual.Name, Is.EqualTo("name"));

        actual = _testingRapplication.NewVar("name", 10, o => o.ToString(CultureInfo.InvariantCulture), "s-int-32");

        Assert.That(actual.Object.Id, Is.EqualTo("10"));
        Assert.That(actual.Name, Is.EqualTo("name"));

        actual = _testingRapplication.NewVar("name", 10, o => _testingRapplication["s-int-32"].Get(o.ToString(CultureInfo.InvariantCulture)));

        Assert.That(actual.Object.Id, Is.EqualTo("10"));
        Assert.That(actual.Name, Is.EqualTo("name"));
    }

    [Test]
    public void Facade_Rapplication_NewVar_creates_null_variable_when_object_is_null()
    {
        ModelsAre(
            Model("s-int-32").IsValue(),
            Model("s-string").IsValue());

        var actual = _testingRapplication.NewVar("name", RobjNull());
        Assert.That(actual.IsNull, Is.True);

        actual = _testingRapplication.NewVar("name", 0, "s-int-32");
        Assert.That(!actual.IsNull, Is.True);
        Assert.That(actual.Object.Id, Is.EqualTo("0"));

        // ReSharper disable ExpressionIsAlwaysNull
        string nullStringValue = null;
        actual = _testingRapplication.NewVar("name", nullStringValue, "s-string");
        Assert.That(actual.IsNull, Is.True);

        actual = _testingRapplication.NewVar("name", nullStringValue, o => o.ToString(CultureInfo.InvariantCulture), "s-string");
        Assert.That(actual.IsNull, Is.True);

        actual = _testingRapplication.NewVar("name", nullStringValue, o => _testingRapplication["s-string"].Get(o.ToString(CultureInfo.InvariantCulture)));
        Assert.That(actual.IsNull, Is.True);
        // ReSharper restore ExpressionIsAlwaysNull
    }

    [Test]
    public void Facade_Rapplication_NewVarList()
    {
        ModelsAre(Model("s-int-32").IsValue());

        var actual = _testingRapplication.NewVarList("name", new List<Robject> { Robj("10", "s-int-32"), Robj("11", "s-int-32") });

        Assert.That(actual.Name, Is.EqualTo("name"));
        Assert.That(actual.List[0].Id, Is.EqualTo("10"));
        Assert.That(actual.List[1].Id, Is.EqualTo("11"));

        actual = _testingRapplication.NewVarList("name", new List<int> { 10, 11 }, "s-int-32");

        Assert.That(actual.Name, Is.EqualTo("name"));
        Assert.That(actual.List[0].Id, Is.EqualTo("10"));
        Assert.That(actual.List[1].Id, Is.EqualTo("11"));

        actual = _testingRapplication.NewVarList("name", new List<int> { 10, 11 }, o => o.ToString(CultureInfo.InvariantCulture), "s-int-32");

        Assert.That(actual.Name, Is.EqualTo("name"));
        Assert.That(actual.List[0].Id, Is.EqualTo("10"));
        Assert.That(actual.List[1].Id, Is.EqualTo("11"));

        actual = _testingRapplication.NewVarList("name", new List<int> { 10, 11 }, o => _testingRapplication["s-int-32"].Get(o.ToString(CultureInfo.InvariantCulture)));

        Assert.That(actual.Name, Is.EqualTo("name"));
        Assert.That(actual.List[0].Id, Is.EqualTo("10"));
        Assert.That(actual.List[1].Id, Is.EqualTo("11"));
    }

    [Test]
    public void Facade_Rapplication_NewVarList_creates_null_variable_when_list_is_null()
    {
        ModelsAre(
            Model("s-int-32").IsValue(),
            Model("s-string").IsValue());

        var actual = _testingRapplication.NewVarList("name", new List<Robject> { RobjNull() });
        Assert.That(actual.List[0].IsNull, Is.True);

        actual = _testingRapplication.NewVarList("name", new List<int> { 0 }, "s-int-32");
        Assert.That(!actual.List[0].IsNull, Is.True);
        Assert.That(actual.List[0].Id, Is.EqualTo("0"));

        // ReSharper disable ExpressionIsAlwaysNull
        string nullString = null;
        actual = _testingRapplication.NewVarList("name", new List<string> { nullString }, "s-string");
        Assert.That(actual.List[0].IsNull, Is.True);

        actual = _testingRapplication.NewVarList("name", new List<string> { nullString }, o => o.ToString(CultureInfo.InvariantCulture), "s-string");
        Assert.That(actual.List[0].IsNull, Is.True);

        actual = _testingRapplication.NewVarList("name", new List<string> { nullString }, o => _testingRapplication["s-string"].Get(o.ToString(CultureInfo.InvariantCulture)));
        Assert.That(actual.List[0].IsNull, Is.True);
        // ReSharper restore ExpressionIsAlwaysNull
    }
}
