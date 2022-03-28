using NUnit.Framework;
using Routine.Client;
using System.Collections.Generic;
using System.Globalization;

namespace Routine.Test.Client
{
    [TestFixture]
    public class FacadeTest : ClientTestBase
    {
        [Test]
        public void Facade_Rvariable_As()
        {
            ModelsAre(Model("s-int-32").IsValue());

            int result = Rvar("value", Robj("10", "s-int-32")).As(robj => int.Parse(robj.Display));

            Assert.AreEqual(10, result);
        }

        [Test]
        public void Facade_Rvariable_As_Returns_default_when_value_is_null()
        {
            int intResult = Rvar("value", RobjNull()).As(robj => int.Parse(robj.Display));

            Assert.AreEqual(0, intResult);

            string stringResult = Rvar("value", RobjNull()).As(robj => robj.Display);

            Assert.IsNull(stringResult);
        }

        [Test]
        public void Facade_Rvariable_AsList()
        {
            ModelsAre(Model("s-int-32").IsValue());

            List<int> result = Rvarlist("value", new[] { Robj("10", "s-int-32"), Robj("11", "s-int-32") }).AsList(robj => int.Parse(robj.Display));

            Assert.AreEqual(10, result[0]);
            Assert.AreEqual(11, result[1]);
        }

        [Test]
        public void Facade_Rvariable_AsList_Puts_default_value_when_an_item_is_null()
        {
            List<int> intResult = Rvarlist("value", new[] { RobjNull() }).AsList(robj => int.Parse(robj.Display));

            Assert.AreEqual(0, intResult[0]);

            List<string> stringResult = Rvarlist("value", new[] { RobjNull() }).AsList(robj => robj.Display);

            Assert.IsNull(stringResult[0]);
        }

        [Test]
        public void Facade_Rapplication_NewVar()
        {
            ModelsAre(Model("s-int-32").IsValue());

            var actual = testingRapplication.NewVar("name", Robj("10", "s-int-32"));

            Assert.AreEqual("10", actual.Object.Id);
            Assert.AreEqual("name", actual.Name);

            actual = testingRapplication.NewVar("name", 10, "s-int-32");

            Assert.AreEqual("10", actual.Object.Id);
            Assert.AreEqual("name", actual.Name);

            actual = testingRapplication.NewVar("name", 10, o => o.ToString(CultureInfo.InvariantCulture), "s-int-32");

            Assert.AreEqual("10", actual.Object.Id);
            Assert.AreEqual("name", actual.Name);

            actual = testingRapplication.NewVar("name", 10, o => testingRapplication["s-int-32"].Get(o.ToString(CultureInfo.InvariantCulture)));

            Assert.AreEqual("10", actual.Object.Id);
            Assert.AreEqual("name", actual.Name);
        }

        [Test]
        public void Facade_Rapplication_NewVar_creates_null_variable_when_object_is_null()
        {
            ModelsAre(
                Model("s-int-32").IsValue(),
                Model("s-string").IsValue());

            var actual = testingRapplication.NewVar("name", RobjNull());
            Assert.IsTrue(actual.IsNull);

            actual = testingRapplication.NewVar("name", 0, "s-int-32");
            Assert.IsTrue(!actual.IsNull);
            Assert.AreEqual("0", actual.Object.Id);

            // ReSharper disable ExpressionIsAlwaysNull
            string nullStringValue = null;
            actual = testingRapplication.NewVar("name", nullStringValue, "s-string");
            Assert.IsTrue(actual.IsNull);

            actual = testingRapplication.NewVar("name", nullStringValue, o => o.ToString(CultureInfo.InvariantCulture), "s-string");
            Assert.IsTrue(actual.IsNull);

            actual = testingRapplication.NewVar("name", nullStringValue, o => testingRapplication["s-string"].Get(o.ToString(CultureInfo.InvariantCulture)));
            Assert.IsTrue(actual.IsNull);
            // ReSharper restore ExpressionIsAlwaysNull
        }

        [Test]
        public void Facade_Rapplication_NewVarList()
        {
            ModelsAre(Model("s-int-32").IsValue());

            var actual = testingRapplication.NewVarList("name", new List<Robject> { Robj("10", "s-int-32"), Robj("11", "s-int-32") });

            Assert.AreEqual("name", actual.Name);
            Assert.AreEqual("10", actual.List[0].Id);
            Assert.AreEqual("11", actual.List[1].Id);

            actual = testingRapplication.NewVarList("name", new List<int> { 10, 11 }, "s-int-32");

            Assert.AreEqual("name", actual.Name);
            Assert.AreEqual("10", actual.List[0].Id);
            Assert.AreEqual("11", actual.List[1].Id);

            actual = testingRapplication.NewVarList("name", new List<int> { 10, 11 }, o => o.ToString(CultureInfo.InvariantCulture), "s-int-32");

            Assert.AreEqual("name", actual.Name);
            Assert.AreEqual("10", actual.List[0].Id);
            Assert.AreEqual("11", actual.List[1].Id);

            actual = testingRapplication.NewVarList("name", new List<int> { 10, 11 }, o => testingRapplication["s-int-32"].Get(o.ToString(CultureInfo.InvariantCulture)));

            Assert.AreEqual("name", actual.Name);
            Assert.AreEqual("10", actual.List[0].Id);
            Assert.AreEqual("11", actual.List[1].Id);
        }

        [Test]
        public void Facade_Rapplication_NewVarList_creates_null_variable_when_list_is_null()
        {
            ModelsAre(
                Model("s-int-32").IsValue(),
                Model("s-string").IsValue());

            var actual = testingRapplication.NewVarList("name", new List<Robject> { RobjNull() });
            Assert.IsTrue(actual.List[0].IsNull);

            actual = testingRapplication.NewVarList("name", new List<int> { 0 }, "s-int-32");
            Assert.IsTrue(!actual.List[0].IsNull);
            Assert.AreEqual("0", actual.List[0].Id);

            // ReSharper disable ExpressionIsAlwaysNull
            string nullString = null;
            actual = testingRapplication.NewVarList("name", new List<string> { nullString }, "s-string");
            Assert.IsTrue(actual.List[0].IsNull);

            actual = testingRapplication.NewVarList("name", new List<string> { nullString }, o => o.ToString(CultureInfo.InvariantCulture), "s-string");
            Assert.IsTrue(actual.List[0].IsNull);

            actual = testingRapplication.NewVarList("name", new List<string> { nullString }, o => testingRapplication["s-string"].Get(o.ToString(CultureInfo.InvariantCulture)));
            Assert.IsTrue(actual.List[0].IsNull);
            // ReSharper restore ExpressionIsAlwaysNull
        }
    }
}
