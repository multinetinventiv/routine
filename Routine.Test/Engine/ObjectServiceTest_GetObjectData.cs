using System.Collections.Generic;
using NUnit.Framework;
using Routine.Test.Engine.Domain.ObjectServiceTest_GetObjectData;

#region Test Model

namespace Routine.Test.Engine.Domain.ObjectServiceTest_GetObjectData
{
	public interface IBusinessData
	{
		IBusinessData SubData { get; }
	}

	public class BusinessData : IBusinessData
	{
		public string Id { get; set; }
		public string Title { get; set; }
		public List<string> Items { get; set; }
		public List<BusinessData> SubDatas { get; set; }

		public void Operation() { }

		IBusinessData IBusinessData.SubData
		{
			get
			{
				return new BusinessData { Id = "sub_" + Id, Title = "Sub " + Title };
			}
		}
	}

	public struct BusinessValue
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

#endregion

namespace Routine.Test.Engine
{
	[TestFixture]
	public class ObjectServiceTest_GetObjectData : ObjectServiceTestBase
	{
		#region Setup & Helpers

		private const string ACTUAL_OMID = "Routine.Test.Engine.Domain.ObjectServiceTest_GetObjectData.BusinessData";
		private const string VIEW_OMID = "Routine.Test.Engine.Domain.ObjectServiceTest_GetObjectData.IBusinessData";
		private const string VALUE_OMID = "Routine.Test.Engine.Domain.ObjectServiceTest_GetObjectData.BusinessValue";

		protected override string DefaultModelId { get { return ACTUAL_OMID; } }
		protected override string RootNamespace { get { return "Routine.Test.Engine.Domain.ObjectServiceTest_GetObjectData"; } }

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			codingStyle
				.TypeId.Set(c => c.By(t => t.FullName))
				.ValueExtractor.Set(c => c.ValueByMember(m => m.Returns<string>("Title")))
				;
		} 

		#endregion

		[Test]
		public void Object_is_located_via_configured_locator_and_its_id_is_extracted_using_corresponding_extractor()
		{
			AddToRepository(new BusinessData { Id = "obj" });

			var actual = testing.Get(Id("obj"));

			Assert.AreEqual("obj", actual.Reference.Id);
		}

		[Test]
		public void Value_is_extracted_using_corresponding_extractor()
		{
			AddToRepository(new BusinessData { Id = "obj", Title = "Obj Title" });

			var actual = testing.Get(Id("obj"));

			Assert.AreEqual("Obj Title", actual.Value);
		}

		[Test]
		public void Value_is_id_when_model_is_value_type()
		{
			testing.GetApplicationModel();

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
		public void Members_are_fetched_eagerly_when_configured_so()
		{
			codingStyle
				.MemberFetchedEagerly.Set(true, m => m.Name == "SubDatas")
				;

			AddToRepository(new BusinessData { Id = "sub1", Items = new List<string> { "sub1_1", "sub1_2" } });
			AddToRepository(new BusinessData { Id = "sub2", Items = new List<string> { "sub2_1", "sub2_2" } });
			AddToRepository(new BusinessData { 
				Id = "obj", 
				SubDatas = new List<BusinessData> { 
					objectRepository["sub1"] as BusinessData, 
					objectRepository["sub2"] as BusinessData 
				} 
			});

			var actual = testing.Get(Id("obj"));
			var actualMember = actual.Members["SubDatas"];

			Assert.AreEqual("sub1_1", actualMember.Values[0].Members["Items"].Values[0].Reference.Id);
			Assert.AreEqual("sub1_2", actualMember.Values[0].Members["Items"].Values[1].Reference.Id);
			Assert.AreEqual("sub2_1", actualMember.Values[1].Members["Items"].Values[0].Reference.Id);
			Assert.AreEqual("sub2_2", actualMember.Values[1].Members["Items"].Values[1].Reference.Id);
		}
	}
}