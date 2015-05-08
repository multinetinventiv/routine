using System.Linq;
using NUnit.Framework;
using Routine.Core.Configuration;
using Routine.Test.Engine.Domain.LaterAdded;
using Routine.Test.Engine.Domain.ObjectServiceTest_GetApplicationModel;

#region Teset Model

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
	public class ObjectServiceTest_GetApplicationModel : ObjectServiceTestBase
	{
		#region Setup & Helpers

		protected override string DefaultModelId { get { return typeof(BusinessModel1).FullName; } }
		protected override string RootNamespace { get { return "Routine.Test.Engine.Domain.ObjectServiceTest_GetApplicationModel"; } }

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			codingStyle
				.TypeId.Set(s => s.By(t => t.FullName).When(t => t.IsDomainType))
				;
		} 

		#endregion

		[Test]
		public void Iterates_through_all_given_types_and_creates_ObjectModel_for_each_type()
		{
			var actual = testing.GetApplicationModel();

			Assert.IsTrue(actual.Models.Any(m => m.Id == typeof(BusinessModel1).FullName), "BusinessModel1 not found in " + actual.Models.ToItemString());
			Assert.IsTrue(actual.Models.Any(m => m.Id == typeof(IBusinessModel2).FullName), "BusinessModel2 not found in " + actual.Models.ToItemString());
			Assert.IsFalse(actual.Models.Any(m => m.Id.EndsWith("BusinesssModel3")), "BusinessModel3 is private, shouldn't be in " + actual.Models.ToItemString());
		}

		[Test]
		public void Includes_non_domain_types_that_have_a_type_id()
		{
			var actual = testing.GetApplicationModel();

			Assert.IsTrue(actual.Models.Any(m => m.Id == "s-string"), "string not found in " + actual.Models.ToItemString());
			Assert.IsTrue(actual.Models.Any(m => m.Id == "s-int-32"), "int not found in " + actual.Models.ToItemString());
			Assert.IsFalse(actual.Models.Any(m => m.Id == "s-date-time"), "DateTime shouldn't be in " + actual.Models.ToItemString());
		}

		[Test]
		public void Application_model_should_be_cached()
		{
			var expected = testing.GetApplicationModel();
			var actual = testing.GetApplicationModel();

			Assert.AreSame(expected, actual);
		}

		[Test]
		public void Later_added_types_cause_an_invalidation_on_cached_types()
		{
			codingStyle.AddTypes(typeof (BusinessModel4));

			var actual = testing.GetApplicationModel().Models.First(m => m.Name == "BusinessModel1");

			Assert.IsTrue(actual.Members.Any(m => m.Id == "PropertyWithLaterAddedType"));
		}

		[Test]
		public void Model_ids_are_not_allowed_to_start_with_the_ref_splitter_character_to_prevent_deserialization_problems()
		{
			codingStyle
				.Override(c => c.TypeId.Set("#test", type.of<string>()))
				;
			Assert.Throws<ConfigurationException>(() => testing.GetApplicationModel());
		}
	}
}