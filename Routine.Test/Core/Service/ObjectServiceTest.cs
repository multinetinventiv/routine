using NUnit.Framework;

namespace Routine.Test.Core.Service.Domain.ObjectServiceTest
{
	public enum BusinessEnum
	{
		Item1,
		Item2,
		Item3
	}
}

namespace Routine.Test.Core.Service
{
	[TestFixture]
	public class ObjectServiceTest : ObjectServiceTestBase
	{
		protected override string DefaultModelId {get{return "Routine.Test.Core.Service.Domain.ObjectServiceTest.BusinessEnum";}}
		public override string[] DomainTypeRootNamespaces{get{return new[]{"Routine.Test.Core.Service.Domain.ObjectServiceTest"};}}

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			codingStyle
				.Use(p => p.EnumPattern())

				.ModelId.Done(s => s.SerializeBy(t => t.FullName).DeserializeBy(id => id.ToType()));

		}

		[Test]
		public void GetAvailableObjects_GetsAvailableObjectsDefinedByAvailableIdsExtractor()
		{
			var actual = testing.GetAvailableObjects(DefaultModelId);

			Assert.AreEqual(3, actual.Count);

			Assert.AreEqual("Item1", actual[0].Reference.Id);
			Assert.AreEqual("Item1", actual[0].Value);

			Assert.AreEqual("Item2", actual[1].Reference.Id);
			Assert.AreEqual("Item2", actual[1].Value);

			Assert.AreEqual("Item3", actual[2].Reference.Id);
			Assert.AreEqual("Item3", actual[2].Value);
		}
	}
}

