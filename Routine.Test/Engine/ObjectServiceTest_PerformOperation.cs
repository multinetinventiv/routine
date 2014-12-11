using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Routine.Core;
using Routine.Engine;
using Routine.Test.Engine.Domain.ObjectServiceTest_PerformOperation;

#region Test Model

namespace Routine.Test.Engine.Domain.ObjectServiceTest_PerformOperation
{
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

		void OverloadArray(string s);
		void OverloadArray(int[] i, string s);

		void DataInput(BusinessMasterInputData input);
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

		void IBusinessOperation.DataInput(BusinessMasterInputData input) { mock.DataInput(input); }
		void IBusinessOperation.OverloadArray(string s) { mock.OverloadArray(s); }
		void IBusinessOperation.OverloadArray(int[] i, string s) { mock.OverloadArray(i, s); }
	}

	public struct BusinessInputData
	{
		public string Data { get; private set; }

		public BusinessInputData(string data)
			: this()
		{
			Data = data;
		}
	}

	public struct BusinessMasterInputData
	{
		public List<BusinessInputData> Datas { get; private set; }

		public BusinessMasterInputData(List<BusinessInputData> datas)
			: this()
		{
			Datas = datas;
		}
	}
}

#endregion

namespace Routine.Test.Engine
{
	[TestFixture]
	public class ObjectServiceTest_PerformOperation : ObjectServiceTestBase
	{
		#region Setup & Helpers

		private const string ACTUAL_OMID = "Routine.Test.Engine.Domain.ObjectServiceTest_PerformOperation.BusinessOperation";
		private const string VIEW_OMID = "Routine.Test.Engine.Domain.ObjectServiceTest_PerformOperation.IBusinessOperation";
		private const string DATA_OMID = "Routine.Test.Engine.Domain.ObjectServiceTest_PerformOperation.BusinessInputData";
		private const string MASTER_DATA_OMID = "Routine.Test.Engine.Domain.ObjectServiceTest_PerformOperation.BusinessMasterInputData";
		private const string VIRTUAL_OMID = "Routine.Test.Engine.Domain.ObjectServiceTest_PerformOperation.VirtualOperation";

		protected override string DefaultModelId { get { return ACTUAL_OMID; } }
		protected override string RootNamespace { get { return "Routine.Test.Engine.Domain.ObjectServiceTest_PerformOperation"; } }

		private Mock<IBusinessOperation> businessMock;
		private BusinessOperation businessObj;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			codingStyle
				.TypeId.Set(c => c.By(t => t.FullName).When(t => t.IsDomainType))
				.ValueExtractor.Set(c => c.ValueByMember(m => m.Returns<string>("Title")))
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
			catch (OperationDoesNotExistException) { }
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

			businessMock.Setup(o => o.GetListResult()).Returns(new List<string> { "a", "b" });

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

			businessMock.Setup(o => o.GetArrayResult()).Returns(new[] { "a", "b" });

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
		public void When_no_reference_id_was_given_and_no_initializer_was_found__service_treats_this_situation_as_if_an_empty_string_was_sent()
		{
			SetUpObject("id");

			testing.PerformOperation(Id("id"), "DoParameterizedOperation",
				Params(
					Param("str", Id(null, "s-string"))
				));

			businessMock.Verify(o => o.DoParameterizedOperation(string.Empty, null));
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

			businessMock.Verify(o => o.DoOperationWithList(new List<string> { "a", "b" }));
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
		public void Data_parameter_support()
		{
			SetUpObject("id");

			testing.PerformOperation(Id("id"), "DataInput",
				Params(
					Param("input",
						Init(MASTER_DATA_OMID,
							Params(
								Param("datas",
									Init(DATA_OMID,
										Params(
											Param("data", Id("test1", "s-string"))
										)
									),
									Init(DATA_OMID,
										Params(
											Param("data", Id("test2", "s-string"))
										)
									)
								)
							)
						)
					)
				)
			);

			businessMock.Verify(o => o.DataInput(It.Is<BusinessMasterInputData>(md =>

				md.Datas.Any(d => d.Data == "test1") && md.Datas.Any(d => d.Data == "test2"))),

				Times.Once());
		}

		[Test]
		[Ignore]
		public void Optional_parameter_support()
		{
			Assert.Fail("not implemented");
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

		[Test]
		public void A_group_can_have_different_index_for_a_common_parameter()
		{
			SetUpObject("id");

			testing.PerformOperation(Id("id"), "OverloadArray",
				Params(
					Param("s", Id("s_value", "s-string")),
					Param("i", Id("1", "s-int-32"), Id("2", "s-int-32"))
				)
			);

			businessMock.Verify(o => o.OverloadArray(It.Is<int[]>(i => i[0] == 1 && i[1] == 2), "s_value"), Times.Once);
		}

		[Test]
		public void Virtual_types_can_behave_as_a_proxy()
		{
			codingStyle
				.Use(p => p.VirtualTypePattern())
				.AddTypes(v => v.FromBasic()
					.Name.Set("VirtualOperation")
					.Namespace.Set(RootNamespace)
					.Operations.Add(p => p.Proxy<IBusinessOperation>("Void").Target(businessObj)))
			;

			testing.GetApplicationModel();

			testing.PerformOperation(Id("virtual", VIRTUAL_OMID), "Void", Params());

			businessMock.Verify(o => o.Void(), Times.Once);
		}

		[Test]
		public void Proxy_operations_includes_overloads()
		{
			codingStyle
				.Use(p => p.VirtualTypePattern())
				.AddTypes(v => v.FromBasic()
					.Name.Set("VirtualOperation")
					.Namespace.Set(RootNamespace)
					.Operations.Add(o => o.Proxy<IBusinessOperation>("OverloadOp").Target(businessObj)))
			;

			testing.GetApplicationModel();

			testing.PerformOperation(Id("virtual", VIRTUAL_OMID), "OverloadOp", Params());

			testing.PerformOperation(Id("virtual", VIRTUAL_OMID), "OverloadOp", Params(
				Param("s", Id("s_value", "s-string"))
			));

			testing.PerformOperation(Id("virtual", VIRTUAL_OMID), "OverloadOp", Params(
				Param("i", Id("1", "s-int-32"))
			));

			testing.PerformOperation(Id("virtual", VIRTUAL_OMID), "OverloadOp", Params(
				Param("s", Id("s_value", "s-string")),
				Param("i", Id("1", "s-int-32"))
			));

			testing.PerformOperation(Id("virtual", VIRTUAL_OMID), "OverloadOp", Params(
				Param("s1", Id("s1_value", "s-string")),
				Param("s", Id("s_value", "s-string")),
				Param("i1", Id("2", "s-int-32"))
			));

			businessMock.Verify(o => o.OverloadOp(), Times.Once);
			businessMock.Verify(o => o.OverloadOp("s_value"), Times.Once);
			businessMock.Verify(o => o.OverloadOp(1), Times.Once);
			businessMock.Verify(o => o.OverloadOp("s_value", 1), Times.Once);
			businessMock.Verify(o => o.OverloadOp("s1_value", "s_value", 2), Times.Once);
		}

		[Test]
		public void When_configured_proxy_operations_optionally_adds_a_parameter_for_target()
		{
			codingStyle
				.Use(p => p.VirtualTypePattern())
				.AddTypes(v => v.FromBasic()
					.Name.Set("VirtualOperation")
					.Namespace.Set(RootNamespace)
					.Operations.Add(p => p.Proxy<IBusinessOperation>("OverloadOp").TargetByParameter("objId")))
			;

			SetUpObject("id");

			testing.PerformOperation(Id("virtual", VIRTUAL_OMID), "OverloadOp", Params(
				Param("objId", Id("id")),
				Param("s1", Id("s1_value", "s-string")),
				Param("s", Id("s_value", "s-string")),
				Param("i1", Id("2", "s-int-32"))
			));

			businessMock.Verify(o => o.OverloadOp("s1_value", "s_value", 2), Times.Once);
		}

		[Test]
		public void Proxy_operations_on_real_types_performs_on_given_target()
		{
			codingStyle
				.Operations.Add(c => c.Build(o => o.Proxy<IBusinessOperation>("Void").TargetBySelf()).When(type.of<BusinessOperation>()))
			;

			SetUpObject("id");

			testing.PerformOperation(Id("id"), "Void", Params());

			businessMock.Verify(o => o.Void(), Times.Once);
		}

		[Test]
		public void Virtual_operations_perform_using_given_delegate()
		{
			codingStyle
				.Use(p => p.VirtualTypePattern())
				.AddTypes(v => v.FromBasic()
					.Name.Set("VirtualOperation")
					.Namespace.Set(RootNamespace)
					.Operations.Add(o => o.Virtual("Ping", (string input) => "ping: " + input)))
			;

			testing.GetApplicationModel();

			var result = testing.PerformOperation(Id("virtual", VIRTUAL_OMID), "Ping", Params(
				Param("input", Id("test", "s-string"))
			));

			Assert.AreEqual("ping: test", result.Values[0].Reference.Id);
		}
	}
}