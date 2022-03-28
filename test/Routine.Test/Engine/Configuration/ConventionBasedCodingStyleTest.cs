#nullable enable

using NUnit.Framework;
using Routine.Engine;
using System;

namespace Routine.Test.Engine.Configuration
{
    [TestFixture]
    public class ConventionBasedCodingStyleTest
    {
        [Test]
        public void When_configuring_nullable_types__type_and_module_names_come_from_the_type_that_is_nullable()
        {
            var testing = BuildRoutine.CodingStyle().FromBasic() as ICodingStyle;

            Assert.AreEqual("Int32?", testing.GetName(type.of<int?>()));
            Assert.AreEqual("System", testing.GetModule(type.of<int?>()));

            Assert.AreEqual("DateTime?", testing.GetName(type.of<DateTime?>()));
            Assert.AreEqual("System", testing.GetModule(type.of<DateTime?>()));

            Assert.AreEqual("Text?", testing.GetName(type.of<Text?>()));
            Assert.AreEqual("Routine.Test", testing.GetModule(type.of<Text?>()));
        }

        [Test]
        public void When_adding_a_value_type__nullable_version_is_added_automatically()
        {
            var testing = BuildRoutine.CodingStyle().FromBasic().AddTypes(typeof(int)) as ICodingStyle;

            Assert.IsTrue(testing.ContainsType(type.of<int?>()));
        }

        public class AClassWithNullableReferenceType
        {
            public string? NullableString { get; set; }
            public string NotNullableString { get; set; } = "initial value";
        }

        [Test]
        public void A_nullable_reference_type_is_treated_just_like_a_not_nullable_one()
        {
            var testing = BuildRoutine.CodingStyle().FromBasic();

            testing.AddTypes(typeof(AClassWithNullableReferenceType));

            testing.Datas.Add(c => c.PublicProperties());

            var datas = ((ICodingStyle)testing).GetDatas(type.of<AClassWithNullableReferenceType>());

            Assert.AreEqual(2, datas.Count);
            Assert.AreEqual(nameof(AClassWithNullableReferenceType.NullableString), datas[0].Name);
            Assert.AreEqual("System.String", datas[0].ReturnType.FullName);
            Assert.AreEqual(nameof(AClassWithNullableReferenceType.NotNullableString), datas[1].Name);
            Assert.AreEqual("System.String", datas[1].ReturnType.FullName);
        }

        public ref struct ARefStruct { }

        [Test]
        public void When_a_ref_struct_is_added__it_is_ignored_automatically()
        {
            var testing = BuildRoutine.CodingStyle().FromBasic().AddTypes(typeof(ARefStruct)) as ICodingStyle;

            Assert.IsFalse(testing.ContainsType(TypeInfo.Get(typeof(ARefStruct))));
        }

        public record ARecord(string Data);

        [Test]
        public void When_a_record_is_added__it_can_be_configured_like_any_other_class()
        {
            var testing = BuildRoutine.CodingStyle().FromBasic();

            testing.AddTypes(typeof(ARecord));

            testing.Initializers.Add(c => c.PublicConstructors().When(type.of<ARecord>()));
            testing.Datas.Add(c => c.PublicProperties().When(type.of<ARecord>()));

            var initializers = ((ICodingStyle)testing).GetInitializers(type.of<ARecord>());
            var datas = ((ICodingStyle)testing).GetDatas(type.of<ARecord>());

            Assert.AreEqual(1, initializers.Count);
            Assert.AreEqual(1, initializers[0].Parameters.Count);
            Assert.AreEqual(nameof(ARecord.Data), initializers[0].Parameters[0].Name);
            Assert.AreEqual(type.of<string>(), initializers[0].Parameters[0].ParameterType);

            Assert.AreEqual(1, datas.Count);
            Assert.AreEqual(nameof(ARecord.Data), datas[0].Name);
            Assert.AreEqual(type.of<string>(), datas[0].ReturnType);
        }

        public readonly struct AReadonlyStruct
        {
            public string Data { get; }

            public AReadonlyStruct(string data)
            {
                Data = data;
            }
        }

        [Test]
        public void When_a_struct_is_readonly__it_can_be_configured_like_any_other_struct()
        {
            var testing = BuildRoutine.CodingStyle().FromBasic();

            testing.AddTypes(typeof(AReadonlyStruct));

            testing.Initializers.Add(c => c.PublicConstructors().When(type.of<AReadonlyStruct>()));
            testing.Datas.Add(c => c.PublicProperties().When(type.of<AReadonlyStruct>()));

            var initializers = ((ICodingStyle)testing).GetInitializers(type.of<AReadonlyStruct>());
            var datas = ((ICodingStyle)testing).GetDatas(type.of<AReadonlyStruct>());

            Assert.AreEqual(1, initializers.Count);
            Assert.AreEqual(1, initializers[0].Parameters.Count);
            Assert.AreEqual("data", initializers[0].Parameters[0].Name);
            Assert.AreEqual(type.of<string>(), initializers[0].Parameters[0].ParameterType);

            Assert.AreEqual(1, datas.Count);
            Assert.AreEqual(nameof(AReadonlyStruct.Data), datas[0].Name);
            Assert.AreEqual(type.of<string>(), datas[0].ReturnType);
        }

        public interface IAnInterface { public string DefaultMethodOp() => "data"; }
        public class AClass : IAnInterface { }

        [Test]
        public void When_an_interface_has_a_default_method__it_can_be_an_operation_of_its_view_model()
        {
            var testing = BuildRoutine.CodingStyle().FromBasic();

            testing.AddTypes(typeof(IAnInterface), typeof(AClass));

            testing.Operations.Add(c => c.PublicMethods().When(type.of<IAnInterface>()));

            var operations = ((ICodingStyle)testing).GetOperations(type.of<IAnInterface>());

            Assert.AreEqual(1, operations.Count);
            Assert.AreEqual(nameof(IAnInterface.DefaultMethodOp), operations[0].Name);
        }
    }
}
