using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Routine.Core;

namespace Routine.Test.Core
{
	#region Test Model

	public interface IBusinessOperation
	{
		string Title { get; }

		void Void();
		IBusinessOperation GetResult();
		void DoParameterizedOperation(string str, IBusinessOperation obj);
		void DoOperationWithList(List<string> list);
		void DoOperationWithArray(string[] array);
		void DoOperationWithParamsArray(params string[] paramsArray);
		List<string> GetListResult();
		string[] GetArrayResult();

		void OverloadOp();
		void OverloadOp(string s);
		void OverloadOp(int i);
		void OverloadOp(string s, int i);
		void OverloadOp(string s1, string s, int i1);
	}

	public class BusinessOperation : IBusinessOperation
	{
		private readonly IBusinessOperation mock;
		internal BusinessOperation(IBusinessOperation mock) { this.mock = mock; }

		public string Id { get; set; }
		public string Title { get; set; }

		void IBusinessOperation.Void() { mock.Void(); }
		IBusinessOperation IBusinessOperation.GetResult() { return mock.GetResult(); }
		void IBusinessOperation.DoParameterizedOperation(string str, IBusinessOperation obj) { mock.DoParameterizedOperation(str, obj); }
		void IBusinessOperation.DoOperationWithList(List<string> list) { mock.DoOperationWithList(list); }
		void IBusinessOperation.DoOperationWithArray(string[] array) { mock.DoOperationWithArray(array); }
		void IBusinessOperation.DoOperationWithParamsArray(params string[] paramsArray) { mock.DoOperationWithParamsArray(paramsArray); }
		List<string> IBusinessOperation.GetListResult() { return mock.GetListResult(); }
		string[] IBusinessOperation.GetArrayResult() { return mock.GetArrayResult(); }

		void IBusinessOperation.OverloadOp() { mock.OverloadOp(); }
		void IBusinessOperation.OverloadOp(string s) { mock.OverloadOp(s); }
		void IBusinessOperation.OverloadOp(int i) { mock.OverloadOp(i); }
		void IBusinessOperation.OverloadOp(string s, int i) { mock.OverloadOp(s, i); }
		void IBusinessOperation.OverloadOp(string s1, string s, int i1) { mock.OverloadOp(s1, s, i1); }
	}

	#endregion

	[TestFixture]
	public class ObjectServiceTest_PerformOperation : ObjectServiceTestBase
	{
		#region Setup & Helpers

		private const string ACTUAL_OMID = "Routine.Test.Core.BusinessOperation";
		private const string VIEW_OMID = "Routine.Test.Core.IBusinessOperation";

		protected override string DefaultModelId { get { return ACTUAL_OMID; } }
		public override string[] DomainTypeRootNamespaces { get { return new[] { "Routine.Test.Core" }; } }

		private Mock<IBusinessOperation> businessMock;
		private BusinessOperation businessObj;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			codingStyle
				.SerializeModelId.Done(s => s.SerializeBy(t => t.FullName).DeserializeBy(id => id.ToType()))
				.ExtractValue.Done(e => e.ByProperty(p => p.Returns<string>("Title")))
				;

			businessMock = new Mock<IBusinessOperation>();
			businessObj = new BusinessOperation(businessMock.Object);
		}

		private void SetUpObject(string id) { SetUpObject(id, id); }
		private void SetUpObject(string id, string title)
		{
			businessObj.Id = id;
			businessObj.Title = title;

			AddToRepository(businessObj);
		}

		protected override ObjectReferenceData Id(string id)
		{
			return Id(id, ACTUAL_OMID, VIEW_OMID);
		} 

		#endregion

		[Test]
		public void Locates_target_object_and_performs_given_operation_via_view_model()
		{
			SetUpObject("id");

			testing.PerformOperation(Id("id"), "Void", Params());

			businessMock.Verify(o => o.Void(), Times.Once());
		}

		[Test]
		public void Throws_exception_when_given_operation_is_not_found()
		{
			SetUpObject("id");

			try
			{
				testing.PerformOperation(Id("id"), "NonExistingOperation", Params());
				Assert.Fail("exception not thrown");
			}
			catch(OperationDoesNotExistException){}
		}

		[Test]
		public void Returns_perform_operation_result_with_reference_and_display_value()
		{
			SetUpObject("id", "title");

			businessMock.Setup(o => o.GetResult()).Returns(businessObj);

			var result = testing.PerformOperation(Id("id"), "GetResult", Params());

			Assert.AreEqual("title", result.Values[0].Value);
			Assert.AreEqual("id", result.Values[0].Reference.Id);
			Assert.AreEqual(ACTUAL_OMID, result.Values[0].Reference.ActualModelId);
			Assert.AreEqual(VIEW_OMID, result.Values[0].Reference.ViewModelId);
			Assert.IsFalse(result.Values[0].Reference.IsNull);
		}

		[Test]
		public void Null_result_support()
		{
			SetUpObject("id");

			businessMock.Setup(o => o.GetResult()).Returns((IBusinessOperation)null);

			var result = testing.PerformOperation(Id("id"), "GetResult", Params());

			Assert.IsTrue(result.Values[0].Reference.IsNull);
		}

		[Test]
		public void List_result_support()
		{
			SetUpObject("id");

			businessMock.Setup(o => o.GetListResult()).Returns(new List<string>{ "a", "b" });

			var result = testing.PerformOperation(Id("id"), "GetListResult", Params());

			Assert.IsTrue(result.IsList);
			Assert.AreEqual(2, result.Values.Count);

			Assert.AreEqual("a", result.Values[0].Reference.Id);
			Assert.AreEqual("a", result.Values[0].Value);
			Assert.AreEqual("s-string", result.Values[0].Reference.ActualModelId);
			Assert.AreEqual("s-string", result.Values[0].Reference.ViewModelId);

			Assert.AreEqual("b", result.Values[1].Reference.Id);
		}
		
		[Test]
		public void Array_result_support()
		{
			SetUpObject("id");

			businessMock.Setup(o => o.GetArrayResult()).Returns(new []{ "a", "b" });

			var result = testing.PerformOperation(Id("id"), "GetArrayResult", Params());

			Assert.IsTrue(result.IsList);
			Assert.AreEqual(2, result.Values.Count);

			Assert.AreEqual("a", result.Values[0].Reference.Id);
			Assert.AreEqual("a", result.Values[0].Value);
			Assert.AreEqual("s-string", result.Values[0].Reference.ActualModelId);
			Assert.AreEqual("s-string", result.Values[0].Reference.ViewModelId);

			Assert.AreEqual("b", result.Values[1].Reference.Id);
		}

		[Test]
		public void When_parameterValues_is_null__it_is_treated_as_if_it_is_an_empty_dictionary()
		{
			SetUpObject("id");

			testing.PerformOperation(Id("id"), "Void", null);

			businessMock.Verify(o => o.Void(), Times.Once());
		}

		[Test]
		public void Locates_given_parameters_and_passes_to_operation()
		{
			SetUpObject("id");

			testing.PerformOperation(Id("id"), "DoParameterizedOperation", 
				Params(
					Param("str", Id("str_value", "s-string")),
					Param("obj", Id("id"))
				));

			businessMock.Verify(o => o.DoParameterizedOperation("str_value", businessObj));
		}

		[Test]
		public void When_a_parameter_is_missing__passes_null_for_it()
		{
			SetUpObject("id");

			testing.PerformOperation(Id("id"), "DoParameterizedOperation", Params(Param("obj", Id("id"))));

			businessMock.Verify(o => o.DoParameterizedOperation(null, businessObj));
		}

		[Test]
		public void When_extra_parameter_is_given__simply_ignores_it()
		{
			SetUpObject("id");

			testing.PerformOperation(Id("id"), "DoParameterizedOperation", 
				Params(
					Param("nonExistingParameter", Id("id")),
					Param("str", Id("str_value", "s-string")),
					Param("obj", Id("id"))
				));

			businessMock.Verify(o => o.DoParameterizedOperation("str_value", businessObj));
		}

		[Test]
		public void Null_parameter_support()
		{
			SetUpObject("id");

			testing.PerformOperation(Id("id"), "DoParameterizedOperation", 
				Params(
					Param("str", IdNull()),
					Param("obj", IdNull())
				));

			businessMock.Verify(o => o.DoParameterizedOperation(null, null));
		}

		[Test]
		public void List_parameter_support()
		{
			SetUpObject("id");

			testing.PerformOperation(Id("id"), "DoOperationWithList", 
				Params(
					Param("list", Id("a", "s-string"), Id("b", "s-string"))
				));

			businessMock.Verify(o => o.DoOperationWithList(new List<string>{"a", "b"}));
		}

		[Test]
		public void Array_parameter_support()
		{
			SetUpObject("id");

			testing.PerformOperation(Id("id"), "DoOperationWithArray",
				Params(
					Param("array", Id("a", "s-string"), Id("b", "s-string"))
				));

			businessMock.Verify(o => o.DoOperationWithArray(new[] { "a", "b" }));
		}

		[Test]
		public void Params_parameter_support()
		{
			SetUpObject("id");

			testing.PerformOperation(Id("id"), "DoOperationWithParamsArray",
				Params(
					Param("paramsArray", Id("a", "s-string"), Id("b", "s-string"))
				));

			businessMock.Verify(o => o.DoOperationWithParamsArray(new[] { "a", "b" }));
		}

		[Test]
		public void By_default_perform_operation_returns_eager_result()
		{
			SetUpObject("id", "title");

			businessMock.Setup(o => o.GetResult()).Returns(businessObj);

			var result = testing.PerformOperation(Id("id"), "GetResult", Params());

			Assert.IsTrue(result.Values[0].Members.ContainsKey("Title"));
			Assert.AreEqual("title", result.Values[0].Members["Title"].Values[0].Reference.Id);
		}

		[Test]
		public void Target_operation_can_have_more_than_one_parameter_group_so_that_domain_object_can_expose_overloaded_methods_as_one_service()
		{
			SetUpObject("id");

			testing.PerformOperation(Id("id"), "OverloadOp",
				Params(
					Param("i", Id("1", "s-int-32"))
				)
			);
			businessMock.Verify(o => o.OverloadOp(1), Times.Once());

			testing.PerformOperation(Id("id"), "OverloadOp",
				Params(
					Param("s", Id("s_value", "s-string"))
				)
			);
			businessMock.Verify(o => o.OverloadOp("s_value"), Times.Once());

			testing.PerformOperation(Id("id"), "OverloadOp",
				Params(
					Param("s", Id("s_value2", "s-string")),
					Param("i", Id("2", "s-int-32"))
				)
			);
			businessMock.Verify(o => o.OverloadOp("s_value2", 2), Times.Once());
		}

		[Test]
		public void When_given_parameters_do_not_match_exactly_with_any_operations__uses_the_group_with_most_matched_parameters()
		{
			SetUpObject("id");

			testing.PerformOperation(Id("id"), "OverloadOp",
				Params(
					Param("s", Id("s_value", "s-string")),
					Param("s1", Id("s_value1", "s-string"))
				)
			);

			//group 0 -> match 0
			//group 1 -> match 1 (s)
			//group 2 -> match 0
			//group 3 -> match 1 (s)
			//group 4 -> match 2 (s1, s) --> WINS!
			businessMock.Verify(o => o.OverloadOp("s_value1", "s_value", 0), Times.Once());
		}

		[Test]
		public void When_there_are_more_than_one_group_having_the_same_number_of_most_matched_parameters__uses_the_group_with_least_non_matched_parameters()
		{
			SetUpObject("id");

			testing.PerformOperation(Id("id"), "OverloadOp",
				Params(
					Param("s", Id("s_value", "s-string")),
					Param("i", Id("1", "s-int-32")),
					Param("s1", Id("s_value1", "s-string"))
				)
			);

			//group 0 -> match 0
			//group 1 -> match 1 (s)
			//group 2 -> match 1 (i)
			//group 3 -> match 2 (s, i) --> non-match 0 --> WINS!
			//group 3 -> match 2 (s1, s) --> non-match 1 (i1)
			businessMock.Verify(o => o.OverloadOp("s_value", 1), Times.Once());

			testing.PerformOperation(Id("id"), "OverloadOp",
				Params(
					Param("a", Id("dummy", "s-string"))
				)
			);

			//group 0 -> match 0 --> non-match 0 --> WINS!
			//group 1 -> match 0 --> non-match 1 (s)
			//group 2 -> match 0 --> non-match 1 (i)
			//group 3 -> match 0 --> non-match 2 (s, i)
			//group 3 -> match 0 --> non-match 3 (s1, s, i1)
			businessMock.Verify(o => o.OverloadOp(), Times.Once());
		}
		
		[Test] [Ignore]
		public void Optional_parameter_support()
		{
			Assert.Fail("not implemented");
		}
	}
}

