using NUnit.Framework;
using Routine.Test.Core.Domain.ObjectServiceTest_GetApplicationModel;
using System.Linq;

namespace Routine.Test.Core.Domain.ObjectServiceTest_GetApplicationModel
{
	public class BusinessModel1 {
		private class BusinessModel3 {}
	}
	public interface IBusinessModel2 {}

}

namespace Routine.Test.Core.Service
{
	[TestFixture]
	public class ObjectServiceTest_GetApplicationModel : ObjectServiceTestBase
	{
		protected override string DefaultModelId {get{return typeof(BusinessModel1).FullName;}}
		public override string[] DomainTypeRootNamespaces{get{return new[]{"Routine.Test.Core.Domain.ObjectServiceTest_GetApplicationModel"};}}

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			codingStyle
				.SerializeModelId.Done(s => s.SerializeBy(t => t.FullName)
									.SerializeWhen(t => t.IsDomainType)
									.DeserializeBy(id => id.ToType()))
				;
		}

		[Test]
		public void IteratesThroughAllTypesInAllReferencedAssembliesAndCreatesObjectModelForEachTypeExceptValueTypesThatCanBeSerializedAsModelId()
		{
			var actual = testing.GetApplicationModel();

			Assert.IsTrue(actual.Models.Any(m => m.Id.EndsWith("BusinessModel1")), "BusinessModel1 not found in " + actual.Models.ToItemString());
			Assert.IsTrue(actual.Models.Any(m => m.Id.EndsWith("BusinessModel2")), "BusinessModel2 not found in " + actual.Models.ToItemString());
			Assert.IsTrue(actual.Models.All(m => !m.Id.EndsWith("BusinessModel3")), "BusinessModel3 is private, shouldn't be in " + actual.Models.ToItemString());
		}

		[Test]
		public void ApplicationModelShouldBeCached()
		{
			var expected = testing.GetApplicationModel();
			var actual = testing.GetApplicationModel();

			Assert.AreSame(expected, actual);
		}
	}
}

