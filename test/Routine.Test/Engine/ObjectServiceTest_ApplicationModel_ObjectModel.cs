using Routine.Engine.Virtual;
using Routine.Engine;
using Routine.Test.Engine.Domain.ObjectServiceTest_GetObjectModel;
using Routine.Test.Engine.Ignored;

using static Routine.Constants;

#region Test Model

namespace Routine.Test.Engine.Ignored
{
    public interface IIgnoredModel { }
    public class IgnoredModel { }
}

namespace Routine.Test.Engine.Domain.ObjectServiceTest_GetObjectModel
{
    public class BusinessModel : IBusinessModel, IIgnoredModel
    {
        public int Id { get; set; }
        public List<string> List { get; set; }

        public void VoidOp() { }
        public string StringOp(List<string> list) => null;
        public List<string> ListOp(bool boolParam) => null;

        public void OverloadOp() { }
        public void OverloadOp(string s) { }
        public void OverloadOp(int i) { }
        public void OverloadOp(string s, int i) { }
        public void OverloadOp(string s1, string s, int i1) { }
        public string OverloadOp(bool b) => null;
        public void OverloadOp(bool s, string s1) { }

        public string OverloadOpWithSecondParamUnknown(string value) => null;
        public string OverloadOpWithSecondParamUnknown(string value, Func<string> exceptionFactory) => null;

        public List<string> OverloadOpWithDifferentParamTypeWithSameName(int param1) => null;
        public List<string> OverloadOpWithDifferentParamTypeWithSameName(int param2, string param1) => null;

        public void OverloadOpBug(List<int> e) { }
        public void OverloadOpBug(List<int> e, int f) { }

        public void OptionalParameterOp(string required, string optional = "default") { }

        public int? NullableParameterOp(int? i) => i;

        public async Task AsyncVoidOp() { await Task.Delay(0); }
        public async Task<string> AsyncStringOp() { await Task.Delay(0); return string.Empty; }

        public IgnoredModel IgnoredModelData { get; set; }
        public IgnoredModel IgnoredModelInReturnType() => null;
        public void IgnoredModelInParameters(IgnoredModel ignoredModel) { }
    }

    public interface IBusinessModel : IBusinessModel2
    {
        void VoidOp();
    }

    public interface IBusinessModel2
    {
    }

    public class BusinessValueModel
    {
        public static BusinessValueModel Parse(string value) => null;

        public string AutoExcludedData { get; set; }
        public void AutoExcludedOperation() { }
    }

    // ReSharper disable UnusedParameter.Local
    public readonly struct BusinessDataModel
    {
        public string Data { get; }

        public BusinessDataModel(string data) : this() { Data = data; }
        public BusinessDataModel(string data, int i) : this(data) { }
        public BusinessDataModel(int data) : this() { }
        public BusinessDataModel(IgnoredModel ignoredModel) : this() { }
    }
    // ReSharper restore UnusedParameter.Local

    public enum BusinessEnum
    {
        Item1,
        Item2,
        Item3
    }
}

#endregion

namespace Routine.Test.Engine
{
    [TestFixture]
    public class ObjectServiceTest_ApplicationModel_ObjectModel : ObjectServiceTestBase
    {
        #region Setup & Helpers

        private const string TESTED_OM_ID = "Test.BusinessModel";
        private const string TESTED_VAM_ID = "Test.BusinessValueModel";
        private const string TESTED_VIM_ID = "Test.IBusinessModel";
        private const string TESTED_VIM2_ID = "Test.IBusinessModel2";
        private const string TESTED_DM_ID = "Test.BusinessDataModel";
        private const string TESTED_EM_ID = "Test.BusinessEnum";
        private const string TESTED_VOM_ID = "Test.Virtual";
        private const string TESTED_VVIM_ID = "Test.IVirtual";

        protected override string DefaultModelId => TESTED_OM_ID;
        protected override string RootNamespace => "Routine.Test.Engine.Domain.ObjectServiceTest_GetObjectModel";

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            codingStyle
                .Use(p => p.EnumPattern())
                .Module.Set("Test", t => t.Namespace.StartsWith("Routine.Test"))
                ;
        }

        #endregion

        [Test]
        public void Object_model_module_is_configured()
        {
            codingStyle.Override(c => c.Module.Set("Module", type.of<BusinessModel>()));

            var actual = testing.ApplicationModel.Model["Module.BusinessModel"];

            Assert.AreEqual("Module", actual.Module);
        }

        [Test]
        public void Object_model_name_is_configured()
        {
            codingStyle.Override(c => c.TypeName.Set("BM", type.of<BusinessModel>()));

            var actual = testing.ApplicationModel.Model["Test.BM"];

            Assert.AreEqual("BM", actual.Name);
        }

        [Test]
        public void Object_model_id_is_built_using_configured_modue_and_name()
        {
            codingStyle.Override(c => c
                .Module.Set("Module", type.of<BusinessModel>())
                .TypeName.Set("BM", type.of<BusinessModel>())
                );

            var actual = testing.ApplicationModel.Model["Module.BM"];

            Assert.AreEqual("Module.BM", actual.Id);
        }

        [Test]
        public void When_module_is_null_or_empty__object_model_id_consists_of_name_only()
        {
            codingStyle.Override(c => c
                .Module.Set(null, type.of<BusinessModel>())
                .TypeName.Set("BM", type.of<BusinessModel>())
                .Module.Set(string.Empty, type.of<BusinessValueModel>())
                .TypeName.Set("BVM", type.of<BusinessValueModel>())
                );

            var actual = testing.ApplicationModel.Model["BM"];

            Assert.AreEqual("BM", actual.Id);

            actual = testing.ApplicationModel.Model["BVM"];

            Assert.AreEqual("BVM", actual.Id);
        }

        [Test]
        public void Object_model_is_marked_via_configuration()
        {
            codingStyle
                .TypeMarks.Add(c => c.Constant("value").When(t => t.CanParse()))
                .TypeMarks.Add(c => c.Constant("all"))
                ;

            var om = testing.ApplicationModel.Model[TESTED_OM_ID];
            var vam = testing.ApplicationModel.Model[TESTED_VAM_ID];

            Assert.IsFalse(om.Marks.Contains("value"));
            Assert.IsTrue(vam.Marks.Contains("value"));

            Assert.IsTrue(om.Marks.Contains("all"));
            Assert.IsTrue(vam.Marks.Contains("all"));
        }

        [Test]
        public void Object_model_can_be_configured_to_be_value_model()
        {
            var actual = testing.ApplicationModel.Model["System.String"];

            Assert.IsTrue(actual.IsValueModel);
        }

        [Test]
        public void Object_models_include_configured_static_instance_ids()
        {
            var actual = testing.ApplicationModel.Model[TESTED_EM_ID].StaticInstances;

            Assert.AreEqual(3, actual.Count);
            Assert.AreEqual("Item1", actual[0].Id);
            Assert.AreEqual("Item2", actual[1].Id);
            Assert.AreEqual("Item3", actual[2].Id);
        }

        [Test]
        public void Value_models_do_not_have_data_or_operation()
        {
            var actual = testing.ApplicationModel.Model[TESTED_VAM_ID];

            Assert.AreEqual(0, actual.Datas.Count);
            Assert.AreEqual(0, actual.Operations.Count);
        }

        [Test]
        public void Object_model_can_be_configured_to_be_view_model()
        {
            var actual = testing.ApplicationModel.Model[TESTED_VIM_ID];

            Assert.IsTrue(actual.IsViewModel);
        }

        [Test]
        public void Object_model_can_be_configured_to_have_view_models()
        {
            codingStyle.Converters.Add(c => c
                .Convert(b => b.By(type.of<IBusinessModel>, (o, _) => o))
                .When(type.of<BusinessModel>())
            );

            var actual = testing.ApplicationModel.Model[TESTED_OM_ID];

            Assert.IsTrue(actual.ViewModelIds.Contains(TESTED_VIM_ID));
        }

        [Test]
        public void When_a_configured_view_type_is_not_added__then_it_is_not_included_in_view_models()
        {
            codingStyle.Converters.Add(c => c
                .Convert(b => b.By(type.of<IIgnoredModel>, (o, _) => o))
                .When(type.of<BusinessModel>())
            );

            var actual = testing.ApplicationModel.Model[TESTED_OM_ID];

            Assert.IsFalse(actual.ViewModelIds.Any(vmid => vmid.Contains("IIgnoredModel")));
        }

        [Test]
        public void When_configured_to_have_a_view_model__object_model_is_automatically_included_in_actual_model_list_of_that_view_model()
        {
            codingStyle.Converters.Add(c => c
                .Convert(b => b.By(type.of<IBusinessModel>, (o, _) => o))
                .When(type.of<BusinessModel>())
            );

            var actual = testing.ApplicationModel.Model[TESTED_VIM_ID];

            Assert.IsTrue(actual.ActualModelIds.Contains(TESTED_OM_ID));
        }

        [Test]
        public void A_view_model_cannot_be_in_an_actual_model_ids_list_of_another_view_model()
        {
            codingStyle.Converters.Add(c => c.Convert(b => b.ByCasting()));

            var actual = testing.ApplicationModel.Model[TESTED_VIM2_ID];

            Assert.IsFalse(actual.ActualModelIds.Contains(TESTED_VIM_ID));
        }

        [Test]
        public void A_model_cannot_be_in_its_own_actual_model_nor_in_its_own_view_models()
        {
            codingStyle.Converters.Add(c => c
                .Convert(b => b.ByCasting())
                .When(type.of<BusinessModel>())
            );

            var actual = testing.ApplicationModel.Model[TESTED_OM_ID];

            Assert.IsFalse(actual.ViewModelIds.Contains(TESTED_OM_ID));
            Assert.IsFalse(actual.ActualModelIds.Contains(TESTED_OM_ID));
        }

        [Test]
        public void Initializers_are_configured()
        {
            var actual = testing.ApplicationModel.Model[TESTED_DM_ID];

            Assert.Greater(actual.Initializer.GroupCount, 0);
        }

        [Test]
        public void An_initializer_is_marked_via_configuration()
        {
            codingStyle
                .InitializerMarks.Add(c => c.Constant("parametric").When(i => !i.HasNoParameters()))
                ;

            var actual = testing.ApplicationModel.Model[TESTED_DM_ID];

            Assert.IsTrue(actual.Initializer.Marks.Contains("parametric"));
        }

        [Test]
        public void Initializers_with_non_matching_InitializedType_value_and_any_unrecognized_parameter_ViewModelIds_are_ignored_automatically()
        {
            codingStyle
                .Initializers.Add(c => c.Constant(this.GetTypeInfo().GetConstructor()))
                ;

            var actual = testing.ApplicationModel.Model[TESTED_DM_ID];

            Assert.IsFalse(actual.Initializer.Parameters.Any(p => p.ViewModelId.Contains("Ignored")),
                "Initializers that accept an 'Ignored' type as a parameter should have been excluded: " +
                $"{string.Join(", ", actual.Initializer.Parameters.Where(p => p.ViewModelId.Contains("Ignored")).Select(p => p.Name))}"
            );
            Assert.AreEqual(2, actual.Initializer.GroupCount);
        }

        [Test]
        public void Datas_are_configured()
        {
            var actual = testing.ApplicationModel.Model[TESTED_OM_ID];

            Assert.IsTrue(actual.Datas.Any(m => m.Name == "Id"));
            Assert.AreEqual("System.Int32", actual.Datas.Single(m => m.Name == "Id").ViewModelId);
            Assert.IsTrue(actual.Datas.Any(m => m.Name == "List"));
            Assert.AreEqual("System.String", actual.Datas.Single(m => m.Name == "List").ViewModelId);
        }

        [Test]
        public void A_data_is_marked_via_configuration()
        {
            codingStyle
                .DataMarks.Add(c => c.Constant("heavy").When(m => m.ReturnsCollection()))
                ;

            var om = testing.ApplicationModel.Model[TESTED_OM_ID];

            Assert.IsFalse(om.Datas.Single(m => m.Name == "Id").Marks.Contains("heavy"));
            Assert.IsTrue(om.Datas.Single(m => m.Name == "List").Marks.Contains("heavy"));
        }

        [Test]
        public void Datas_automatically_marked_as_list_when_their_type_conforms_to_a_generic_collection()
        {
            var actual = testing.ApplicationModel.Model[TESTED_OM_ID];

            Assert.IsFalse(actual.Datas.Single(m => m.Name == "Id").IsList);
            Assert.IsTrue(actual.Datas.Single(m => m.Name == "List").IsList);
        }

        [Test]
        public void Datas_with_unrecognized_ViewModelIds_are_ignored_automatically()
        {
            var actual = testing.ApplicationModel.Model[TESTED_OM_ID];

            Assert.IsFalse(actual.Datas.Any(d => d.ViewModelId.Contains("Ignored")),
                "Properties that returns an 'Ignored' type should have been excluded: " +
                $"{string.Join(", ", actual.Datas.Where(d => d.ViewModelId.Contains("Ignored")).Select(d => d.Name))}"
            );
        }

        [Test]
        public void Operations_are_configured()
        {
            var actual = testing.ApplicationModel.Model[TESTED_OM_ID];

            Assert.IsTrue(actual.Operations.Any(o => o.Name == "VoidOp"));
            Assert.AreEqual(MODEL_ID_VOID, actual.Operations.Single(o => o.Name == "VoidOp").Result.ViewModelId);
            Assert.IsTrue(actual.Operations.Any(o => o.Name == "StringOp"));
            Assert.AreEqual("System.String", actual.Operations.Single(o => o.Name == "StringOp").Result.ViewModelId);
            Assert.IsTrue(actual.Operations.Any(o => o.Name == "ListOp"));
            Assert.AreEqual("System.String", actual.Operations.Single(o => o.Name == "ListOp").Result.ViewModelId);
        }

        [Test]
        public void An_operation_is_marked_via_configuration()
        {
            codingStyle
                .OperationMarks.Add(c => c.Constant("heavy").When(o => o.Parameters.Any(p => p.ParameterType.CanBeCollection())))
                ;

            var om = testing.ApplicationModel.Model[TESTED_OM_ID];

            Assert.IsFalse(om.Operations.Single(o => o.Name == "VoidOp").Marks.Contains("heavy"));
            Assert.IsTrue(om.Operations.Single(o => o.Name == "StringOp").Marks.Contains("heavy"));
            Assert.IsFalse(om.Operations.Single(o => o.Name == "ListOp").Marks.Contains("heavy"));
        }

        [Test]
        public void An_operation_can_return_void()
        {
            var actual = testing.ApplicationModel.Model[TESTED_OM_ID];

            Assert.IsTrue(actual.Operation["VoidOp"].Result.IsVoid);
            Assert.IsFalse(actual.Operation["StringOp"].Result.IsVoid);
            Assert.IsFalse(actual.Operation["ListOp"].Result.IsVoid);
        }

        [Test]
        public void An_operation_can_return_list()
        {
            var actual = testing.ApplicationModel.Model[TESTED_OM_ID];

            Assert.IsFalse(actual.Operations.Single(o => o.Name == "VoidOp").Result.IsList);
            Assert.IsFalse(actual.Operations.Single(o => o.Name == "StringOp").Result.IsList);
            Assert.IsTrue(actual.Operations.Single(o => o.Name == "ListOp").Result.IsList);
        }

        [Test]
        public void Async_void_methods_are_void_operations()
        {
            var actual = testing.ApplicationModel.Model[TESTED_OM_ID].Operation["AsyncVoidOp"];

            Assert.IsTrue(actual.Result.IsVoid);
        }

        [Test]
        public void Operations_use_generic_argument_of_task_as_its_result_view_model()
        {
            var actual = testing.ApplicationModel.Model[TESTED_OM_ID].Operation["AsyncStringOp"];

            Assert.AreEqual("System.String", actual.Result.ViewModelId);
        }

        [Test]
        public void Operations_with_unrecognized_result_ViewModelIds_are_ignored_automatically()
        {
            var actual = testing.ApplicationModel.Model[TESTED_OM_ID];

            Assert.IsFalse(actual.Operations.Any(o => o.Result.ViewModelId.Contains("Ignored")),
                "Operations that returns an 'Ignored' type should have been excluded: " +
                $"{string.Join(", ", actual.Operations.Where(o => o.Result.ViewModelId.Contains("Ignored")).Select(o => o.Name))}"
            );
        }

        [Test]
        public void An_initializer_can_have_parameters()
        {
            var actual = testing.ApplicationModel.Model[TESTED_DM_ID];

            Assert.AreEqual(2, actual.Initializer.Parameters.Count);
            Assert.AreEqual("data", actual.Initializer.Parameters[0].Name);
            Assert.AreEqual("System.String", actual.Initializer.Parameters[0].ViewModelId);
            Assert.AreEqual("i", actual.Initializer.Parameters[1].Name);
            Assert.AreEqual("System.Int32", actual.Initializer.Parameters[1].ViewModelId);
        }

        [Test]
        public void An_operation_can_have_parameters()
        {
            var actual = testing.ApplicationModel.Model[TESTED_OM_ID];

            Assert.AreEqual(0, actual.Operations.Single(o => o.Name == "VoidOp").Parameters.Count);
            Assert.AreEqual(1, actual.Operations.Single(o => o.Name == "StringOp").Parameters.Count);
            Assert.AreEqual("list", actual.Operations.Single(o => o.Name == "StringOp").Parameters[0].Name);
            Assert.AreEqual("System.String", actual.Operations.Single(o => o.Name == "StringOp").Parameters[0].ViewModelId);
            Assert.AreEqual(1, actual.Operations.Single(o => o.Name == "ListOp").Parameters.Count);
            Assert.AreEqual("boolParam", actual.Operations.Single(o => o.Name == "ListOp").Parameters[0].Name);
            Assert.AreEqual("System.Boolean", actual.Operations.Single(o => o.Name == "ListOp").Parameters[0].ViewModelId);
        }

        [Test]
        public void Operations_with_any_unrecognized_parameter_ViewModelIds_are_ignored_automatically()
        {
            var actual = testing.ApplicationModel.Model[TESTED_OM_ID];

            Assert.IsFalse(actual.Operations.Any(o => o.Parameters.Any(p => p.ViewModelId.Contains("Ignored"))),
                "Operations that accept an 'Ignored' type as a parameter should have been excluded: " +
                $"{string.Join(", ", actual.Operations.Where(o => o.Parameters.Any(p => p.ViewModelId.Contains("Ignored"))).Select(o => o.Name))}"
            );
        }

        [Test]
        public void A_parameter_can_be_of_list_type()
        {
            var actual = testing.ApplicationModel.Model[TESTED_OM_ID];

            Assert.IsTrue(actual.Operations.Single(o => o.Name == "StringOp").Parameters[0].IsList);
            Assert.IsFalse(actual.Operations.Single(o => o.Name == "ListOp").Parameters[0].IsList);
        }

        [Test]
        public void A_parameter_can_be_optional_and_include_its_default_value()
        {
            var actual = testing.ApplicationModel.Model[TESTED_OM_ID].Operations.Single(o => o.Name == "OptionalParameterOp");

            Assert.IsFalse(actual.Parameters[0].IsOptional);
            Assert.IsTrue(actual.Parameters[1].IsOptional);
            Assert.AreEqual("default", actual.Parameters[1].DefaultValue.Values[0].Id);
        }

        [Test]
        public void A_parameter_and_return_type_can_be_nullable_value()
        {
            var actual = testing.ApplicationModel.Model[TESTED_OM_ID].Operations.Single(o => o.Name == "NullableParameterOp");

            Assert.AreEqual("System.Int32?", actual.Parameters[0].ViewModelId);
            Assert.AreEqual("System.Int32?", actual.Result.ViewModelId);
        }

        [Test]
        public void A_parameter_is_marked_via_configuration()
        {
            codingStyle
                .ParameterMarks.Add(c => c.Constant("list").When(p => p.ParameterType.CanBeCollection()))
                ;

            var om = testing.ApplicationModel.Model[TESTED_OM_ID];

            Assert.IsTrue(om.Operations.Single(o => o.Name == "StringOp").Parameters[0].Marks.Contains("list"));
            Assert.IsFalse(om.Operations.Single(o => o.Name == "ListOp").Parameters[0].Marks.Contains("list"));
        }

        [Test]
        public void Initializer_can_have_multiple_parameter_groups()
        {
            var actual = testing.ApplicationModel.Model[TESTED_DM_ID];

            Assert.AreEqual(new List<int> { 0, 1 }, actual.Initializer.Parameters.Single(p => p.Name == "data").Groups);
            Assert.AreEqual(new List<int> { 1 }, actual.Initializer.Parameters.Single(p => p.Name == "i").Groups);
        }

        [Test]
        public void Initializer_s_marks_are_those_merged_from_all_its_groups()
        {
            codingStyle
                .InitializerMarks.Add(c => c.By(i => new List<string> { "ovr-" + i.Parameters.Count }))
                ;

            var actual = testing.ApplicationModel.Model[TESTED_DM_ID];

            Assert.IsTrue(actual.Initializer.Marks.Contains("ovr-1"));
            Assert.IsTrue(actual.Initializer.Marks.Contains("ovr-2"));
            Assert.AreEqual(2, actual.Initializer.Marks.Count);
        }

        [Test]
        public void Operations_with_same_name_are_considered_as_parameter_groups_and_represented_as_a_single_operation_with_parameter_groups()
        {
            var actual = testing.ApplicationModel.Model[TESTED_OM_ID];

            Assert.IsTrue(actual.Operations.Any(m => m.Name.Contains("OverloadOp")));
            Assert.IsFalse(actual.Operations.Single(o => o.Name == "OverloadOp").Parameters.Any(p => p.Groups.Contains(0)));
            Assert.AreEqual(new List<int> { 1, 3, 4 }, actual.Operations.Single(o => o.Name == "OverloadOp").Parameters.Single(p => p.Name == "s").Groups);
            Assert.AreEqual(new List<int> { 2, 3 }, actual.Operations.Single(o => o.Name == "OverloadOp").Parameters.Single(p => p.Name == "i").Groups);
            Assert.AreEqual(new List<int> { 4 }, actual.Operations.Single(o => o.Name == "OverloadOp").Parameters.Single(p => p.Name == "s1").Groups);
            Assert.AreEqual(new List<int> { 4 }, actual.Operations.Single(o => o.Name == "OverloadOp").Parameters.Single(p => p.Name == "i1").Groups);
        }

        [Test]
        public void Bug__when_second_overload_has_an_unconfigured_type_as_second_parameter__group_count_of_first_parameter_does_not_rollback_and_operation_group_count_stays_as_1_but_group_count_of_first_parameter_becomes_2()
        {
            var actual = testing.ApplicationModel.Model[TESTED_OM_ID];
            var operation = actual.Operations.Single(o => o.Name == "OverloadOpWithSecondParamUnknown");

            Assert.AreEqual(operation.GroupCount, operation.Parameters.Single(p => p.Name == "value").Groups.Count);
        }

        [Test]
        public void Bug__when_second_overload_is_skipped_due_to_different_parameter_type_with_the_same_parameter_name__other_parameter_of_second_overload_exists_in_operation_where_it_should_not()
        {
            var actual = testing.ApplicationModel.Model[TESTED_OM_ID];
            var operation = actual.Operations.Single(o => o.Name == "OverloadOpWithDifferentParamTypeWithSameName");

            Assert.AreEqual(1, operation.GroupCount);
            Assert.IsFalse(operation.Parameters.Any(p => p.Name == "param2"), "param2 should not exist, because its overload has been skipped");
        }

        [Test]
        public void Operations_with_the_same_name_but_different_results_are_ignored()
        {
            var om = testing.ApplicationModel.Model[TESTED_OM_ID];
            var actual = om.Operations.Single(o => o.Name == "OverloadOp");

            Assert.AreEqual(5, actual.GroupCount);
            Assert.IsTrue(actual.Result.IsVoid);
        }

        [Test]
        public void An_operation_s_marks_are_those_merged_from_all_its_groups()
        {
            codingStyle
                .OperationMarks.Add(s => s
                    .By(o => new List<string> { "ovr-" + o.Parameters.Count })
                    .When(o => o.Name == "OverloadOp")
                );

            var actual = testing.ApplicationModel.Model[TESTED_OM_ID];

            Assert.IsTrue(actual.Operations.Single(o => o.Name == "OverloadOp").Marks.Contains("ovr-0"));
            Assert.IsTrue(actual.Operations.Single(o => o.Name == "OverloadOp").Marks.Contains("ovr-1"));
            Assert.IsTrue(actual.Operations.Single(o => o.Name == "OverloadOp").Marks.Contains("ovr-2"));
            Assert.IsTrue(actual.Operations.Single(o => o.Name == "OverloadOp").Marks.Contains("ovr-3"));
            Assert.AreEqual(4, actual.Operations.Single(o => o.Name == "OverloadOp").Marks.Count);
        }

        [Test]
        public void A_parameter_s_marks_are_those_merged_from_all_its_groups()
        {
            codingStyle
                .ParameterMarks.Add(s => s
                    .By(p => new List<string> { "ovr-" + p.Owner.Parameters.Count })
                    .When(p => p.Name == "s" && p.Owner.Name == "OverloadOp")
                );

            var om = testing.ApplicationModel.Model[TESTED_OM_ID];
            var actual = om.Operations.Single(o => o.Name == "OverloadOp").Parameters.Single(p => p.Name == "s");

            Assert.IsTrue(actual.Marks.Contains("ovr-1"));
            Assert.IsTrue(actual.Marks.Contains("ovr-2"));
            Assert.IsTrue(actual.Marks.Contains("ovr-3"));
            Assert.AreEqual(3, actual.Marks.Count);
        }

        [Test]
        public void Parameters_with_the_same_name_but_different_types_are_ignored()
        {
            var om = testing.ApplicationModel.Model[TESTED_OM_ID];
            var actual = om.Operations.Single(o => o.Name == "OverloadOp");

            Assert.AreEqual(5, actual.GroupCount);
            Assert.IsFalse(actual.Parameters.Any(p => p.ViewModelId == "s-boolean" && p.Name == "s"));

            om = testing.ApplicationModel.Model[TESTED_DM_ID];

            Assert.AreEqual(2, om.Initializer.GroupCount);
            Assert.IsFalse(om.Initializer.Parameters.Any(p => p.ViewModelId == "s-int-32" && p.Name == "data"));
        }

        [Test]
        public void Bug__List_parameter_types_makes_invalid_check_for_overloads()
        {
            var om = testing.ApplicationModel.Model[TESTED_OM_ID];
            var actual = om.Operations.Single(o => o.Name == "OverloadOpBug");

            Assert.AreEqual(2, actual.GroupCount);
        }

        [Test]
        public void When_two_identical_operations_added_as_a_group__second_one_is_automatically_skipped()
        {
            codingStyle
                .Operations.Add(c => c.Build(o => o.Virtual("VoidOp", () => { })).When(type.of<BusinessModel>()))
                // ReSharper disable once UnusedParameter.Local
                .Operations.Add(c => c.Build(o => o.Virtual("StringOp", (List<string> list) => "dummy")).When(type.of<BusinessModel>()))
                ;

            var om = testing.ApplicationModel.Model[TESTED_OM_ID];

            Assert.AreEqual(1, om.Operations.Single(o => o.Name == "VoidOp").GroupCount);
            Assert.AreEqual(1, om.Operations.Single(o => o.Name == "StringOp").GroupCount);
        }

        [Test]
        public void When_two_identical_initializer_added_as_a_group__second_one_is_automatically_skipped()
        {
            var parameterMock = new Mock<IParameter>();
            parameterMock.Setup(obj => obj.Index).Returns(0);
            parameterMock.Setup(obj => obj.ParameterType).Returns(type.of<string>());
            parameterMock.Setup(obj => obj.Name).Returns("data");
            parameterMock.Setup(obj => obj.ParentType).Returns(type.of<ObjectServiceTest_ApplicationModel_ObjectModel>());
            parameterMock.Setup(obj => obj.GetCustomAttributes()).Returns(Array.Empty<object>());

            var initializerMock = new Mock<IConstructor>();
            initializerMock.Setup(obj => obj.IsPublic).Returns(true);
            initializerMock.Setup(obj => obj.Name).Returns("mock_ctor");
            initializerMock.Setup(obj => obj.GetCustomAttributes()).Returns(Array.Empty<object>());
            initializerMock.Setup(obj => obj.ParentType).Returns(type.of<ObjectServiceTest_ApplicationModel_ObjectModel>());
            initializerMock.Setup(obj => obj.InitializedType).Returns(type.of<BusinessDataModel>());
            initializerMock.Setup(obj => obj.Parameters).Returns(new List<IParameter> { parameterMock.Object });

            parameterMock.Setup(obj => obj.Owner).Returns(initializerMock.Object);

            codingStyle.Initializers.Add(c => c.Constant(initializerMock.Object).When(type.of<BusinessDataModel>()));

            var om = testing.ApplicationModel.Model[TESTED_DM_ID];

            Assert.AreEqual(2, om.Initializer.GroupCount);
        }

        [Test]
        public void Virtual_types_are_rendered_as_regular_object_models()
        {
            codingStyle.AddTypes(v => v.FromBasic()
                .Name.Set("Virtual")
                .Namespace.Set(RootNamespace));

            var om = testing.ApplicationModel.Model[TESTED_VOM_ID];

            Assert.AreEqual("Virtual", om.Name);
        }

        [Test]
        public void Virtual_types_are_marked_as_virtual()
        {
            codingStyle
                .Use(p => p.VirtualTypePattern("virtual"))
                .AddTypes(v => v.FromBasic()
                    .Name.Set("Virtual")
                    .Namespace.Set(RootNamespace));

            var om = testing.ApplicationModel.Model[TESTED_VOM_ID];

            Assert.IsTrue(om.Marks.Contains("virtual"));
        }

        [Test]
        public void Virtual_types_can_have_static_instances()
        {
            codingStyle
                .Use(p => p.VirtualTypePattern())
                .AddTypes(v => v.FromBasic()
                    .Name.Set("Virtual")
                    .Namespace.Set(RootNamespace))
                .StaticInstances.Add(c => c.By(t => new VirtualObject("Instance", t as VirtualType)).When(t => t is VirtualType))
                ;

            var om = testing.ApplicationModel.Model[TESTED_VOM_ID];

            Assert.AreEqual(1, om.StaticInstances.Count);
            Assert.AreEqual("Instance", om.StaticInstances[0].Id);
        }

        [Test]
        public void Virtual_types_can_have_virtual_view_types()
        {
            codingStyle
                .Use(p => p.VirtualTypePattern())
                .AddTypes(v => v
                    .FromBasic()
                    .Name.Set("Virtual")
                    .Namespace.Set(RootNamespace)
                    .AssignableTypes.Add(iv => iv
                        .FromBasic()
                        .IsInterface.Set(true)
                        .Name.Set("IVirtual")
                        .Namespace.Set(RootNamespace)
                    )
                )
                ;

            var vom = testing.ApplicationModel.Model[TESTED_VOM_ID];

            Assert.AreEqual(1, vom.ViewModelIds.Count);
            Assert.AreEqual(TESTED_VVIM_ID, vom.ViewModelIds[0]);

            var vvim = testing.ApplicationModel.Model[TESTED_VVIM_ID];

            Assert.AreEqual(1, vvim.ActualModelIds.Count);
            Assert.AreEqual(TESTED_VOM_ID, vvim.ActualModelIds[0]);
        }

        [Test]
        public void Proxy_operations_can_be_added_to_a_virtual_type()
        {
            codingStyle
                .Use(p => p.VirtualTypePattern())
                .AddTypes(v => v.FromBasic()
                    .Name.Set("Virtual")
                    .Namespace.Set(RootNamespace)
                    .Methods.Add(o => o.Proxy<IBusinessModel>("VoidOp").TargetByParameter<BusinessModel>()))
                ;

            var om = testing.ApplicationModel.Model[TESTED_VOM_ID];

            Assert.IsTrue(om.Operations.Any(o => o.Name == "VoidOp"));
            Assert.IsTrue(om.Operations.Single(o => o.Name == "VoidOp").Parameters.Any(p => p.Name == "businessModel" && p.ViewModelId == TESTED_OM_ID));
        }

        [Test]
        public void Proxy_operations_can_be_added_to_an_existing_type()
        {
            codingStyle
                .Operations.Add(c => c.Build(o => o.Proxy<string>("Insert").TargetByParameter("str")).When(type.of<BusinessModel>()))
                ;

            var om = testing.ApplicationModel.Model[TESTED_OM_ID];

            Assert.IsTrue(om.Operations.Any(o => o.Name == "Insert"));
            Assert.IsTrue(om.Operations.Single(o => o.Name == "Insert").Parameters.Any(p => p.Name == "str" && p.ViewModelId == "System.String"));
        }

        [Test]
        public void Proxy_operation_names_can_be_altered()
        {
            codingStyle
                .Operations.Add(c => c.Build(ob => ob.Proxy<string>("Insert")
                    .Name.Set(nc => nc.By(o => "Overridden" + o.Name))
                    .TargetByParameter("str"))
                    .When(type.of<BusinessModel>())
                )
                ;

            var om = testing.ApplicationModel.Model[TESTED_OM_ID];

            Assert.IsTrue(om.Operations.Any(o => o.Name == "OverriddenInsert"));
            Assert.IsTrue(om.Operations.Single(o => o.Name == "OverriddenInsert").Parameters.Any(p => p.Name == "str" && p.ViewModelId == "System.String"));
        }

        [Test]
        public void Virtual_operations_can_be_added_to_an_existing_type()
        {
            codingStyle
                .Operations.Add(c => c.Build(o => o.Virtual("Ping", (string input) => input)).When(type.of<BusinessModel>()))
                ;

            var om = testing.ApplicationModel.Model[TESTED_OM_ID];

            Assert.IsTrue(om.Operations.Any(o => o.Name == "Ping"));
            Assert.IsTrue(om.Operations.Single(o => o.Name == "Ping").Parameters.Any(p => p.Name == "input" && p.ViewModelId == "System.String"));
            Assert.AreEqual("System.String", om.Operations.Single(o => o.Name == "Ping").Result.ViewModelId);
        }

        [Test]
        public void Virtual_operations_can_be_added_to_a_virtual_type()
        {
            codingStyle
                .Use(p => p.VirtualTypePattern())
                .AddTypes(v => v.FromBasic()
                    .Name.Set("Virtual")
                    .Namespace.Set(RootNamespace)
                    .Methods.Add(o => o.Virtual("Ping", (string input) => input))
                )
                ;

            var om = testing.ApplicationModel.Model[TESTED_VOM_ID];

            Assert.IsTrue(om.Operations.Any(o => o.Name == "Ping"));
            Assert.IsTrue(om.Operations.Single(o => o.Name == "Ping").Parameters.Any(p => p.Name == "input" && p.ViewModelId == "System.String"));
            Assert.AreEqual("System.String", om.Operations.Single(o => o.Name == "Ping").Result.ViewModelId);
        }
    }
}
