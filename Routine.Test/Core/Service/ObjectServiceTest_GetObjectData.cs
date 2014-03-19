using System.Collections.Generic;
using NUnit.Framework;
using Routine.Core.Service;
using System.Linq;
using Routine.Core.Service.Impl;
using Routine.Test.Core.Service.Domain.ObjectServiceTest_GetObjectData;

namespace Routine.Test.Core.Service.Domain.ObjectServiceTest_GetObjectData
{
	public interface IBusinessData
	{
		IBusinessData SubData{get;}
	}

	public class BusinessData : IBusinessData
	{
		public string Id {get;set;}
		public string Title{get;set;}
		public List<string> Items{get;set;}

		public void Operation() {}
		public List<string> HeavyOperation() {return null;}

		IBusinessData IBusinessData.SubData
		{
			get
			{
				return new BusinessData{Id = "sub_" + Id, Title = "Sub " + Title};
			}
		}
	}

	public class BusinessValue
	{
		public static BusinessValue Parse(string value)
		{
			return new BusinessValue(value);
		}

		private readonly string value;
		public BusinessValue(string value)
		{
			this.value = value;
		}

		public override string ToString()
		{
			return value;
		}
	}
}

namespace Routine.Test.Core.Service
{
	[TestFixture]
	public class ObjectServiceTest_GetObjectData : ObjectServiceTestBase
	{
		private const string ACTUAL_OMID = "Routine.Test.Core.Service.Domain.ObjectServiceTest_GetObjectData.BusinessData";
		private const string VIEW_OMID = "Routine.Test.Core.Service.Domain.ObjectServiceTest_GetObjectData.IBusinessData";
		private const string VALUE_OMID = ":Routine.Test.Core.Service.Domain.ObjectServiceTest_GetObjectData.BusinessValue";

		protected override string DefaultModelId { get { return ACTUAL_OMID; } }
		public override string[] DomainTypeRootNamespaces{get{return new[]{"Routine.Test.Core.Service.Domain.ObjectServiceTest_GetObjectData"};}}

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			codingStyle
				.SerializeModelId.Done(s => s.SerializeBy(t => t.FullName).DeserializeBy(id => id.ToType()))

				.ExtractDisplayValue.Done(e => e.ByProperty(p => p.Returns<string>("Title")))

				.ExtractMemberIsHeavy.Done(e => e.ByConverting(m => m.ReturnsCollection()))
				.ExtractOperationIsHeavy.Done(e => e.ByConverting(o => o.ReturnsCollection()))
				;
		}

		[Test]
		public void ObjectIsLocatedViaConfiguredLocatorAndItsIdIsExtractedUsingCorrespondingExtractor()
		{
			AddToRepository(new BusinessData { Id = "obj" });

			var actual = testing.Get(Id("obj"));

			Assert.AreEqual("obj", actual.Reference.Id);
		}

		[Test]
		public void DisplayValueIsExtractedUsingCorrespondingExtractor()
		{
			AddToRepository(new BusinessData { Id = "obj", Title = "Obj Title" });

			var actual = testing.Get(Id("obj"));

			Assert.AreEqual("Obj Title", actual.Value);
		}

		[Test]
		public void DisplayValueIsIdWhenModelIsValueType()
		{
			var actual = testing.Get(Id("sample", VALUE_OMID));

			Assert.AreEqual("sample", actual.Reference.Id);
			Assert.AreEqual("sample", actual.Value);
		}

		[Test]
		public void MembersAreFetchedUsingGivenViewModelId()
		{
			AddToRepository(new BusinessData { Id = "obj", Title = "Obj Title" });

			var actual = testing.Get(Id("obj", ACTUAL_OMID, VIEW_OMID));

			Assert.IsTrue(actual.Members.Any(m => m.ModelId == "SubData"));
		}

		[Test]
		public void MemberDisplayValuesAreFetchedAlongWithTheirIds()
		{
			AddToRepository(new BusinessData { Id = "obj", Title = "Obj Title" });

			var actual = testing.Get(Id("obj", ACTUAL_OMID, VIEW_OMID));
			var actualMember = actual.Members.Single(m => m.ModelId == "SubData");

			Assert.AreEqual("sub_obj", actualMember.Value.Values[0].Reference.Id);
			Assert.AreEqual("Sub Obj Title", actualMember.Value.Values[0].Value);
		}

		[Test]
		public void ObjectIsMarkedAsNullIfItIsNull()
		{
			AddToRepository(new BusinessData { Id = "obj", Title = null});

			var actual = testing.Get(Id("obj"));
			var actualMember = actual.Members.Single(m => m.ModelId == "Title");

			Assert.IsTrue(actualMember.Value.Values[0].Reference.IsNull);
		}

		[Test]
		public void WhenGettingObjectOnlyLightMembersAreFetched()
		{
			AddToRepository(new BusinessData { Id = "obj", Title = "Obj Title", Items = new List<string>{ "obj_item1", "obj_item2" } });

			var actual = testing.Get(Id("obj"));

			Assert.IsFalse(actual.Members.Any(m => m.ModelId == "Items"));
		}

		[Test]
		public void HeavyMembersCanBeFetchedSeparately()
		{
			AddToRepository(new BusinessData { Id = "obj", Title = "Obj Title", Items = new List<string>{ "obj_item1", "obj_item2" } });

			var actual = testing.GetMember(Id("obj"), "Items");

			Assert.IsTrue(actual.Value.IsList);
			Assert.AreEqual("obj_item1", actual.Value.Values[0].Reference.Id);
			Assert.AreEqual("obj_item2", actual.Value.Values[1].Reference.Id);
		}

		[Test]
		public void WhenWantedMemberDoesNotExistMemberDoesNotExistExceptionIsThrown()
		{
			AddToRepository(new BusinessData { Id = "obj" });

			try
			{
				testing.GetMember(Id("obj"), "NonExistingMember");
				Assert.Fail("exception not thrown");
			}
			catch(MemberDoesNotExistException){}
		}

		[Test]
		public void OperationsAreFetchedUsingGivenViewModelId()
		{
			AddToRepository(new BusinessData { Id = "obj"});

			var actual = testing.Get(Id("obj"));
			var actualOperation = actual.Operations.SingleOrDefault(o => o.ModelId == "Operation");

			Assert.IsNotNull(actualOperation);
			Assert.AreEqual(0, actualOperation.Parameters.Count);
		}

		[Test]
		public void WhenGettingObjectOnlyLightOperationsAreFetched()
		{
			AddToRepository(new BusinessData { Id = "obj"});

			var actual = testing.Get(Id("obj"));

			Assert.IsFalse(actual.Operations.Any(o => o.ModelId == "HeavyOperation"));
		}

		[Test]
		public void HeavyOperationsCanBeFetchedSeparately()
		{
			AddToRepository(new BusinessData { Id = "obj"});

			var actual = testing.GetOperation(Id("obj"), "HeavyOperation");

			Assert.AreEqual("HeavyOperation", actual.ModelId);
			Assert.AreEqual(0, actual.Parameters.Count);
		}

		[Test]
		public void WhenWantedOperationDoesNotExistOperationDoesNotExistExceptionIsThrown()
		{
			AddToRepository(new BusinessData { Id = "obj" });

			try
			{
				testing.GetOperation(Id("obj"), "NonExistingOperation");
				Assert.Fail("exception not thrown");
			}
			catch(OperationDoesNotExistException){}
		}

		[Test] [Ignore]
		public void GetMemberAsTableFeature()
		{
			Assert.Fail("not implemented");
		}
	}
}

