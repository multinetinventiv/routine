using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Routine.Core.Service;
using Routine.Core.Service.Impl;

namespace Routine.Test.Core.Service
{
	public interface IBusinessOperation
	{
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
		private const string ACTUAL_OMID = "Routine.Test.Core.Service.BusinessOperation";
		private const string VIEW_OMID = "Routine.Test.Core.Service.IBusinessOperation";

		protected override string DefaultModelId { get { return ACTUAL_OMID; } }
		public override string[] DomainTypeRootNamespaces{get{return new[]{"Routine.Test.Core.Service"};}}

		private Mock<IBusinessOperation> businessMock;
		private BusinessOperation businessObj;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			codingStyle
				.ModelId.Done(s => s.SerializeBy(t => t.FullName).DeserializeBy(id => id.ToType()))

				.DisplayValue.Done(e => e.ByProperty(p => p.Returns<string>("Title")))
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
		public void LocatesTargetObjectAndPerformsGivenOperationViaViewModel()
		{
			SetUpObject("id");

			testing.PerformOperation(Id("id"), "Void", Params());

			businessMock.Verify(o => o.Void(), Times.Once());
		}

		[Test]
		public void ThrowsExceptionWhenGivenOperationIsNotFound()
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
		public void ReturnsPerformOperationResultWithReferenceAndDisplayValue()
		{
			SetUpObject("id", "title");

			businessMock.Setup(o => o.GetResult()).Returns(businessObj);

			var result = testing.PerformOperation(Id("id"), "GetResult", Params());

			Assert.AreEqual("title", result.Value.Values[0].Value);
			Assert.AreEqual("id", result.Value.Values[0].Reference.Id);
			Assert.AreEqual(ACTUAL_OMID, result.Value.Values[0].Reference.ActualModelId);
			Assert.AreEqual(VIEW_OMID, result.Value.Values[0].Reference.ViewModelId);
			Assert.IsFalse(result.Value.Values[0].Reference.IsNull);
		}

		[Test]
		public void NullResultSupport()
		{
			SetUpObject("id");

			businessMock.Setup(o => o.GetResult()).Returns((IBusinessOperation)null);

			var result = testing.PerformOperation(Id("id"), "GetResult", Params());

			Assert.IsTrue(result.Value.Values[0].Reference.IsNull);
		}

		[Test]
		public void ListResultSupport()
		{
			SetUpObject("id");

			businessMock.Setup(o => o.GetListResult()).Returns(new List<string>{ "a", "b" });

			var result = testing.PerformOperation(Id("id"), "GetListResult", Params());

			Assert.IsTrue(result.Value.IsList);
			Assert.AreEqual(2, result.Value.Values.Count);

			Assert.AreEqual("a", result.Value.Values[0].Reference.Id);
			Assert.AreEqual("a", result.Value.Values[0].Value);
			Assert.AreEqual(":System.String", result.Value.Values[0].Reference.ActualModelId);
			Assert.AreEqual(":System.String", result.Value.Values[0].Reference.ViewModelId);

			Assert.AreEqual("b", result.Value.Values[1].Reference.Id);
		}
		
		[Test]
		public void ArrayResultSupport()
		{
			SetUpObject("id");

			businessMock.Setup(o => o.GetArrayResult()).Returns(new []{ "a", "b" });

			var result = testing.PerformOperation(Id("id"), "GetArrayResult", Params());

			Assert.IsTrue(result.Value.IsList);
			Assert.AreEqual(2, result.Value.Values.Count);

			Assert.AreEqual("a", result.Value.Values[0].Reference.Id);
			Assert.AreEqual("a", result.Value.Values[0].Value);
			Assert.AreEqual(":System.String", result.Value.Values[0].Reference.ActualModelId);
			Assert.AreEqual(":System.String", result.Value.Values[0].Reference.ViewModelId);

			Assert.AreEqual("b", result.Value.Values[1].Reference.Id);
		}

		[Test]
		public void LocatesGivenParametersAndPassesToOperation()
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
		public void WhenAParametersIsMissingPassesNullForIt()
		{
			SetUpObject("id");

			testing.PerformOperation(Id("id"), "DoParameterizedOperation", Params(Param("obj", Id("id"))));

			businessMock.Verify(o => o.DoParameterizedOperation(null, businessObj));
		}

		[Test]
		public void WhenExtraParameterIsGivenSimplyIgnoresIt()
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
		public void NullParameterSupport()
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
		public void ListParameterSupport()
		{
			SetUpObject("id");

			testing.PerformOperation(Id("id"), "DoOperationWithList", 
				Params(
					Param("list", Id("a", "System.String"), Id("b", "System.String"))
				));

			businessMock.Verify(o => o.DoOperationWithList(new List<string>{"a", "b"}));
		}

		[Test] [Ignore]
		public void WhenNotAvailableOperationIsCalledOperationIsAutomaticallyCancelled()
		{
			Assert.Fail("not implemented");
		}

		[Test] [Ignore]
		public void PerformAsTableFeature()
		{
			Assert.Fail("not implemented");
		}

		[Test] [Ignore]
		public void ValidatesGivenParametersAgainstParameterModel()
		{
			Assert.Fail("not implemented");
		}

		[Test] [Ignore]
		public void ArrayParameterSupport()
		{
			Assert.Fail("not implemented");
		}
		
		[Test] [Ignore]
		public void ParamsSupport()
		{
			Assert.Fail("not implemented");
		}

		[Test] [Ignore]
		public void MethodOverloadSupport()
		{
			//TODO resolve optional parameters with overloading
			Assert.Fail("not implemented");
		}
		
		[Test] [Ignore]
		public void OptionalParameterSupport()
		{
			Assert.Fail("not implemented");
		}
	}
}

