#nullable enable

using Routine.Engine;

namespace Routine.Test.Engine.Configuration;

[TestFixture]
public class ConventionBasedCodingStyleTest
{
    [Test]
    public void When_configuring_nullable_types__type_and_module_names_come_from_the_type_that_is_nullable()
    {
        var testing = BuildRoutine.CodingStyle().FromBasic() as ICodingStyle;

        Assert.That(testing.GetName(type.of<int?>()), Is.EqualTo("Int32?"));
        Assert.That(testing.GetModule(type.of<int?>()), Is.EqualTo("System"));

        Assert.That(testing.GetName(type.of<DateTime?>()), Is.EqualTo("DateTime?"));
        Assert.That(testing.GetModule(type.of<DateTime?>()), Is.EqualTo("System"));

        Assert.That(testing.GetName(type.of<Text?>()), Is.EqualTo("Text?"));
        Assert.That(testing.GetModule(type.of<Text?>()), Is.EqualTo("Routine.Test"));
    }

    [Test]
    public void When_adding_a_value_type__nullable_version_is_added_automatically()
    {
        var testing = BuildRoutine.CodingStyle().FromBasic().AddTypes(typeof(int)) as ICodingStyle;

        Assert.That(testing.ContainsType(type.of<int?>()), Is.True);
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

        Assert.That(datas.Count, Is.EqualTo(2));
        Assert.That(datas[0].Name, Is.EqualTo(nameof(AClassWithNullableReferenceType.NullableString)));
        Assert.That(datas[0].ReturnType.FullName, Is.EqualTo("System.String"));
        Assert.That(datas[1].Name, Is.EqualTo(nameof(AClassWithNullableReferenceType.NotNullableString)));
        Assert.That(datas[1].ReturnType.FullName, Is.EqualTo("System.String"));
    }

    public ref struct ARefStruct { }

    [Test]
    public void When_a_ref_struct_is_added__it_is_ignored_automatically()
    {
        var testing = BuildRoutine.CodingStyle().FromBasic().AddTypes(typeof(ARefStruct)) as ICodingStyle;

        Assert.That(testing.ContainsType(TypeInfo.Get(typeof(ARefStruct))), Is.False);
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

        Assert.That(initializers.Count, Is.EqualTo(1));
        Assert.That(initializers[0].Parameters.Count, Is.EqualTo(1));
        Assert.That(initializers[0].Parameters[0].Name, Is.EqualTo(nameof(ARecord.Data)));
        Assert.That(initializers[0].Parameters[0].ParameterType, Is.EqualTo(type.of<string>()));

        Assert.That(datas.Count, Is.EqualTo(1));
        Assert.That(datas[0].Name, Is.EqualTo(nameof(ARecord.Data)));
        Assert.That(datas[0].ReturnType, Is.EqualTo(type.of<string>()));
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

        Assert.That(initializers.Count, Is.EqualTo(1));
        Assert.That(initializers[0].Parameters.Count, Is.EqualTo(1));
        Assert.That(initializers[0].Parameters[0].Name, Is.EqualTo("data"));
        Assert.That(initializers[0].Parameters[0].ParameterType, Is.EqualTo(type.of<string>()));

        Assert.That(datas.Count, Is.EqualTo(1));
        Assert.That(datas[0].Name, Is.EqualTo(nameof(AReadonlyStruct.Data)));
        Assert.That(datas[0].ReturnType, Is.EqualTo(type.of<string>()));
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

        Assert.That(operations.Count, Is.EqualTo(1));
        Assert.That(operations[0].Name, Is.EqualTo(nameof(IAnInterface.DefaultMethodOp)));
    }
}
