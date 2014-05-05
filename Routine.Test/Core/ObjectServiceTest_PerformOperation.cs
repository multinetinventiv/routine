using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Routine.Core;

namespace Routine.Test.Core
{
	public interface IBusinessOperation
	{
		string Title{get;}

		void Void();
		IBusinessOperation GetResult();
		void DoParameterizedOperation(string str, IBusinessOperation obj);
		void DoOperationWithList(List<string> list);
		List<string> GetListResult();
		string[] GetArrayResult();
	}

	public class BusinessOperation : IBusinessOperation
	{
		private readonly IBusinessOperation mock;
		internal BusinessOperation(IBusinessOperation mock) { this.mock = mock; }

		public string Id{get;set;}
		public string Title{get;set;}

		void IBusinessOperation.Void() { mock.Void(); }
		IBusinessOperation IBusinessOperation.GetResult() { return mock.GetResult(); }
		void IBusinessOperation.DoParameterizedOperation(string str, IBusinessOperation obj) { mock.DoParameterizedOperation(str, obj); }
		void IBusinessOperation.DoOperationWithList(List<string> list) { mock.DoOperationWithList(list); }
		List<string> IBusinessOperation.GetListResult() { return mock.GetListResult(); }
		string[] IBusinessOperation.GetArrayResult() { return mock.GetArrayResult(); }
	}

	[TestFixture]
	public class ObjectServiceTest_PerformOperation : ObjectServiceTestBase
	{
		private const string ACTUAL_OMID = "Routine.Test.Core.BusinessOperation";
		private const string VIEW_OMID = "Routine.Test.Core.IBusinessOperation";

		protected override string DefaultModelId { get { return ACTUAL_OMID; } }
		public override string[] DomainTypeRootNamespaces{get{return new[]{"Routine.Test.Core"};}}

		private Mock<IBusinessOperation> businessMock;
		private BusinessOperation businessObj;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			codingStyle
				.SerializeModelId.Done(s => s.SerializeBy(t => t.FullName).DeserializeBy(id => id.ToType()))

				.ExtractDisplayValue.Done(e => e.ByProperty(p => p.Returns<string>("Title")))
				;

			businessMock = new Mock<IBusinessOperation>();
			businessObj = new BusinessOperation(businessMock.Object);
		}

		private void SetUpObject(string id) {SetUpObject(id, id);}
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
		public void When_parameter_values_is_null__it_is_treated_as_if_it_is_an_empty_list()
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
					Param("str", Id("str_value", "System.String")),
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
					Param("str", Id("str_value", "System.String")),
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
					Param("list", Id("a", "System.String"), Id("b", "System.String"))
				));

			businessMock.Verify(o => o.DoOperationWithList(new List<string>{"a", "b"}));
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

		[Test] [Ignore]
		public void Validates_given_parameters_against_parameter_model()
		{
			Assert.Fail("not implemented");
		}

		[Test] [Ignore]
		public void Array_parameter_support()
		{
			Assert.Fail("not implemented");
		}
		
		[Test] [Ignore]
		public void Params_support()
		{
			Assert.Fail("not implemented");
		}

		[Test] [Ignore]
		public void Method_overload_support()
		{
			//TODO resolve optional parameters with overloading
			Assert.Fail("not implemented");
		}
		
		[Test] [Ignore]
		public void Optional_parameter_support()
		{
			Assert.Fail("not implemented");
		}
	}
}

