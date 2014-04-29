using System.Collections.Generic;
using NUnit.Framework;
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
		private const string VALUE_OMID = "Routine.Test.Core.Service.Domain.ObjectServiceTest_GetObjectData.BusinessValue";

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
		public void Object_is_located_via_configured_locator_and_its_id_is_extracted_using_corresponding_extractor()
		{
			AddToRepository(new BusinessData { Id = "obj" });

			var actual = testing.Get(Id("obj"));

			Assert.AreEqual("obj", actual.Reference.Id);
		}

		[Test]
		public void Display_value_is_extracted_using_corresponding_extractor()
		{
			AddToRepository(new BusinessData { Id = "obj", Title = "Obj Title" });

			var actual = testing.Get(Id("obj"));

			Assert.AreEqual("Obj Title", actual.Value);
		}

		[Test]
		public void Display_value_is_id_when_model_is_value_type()
		{
			var actual = testing.Get(Id("sample", VALUE_OMID));

			Assert.AreEqual("sample", actual.Reference.Id);
			Assert.AreEqual("sample", actual.Value);
		}

		[Test]
		public void Members_are_fetched_using_given_view_model_id()
		{
			AddToRepository(new BusinessData { Id = "obj", Title = "Obj Title" });

			var actual = testing.Get(Id("obj", ACTUAL_OMID, VIEW_OMID));

			Assert.IsTrue(actual.Members.ContainsKey("SubData"));
		}

		[Test]
		public void Member_display_values_are_fetched_along_with_their_ids()
		{
			AddToRepository(new BusinessData { Id = "obj", Title = "Obj Title" });

			var actual = testing.Get(Id("obj", ACTUAL_OMID, VIEW_OMID));
			var actualMember = actual.Members["SubData"];

			Assert.AreEqual("sub_obj", actualMember.Values[0].Reference.Id);
			Assert.AreEqual("Sub Obj Title", actualMember.Values[0].Value);
		}

		[Test]
		public void Object_is_marked_as_null_if_it_is_null()
		{
			AddToRepository(new BusinessData { Id = "obj", Title = null});

			var actual = testing.Get(Id("obj"));
			var actualMember = actual.Members["Title"];

			Assert.IsTrue(actualMember.Values[0].Reference.IsNull);
		}

		[Test]
		public void When_getting_object__only_light_members_are_fetched()
		{
			AddToRepository(new BusinessData { Id = "obj", Title = "Obj Title", Items = new List<string>{ "obj_item1", "obj_item2" } });

			var actual = testing.Get(Id("obj"));

			Assert.IsFalse(actual.Members.ContainsKey("Items"));
		}

		[Test]
		public void Heavy_members_can_be_fetched_separately()
		{
			AddToRepository(new BusinessData { Id = "obj", Title = "Obj Title", Items = new List<string>{ "obj_item1", "obj_item2" } });

			var actual = testing.GetMember(Id("obj"), "Items");

			Assert.IsTrue(actual.IsList);
			Assert.AreEqual("obj_item1", actual.Values[0].Reference.Id);
			Assert.AreEqual("obj_item2", actual.Values[1].Reference.Id);
		}

		[Test]
		public void When_wanted_member_does_not_exist__exception_is_thrown()
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
		public void By_default_get_member_returns_eager_result()
		{
			AddToRepository(new BusinessData { Id = "obj", Title = "Obj Title" });

			var actual = testing.GetMember(Id("obj", ACTUAL_OMID, VIEW_OMID), "SubData");

			Assert.IsTrue(actual.Values[0].Members.ContainsKey("SubData"));
			Assert.AreEqual("sub_sub_obj", actual.Values[0].Members["SubData"].Values[0].Reference.Id);
			Assert.AreEqual("Sub Sub Obj Title", actual.Values[0].Members["SubData"].Values[0].Value);
		}
	}
}

