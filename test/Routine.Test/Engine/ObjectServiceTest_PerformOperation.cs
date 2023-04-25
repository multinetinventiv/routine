using Routine.Core;
using Routine.Engine;
using Routine.Test.Engine.Domain.ObjectServiceTest_PerformOperation;
using Routine.Test.Engine.Stubs.ObjectServiceInvokers;
using System.Globalization;

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
        void DoOperationWithIntList(List<int> list);
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

        void OptionalParameterOp(string required, string optional = "default");

        int? NullableParameterOp(int? nullableInt);

        Task<string> AsyncOp();

        void DataInput(BusinessMasterInputData input);
        void InitLocateInput(List<BusinessInitializableLocatable> input);
    }

    public class BusinessOperation : IBusinessOperation
    {
        private readonly IBusinessOperation _mock;
        internal BusinessOperation(IBusinessOperation mock) { _mock = mock; }

        public string Id { get; set; }
        public string Title { get; set; }

        void IBusinessOperation.Void() => _mock.Void();
        IBusinessOperation IBusinessOperation.GetResult() => _mock.GetResult();
        void IBusinessOperation.DoParameterizedOperation(string str, IBusinessOperation obj) => _mock.DoParameterizedOperation(str, obj);
        void IBusinessOperation.DoOperationWithList(List<string> list) => _mock.DoOperationWithList(list);
        void IBusinessOperation.DoOperationWithIntList(List<int> list) => _mock.DoOperationWithIntList(list);
        void IBusinessOperation.DoOperationWithArray(string[] array) => _mock.DoOperationWithArray(array);
        void IBusinessOperation.DoOperationWithParamsArray(params string[] paramsArray) => _mock.DoOperationWithParamsArray(paramsArray);
        List<string> IBusinessOperation.GetListResult() => _mock.GetListResult();
        string[] IBusinessOperation.GetArrayResult() => _mock.GetArrayResult();

        void IBusinessOperation.OverloadOp() => _mock.OverloadOp();
        void IBusinessOperation.OverloadOp(string s) => _mock.OverloadOp(s);
        void IBusinessOperation.OverloadOp(int i) => _mock.OverloadOp(i);
        void IBusinessOperation.OverloadOp(string s, int i) => _mock.OverloadOp(s, i);
        void IBusinessOperation.OverloadOp(string s1, string s, int i1) => _mock.OverloadOp(s1, s, i1);

        void IBusinessOperation.DataInput(BusinessMasterInputData input) => _mock.DataInput(input);
        void IBusinessOperation.OverloadArray(string s) => _mock.OverloadArray(s);
        void IBusinessOperation.OverloadArray(int[] i, string s) => _mock.OverloadArray(i, s);

        void IBusinessOperation.OptionalParameterOp(string required, string optional) => _mock.OptionalParameterOp(required, optional);

        int? IBusinessOperation.NullableParameterOp(int? nullableInt) => _mock.NullableParameterOp(nullableInt);

        async Task<string> IBusinessOperation.AsyncOp() => await _mock.AsyncOp();

        void IBusinessOperation.InitLocateInput(List<BusinessInitializableLocatable> input) => _mock.InitLocateInput(input);
    }

    public readonly struct BusinessInputData
    {
        public string Data { get; }

        public BusinessInputData(string data)
        {
            Data = data;
        }
    }

    public readonly struct BusinessMasterInputData
    {
        public List<BusinessInputData> Datas { get; }

        public BusinessMasterInputData(List<BusinessInputData> datas)
        {
            Datas = datas;
        }
    }

    public class BusinessInitializableLocatable
    {
        public string Value { get; }

        public static BusinessInitializableLocatable Get(string value) => new(value);

        private BusinessInitializableLocatable(string value)
        {
            Value = value;
        }

        public BusinessInitializableLocatable(int value)
        {
            Value = value.ToString(CultureInfo.InvariantCulture);
        }
    }
}

#endregion

namespace Routine.Test.Engine
{
    [TestFixture(typeof(Sync))]
    [TestFixture(typeof(Async))]
    public class ObjectServiceTest_PerformOperation<TObjectServiceInvoker> : ObjectServiceTestBase
        where TObjectServiceInvoker : IObjectServiceInvoker, new()
    {
        #region Setup & Helper

        private const string ACTUAL_OMID = "Routine.Test.Engine.Domain.ObjectServiceTest_PerformOperation.BusinessOperation";
        private const string VIEW_OMID = "Routine.Test.Engine.Domain.ObjectServiceTest_PerformOperation.IBusinessOperation";
        private const string DATA_OMID = "Routine.Test.Engine.Domain.ObjectServiceTest_PerformOperation.BusinessInputData";
        private const string MASTER_DATA_OMID = "Routine.Test.Engine.Domain.ObjectServiceTest_PerformOperation.BusinessMasterInputData";
        private const string VIRTUAL_OMID = "Routine.Test.Engine.Domain.ObjectServiceTest_PerformOperation.VirtualOperation";
        private const string VIRTUAL_VIMID = "Routine.Test.Engine.Domain.ObjectServiceTest_PerformOperation.IVirtualOperation";
        private const string INIT_LOCATE_OMID = "Routine.Test.Engine.Domain.ObjectServiceTest_PerformOperation.BusinessInitializableLocatable";

        protected override string DefaultModelId => ACTUAL_OMID;
        protected override string RootNamespace => "Routine.Test.Engine.Domain.ObjectServiceTest_PerformOperation";

        private Mock<IBusinessOperation> _businessMock;
        private BusinessOperation _businessObj;

        private IObjectServiceInvoker _invoker;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            _codingStyle
                .ValueExtractor.Set(c => c.ValueByProperty(m => m.Returns<string>("Title")))
                .Locator.SetDefault(type.of<BusinessInputData>())
                .Locator.SetDefault(type.of<BusinessMasterInputData>())
                ;

            _businessMock = new();
            _businessObj = new(_businessMock.Object);

            _invoker = new TObjectServiceInvoker();
        }

        private void SetUpObject(string id) { SetUpObject(id, id); }
        private void SetUpObject(string id, string title)
        {
            _businessObj.Id = id;
            _businessObj.Title = title;

            AddToRepository(_businessObj);
        }

        protected override ReferenceData Id(string id) => Id(id, ACTUAL_OMID, VIEW_OMID);

        #endregion

        [Test]
        public void Locates_target_object_and_performs_given_operation_via_view_model()
        {
            SetUpObject("id");

            _invoker.InvokeDo(_testing, Id("id"), "Void", Params());

            _businessMock.Verify(o => o.Void(), Times.Once());
        }

        [Test]
        public void Locates_target_object_using_actual_model__but_operation_is_performed_on_view_target()
        {
            var convertedBusinessMock = new Mock<IBusinessOperation>();
            var convertedBusinessObj = new BusinessOperation(convertedBusinessMock.Object);

            _codingStyle.Override(cs => cs.Converters.Add(c => c
                .Convert(cb => cb
                    .By(type.of<IBusinessOperation>, (o, _) =>
                    {
                        Assert.That(o, Is.SameAs(_businessObj));

                        return convertedBusinessObj;
                    }))
                .When(type.of<BusinessOperation>())
            ));

            SetUpObject("id");

            _invoker.InvokeDo(_testing, Id("id"), "Void", Params());

            _businessMock.Verify(o => o.Void(), Times.Never());
            convertedBusinessMock.Verify(o => o.Void(), Times.Once());
        }

        [Test]
        public void Throws_exception_when_given_operation_is_not_found()
        {
            SetUpObject("id");

            Assert.That(() => _invoker.InvokeDo(_testing, Id("id"), "NonExistingOperation", Params()),
                Throws.Exception.TypeOf<OperationDoesNotExistException>()
            );
        }

        [Test]
        public void Returns_perform_operation_result_with_reference_and_display_value()
        {
            SetUpObject("id", "title");

            _businessMock.Setup(o => o.GetResult()).Returns(_businessObj);

            var result = _invoker.InvokeDo(_testing, Id("id"), "GetResult", Params());

            Assert.That(result.Values[0], Is.Not.Null);
            Assert.That(result.Values[0].Display, Is.EqualTo("title"));
            Assert.That(result.Values[0].Id, Is.EqualTo("id"));
            Assert.That(result.Values[0].ModelId, Is.EqualTo(ACTUAL_OMID));
        }

        [Test]
        public void Async_support()
        {
            SetUpObject("id");

            _businessMock.Setup(o => o.AsyncOp()).ReturnsAsync("async result");

            var result = _invoker.InvokeDo(_testing, Id("id"), "AsyncOp", Params());

            Assert.That(result.Values[0], Is.Not.Null);
            Assert.That(result.Values[0].Id, Is.EqualTo("async result"));
            Assert.That(result.Values[0].ModelId, Is.EqualTo("System.String"));
        }

        [Test]
        public void Null_result_support()
        {
            SetUpObject("id");

            _businessMock.Setup(o => o.GetResult()).Returns((IBusinessOperation)null);

            var result = _invoker.InvokeDo(_testing, Id("id"), "GetResult", Params());

            Assert.That(result, Is.EqualTo(new VariableData()));
        }

        [Test]
        public void Nullable_value_result_support()
        {
            SetUpObject("id");

            _businessMock.Setup(o => o.NullableParameterOp(It.IsAny<int?>())).Returns((int?)null);

            var result = _invoker.InvokeDo(_testing, Id("id"), "NullableParameterOp", Params());

            Assert.That(result, Is.EqualTo(new VariableData()));
        }

        [Test]
        public void List_result_support()
        {
            SetUpObject("id");

            _businessMock.Setup(o => o.GetListResult()).Returns(new List<string> { "a", "b" });

            var result = _invoker.InvokeDo(_testing, Id("id"), "GetListResult", Params());

            Assert.That(result.IsList, Is.True);
            Assert.That(result.Values.Count, Is.EqualTo(2));

            Assert.That(result.Values[0].Id, Is.EqualTo("a"));
            Assert.That(result.Values[0].Display, Is.EqualTo("a"));
            Assert.That(result.Values[0].ModelId, Is.EqualTo("System.String"));

            Assert.That(result.Values[1].Id, Is.EqualTo("b"));
        }

        [Test]
        public void Array_result_support()
        {
            SetUpObject("id");

            _businessMock.Setup(o => o.GetArrayResult()).Returns(new[] { "a", "b" });

            var result = _invoker.InvokeDo(_testing, Id("id"), "GetArrayResult", Params());

            Assert.That(result.IsList, Is.True);
            Assert.That(result.Values.Count, Is.EqualTo(2));

            Assert.That(result.Values[0].Id, Is.EqualTo("a"));
            Assert.That(result.Values[0].Display, Is.EqualTo("a"));
            Assert.That(result.Values[0].ModelId, Is.EqualTo("System.String"));

            Assert.That(result.Values[1].Id, Is.EqualTo("b"));
        }

        [Test]
        public void When_parameterValues_is_null__it_is_treated_as_if_it_is_an_empty_dictionary()
        {
            SetUpObject("id");

            _invoker.InvokeDo(_testing, Id("id"), "Void", null);

            _businessMock.Verify(o => o.Void(), Times.Once());
        }

        [Test]
        public void Locates_given_parameters_and_passes_to_operation()
        {
            SetUpObject("id");

            _invoker.InvokeDo(_testing, Id("id"), "DoParameterizedOperation",
                Params(
                    Param("str", Id("str_value", "System.String")),
                    Param("obj", Id("id"))
                ));

            _businessMock.Verify(o => o.DoParameterizedOperation("str_value", _businessObj));
        }

        [Test]
        public void When_a_parameter_is_missing__passes_null_for_it()
        {
            SetUpObject("id");

            _invoker.InvokeDo(_testing, Id("id"), "DoParameterizedOperation", Params(Param("obj", Id("id"))));

            _businessMock.Verify(o => o.DoParameterizedOperation(null, _businessObj));
        }

        [Test]
        public void When_extra_parameter_is_given__simply_ignores_it()
        {
            SetUpObject("id");

            _invoker.InvokeDo(_testing, Id("id"), "DoParameterizedOperation",
                Params(
                    Param("nonExistingParameter", Id("id")),
                    Param("str", Id("str_value", "System.String")),
                    Param("obj", Id("id"))
                ));

            _businessMock.Verify(o => o.DoParameterizedOperation("str_value", _businessObj));
        }

        [Test]
        public void When_no_reference_id_was_given_and_no_initializer_was_found__service_treats_this_situation_as_if_an_empty_string_was_sent()
        {
            SetUpObject("id");

            _invoker.InvokeDo(_testing, Id("id"), "DoParameterizedOperation",
                Params(
                    Param("str", Id(null, "System.String"))
                ));

            _businessMock.Verify(o => o.DoParameterizedOperation(string.Empty, null));
        }

        [Test]
        public void Null_parameter_support()
        {
            SetUpObject("id");

            _invoker.InvokeDo(_testing, Id("id"), "DoParameterizedOperation",
                Params(
                    Param("str", IdNull()),
                    Param("obj", IdNull())
                ));

            _businessMock.Verify(o => o.DoParameterizedOperation(null, null));
        }

        [Test]
        public void Nullable_value_parameter_support()
        {
            SetUpObject("id");

            _invoker.InvokeDo(_testing, Id("id"), "NullableParameterOp",
                Params(
                    Param("nullableInt", IdNull())
                ));

            _businessMock.Verify(o => o.NullableParameterOp(null));
        }

        [Test]
        public void List_parameter_support()
        {
            SetUpObject("id");

            _invoker.InvokeDo(_testing, Id("id"), "DoOperationWithList",
                Params(
                    Param("list", Id("a", "System.String"), Id("b", "System.String"))
                ));

            _businessMock.Verify(o => o.DoOperationWithList(new List<string> { "a", "b" }));
        }

        [Test]
        public void Array_parameter_support()
        {
            SetUpObject("id");

            _invoker.InvokeDo(_testing, Id("id"), "DoOperationWithArray",
                Params(
                    Param("array", Id("a", "System.String"), Id("b", "System.String"))
                ));

            _businessMock.Verify(o => o.DoOperationWithArray(new[] { "a", "b" }));
        }

        [Test]
        public void Params_parameter_support()
        {
            SetUpObject("id");

            _invoker.InvokeDo(_testing, Id("id"), "DoOperationWithParamsArray",
                Params(
                    Param("paramsArray", Id("a", "System.String"), Id("b", "System.String"))
                ));

            _businessMock.Verify(o => o.DoOperationWithParamsArray(new[] { "a", "b" }));
        }

        [Test]
        public void Data_parameter_support()
        {
            SetUpObject("id");

            _invoker.InvokeDo(_testing, Id("id"), "DataInput",
                Params(
                    Param("input",
                        Init(MASTER_DATA_OMID,
                            Params(
                                Param("datas",
                                    Init(DATA_OMID,
                                        Params(
                                            Param("data", Id("test1", "System.String"))
                                        )
                                    ),
                                    Init(DATA_OMID,
                                        Params(
                                            Param("data", Id("test2", "System.String"))
                                        )
                                    )
                                )
                            )
                        )
                    )
                )
            );

            _businessMock.Verify(o =>
                    o.DataInput(It.Is<BusinessMasterInputData>(md =>
                        md.Datas.Any(d => d.Data == "test1") &&
                        md.Datas.Any(d => d.Data == "test2")
                    )),
                Times.Once()
            );
        }

        [Test]
        public void When_all_arguments_are_of_the_same_domain_type__list_parameters_are_located_at_once()
        {
            _codingStyle
                .Locator.Set(c => c.Locator(l => l.By(ids => ids.Select(id => int.Parse(id) + 100))).When(type.of<int>()))
            ;

            SetUpObject("id");

            _invoker.InvokeDo(_testing, Id("id"), "DoOperationWithIntList",
                Params(
                    Param("list", Id("1", "System.Int32"), Id("2", "System.Int32"))
                ));

            _businessMock.Verify(o => o.DoOperationWithIntList(new List<int> { 101, 102 }));
        }

        [Test]
        public void When_multiple_locating__null_and_initialized_arguments_are_not_sent_to_locator_but_their_indices_are_kept()
        {
            _codingStyle
                .Initializers.Add(c => c.PublicConstructors().When(type.of<BusinessInitializableLocatable>()))
                .Locator.Set(c => c.Locator(l => l.By(ids => ids.Select(id => BusinessInitializableLocatable.Get(id + " test")))).When(type.of<BusinessInitializableLocatable>()))
            ;

            SetUpObject("id");

            _invoker.InvokeDo(_testing, Id("id"), "InitLocateInput",
                Params(
                    Param("input",
                        PD(Id("1", INIT_LOCATE_OMID)),
                        Init(INIT_LOCATE_OMID,
                            Params(
                                Param("value", Id("2", "System.Int32"))
                            )
                        ),
                        PD(Id("3", INIT_LOCATE_OMID)),
                        PD(IdNull()),
                        PD(Id("5", INIT_LOCATE_OMID))
                    )
                )
            );

            _businessMock.Verify(o => o
                .InitLocateInput(
                    It.Is<List<BusinessInitializableLocatable>>(list =>
                        list[0].Value == "1 test" &&
                        list[1].Value == "2" &&
                        list[2].Value == "3 test" &&
                        list[3] == null &&
                        list[4].Value == "5 test"
                    )
                ),
                Times.Once()
            );
        }

        [Test]
        public void By_default_perform_operation_returns_eager_result()
        {
            SetUpObject("id", "title");

            _businessMock.Setup(o => o.GetResult()).Returns(_businessObj);

            var result = _invoker.InvokeDo(_testing, Id("id"), "GetResult", Params());

            Assert.That(result.Values[0].Data.ContainsKey("Title"), Is.True);
            Assert.That(result.Values[0].Data["Title"].Values[0].Id, Is.EqualTo("title"));
        }

        [Test]
        public void Target_operation_can_have_more_than_one_parameter_group_so_that_domain_object_can_expose_overloaded_methods_as_one_service()
        {
            SetUpObject("id");

            _invoker.InvokeDo(_testing, Id("id"), "OverloadOp",
                Params(
                    Param("i", Id("1", "System.Int32"))
                )
            );
            _businessMock.Verify(o => o.OverloadOp(1), Times.Once());

            _invoker.InvokeDo(_testing, Id("id"), "OverloadOp",
                Params(
                    Param("s", Id("s_value", "System.String"))
                )
            );
            _businessMock.Verify(o => o.OverloadOp("s_value"), Times.Once());

            _invoker.InvokeDo(_testing, Id("id"), "OverloadOp",
                Params(
                    Param("s", Id("s_value2", "System.String")),
                    Param("i", Id("2", "System.Int32"))
                )
            );
            _businessMock.Verify(o => o.OverloadOp("s_value2", 2), Times.Once());
        }

        [Test]
        public void When_given_parameters_do_not_match_exactly_with_any_operations__uses_the_group_with_most_matched_parameters()
        {
            SetUpObject("id");

            _invoker.InvokeDo(_testing, Id("id"), "OverloadOp",
                Params(
                    Param("s", Id("s_value", "System.String")),
                    Param("s1", Id("s_value1", "System.String"))
                )
            );

            //group 0 -> match 0
            //group 1 -> match 1 (s)
            //group 2 -> match 0
            //group 3 -> match 1 (s)
            //group 4 -> match 2 (s1, s) --> WINS!
            _businessMock.Verify(o => o.OverloadOp("s_value1", "s_value", 0), Times.Once());
        }

        [Test]
        public void When_there_are_more_than_one_group_having_the_same_number_of_most_matched_parameters__uses_the_group_with_least_non_matched_parameters()
        {
            SetUpObject("id");

            _invoker.InvokeDo(_testing, Id("id"), "OverloadOp",
                Params(
                    Param("s", Id("s_value", "System.String")),
                    Param("i", Id("1", "System.Int32")),
                    Param("s1", Id("s_value1", "System.String"))
                )
            );

            //group 0 -> match 0
            //group 1 -> match 1 (s)
            //group 2 -> match 1 (i)
            //group 3 -> match 2 (s, i) --> non-match 0 --> WINS!
            //group 3 -> match 2 (s1, s) --> non-match 1 (i1)
            _businessMock.Verify(o => o.OverloadOp("s_value", 1), Times.Once());

            _invoker.InvokeDo(_testing, Id("id"), "OverloadOp",
                Params(
                    Param("a", Id("dummy", "System.String"))
                )
            );

            //group 0 -> match 0 --> non-match 0 --> WINS!
            //group 1 -> match 0 --> non-match 1 (s)
            //group 2 -> match 0 --> non-match 1 (i)
            //group 3 -> match 0 --> non-match 2 (s, i)
            //group 3 -> match 0 --> non-match 3 (s1, s, i1)
            _businessMock.Verify(o => o.OverloadOp(), Times.Once());
        }

        [Test]
        public void A_group_can_have_different_index_for_a_common_parameter()
        {
            SetUpObject("id");

            _invoker.InvokeDo(_testing, Id("id"), "OverloadArray",
                Params(
                    Param("s", Id("s_value", "System.String")),
                    Param("i", Id("1", "System.Int32"), Id("2", "System.Int32"))
                )
            );

            _businessMock.Verify(o => o.OverloadArray(It.Is<int[]>(i => i[0] == 1 && i[1] == 2), "s_value"), Times.Once);
        }

        [Test]
        public void When_an_optional_parameter_is_not_present__it_passes_default_value()
        {
            SetUpObject("id");

            _invoker.InvokeDo(_testing, Id("id"), "OptionalParameterOp",
                Params(
                    Param("required", Id("required_value", "System.String"))
                )
            );

            _businessMock.Verify(o => o.OptionalParameterOp("required_value", "default"), Times.Once);
        }
        [Test]
        public void When_an_optional_parameter_is_present_but_its_null__it_passes_null_as_its_value()
        {
            SetUpObject("id");

            _invoker.InvokeDo(_testing, Id("id"), "OptionalParameterOp",
                Params(
                    Param("required", Id("required_value", "System.String")),
                    Param("optional", IdNull())
                )
            );

            _businessMock.Verify(o => o.OptionalParameterOp("required_value", null), Times.Once);
        }

        [Test]
        public void Virtual_types_can_behave_as_a_proxy()
        {
            _codingStyle
                .Use(p => p.VirtualTypePattern())
                .AddTypes(v => v.FromBasic()
                    .Name.Set("VirtualOperation")
                    .Namespace.Set(RootNamespace)
                    .Methods.Add(p => p.Proxy<IBusinessOperation>("Void").Target(_businessObj)))
            ;

            var dummy = _testing.ApplicationModel;

            _invoker.InvokeDo(_testing, Id("virtual", VIRTUAL_OMID), "Void", Params());

            _businessMock.Verify(o => o.Void(), Times.Once);
        }

        [Test]
        public void Virtual_inheritance_allows_target_to_be_cast_to_virtual_interface()
        {
            _codingStyle
                .Use(p => p.VirtualTypePattern())
                .AddTypes(v => v.FromBasic()
                    .Name.Set("VirtualOperation")
                    .Namespace.Set(RootNamespace)
                    .AssignableTypes.Add(vi => vi
                        .FromBasic()
                        .IsInterface.Set(true)
                        .Name.Set("IVirtualOperation")
                        .Namespace.Set(RootNamespace)
                        .Methods.Add(p => p.Proxy<IBusinessOperation>("Void").Target(_businessObj))
                    )
                )
            ;

            var dummy = _testing.ApplicationModel;

            _invoker.InvokeDo(_testing, Id("virtual", VIRTUAL_OMID, VIRTUAL_VIMID), "Void", Params());

            _businessMock.Verify(o => o.Void(), Times.Once);
        }

        [Test]
        public void Proxy_operations_includes_overloads()
        {
            _codingStyle
                .Use(p => p.VirtualTypePattern())
                .AddTypes(v => v.FromBasic()
                    .Name.Set("VirtualOperation")
                    .Namespace.Set(RootNamespace)
                    .Methods.Add(o => o.Proxy<IBusinessOperation>("OverloadOp").Target(_businessObj)))
            ;

            var dummy = _testing.ApplicationModel;

            _invoker.InvokeDo(_testing, Id("virtual", VIRTUAL_OMID), "OverloadOp", Params());

            _invoker.InvokeDo(_testing, Id("virtual", VIRTUAL_OMID), "OverloadOp", Params(
                Param("s", Id("s_value", "System.String"))
            ));

            _invoker.InvokeDo(_testing, Id("virtual", VIRTUAL_OMID), "OverloadOp", Params(
                Param("i", Id("1", "System.Int32"))
            ));

            _invoker.InvokeDo(_testing, Id("virtual", VIRTUAL_OMID), "OverloadOp", Params(
                Param("s", Id("s_value", "System.String")),
                Param("i", Id("1", "System.Int32"))
            ));

            _invoker.InvokeDo(_testing, Id("virtual", VIRTUAL_OMID), "OverloadOp", Params(
                Param("s1", Id("s1_value", "System.String")),
                Param("s", Id("s_value", "System.String")),
                Param("i1", Id("2", "System.Int32"))
            ));

            _businessMock.Verify(o => o.OverloadOp(), Times.Once);
            _businessMock.Verify(o => o.OverloadOp("s_value"), Times.Once);
            _businessMock.Verify(o => o.OverloadOp(1), Times.Once);
            _businessMock.Verify(o => o.OverloadOp("s_value", 1), Times.Once);
            _businessMock.Verify(o => o.OverloadOp("s1_value", "s_value", 2), Times.Once);
        }

        [Test]
        public void When_configured_proxy_operations_optionally_adds_a_parameter_for_target()
        {
            _codingStyle
                .Use(p => p.VirtualTypePattern())
                .AddTypes(v => v.FromBasic()
                    .Name.Set("VirtualOperation")
                    .Namespace.Set(RootNamespace)
                    .Methods.Add(p => p.Proxy<IBusinessOperation>("OverloadOp").TargetByParameter("objId")))
            ;

            SetUpObject("id");

            _invoker.InvokeDo(_testing, Id("virtual", VIRTUAL_OMID), "OverloadOp", Params(
                Param("objId", Id("id")),
                Param("s1", Id("s1_value", "System.String")),
                Param("s", Id("s_value", "System.String")),
                Param("i1", Id("2", "System.Int32"))
            ));

            _businessMock.Verify(o => o.OverloadOp("s1_value", "s_value", 2), Times.Once);
        }

        [Test]
        public void Proxy_operations_on_real_types_performs_on_given_target()
        {
            _codingStyle
                .Operations.Add(c => c.Build(o => o.Proxy<IBusinessOperation>("Void").TargetBySelf()).When(type.of<BusinessOperation>()))
            ;

            SetUpObject("id");

            _invoker.InvokeDo(_testing, Id("id"), "Void", Params());

            _businessMock.Verify(o => o.Void(), Times.Once);
        }

        [Test]
        public void Virtual_operations_perform_using_given_delegate()
        {
            _codingStyle
                .Use(p => p.VirtualTypePattern())
                .AddTypes(v => v.FromBasic()
                    .Name.Set("VirtualOperation")
                    .Namespace.Set(RootNamespace)
                    .Methods.Add(o => o.Virtual("Ping", (string input) => "ping: " + input)))
            ;

            var dummy = _testing.ApplicationModel;

            var result = _invoker.InvokeDo(_testing, Id("virtual", VIRTUAL_OMID), "Ping", Params(
                Param("input", Id("test", "System.String"))
            ));

            Assert.That(result.Values[0].Id, Is.EqualTo("ping: test"));
        }
    }
}
