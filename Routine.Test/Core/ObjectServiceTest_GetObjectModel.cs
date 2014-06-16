using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Routine.Core.Configuration;
using Routine.Core.DomainApi;
using Routine.Test.Core.Domain.ObjectServiceTest_GetObjectModel;
using Routine.Test.Core.Ignored;

#region Test Model

namespace Routine.Test.Core.Ignored
{
	public class IgnoredModel { }
}

namespace Routine.Test.Core.Domain.ObjectServiceTest_GetObjectModel
{
	public class BusinessModel
	{
		public int Id { get; set; }
		public List<string> List { get; set; }

		public void VoidOp() { }
		public string StringOp(List<string> list) { return null; }
		public List<string> ListOp(bool boolParam) { return null; }

		public void OverloadOp() { }
		public void OverloadOp(string s) { }
		public void OverloadOp(int i) { }
		public void OverloadOp(string s, int i) { }
		public void OverloadOp(string s1, string s, int i1) { }
		public string OverloadOp(bool b) { return null; }
		public void OverloadOp(bool s, string s1) { }

		public IgnoredModel IgnoredMember { get; set; }
		public IgnoredModel IgnoredForReturnType() { return null; }
		public void IgnoredForParameter(IgnoredModel ignoreReason) { }
	}

	public interface IBusinessModel { }

	public class BusinessValueModel
	{
		public static BusinessValueModel Parse(string value) { return null; }

		public string AutoExcludedMember { get; set; }
		public void AutoExcludedOperation() { }
	}

	public struct BusinessDataModel
	{
		public string Data { get; private set; }

		public BusinessDataModel(string data) : this() { Data = data; }
		public BusinessDataModel(string data, int i) : this(data) { }
		public BusinessDataModel(int data) : this() { }
		public BusinessDataModel(IgnoredModel ignoredModel) : this() { }
	}
} 

#endregion

namespace Routine.Test.Core.Service
{
	[TestFixture]
	public class ObjectServiceTest_GetObjectModel : ObjectServiceTestBase
	{
		#region Setup & Helpers

		private const string TESTED_OM_ID = "r-object-service-test_Get-object-model--business-model";
		private const string TESTED_VAM_ID = "r-object-service-test_Get-object-model--business-value-model";
		private const string TESTED_VIM_ID = "r-object-service-test_Get-object-model--i-business-model";
		private const string TESTED_DM_ID = "r-object-service-test_Get-object-model--business-data-model";

		protected override string DefaultModelId { get { return TESTED_OM_ID; } }
		public override string[] DomainTypeRootNamespaces { get { return new[] { "Routine.Test.Core.Domain.ObjectServiceTest_GetObjectModel" }; } }

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			codingStyle
				.Use(p => p.ShortModelIdPattern("Routine.Test.Core.Domain", "r"))
				.ExtractModelModule.Done(e => e.Always("Test"))
				;
		} 

		#endregion

		[Test]
		public void Model_id_is_created_using_given_serializer()
		{
			var actual = testing.GetObjectModel(TESTED_OM_ID);

			Assert.AreEqual("r-object-service-test_Get-object-model--business-model", actual.Id);
		}

		[Test]
		public void Object_model_is_marked_with_given_selectors()
		{
			codingStyle
				.SelectModelMarks.Done(s => s.Always("value").When(t => t.CanParse()))
				.SelectModelMarks.Done(s => s.Always("all"))
				;

			var om = testing.GetObjectModel(TESTED_OM_ID);
			var vam = testing.GetObjectModel(TESTED_VAM_ID);

			Assert.IsFalse(om.Marks.Any(m => m == "value"));
			Assert.IsTrue(vam.Marks.Any(m => m == "value"));

			Assert.IsTrue(om.Marks.Any(m => m == "all"));
			Assert.IsTrue(vam.Marks.Any(m => m == "all"));
		}

		[Test]
		public void Object_model_is_value_model_if_corresponding_extractor_says_so()
		{
			var actual = testing.GetObjectModel("s-string");

			Assert.IsTrue(actual.IsValueModel);
		}

		[Test]
		public void Value_models_do_not_have_member_or_operation()
		{
			var actual = testing.GetObjectModel(TESTED_VAM_ID);

			Assert.AreEqual(0, actual.Members.Count);
			Assert.AreEqual(0, actual.Operations.Count);
		}

		[Test]
		public void Object_model_is_view_model_if_corresponding_extractor_says_so()
		{
			var actual = testing.GetObjectModel(TESTED_VIM_ID);

			Assert.IsTrue(actual.IsViewModel);
		}

		[Test]
		public void Name_is_the_name_with_corresponding_type_name()
		{
			var actual = testing.GetObjectModel(TESTED_OM_ID);

			Assert.AreEqual("BusinessModel", actual.Name);
		}

		[Test]
		public void Module_is_extracted_using_corresponding_extractor()
		{
			var actual = testing.GetObjectModel(TESTED_OM_ID);

			Assert.AreEqual("Test", actual.Module);
		}

		[Test]
		public void Initializer_list_is_created_using_given_selector()
		{
			var actual = testing.GetObjectModel(TESTED_DM_ID);

			Assert.Greater(actual.Initializer.GroupCount, 0);
		}

		[Test]
		public void An_initializer_is_marked_with_given_selectors()
		{
			codingStyle
				.SelectInitializerMarks.Done(s => s.Always("parametric").When(i => !i.HasNoParameters()))
				;

			var actual = testing.GetObjectModel(TESTED_DM_ID);

			Assert.IsTrue(actual.Initializer.Marks.Contains("parametric"));
		}

		[Test]
		public void Initializers_with_non_matching_InitializedType_value_and_any_unrecognized_parameter_ViewModelIds_are_ignored_automatically()
		{
			codingStyle
				.SelectInitializers.Done(s => s.Always(new ConstructorInitializer(this.GetTypeInfo().GetConstructor())))
				;

			var actual = testing.GetObjectModel(TESTED_DM_ID);

			Assert.AreEqual(typeof(BusinessDataModel).GetConstructors().Length - 2, actual.Initializer.GroupCount);
		}

		[Test]
		public void Member_list_is_created_using_given_selector()
		{
			var actual = testing.GetObjectModel(TESTED_OM_ID);

			Assert.IsTrue(actual.Members.Any(m => m.Id == "Id"));
			Assert.AreEqual("s-int-32", actual.Members.Single(m => m.Id == "Id").ViewModelId);
			Assert.IsTrue(actual.Members.Any(m => m.Id == "List"));
			Assert.AreEqual("s-string", actual.Members.Single(m => m.Id == "List").ViewModelId);
		}

		[Test]
		public void A_member_is_marked_with_given_selectors()
		{
			codingStyle
				.SelectMemberMarks.Done(s => s.Always("heavy").When(m => m.ReturnsCollection()))
				;

			var om = testing.GetObjectModel(TESTED_OM_ID);

			Assert.IsFalse(om.Members.Single(m => m.Id == "Id").Marks.Any(m => m == "heavy"));
			Assert.IsTrue(om.Members.Single(m => m.Id == "List").Marks.Any(m => m == "heavy"));
		}

		[Test]
		public void Members_automatically_marked_as_list_when_their_type_conforms_to_a_generic_collection()
		{
			var actual = testing.GetObjectModel(TESTED_OM_ID);

			Assert.IsFalse(actual.Members.Single(m => m.Id == "Id").IsList);
			Assert.IsTrue(actual.Members.Single(m => m.Id == "List").IsList);
 		}

		[Test]
		public void Members_with_unrecognized_ViewModelIds_are_ignored_automatically()
		{
			var actual = testing.GetObjectModel(TESTED_OM_ID);

			Assert.IsFalse(actual.Members.Any(m => m.ViewModelId.Contains("ignored")));
		}

		[Test]
		public void Operation_list_is_created_using_given_selector()
		{
			var actual = testing.GetObjectModel(TESTED_OM_ID);

			Assert.IsTrue(actual.Operations.Any(o => o.Id == "VoidOp"));
			Assert.AreEqual(Constants.VOID_MODEL_ID, actual.Operations.Single(o => o.Id == "VoidOp").Result.ViewModelId);
			Assert.IsTrue(actual.Operations.Any(o => o.Id == "StringOp"));
			Assert.AreEqual("s-string", actual.Operations.Single(o => o.Id == "StringOp").Result.ViewModelId);
			Assert.IsTrue(actual.Operations.Any(o => o.Id == "ListOp"));
			Assert.AreEqual("s-string", actual.Operations.Single(o => o.Id == "ListOp").Result.ViewModelId);
		}

		[Test]
		public void An_operation_is_marked_with_given_selectors()
		{
			codingStyle
				.SelectOperationMarks.Done(s => s.Always("heavy").When(o => o.Parameters.Any(p => p.ParameterType.CanBeCollection())))
				;

			var om = testing.GetObjectModel(TESTED_OM_ID);

			Assert.IsFalse(om.Operations.Single(o => o.Id == "VoidOp").Marks.Any(m => m == "heavy"));
			Assert.IsTrue(om.Operations.Single(o => o.Id == "StringOp").Marks.Any(m => m == "heavy"));
			Assert.IsFalse(om.Operations.Single(o => o.Id == "ListOp").Marks.Any(m => m == "heavy"));
		}

		[Test]
		public void An_operation_can_return_void()
		{
			var actual = testing.GetObjectModel(TESTED_OM_ID);

			Assert.IsTrue(actual.Operations.Single(o => o.Id == "VoidOp").Result.IsVoid);
			Assert.IsFalse(actual.Operations.Single(o => o.Id == "StringOp").Result.IsVoid);
			Assert.IsFalse(actual.Operations.Single(o => o.Id == "ListOp").Result.IsVoid);
		}

		[Test]
		public void An_operation_can_return_list()
		{
			var actual = testing.GetObjectModel(TESTED_OM_ID);

			Assert.IsFalse(actual.Operations.Single(o => o.Id == "VoidOp").Result.IsList);
			Assert.IsFalse(actual.Operations.Single(o => o.Id == "StringOp").Result.IsList);
			Assert.IsTrue(actual.Operations.Single(o => o.Id == "ListOp").Result.IsList);
		}

		[Test]
		public void Operations_with_unrecognized_result_ViewModelIds_are_ignored_automatically()
		{
			var actual = testing.GetObjectModel(TESTED_OM_ID);

			Assert.IsFalse(actual.Operations.Any(m => m.Result.ViewModelId.Contains("ignored")));
		}

		[Test]
		public void An_initializer_can_have_parameters()
		{
			var actual = testing.GetObjectModel(TESTED_DM_ID);

			Assert.AreEqual(2, actual.Initializer.Parameters.Count);
			Assert.AreEqual("data", actual.Initializer.Parameters[0].Id);
			Assert.AreEqual("s-string", actual.Initializer.Parameters[0].ViewModelId);
			Assert.AreEqual("i", actual.Initializer.Parameters[1].Id);
			Assert.AreEqual("s-int-32", actual.Initializer.Parameters[1].ViewModelId);
		}

		[Test]
		public void An_operation_can_have_parameters()
		{
			var actual = testing.GetObjectModel(TESTED_OM_ID);

			Assert.AreEqual(0, actual.Operations.Single(o => o.Id == "VoidOp").Parameters.Count);
			Assert.AreEqual(1, actual.Operations.Single(o => o.Id == "StringOp").Parameters.Count);
			Assert.AreEqual("list", actual.Operations.Single(o => o.Id == "StringOp").Parameters[0].Id);
			Assert.AreEqual("s-string", actual.Operations.Single(o => o.Id == "StringOp").Parameters[0].ViewModelId);
			Assert.AreEqual(1, actual.Operations.Single(o => o.Id == "ListOp").Parameters.Count);
			Assert.AreEqual("boolParam", actual.Operations.Single(o => o.Id == "ListOp").Parameters[0].Id);
			Assert.AreEqual("s-boolean", actual.Operations.Single(o => o.Id == "ListOp").Parameters[0].ViewModelId);
		}

		[Test]
		public void Operations_with_any_unrecognized_parameter_ViewModelIds_are_ignored_automatically()
		{
			var actual = testing.GetObjectModel(TESTED_OM_ID);

			Assert.IsFalse(actual.Operations.Any(m => m.Parameters.Any(p => p.ViewModelId.Contains("ignored"))));
		}

		[Test]
		public void A_parameter_can_be_of_list_type()
		{
			var actual = testing.GetObjectModel(TESTED_OM_ID);

			Assert.IsTrue(actual.Operations.Single(o => o.Id == "StringOp").Parameters[0].IsList);
			Assert.IsFalse(actual.Operations.Single(o => o.Id == "ListOp").Parameters[0].IsList);		
		}

		[Test]
		public void A_parameter_is_marked_with_given_selectors()
		{
			codingStyle
				.SelectParameterMarks.Done(s => s.Always("list").When(p => p.ParameterType.CanBeCollection()))
				;

			var om = testing.GetObjectModel(TESTED_OM_ID);

			Assert.IsTrue(om.Operations.Single(o => o.Id == "StringOp").Parameters[0].Marks.Any(m => m == "list"));
			Assert.IsFalse(om.Operations.Single(o => o.Id == "ListOp").Parameters[0].Marks.Any(m => m == "list"));
		}

		[Test]
		public void Initializer_can_have_multiple_parameter_groups()
		{
			var actual = testing.GetObjectModel(TESTED_DM_ID);

			Assert.AreEqual(new List<int> { 0, 1 }, actual.Initializer.Parameters.Single(p => p.Id == "data").Groups);
			Assert.AreEqual(new List<int> { 1 }, actual.Initializer.Parameters.Single(p => p.Id == "i").Groups);
		}

		[Test]
		public void Initializer_s_marks_are_those_merged_from_all_its_groups()
		{
			codingStyle
				.SelectInitializerMarks.Done(s => s.By(i => "ovr-" + i.Parameters.Count))
				;

			var actual = testing.GetObjectModel(TESTED_DM_ID);

			Assert.IsTrue(actual.Initializer.Marks.Contains("ovr-1"));
			Assert.IsTrue(actual.Initializer.Marks.Contains("ovr-2"));
			Assert.AreEqual(2, actual.Initializer.Marks.Count);
		}

		[Test]
		public void Operations_with_same_name_are_considered_as_parameter_groups_and_represented_as_a_single_operation_with_parameter_groups()
		{
			var actual = testing.GetObjectModel(TESTED_OM_ID);

			Assert.IsTrue(actual.Operations.Any(m => m.Id.Contains("OverloadOp")));
			Assert.IsFalse(actual.Operations.Single(o => o.Id == "OverloadOp").Parameters.Any(p => p.Groups.Contains(0)));
			Assert.AreEqual(new List<int> { 1, 3, 4 }, actual.Operations.Single(o => o.Id == "OverloadOp").Parameters.Single(p => p.Id == "s").Groups);
			Assert.AreEqual(new List<int> { 2, 3 }, actual.Operations.Single(o => o.Id == "OverloadOp").Parameters.Single(p => p.Id == "i").Groups);
			Assert.AreEqual(new List<int> { 4 }, actual.Operations.Single(o => o.Id == "OverloadOp").Parameters.Single(p => p.Id == "s1").Groups);
			Assert.AreEqual(new List<int> { 4 }, actual.Operations.Single(o => o.Id == "OverloadOp").Parameters.Single(p => p.Id == "i1").Groups);
		}

		[Test]
		public void Operations_with_the_same_name_but_different_results_are_ignored()
		{
			var om = testing.GetObjectModel(TESTED_OM_ID);
			var actual = om.Operations.Single(o => o.Id == "OverloadOp");

			Assert.AreEqual(5, actual.GroupCount);
			Assert.IsTrue(actual.Result.IsVoid);
		}

		[Test]
		public void An_operation_s_marks_are_those_merged_from_all_its_groups()
		{
			codingStyle
				.SelectOperationMarks.Done(s => s.By(o => "ovr-" + o.Parameters.Count).When(o => o.Name == "OverloadOp"))
				;

			var actual = testing.GetObjectModel(TESTED_OM_ID);

			Assert.IsTrue(actual.Operations.Single(o => o.Id == "OverloadOp").Marks.Contains("ovr-0"));
			Assert.IsTrue(actual.Operations.Single(o => o.Id == "OverloadOp").Marks.Contains("ovr-1"));
			Assert.IsTrue(actual.Operations.Single(o => o.Id == "OverloadOp").Marks.Contains("ovr-2"));
			Assert.IsTrue(actual.Operations.Single(o => o.Id == "OverloadOp").Marks.Contains("ovr-3"));
			Assert.AreEqual(4, actual.Operations.Single(o => o.Id == "OverloadOp").Marks.Count);
		}

		[Test]
		public void A_parameter_s_marks_are_those_merged_from_all_its_groups()
		{
			codingStyle
				.SelectParameterMarks.Done(s => s.By(p => "ovr-" + p.Owner.Parameters.Count).When(p => p.Name == "s" && p.Owner.Name == "OverloadOp"))
				;

			var om = testing.GetObjectModel(TESTED_OM_ID);
			var actual = om.Operations.Single(o => o.Id == "OverloadOp").Parameters.Single(p => p.Id == "s");

			Assert.IsTrue(actual.Marks.Contains("ovr-1"));
			Assert.IsTrue(actual.Marks.Contains("ovr-2"));
			Assert.IsTrue(actual.Marks.Contains("ovr-3"));
			Assert.AreEqual(3, actual.Marks.Count);
		}

		[Test]
		public void Parameters_with_the_same_name_but_different_types_are_ignored()
		{
			var om = testing.GetObjectModel(TESTED_OM_ID);
			var actual = om.Operations.Single(o => o.Id == "OverloadOp");

			Assert.AreEqual(5, actual.GroupCount);
			Assert.IsFalse(actual.Parameters.Any(p => p.ViewModelId == "s-boolean" && p.Id == "s"));

			om = testing.GetObjectModel(TESTED_DM_ID);

			Assert.AreEqual(2, om.Initializer.GroupCount);
			Assert.IsFalse(om.Initializer.Parameters.Any(p => p.ViewModelId == "s-int-32" && p.Id == "data"));
		}
	}
}

