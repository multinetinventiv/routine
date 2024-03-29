using Routine.Test.Engine.Domain.LaterAdded;
using Routine.Test.Engine.Domain.ObjectServiceTest_GetApplicationModel;

#region Test Model

namespace Routine.Test.Engine.Domain.LaterAdded
{
    public class BusinessModel4
    {
        public string Name { get; set; }
    }
}
namespace Routine.Test.Engine.Domain.ObjectServiceTest_GetApplicationModel
{
    public class BusinessModel1
    {
        // ReSharper disable once UnusedType.Local
        private class BusinessModel3 { }

        public string GetString(int i) { return null; }

        public BusinessModel4 PropertyWithLaterAddedType { get; set; }
    }

    public interface IBusinessModel2 { }
}

#endregion

namespace Routine.Test.Engine
{
    [TestFixture]
    public class ObjectServiceTest_ApplicationModel : ObjectServiceTestBase
    {
        #region Setup & Helpers

        protected override string DefaultModelId => typeof(BusinessModel1).FullName;
        protected override string RootNamespace => "Routine.Test.Engine.Domain.ObjectServiceTest_GetApplicationModel";

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        }

        #endregion

        [Test]
        public void Iterates_through_all_given_types_and_creates_ObjectModel_for_each_type()
        {
            var actual = _testing.ApplicationModel;

            Assert.That(actual.Models.Any(m => m.Id == typeof(BusinessModel1).FullName), Is.True, $"BusinessModel1 not found in {actual.Models.ToItemString()}");
            Assert.That(actual.Models.Any(m => m.Id == typeof(IBusinessModel2).FullName), Is.True, $"BusinessModel2 not found in {actual.Models.ToItemString()}");
            Assert.That(actual.Models.Any(m => m.Id.EndsWith("BusinesssModel3")), Is.False, $"BusinessModel3 is private, shouldn't be in {actual.Models.ToItemString()}");
        }

        [Test]
        public void Does_not_include_types_that_were_not_added()
        {
            var actual = _testing.ApplicationModel;

            Assert.That(actual.Models.Any(m => m.Id == "Int64"), Is.False, $"Int64 found in {actual.Models.ToItemString()}");
        }

        [Test]
        public void Application_model_should_be_cached()
        {
            var expected = _testing.ApplicationModel;
            var actual = _testing.ApplicationModel;

            Assert.That(actual, Is.SameAs(expected));
        }

        [Test]
        public void Later_added_types_cause_an_invalidation_on_cached_types()
        {
            _codingStyle.AddTypes(typeof(BusinessModel4));

            var actual = _testing.ApplicationModel.Models.First(m => m.Name == "BusinessModel1");

            Assert.That(actual.Datas.Any(m => m.Name == "PropertyWithLaterAddedType"), Is.True);
        }
    }
}
