using Routine.Client;
using Routine.Core;
using Routine.Engine.Context;
using Routine.Test.Client.Stubs;

namespace Routine.Test.Client;

[TestFixture]
public class ClientTest : ClientTestBase
{
    public static List<IStubber> Stubbers() => new() { new Sync(), new Async() };

    [Test]
    public void Client_gets_all_types_in_application_via_Rapplication()
    {
        ModelsAre(Model("model1"), Model("model2"));

        var actual = _testingRapplication.Types;

        Assert.That(actual.Any(m => m.Id == "model1"), Is.True);
        Assert.That(actual.Any(m => m.Id == "model2"), Is.True);
    }

    [Test]
    public void Client_can_access_a_business_object_by_creating_a_Robject_using_model_and_id_information()
    {
        ModelsAre(Model("actualModel").ViewModelIds("viewModel"), Model("viewModel").IsView("actualModel"));
        ObjectsAre(Object(Id("id", "actualModel", "viewModel")).Display("value"));

        var testingRobject = Robj("id", "actualModel", "viewModel");

        Assert.That(testingRobject.Display, Is.EqualTo("value"));
    }

    [Test]
    public void When_creating_Robject__Cached_application_model_is_used()
    {
        ModelsAre(Model("actualModel"));
        ObjectsAre(Object(Id("id", "actualModel")));

        Robj("id", "actualModel");
        Robj("id", "actualModel");
        Robj("id", "actualModel");

        _mockObjectService.Verify(o => o.ApplicationModel, Times.Once());
    }

    [Test]
    public void Client_gets_object_model_information_via_Rtype()
    {
        ModelsAre(Model("model").Module("module"));

        var testingRtype = Rtyp("model");

        Assert.That(testingRtype.Id, Is.EqualTo("model"));
        Assert.That(testingRtype.Module, Is.EqualTo("module"));
    }

    [Test]
    public void Rtype_lists_its_view_types()
    {
        ModelsAre(
            Model("viewModel1").IsView("actualModel"),
            Model("viewModel2").IsView("actualModel"),
            Model("actualModel").ViewModelIds("viewModel1", "viewModel2")
        );

        var testingRtype = Rtyp("actualModel");

        Assert.That(testingRtype.ViewTypes.Count, Is.EqualTo(2));
        Assert.That(testingRtype.ViewTypes[0].Id, Is.EqualTo("viewModel1"));
        Assert.That(testingRtype.ViewTypes[1].Id, Is.EqualTo("viewModel2"));
    }

    [Test]
    public void Rtype_list_its_actual_types()
    {
        ModelsAre(
            Model("viewModel").IsView("actualModel1", "actualModel2"),
            Model("actualModel1").ViewModelIds("viewModel"),
            Model("actualModel2").ViewModelIds("viewModel")
        );

        var testingRtype = Rtyp("viewModel");

        Assert.That(testingRtype.ActualTypes.Count, Is.EqualTo(2));
        Assert.That(testingRtype.ActualTypes[0].Id, Is.EqualTo("actualModel1"));
        Assert.That(testingRtype.ActualTypes[1].Id, Is.EqualTo("actualModel2"));
    }

    [Test]
    public void Client_gets_object_id_and_value_information_via_Robject()
    {
        ModelsAre(
            Model("actualModel").Module("actualModule").ViewModelIds("viewModel"),
            Model("viewModel").Module("viewModule").IsView("actualModel")
        );

        ObjectsAre(Object(Id("id", "actualModel", "viewModel")).Display("value"));

        var testingRobject = Robj("id", "actualModel", "viewModel");

        Assert.That(testingRobject.Id, Is.EqualTo("id"));
        Assert.That(testingRobject.ActualType.Id, Is.EqualTo("actualModel"));
        Assert.That(testingRobject.Display, Is.EqualTo("value"));
        Assert.That(testingRobject.ViewType.Id, Is.EqualTo("viewModel"));
        Assert.That(testingRobject.Type.Module, Is.EqualTo("viewModule"));
    }

    [TestCaseSource(nameof(Stubbers))]
    public void Robjects_fetch_data_when_asked(IStubber stubber)
    {
        ModelsAre(
            Model("model")
                .Data("data1")
                .Data("data2")
        );

        ObjectsAre(
            Object(Id("id1")),
            Object(Id("id2")),
            Object(Id("id", "model"))
                .Data("data1", Id("id1"))
                .Data("data2", Id("id2"))
        );

        var testingRobject = Robj("id", "model");
        var data1 = testingRobject.DataValues[0];
        var data2 = testingRobject.DataValues[1];

        Assert.That(data1.Data.Name, Is.EqualTo("data1"));
        Assert.That(stubber.Get(data1).Object.Id, Is.EqualTo("id1"));

        Assert.That(data2.Data.Name, Is.EqualTo("data2"));
        Assert.That(stubber.Get(data2).Object.Id, Is.EqualTo("id2"));

        stubber.VerifyGet(_mockObjectService);
    }

    [TestCaseSource(nameof(Stubbers))]
    public void Robjects_fetch_data_only_once(IStubber stubber)
    {
        ModelsAre(
            Model("model")
                .Data("data1")
                .Data("data2")
        );

        ObjectsAre(
            Object(Id("id1")),
            Object(Id("id2")),
            Object(Id("id", "model"))
                .Data("data1", Id("id1"))
                .Data("data2", Id("id2"))
        );

        var testingRobject = Robj("id", "model");

        var datasFirstFetch = testingRobject.DataValues;
        stubber.Get(datasFirstFetch[0]);
        stubber.Get(datasFirstFetch[1]);

        var datasSecondFetch = testingRobject.DataValues;
        stubber.Get(datasSecondFetch[0]);

        stubber.VerifyGet(_mockObjectService, Times.Once());
    }

    [TestCaseSource(nameof(Stubbers))]
    public void Robjects_fetch_value_along_with_data(IStubber stubber)
    {
        ModelsAre(Model("model").Data("data1"));

        ObjectsAre(
            Object(Id("id1")).Display("value1"),
            Object(Id("id", "model"))
                .Display("value")
                .Data("data1", Id("id1"))
        );

        var testingRobject = Robj("id", "model");
        stubber.Get(testingRobject.DataValues[0]);

        Assert.That(testingRobject.Display, Is.EqualTo("value"));
    }

    [TestCaseSource(nameof(Stubbers))]
    public void Robjects_load_data_on_demand(IStubber stubber)
    {
        ModelsAre(Model("model").Data("data1"));

        ObjectsAre(
            Object(Id("id1")),
            Object(Id("id", "model"))
                .Data("data1", Id("id1"))
        );

        var testingRobject = Robj("id", "model");

        stubber.Load(testingRobject);

        stubber.VerifyGet(_mockObjectService);

        stubber.Get(testingRobject.DataValues[0]);

        stubber.VerifyGet(_mockObjectService, Times.Once());
    }

    [TestCaseSource(nameof(Stubbers))]
    public void Rvariable_behaves_optimistic_on_single__list__void_and_null_mode(IStubber stubber)
    {
        ModelsAre(
            Model("model")
                .Operation("operation", true)
        );

        ObjectsAre(
            Object(Id("id1")),
            Object(Id("id2")),
            Object(Id("id3", "model"))
        );

        var obj1 = Robj("id1");
        var obj2 = Robj("id2");
        var obj3 = Robj("id3", "model");

        //accessing single as list returns a list with that single object
        var testingRvariable = Rvar("testingSingle", obj1);
        Assert.That(testingRvariable.List.Count, Is.EqualTo(1));
        Assert.That(testingRvariable.List[0].Id, Is.EqualTo("id1"));

        //accessing list as single returns the first item
        testingRvariable = Rvarlist("testingList", new[] { obj1, obj2 });
        Assert.That(testingRvariable.Object.Id, Is.EqualTo("id1"));

        //accessing list as single returns null if there is no items in the list
        testingRvariable = Rvarlist("testingEmptyList", Array.Empty<Robject>());
        Assert.That(testingRvariable.Object.IsNull, Is.True);

        //accessing void as single returns null object
        stubber.SetUp(_mockObjectService,
            id: Id("id3", "model"),
            operation: "operation",
            result: Void()
        );
        testingRvariable = stubber.Perform(obj3, "operation");
        Assert.That(testingRvariable.IsNull, Is.True);
        Assert.That(testingRvariable.Object.IsNull, Is.True);

        //accessing void as list returns empty list
        Assert.That(testingRvariable.List.Count, Is.EqualTo(0));
    }

    [Test]
    public void Rvariable_can_be_null()
    {
        ModelsAre(Model("model").Data("data"));

        ObjectsAre(Object(Id("id", "model")).Data("data", Null()));

        var robj = Robj("id", "model");
        var testingRvariable = robj.DataValues[0].Get();

        Assert.That(testingRvariable.IsNull, Is.True);
        Assert.That(testingRvariable.ToList().IsNull, Is.True);
    }

    [Test]
    public void Robject_can_be_null()
    {
        ModelsAre(Model("model").Data("data"));

        ObjectsAre(Object(Id("id", "model")).Data("data", Null()));

        var robj = Robj("id", "model");
        var testingRvariable = robj.DataValues[0].Get();

        Assert.That(testingRvariable.Object.IsNull, Is.True);
    }

    [TestCaseSource(nameof(Stubbers))]
    public void Robject_behaves_optimistic_when_it_is_null(IStubber stubber)
    {
        ModelsAre(Model("model").Data("data"));

        ObjectsAre(Object(Id("id", "model")).Data("data", Null()));

        var robj = Robj("id", "model");
        var testingRobject = stubber.Get(robj.DataValues[0]).Object;

        Assert.That(testingRobject.DataValues.Count, Is.EqualTo(0));
        Assert.That(testingRobject.Type, Is.Null);
        Assert.That(stubber.Perform(testingRobject, "some non existing operation").IsNull);
    }

    [TestCaseSource(nameof(Stubbers))]
    public void Client_performs_operation_via_Robject(IStubber stubber)
    {
        ModelsAre(
            Model("view_model").IsView("actual_model")
                .Operation("operation", DefaultObjectModelId,
                    PModel("param1"),
                    PModel("param2")
                ),
            Model("actual_model").ViewModelIds("view_model")
        );

        ObjectsAre(
            Object(Id("id", "actual_model", "view_model")),
            Object(Id("id_param1")),
            Object(Id("id_param2")),
            Object(Id("id_result"))
        );

        stubber.SetUp(_mockObjectService,
            id: Id("id", "actual_model", "view_model"),
            operation: "operation",
            match: p =>
                p["param1"].Values[0].Id == "id_param1" &&
                p["param2"].Values[0].Id == "id_param2",
            result: Result(Id("id_result"))
        );

        var testingRobject = Robj("id", "actual_model", "view_model");

        var result = stubber.Perform(testingRobject, "operation",
            Rvar("param1", Robj("id_param1")),
            Rvar("param2", Robj("id_param2"))
        );

        Assert.That(result.Object.Id, Is.EqualTo("id_result"));
    }

    [TestCaseSource(nameof(Stubbers))]
    public void Client_can_initialize_objects_via_Robject(IStubber stubber)
    {
        ModelsAre(
            Model("sub_data_model")
                .Initializer(PModel("param1")),
            Model("data_model")
                .Initializer(
                    PModel("param1", "sub_data_model"),
                    PModel("param2")
                ),
            Model("operational_model")
                .Operation("data_input", DefaultObjectModelId, PModel("data", "data_model"))
        );

        ObjectsAre(
            Object(Id("id", "operational_model")),
            Object(Id("id_sub_data_param1")),
            Object(Id("id_data_param2")),
            Object(Id("id_result"))
        );

        stubber.SetUp(_mockObjectService,
            id: Id("id", "operational_model"),
            operation: "data_input",
            match: p =>
                p["data"].Values[0].ModelId == "data_model" &&
                p["data"].Values[0].Id == null &&
                p["data"].Values[0].InitializationParameters["param1"].Values[0].ModelId == "sub_data_model" &&
                p["data"].Values[0].InitializationParameters["param1"].Values[0].Id == null &&
                p["data"].Values[0].InitializationParameters["param1"].Values[0].InitializationParameters["param1"].Values[0].Id == "id_sub_data_param1" &&
                p["data"].Values[0].InitializationParameters["param2"].Values[0].Id == "id_data_param2",
            result: Result(Id("id_result"))
        );

        var testingRobject = Robj("id", "operational_model");

        var result = stubber.Perform(testingRobject, "data_input",
            Rvar("data",
                Robj("data_model",
                    Rvar("param1",
                        Robj("sub_data_model",
                            Rvar("param1", Robj("id_sub_data_param1"))
                        )
                    ),
                    Rvar("param2",
                        Robj("id_data_param2")
                    )
                )
            )
        );

        Assert.That(result.Object.Id, Is.EqualTo("id_result"));
    }

    [Test]
    public void Initialized_robjects_throws_RobjectIsInitializedOnClientException_when_value_is_accessed()
    {
        ModelsAre(Model("data_model").Initializer(PModel("param1")));

        var robj =
            Robj("data_model",
                Rvar("param1", Robj("id_data_param1"))
            );
        Assert.That(() => { var _ = robj.Display; }, Throws.TypeOf<RobjectIsInitializedOnClientException>(), "exception not thrown");
    }

    [Test]
    public void Initialized_robjects_return_null_when_id_is_accessed()
    {
        ModelsAre(Model("data_model").Initializer(PModel("param1")));

        var robj =
            Robj("data_model",
                Rvar("param1", Robj("id_data_param1"))
            );

        Assert.That(robj.Id, Is.Null);
    }

    [Test]
    public void Initialized_robjects_are_only_equal_to_themselves()
    {
        ModelsAre(Model("data_model").Initializer(PModel("param1")));

        var robj1 =
            Robj("data_model",
                Rvar("param1", Robj("id_data_param1"))
            );

        var robj2 =
            Robj("data_model",
                Rvar("param1", Robj("id_data_param1"))
            );

        Assert.That(robj1, Is.EqualTo(robj1));
        Assert.That(robj2, Is.Not.EqualTo(robj1));
    }

    [TestCaseSource(nameof(Stubbers))]
    public void Roperation_can_return_void_result(IStubber stubber)
    {
        ModelsAre(Model("model").Operation("operation1", true));

        ObjectsAre(Object(Id("id", "model")));

        stubber.SetUp(_mockObjectService,
            id: Id("id", "model"),
            operation: "operation1",
            result: Void()
        );

        var result = stubber.Perform(Robj("id", "model"), "operation1");

        Assert.That(result.IsVoid, Is.True);
    }

    [Test]
    public void Robject_is_naked_when_it_does_not_have_a_view_model_id()
    {
        ModelsAre(
            Model("actual_model").ViewModelIds("view_model"),
            Model("view_model").IsView("actual_model")
        );

        ObjectsAre(
            Object(Id("id")),
            Object(Id("id", "actual_model", "view_model"))
        );

        Assert.That(Robj("id").IsNaked, Is.True);
        Assert.That(Robj("id", "actual_model", "view_model").IsNaked, Is.False);
    }

    [Test]
    public void Rtype_can_be_value_type()
    {
        ModelsAre(
            Model("value_model").IsValue(),
            Model("domain_model1"),
            Model("domain_model2")
        );

        ObjectsAre(
            Object(Id("id", "value_model")),
            Object(Id("id1", "domain_model1")),
            Object(Id("id2", "domain_model2"))
        );

        Assert.That(Robj("id", "value_model").Type.IsValueType, Is.True);
        Assert.That(Robj("id1", "domain_model1").Type.IsValueType, Is.False);
        Assert.That(Robj("id2", "domain_model2").Type.IsValueType, Is.False);
    }

    [Test]
    public void Rparameter_can_create_value_variable_using_given_Robjects()
    {
        ModelsAre(
            Model("model").Operation("operation", PModel("param", "param_model", true)),
            Model("param_model")
        );

        ObjectsAre(
            Object(Id("id_root", "model")),
            Object(Id("id_param_value"))
        );

        var root = Robj("id_root", "model");
        var rparam = root.Type.Operations[0].Parameters[0];

        var paramvar = rparam.CreateVariable(Robj("id_param_value"));

        Assert.That(paramvar.Name, Is.EqualTo("param"));
        Assert.That(paramvar.IsList, Is.True);
    }

    [Test]
    public void Rparameter_returns_its_default_value_when_its_optional()
    {
        ModelsAre(
            Model("model").Operation("operation", PModel("param", "param_model", defaultValue: "default")),
            Model("param_model")
        );

        ObjectsAre(
            Object(Id("id_root", "model")),
            Object(Id("default", "param_model"))
        );

        var root = Robj("id_root", "model");
        var rparam = root.Type.Operations[0].Parameters[0];

        Assert.That(rparam.IsOptional, Is.True);
        Assert.That(rparam.Default.Object.Id, Is.EqualTo("default"));
    }

    [TestCaseSource(nameof(Stubbers))]
    public void Robject_does_not_fetch_object_data_if_model_is_value(IStubber stubber)
    {
        ModelsAre(Model("model").IsValue());

        ObjectsAre(Object(Id("value", "model")));

        var robj = Robj("value", "model");

        var actual = robj.Display;
        Assert.That(actual, Is.EqualTo("value"));
        stubber.VerifyGet(_mockObjectService, Times.Never());

        stubber.Load(robj);
        stubber.VerifyGet(_mockObjectService, Times.Never());
    }

    [Test]
    public void Robjects_cannot_be_created_from_view_types()
    {
        ModelsAre(
            Model("view").IsView("actual"),
            Model("actual").ViewModelIds("view")
        );

        Assert.That(() => Robj("dummy", "view"), Throws.TypeOf<CannotCreateRobjectException>());
    }

    [Test]
    public void Robjects_can_convert_to_one_of_its_view_type()
    {
        ModelsAre(
            Model("string").IsValue(),
            Model("view").IsView("actual")
                .Data("Text", "string"),
            Model("actual").ViewModelIds("view")
                .Data("Text", "string")
        );

        ObjectsAre(
            Object(Id("id", "actual"))
                .Data("Text", Id("from actual", "string")),
            Object(Id("id", "actual", "view"))
                .Data("Text", Id("from view", "string"))
        );

        var robjActual = Robj("id", "actual");
        var robjView = robjActual.As(Rtyp("view"));

        Assert.That(robjView.Id, Is.EqualTo("id"));
        Assert.That(robjView.Type.Id, Is.EqualTo("view"));
        Assert.That(robjView["Text"].Get().Object.Id, Is.EqualTo("from view"));
    }

    [Test]
    public void Robjects_cannot_be_converted_to_a_view_type_that_does_not_belong_to_the_given_actual_type()
    {
        ModelsAre(
            Model("view").IsView("actual1"),
            Model("actual1").ViewModelIds("view"),
            Model("actual2")
        );

        ObjectsAre(Object(Id("id", "actual2")));

        var robjActual = Robj("id", "actual2");

        Assert.That(() => robjActual.As(Rtyp("view")), Throws.TypeOf<CannotCreateRobjectException>());
    }

    [Test]
    public void When_no_model_is_found_TypeNotFoundException_is_thrown()
    {
        _mockObjectService.Setup(o => o.ApplicationModel).Returns(new ApplicationModel());

        ObjectsAre(Object(Id("value", "model")));

        Assert.That(() => Robj("value", "model"), Throws.TypeOf<TypeNotFoundException>());
    }

    [Test]
    public void Client_fetches_static_instances_via_Rapplication()
    {
        ModelsAre(Model("model").StaticInstanceIds("id1", "id2"));

        ObjectsAre(
            Object(Id("id1", "model")).Display("value 1"),
            Object(Id("id2", "model")).Display("value 2")
        );

        var actual = Rtyp("model").StaticInstances;

        Assert.That(actual.Count, Is.EqualTo(2));
        Assert.That(actual[0].Id, Is.EqualTo("id1"));
        Assert.That(actual[0].Display, Is.EqualTo("value 1"));
        Assert.That(actual[1].Id, Is.EqualTo("id2"));
        Assert.That(actual[1].Display, Is.EqualTo("value 2"));
    }

    [TestCaseSource(nameof(Stubbers))]
    public void When_invalidated__Robject_clears_fetched_data(IStubber stubber)
    {
        ModelsAre(Model("model").Data("data1"));

        ObjectsAre(
            Object(Id("id1")),
            Object(Id("id", "model")).Display("value")
                .Data("data1", Id("id1"))
        );

        var testingRobject = Robj("id", "model");

        stubber.Get(testingRobject["data1"]);
        var value1 = testingRobject.Display;

        Assert.That(value1, Is.EqualTo("value"));

        testingRobject.Invalidate();

        stubber.Get(testingRobject["data1"]);
        var value2 = testingRobject.Display;

        Assert.That(value2, Is.EqualTo("value"));
        stubber.VerifyGet(_mockObjectService, Times.Exactly(2));
    }

    [Test]
    public void Rtype_can_check_if_its_model_is_marked_as_given_mark()
    {
        ModelsAre(Model("model").Mark("mark"));

        ObjectsAre(Object(Id("id1", "model")));

        var testingRobject = Robj("id", "model");

        Assert.That(testingRobject.Type.MarkedAs("mark"), Is.True);
        Assert.That(testingRobject.Type.MarkedAs("nonexistingmark"), Is.False);
    }

    [Test]
    public void Rdata_can_check_if_its_model_is_marked_as_given_mark()
    {
        ModelsAre(
            Model("model")
                .Data("data")
                .MarkData("data", "mark")
        );

        ObjectsAre(
            Object(Id("id")),
            Object(Id("id1", "model"))
                .Data("data", Id("id"))
        );

        var testingRdata = Robj("id", "model")["data"];

        Assert.That(testingRdata.Data.MarkedAs("mark"), Is.True);
        Assert.That(testingRdata.Data.MarkedAs("nonexistingmark"), Is.False);
    }

    [Test]
    public void Roperation_can_check_if_its_model_is_marked_as_given_mark()
    {
        ModelsAre(
            Model("model")
                .Operation("operation")
                .MarkOperation("operation", "mark")
        );

        ObjectsAre(Object(Id("id1", "model")));

        var testingRoperation = Robj("id", "model").Type.Operations.Single(o => o.Name == "operation");

        Assert.That(testingRoperation.MarkedAs("mark"), Is.True);
        Assert.That(testingRoperation.MarkedAs("nonexistingmark"), Is.False);
    }

    [Test]
    public void Roperation_and_throws_exception_when_a_parameter_has_a_group_that_does_not_exist_on_operation()
    {
        ModelsAre(
            Model("model")
                .Operation(new OperationModel
                {
                    GroupCount = 1,
                    Name = "operation",
                    Result = new() { IsVoid = true },
                    Parameters = new() { PModel("id", 0, 1) }
                })
            );

        Assert.That(() => Rtyp("model2"), Throws.TypeOf<InvalidOperationException>());
    }

    [Test]
    public void Roperation_returns_group_parameters_when_asked()
    {
        ModelsAre(
            Model("model")
                .Operation("operation",
                    PModel("param1", "param_model", 0, 1),
                    PModel("param2", "param_model", 1, 2)
                ),
            Model("param_model")
        );

        ObjectsAre(Object(Id("id_root", "model")));

        var root = Robj("id_root", "model");
        var rop = root.Type.Operations[0];

        var groups = rop.Groups;

        Assert.That(groups.Count, Is.EqualTo(3));
        Assert.That(groups[0][0].Name, Is.EqualTo("param1"));
        Assert.That(groups[1][0].Name, Is.EqualTo("param1"));
        Assert.That(groups[1][1].Name, Is.EqualTo("param2"));
        Assert.That(groups[2][0].Name, Is.EqualTo("param2"));
    }

    [Test]
    public void Rinitializer_throws_exception_when_a_parameter_has_a_group_that_does_not_exist_on_or_initializer()
    {
        ModelsAre(Model("model").Initializer(1, PModel("id", 0, 1)));

        Assert.That(() => Rtyp("model"), Throws.TypeOf<InvalidOperationException>());
    }
}