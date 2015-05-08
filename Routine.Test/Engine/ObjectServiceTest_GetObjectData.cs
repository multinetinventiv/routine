using System.Collections.Generic;
using NUnit.Framework;
using Routine.Core;
using Routine.Core.Configuration;
using Routine.Engine;
using Routine.Test.Engine.Domain.ObjectServiceTest_GetObjectData;

#region Test Model

namespace Routine.Test.Engine.Domain.ObjectServiceTest_GetObjectData
{
	public interface IBusinessDataWithNoImplementor { }

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
		public NotLocatable NotLocatable { get; set; }

		public void Operation() { }

		IBusinessData IBusinessData.SubData
		{
			get
			{
				return new BusinessData { Id = "sub_" + Id, Title = "Sub " + Title };
			}
		}

		public override string ToString() { return Title; }
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

	public class NotLocatable : IBusinessData
	{
		public string Title { get; set; }
		public IBusinessData SubData { get; set; }
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
		private const string VIEW_WITH_NO_IMPLEMENTOR_OMID = "Routine.Test.Engine.Domain.ObjectServiceTest_GetObjectData.IBusinessDataWithNoImplementor";
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
				.Use(p => p.PolymorphismPattern(t => t.IsDomainType))
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
		public void When_it_is_given_a_view_model_that_is_not_in_the_list_of_given_actual_model__cannot_convert_exception_is_thrown()
		{
			AddToRepository(new BusinessData { Id = "obj" });

			Assert.Throws<CannotConvertException>(() => testing.Get(Id("obj", ACTUAL_OMID, VIEW_WITH_NO_IMPLEMENTOR_OMID)));
		}
		[Test]
		public void When_it_given_actual_model_does_not_have_a_converter_for_its_view_model__configuration_exception_is_thrown()
		{
			codingStyle.Override(cs => cs
				.Converter.SetDefault()
			);

			AddToRepository(new BusinessData { Id = "obj" });

			Assert.Throws<ConfigurationException>(() => testing.Get(Id("obj", ACTUAL_OMID, VIEW_OMID)));
		}

		[Test]
		public void Locating_and_id_extraction_is_done_via_actual_model_id__even_if_there_exists_a_view_model_id()
		{
			codingStyle
				.IdExtractor.Set(c => c.Id(i => i.Constant("wrong")).When(type.of<IBusinessData>()))
				.ValueExtractor.Set(c => c.Value(v => v.Constant("dummy")).When(type.of<IBusinessData>()))
				.Locator.Set(c => c.Locator(l => l.Constant(new BusinessData { Id = "wrong" })).When(type.of<IBusinessData>()))
			;

			AddToRepository(new BusinessData { Id = "obj" });

			var actual = testing.Get(Id("obj", ACTUAL_OMID, VIEW_OMID));

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
		public void Value_is_extracted_using_corresponding_extractor_of_view_types()
		{
			codingStyle
				.ValueExtractor.Set(c => c.Value(v => v.Constant("view value")).When(type.of<IBusinessData>()))
			;

			AddToRepository(new BusinessData { Id = "obj", Title = "Obj Title" });

			var actual = testing.Get(Id("obj", ACTUAL_OMID, VIEW_OMID));

			Assert.AreEqual("view value", actual.Value);
		}

		[Test]
		public void When_view_type_does_not_have_a_value_extractor__extractor_of_actual_type_is_used()
		{
			codingStyle
				.ValueExtractor.Set(null, type.of<IBusinessData>())
			;
			AddToRepository(new BusinessData { Id = "obj", Title = "Obj Title" });

			var actual = testing.Get(Id("obj", ACTUAL_OMID, VIEW_OMID));

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
		public void When_value_extractor_of_view_type_is_used__view_target_is_used()
		{
			var obj = new BusinessData {Id = "obj"};
			var obj_converted = new BusinessData {Id = "obj_converted", Title = "Converted Obj Title"};

			codingStyle
				.Override(cs => cs
					.Converter.Set(c => c.Converter(cb => cb.Constant(obj_converted))
										 .When(type.of<BusinessData>()))
					.ValueExtractor.Set(c => c.Value(v => v.By(o => string.Format("{0}", o)))
											  .When(type.of<IBusinessData>()))
				)
			;

			AddToRepository(obj);

			var actual = testing.Get(Id("obj", ACTUAL_OMID, VIEW_OMID));

			Assert.AreEqual("Converted Obj Title", actual.Value);
		}

		[Test]
		public void When_value_extractor_of_actual_type_is_used__actual_target_is_used()
		{
			var obj = new BusinessData { Id = "obj", Title = "Obj Title" };
			var obj_converted = new BusinessData { Id = "obj_converted" };

			codingStyle
				.Converter.Set(c => c.Converter(cb => cb.Constant(obj_converted))
									 .When(type.of<BusinessData>()))
				.ValueExtractor.Set(null, type.of<IBusinessData>())
			;

			AddToRepository(obj);

			var actual = testing.Get(Id("obj", ACTUAL_OMID, VIEW_OMID));

			Assert.AreEqual("Obj Title", actual.Value);
		}

		[Test]
		public void Members_are_fetched_using_given_view_model_id()
		{
			AddToRepository(new BusinessData { Id = "obj", Title = "Obj Title" });

			var actual = testing.Get(Id("obj", ACTUAL_OMID, VIEW_OMID));

			Assert.IsTrue(actual.Members.ContainsKey("SubData"));
		}

		[Test]
		public void Members_are_fetched_using_view_target()
		{
			var obj = new BusinessData { Id = "obj", Title = "Obj Title" };
			var obj_converted = new BusinessData { Id = "obj_converted", Title = "Converted Obj Title" };

			codingStyle
				.Override(cs => cs
					.Converter.Set(c => c.Converter(cb => cb.Constant(obj_converted))
										 .When(type.of<BusinessData>()))
				)
			;

			AddToRepository(obj);

			var actual = testing.Get(Id("obj", ACTUAL_OMID, VIEW_OMID));
			var actual_subdata = actual.Members["SubData"].Values[0];

			Assert.AreEqual("sub_obj_converted", actual_subdata.Reference.Id);
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
			AddToRepository(new BusinessData { Id = "obj", Title = null });

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
			AddToRepository(new BusinessData
			{
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

		[Test]
		public void When_member_type_cannot_be_located__it_is_fetched_eagerly_no_matter_what_configuration_was_given()
		{
			codingStyle.MemberFetchedEagerly.Set(false, m => m.Returns<NotLocatable>("NotLocatable"));

			AddToRepository(new BusinessData { Id = "obj", NotLocatable = new NotLocatable { Title = "fetched eagerly" } });

			var actual = testing.Get(Id("obj"));
			var actualMemberValue = actual.Members["NotLocatable"].Values[0];

			Assert.IsTrue(actualMemberValue.Members.ContainsKey("Title"), "Member was not fetched eagerly");
			Assert.AreEqual("fetched eagerly", actualMemberValue.Members["Title"].Values[0].Reference.Id);
			Assert.AreEqual(string.Empty, actualMemberValue.Reference.Id);
		}

		[Test]
		public void When_member_type_is_locatable_but_actual_type_of_that_member_is_not_locatable__it_is_fetched_eagerly_no_matter_what_configuration_was_given()
		{
			codingStyle.MemberFetchedEagerly.Set(false, m => m.Returns<NotLocatable>("NotLocatable"));

			var sub_businessdata = new BusinessData { Id = "sub_businessdata" };
			var sub_notlocatable = new NotLocatable { SubData = sub_businessdata, Title = "sub not locatable" };
			var notlocatable = new NotLocatable { SubData = sub_notlocatable, Title = "notlocatable" };
			var root_businessdata = new BusinessData { Id = "root_businessdata", NotLocatable = notlocatable };

			AddToRepository(sub_businessdata);
			AddToRepository(root_businessdata);

			var actual_root_businessdata = testing.Get(Id("root_businessdata"));
			var actual_notlocatable = actual_root_businessdata.Members["NotLocatable"].Values[0];

			var actual_sub_notlocatable = actual_notlocatable.Members["SubData"].Values[0];

			Assert.AreEqual("sub not locatable", actual_sub_notlocatable.Value);
			Assert.IsTrue(actual_sub_notlocatable.Members.ContainsKey("SubData"), "Member was not fetched eagerly, instance was not locatable and supposed to be fetched eagerly");

			var actual_sub_businessdata = actual_sub_notlocatable.Members["SubData"].Values[0];

			Assert.AreEqual("sub_businessdata", actual_sub_businessdata.Reference.Id);
			Assert.IsFalse(actual_sub_businessdata.Members.ContainsKey("SubData"), "Member was fetched eagerly, instance was locatable and not supposed to be fetched eagerly");
		}

		[Test]
		public void Eager_fetching_is_allowed_to_a_max_depth_to_prevent_infinite_recursion()
		{
			codingStyle.MaxFetchDepth.Set(2);

			var notlocatable2 = new NotLocatable { Title = "notlocatable2" };
			var notlocatable1 = new NotLocatable { Title = "notlocatable1", SubData = notlocatable2 };
			var root_businessdata = new BusinessData { Id = "root_businessdata", NotLocatable = notlocatable1 };

			AddToRepository(root_businessdata);

			Assert.Throws<MaxFetchDepthExceededException>(() => testing.Get(Id("root_businessdata")));
		}
	}
}