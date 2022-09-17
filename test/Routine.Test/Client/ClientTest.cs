using Routine.Client;
using Routine.Core;
using Routine.Engine.Context;
using Routine.Test.Client.Stubs.Performers;
using Routine.Test.Client.Stubs.Getters;

using SyncGetter = Routine.Test.Client.Stubs.Getters.Sync;
using AsyncGetter = Routine.Test.Client.Stubs.Getters.Async;
using SyncPerformer = Routine.Test.Client.Stubs.Performers.Sync;
using AsyncPerformer = Routine.Test.Client.Stubs.Performers.Async;

namespace Routine.Test.Client;

[TestFixture]
public class ClientTest : ClientTestBase
{
    public static List<IPerformer> Performers() => new() { new SyncPerformer(), new AsyncPerformer() };
    public static List<IGetter> Getters() => new() { new SyncGetter(), new AsyncGetter() };

    [Test]
    public void Client_gets_all_types_in_application_via_Rapplication()
    {
        ModelsAre(Model("model1"), Model("model2"));

        var actual = testingRapplication.Types;

        Assert.IsTrue(actual.Any(m => m.Id == "model1"));
        Assert.IsTrue(actual.Any(m => m.Id == "model2"));
    }

    [Test]
    public void Client_can_access_a_business_object_by_creating_a_Robject_using_model_and_id_information()
    {
        ModelsAre(Model("actualModel").ViewModelIds("viewModel"), Model("viewModel").IsView("actualModel"));
        ObjectsAre(Object(Id("id", "actualModel", "viewModel")).Display("value"));

        var testingRobject = Robj("id", "actualModel", "viewModel");

        Assert.AreEqual("value", testingRobject.Display);
    }

    [Test]
    public void When_creating_Robject__Cached_application_model_is_used()
    {
        ModelsAre(Model("actualModel"));
        ObjectsAre(Object(Id("id", "actualModel")));

        Robj("id", "actualModel");
        Robj("id", "actualModel");
        Robj("id", "actualModel");

        objectServiceMock.Verify(o => o.ApplicationModel, Times.Once());
    }

    [Test]
    public void Client_gets_object_model_information_via_Rtype()
    {
        ModelsAre(Model("model").Module("module"));

        var testingRtype = Rtyp("model");

        Assert.AreEqual("model", testingRtype.Id);
        Assert.AreEqual("module", testingRtype.Module);
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

        Assert.AreEqual(2, testingRtype.ViewTypes.Count);
        Assert.AreEqual("viewModel1", testingRtype.ViewTypes[0].Id);
        Assert.AreEqual("viewModel2", testingRtype.ViewTypes[1].Id);
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

        Assert.AreEqual(2, testingRtype.ActualTypes.Count);
        Assert.AreEqual("actualModel1", testingRtype.ActualTypes[0].Id);
        Assert.AreEqual("actualModel2", testingRtype.ActualTypes[1].Id);
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

        Assert.AreEqual("id", testingRobject.Id);
        Assert.AreEqual("actualModel", testingRobject.ActualType.Id);
        Assert.AreEqual("value", testingRobject.Display);
        Assert.AreEqual("viewModel", testingRobject.ViewType.Id);
        Assert.AreEqual("viewModule", testingRobject.Type.Module);
    }

    [TestCaseSource(nameof(Getters))]
    public void Robjects_fetch_data_when_asked(IGetter getter)
    {
        ModelsAre(
            Model("model")
            .Data("data1")
            .Data("data2")
        );

        ObjectsAre(
            Object(Id("id1")),
            Object(Id("id2"))
        );

        ObjectsAre(
            Object(Id("id", "model"))
            .Data("data1", Id("id1"))
            .Data("data2", Id("id2"))
        );

        var testingRobject = Robj("id", "model");

        var datasFirstFetch = testingRobject.DataValues;
        Assert.AreEqual("data1", datasFirstFetch[0].Data.Name);
        Assert.AreEqual("id1", getter.Get(datasFirstFetch[0]).Object.Id);

        Assert.AreEqual("data2", datasFirstFetch[1].Data.Name);
        Assert.AreEqual("id2", getter.Get(datasFirstFetch[1]).Object.Id);

        getter.VerifyGet(objectServiceMock);
    }

    [TestCaseSource(nameof(Getters))]
    public void Robjects_fetch_data_only_once(IGetter getter)
    {
        ModelsAre(
            Model("model")
            .Data("data1")
            .Data("data2")
        );

        ObjectsAre(
            Object(Id("id1")),
            Object(Id("id2"))
        );

        ObjectsAre(
            Object(Id("id", "model"))
            .Data("data1", Id("id1"))
            .Data("data2", Id("id2"))
        );

        var testingRobject = Robj("id", "model");

        var datasFirstFetch = testingRobject.DataValues;
        getter.Get(datasFirstFetch[0]);
        getter.Get(datasFirstFetch[1]);

        var datasSecondFetch = testingRobject.DataValues;
        getter.Get(datasSecondFetch[0]);

        getter.VerifyGet(objectServiceMock, Times.Once());
    }

    [TestCaseSource(nameof(Getters))]
    public void Robjects_fetch_value_along_with_data(IGetter getter)
    {
        ModelsAre(Model("model").Data("data1"));
        ObjectsAre(
            Object(Id("id1")).Display("value1"),
            Object(Id("id", "model"))
            .Display("value")
            .Data("data1", Id("id1")));

        var testingRobject = Robj("id", "model");
        getter.Get(testingRobject.DataValues[0]);

        Assert.AreEqual("value", testingRobject.Display);
    }

    // [TestCaseSource(nameof(Loaders))]
    public void Robjects_load_data_on_demand()
    {
        Assert.Fail("not tested");
        ModelsAre(
            Model("model")
            .Data("data1")
            .Data("data2"));

        ObjectsAre(
            Object(Id("id1")),
            Object(Id("id2")));
        ObjectsAre(
            Object(Id("id", "model"))
            .Data("data1", Id("id1"))
            .Data("data2", Id("id2")));

        var testingRobject = Robj("id", "model");

        testingRobject.LoadObject();

        var datasFirstFetch = testingRobject.DataValues;
        Assert.AreEqual("data1", datasFirstFetch[0].Data.Name);
        Assert.AreEqual("id1", datasFirstFetch[0].Get().Object.Id);

        Assert.AreEqual("data2", datasFirstFetch[1].Data.Name);
        Assert.AreEqual("id2", datasFirstFetch[1].Get().Object.Id);

        var datasSecondFetch = testingRobject.DataValues;
        Assert.AreEqual("data1", datasSecondFetch[0].Data.Name);
        Assert.AreEqual("id1", datasSecondFetch[0].Get().Object.Id);

        Assert.AreEqual("data2", datasSecondFetch[1].Data.Name);
        Assert.AreEqual("id2", datasSecondFetch[1].Get().Object.Id);

        objectServiceMock.Verify(o => o.Get(It.IsAny<ReferenceData>()), Times.Once());
    }

    [TestCaseSource(nameof(Performers))]
    public void Rvariable_behaves_optimistic_on_single__list__void_and_null_mode(IPerformer performer)
    {
        ModelsAre(
            Model("model")
            .Operation("operation", true));

        ObjectsAre(
            Object(Id("id1")),
            Object(Id("id2")),
            Object(Id("id3", "model")));

        var obj1 = Robj("id1");
        var obj2 = Robj("id2");
        var obj3 = Robj("id3", "model");

        //accessing single as list returns a list with that single object
        var testingRvariable = Rvar("testingSingle", obj1);
        Assert.AreEqual(1, testingRvariable.List.Count);
        Assert.AreEqual("id1", testingRvariable.List[0].Id);

        //accessing list as single returns the first item
        testingRvariable = Rvarlist("testingList", new[] { obj1, obj2 });
        Assert.AreEqual("id1", testingRvariable.Object.Id);

        //accessing list as single returns null if there is no items in the list
        testingRvariable = Rvarlist("testingEmptyList", Array.Empty<Robject>());
        Assert.IsTrue(testingRvariable.Object.IsNull);

        //accessing void as single returns null object
        performer.SetUp(objectServiceMock,
            id: Id("id3", "model"),
            operation: "operation",
            result: Void()
        );
        testingRvariable = performer.Perform(obj3, "operation");
        Assert.IsTrue(testingRvariable.IsNull);
        Assert.IsTrue(testingRvariable.Object.IsNull);

        //accessing void as list returns empty list
        Assert.AreEqual(0, testingRvariable.List.Count);
    }

    [Test]
    public void Rvariable_can_be_null()
    {
        ModelsAre(Model("model").Data("data"));

        ObjectsAre(Object(Id("id", "model")).Data("data", Null()));

        var robj = Robj("id", "model");
        var testingRvariable = robj.DataValues[0].Get();

        Assert.IsTrue(testingRvariable.IsNull);
        Assert.IsTrue(testingRvariable.ToList().IsNull);
    }

    [Test]
    public void Robject_can_be_null()
    {
        ModelsAre(Model("model").Data("data"));

        ObjectsAre(Object(Id("id", "model")).Data("data", Null()));

        var robj = Robj("id", "model");
        var testingRvariable = robj.DataValues[0].Get();

        Assert.IsTrue(testingRvariable.Object.IsNull);
    }

    [TestCaseSource(nameof(Performers))]
    public void Robject_behaves_optimistic_when_it_is_null(IPerformer performer)
    {
        Assert.Fail("add GetAsync");

        ModelsAre(Model("model").Data("data"));

        ObjectsAre(Object(Id("id", "model")).Data("data", Null()));

        var robj = Robj("id", "model");
        var testingRobject = robj.DataValues[0].Get().Object;

        Assert.AreEqual(0, testingRobject.DataValues.Count);
        Assert.IsNull(testingRobject.Type);
        Assert.IsTrue(performer.Perform(testingRobject, "some non existing operation").IsNull);
    }

    [TestCaseSource(nameof(Performers))]
    public void Client_performs_operation_via_Robject(IPerformer performer)
    {
        ModelsAre(
            Model("view_model").IsView("actual_model")
            .Operation("operation", DefaultObjectModelId, PModel("param1"), PModel("param2")),
            Model("actual_model").ViewModelIds("view_model"));

        ObjectsAre(
            Object(Id("id", "actual_model", "view_model")),
            Object(Id("id_param1")),
            Object(Id("id_param2")),
            Object(Id("id_result")));

        performer.SetUp(objectServiceMock,
            id: Id("id", "actual_model", "view_model"),
            operation: "operation",
            match: p =>
                p["param1"].Values[0].Id == "id_param1" &&
                p["param2"].Values[0].Id == "id_param2",
            result: Result(Id("id_result"))
        );

        var testingRobject = Robj("id", "actual_model", "view_model");

        var result = performer.Perform(testingRobject, "operation",
            Rvar("param1", Robj("id_param1")),
            Rvar("param2", Robj("id_param2"))
        );

        Assert.AreEqual("id_result", result.Object.Id);
    }

    [TestCaseSource(nameof(Performers))]
    public void Client_can_initialize_objects_via_Robject(IPerformer performer)
    {
        ModelsAre(
            Model("sub_data_model")
                .Initializer(PModel("param1")),
            Model("data_model")
                .Initializer(PModel("param1", "sub_data_model"), PModel("param2")),
            Model("operational_model")
                .Operation("data_input", DefaultObjectModelId, PModel("data", "data_model"))
        );

        ObjectsAre(
            Object(Id("id", "operational_model")),
            Object(Id("id_sub_data_param1")),
            Object(Id("id_data_param2")),
            Object(Id("id_result"))
        );

        performer.SetUp(objectServiceMock,
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

        var result = performer.Perform(testingRobject, "data_input",
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

        Assert.AreEqual("id_result", result.Object.Id);
    }

    [Test]
    public void Initialized_robjects_throws_RobjectIsInitializedOnClientException_when_value_is_accessed()
    {
        ModelsAre(
            Model("data_model")
                .Initializer(PModel("param1"))
        );

        var robj = Robj("data_model",
                        Rvar("param1", Robj("id_data_param1"))
                    );

        Assert.Throws<RobjectIsInitializedOnClientException>(() => { var _ = robj.Display; }, "exception not thrown");
    }

    [Test]
    public void Initialized_robjects_return_null_when_id_is_accessed()
    {
        ModelsAre(
            Model("data_model")
                .Initializer(PModel("param1"))
        );

        var robj = Robj("data_model",
                        Rvar("param1", Robj("id_data_param1"))
                    );

        Assert.IsNull(robj.Id);
    }

    [Test]
    public void Initialized_robjects_are_only_equal_to_themselves()
    {
        ModelsAre(
            Model("data_model")
                .Initializer(PModel("param1"))
        );

        var robj1 = Robj("data_model",
                        Rvar("param1", Robj("id_data_param1"))
                    );

        var robj2 = Robj("data_model",
                        Rvar("param1", Robj("id_data_param1"))
                    );

        Assert.AreEqual(robj1, robj1);
        Assert.AreNotEqual(robj1, robj2);
    }

    [TestCaseSource(nameof(Performers))]
    public void Roperation_can_return_void_result(IPerformer performer)
    {
        ModelsAre(
            Model("model")
            .Operation("operation1", true));

        ObjectsAre(Object(Id("id", "model")));

        performer.SetUp(objectServiceMock,
            id: Id("id", "model"),
            operation: "operation1",
            result: Void()
        );

        var result = performer.Perform(Robj("id", "model"), "operation1");

        Assert.IsTrue(result.IsVoid);
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

        Assert.IsTrue(Robj("id").IsNaked);
        Assert.IsFalse(Robj("id", "actual_model", "view_model").IsNaked);
    }

    [Test]
    public void Rtype_can_be_value_type()
    {
        ModelsAre(
            Model("value_model").IsValue(),
            Model("domain_model1"),
            Model("domain_model2"));

        ObjectsAre(
            Object(Id("id", "value_model")),
            Object(Id("id1", "domain_model1")),
            Object(Id("id2", "domain_model2")));

        Assert.IsTrue(Robj("id", "value_model").Type.IsValueType);
        Assert.IsFalse(Robj("id1", "domain_model1").Type.IsValueType);
        Assert.IsFalse(Robj("id2", "domain_model2").Type.IsValueType);
    }

    [Test]
    public void Rparameter_can_create_value_variable_using_given_Robjects()
    {
        ModelsAre(
            Model("model")
            .Operation("operation", PModel("param", "param_model", true)),
            Model("param_model"));

        ObjectsAre(
            Object(Id("id_root", "model")),
            Object(Id("id_param_value")));

        var root = Robj("id_root", "model");
        var rparam = root.Type.Operations[0].Parameters[0];

        var paramvar = rparam.CreateVariable(Robj("id_param_value"));

        Assert.AreEqual("param", paramvar.Name);
        Assert.IsTrue(paramvar.IsList);
    }

    [Test]
    public void Rparameter_returns_its_default_value_when_its_optional()
    {
        ModelsAre(
            Model("model")
            .Operation("operation", PModel("param", "param_model", defaultValue: "default")),
            Model("param_model"));

        ObjectsAre(
            Object(Id("id_root", "model")),
            Object(Id("default", "param_model")));

        var root = Robj("id_root", "model");
        var rparam = root.Type.Operations[0].Parameters[0];

        Assert.IsTrue(rparam.IsOptional);
        Assert.AreEqual("default", rparam.Default.Object.Id);
    }

    [Test]
    public void Robject_does_not_fetch_object_data_if_model_is_value()
    {
        Assert.Fail("Add LoadObject & LoadObjectAsync");
        ModelsAre(Model("model").IsValue());

        ObjectsAre(Object(Id("value", "model")));

        var robj = Robj("value", "model");

        var actual = robj.Display;

        Assert.AreEqual("value", actual);
        objectServiceMock.Verify(o => o.Get(It.IsAny<ReferenceData>()), Times.Never());
    }

    [Test]
    public void Robjects_cannot_be_created_from_view_types()
    {
        ModelsAre(
            Model("view").IsView("actual"),
            Model("actual").ViewModelIds("view")
        );

        Assert.Throws<CannotCreateRobjectException>(() => Robj("dummy", "view"));
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

        Assert.AreEqual("id", robjView.Id);
        Assert.AreEqual("view", robjView.Type.Id);
        Assert.AreEqual("from view", robjView["Text"].Get().Object.Id);
    }

    [Test]
    public void Robjects_cannot_be_converted_to_a_view_type_that_does_not_belong_to_the_given_actual_type()
    {
        ModelsAre(
            Model("view").IsView("actual1"),
            Model("actual1").ViewModelIds("view"),
            Model("actual2")
        );

        ObjectsAre(
            Object(Id("id", "actual2"))
        );

        var robjActual = Robj("id", "actual2");

        Assert.Throws<CannotCreateRobjectException>(() => robjActual.As(Rtyp("view")));
    }

    [Test]
    public void When_no_model_is_found_TypeNotFoundException_is_thrown()
    {
        objectServiceMock.Setup(o => o.ApplicationModel).Returns(new ApplicationModel());

        ObjectsAre(Object(Id("value", "model")));

        Assert.Throws<TypeNotFoundException>(() => Robj("value", "model"));
    }

    [Test]
    public void Client_fetches_static_instances_via_Rapplication()
    {
        ModelsAre(Model("model").StaticInstanceIds("id1", "id2"));

        ObjectsAre(
            Object(Id("id1", "model")).Display("value 1"),
            Object(Id("id2", "model")).Display("value 2"));

        var actual = Rtyp("model").StaticInstances;

        Assert.AreEqual(2, actual.Count);
        Assert.AreEqual("id1", actual[0].Id);
        Assert.AreEqual("value 1", actual[0].Display);
        Assert.AreEqual("id2", actual[1].Id);
        Assert.AreEqual("value 2", actual[1].Display);
    }

    [Test]
    public void When_invalidated__Robject_clears_fetched_data()
    {
        Assert.Fail("add GetAsync");

        ModelsAre(
            Model("model")
            .Data("data1")
            .Data("data2"));

        ObjectsAre(
            Object(Id("id1")),
            Object(Id("id2")));
        ObjectsAre(
            Object(Id("id", "model")).Display("value")
            .Data("data1", Id("id1"))
            .Data("data2", Id("id2")));

        var testingRobject = Robj("id", "model");
        var value1 = testingRobject.Display;
        testingRobject["data1"].Get();
        testingRobject["data2"].Get();

        Assert.AreEqual("value", value1);

        testingRobject.Invalidate();

        var value2 = testingRobject.Display;
        testingRobject["data1"].Get();
        testingRobject["data2"].Get();

        Assert.AreEqual("value", value2);
        objectServiceMock.Verify(o => o.Get(It.IsAny<ReferenceData>()), Times.Exactly(2));
    }

    [Test]
    public void Rtype_can_check_if_its_model_is_marked_as_given_mark()
    {
        ModelsAre(
            Model("model").Mark("mark"));

        ObjectsAre(
            Object(Id("id1", "model")));

        var testingRobject = Robj("id", "model");

        Assert.IsTrue(testingRobject.Type.MarkedAs("mark"));
        Assert.IsFalse(testingRobject.Type.MarkedAs("nonexistingmark"));
    }

    [Test]
    public void Rdata_can_check_if_its_model_is_marked_as_given_mark()
    {
        ModelsAre(
            Model("model")
            .Data("data")
            .MarkData("data", "mark"));

        ObjectsAre(
            Object(Id("id")));

        ObjectsAre(
            Object(Id("id1", "model"))
            .Data("data", Id("id")));

        var testingRdata = Robj("id", "model")["data"];

        Assert.IsTrue(testingRdata.Data.MarkedAs("mark"));
        Assert.IsFalse(testingRdata.Data.MarkedAs("nonexistingmark"));
    }

    [Test]
    public void Roperation_can_check_if_its_model_is_marked_as_given_mark()
    {
        ModelsAre(
            Model("model")
            .Operation("operation")
            .MarkOperation("operation", "mark"));

        ObjectsAre(
            Object(Id("id1", "model")));

        var testingRoperation = Robj("id", "model").Type.Operations.Single(o => o.Name == "operation");

        Assert.IsTrue(testingRoperation.MarkedAs("mark"));
        Assert.IsFalse(testingRoperation.MarkedAs("nonexistingmark"));
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
                    Result = new ResultModel { IsVoid = true },
                    Parameters = new List<ParameterModel> { PModel("id", 0, 1) }
                })
            );

        Assert.Throws<InvalidOperationException>(() => Rtyp("model2"));
    }

    [Test]
    public void Roperation_returns_group_parameters_when_asked()
    {
        ModelsAre(
            Model("model")
            .Operation("operation", PModel("param1", "param_model", 0, 1), PModel("param2", "param_model", 1, 2)),
            Model("param_model"));

        ObjectsAre(Object(Id("id_root", "model")));

        var root = Robj("id_root", "model");
        var rop = root.Type.Operations[0];

        var groups = rop.Groups;

        Assert.AreEqual(3, groups.Count);
        Assert.AreEqual("param1", groups[0][0].Name);
        Assert.AreEqual("param1", groups[1][0].Name);
        Assert.AreEqual("param2", groups[1][1].Name);
        Assert.AreEqual("param2", groups[2][0].Name);
    }

    [Test]
    public void Rinitializer_throws_exception_when_a_parameter_has_a_group_that_does_not_exist_on_or_initializer()
    {
        ModelsAre(
            Model("model")
                .Initializer(1, PModel("id", 0, 1))
            );

        Assert.Throws<InvalidOperationException>(() => Rtyp("model"));
    }
}
