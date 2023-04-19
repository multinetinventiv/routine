using Routine.Engine;

namespace Routine.Test.Engine.Converter;

[TestFixture]
public class TypeCastConverterTest
{
    [Test]
    public void Converts_object_to_target_type_using_cast_method_of_IType()
    {
        var typeMock = new Mock<IType>();

        typeMock.Setup(t => t.Cast(It.IsAny<object>(), type.of<string>())).Returns("success");
        typeMock.Setup(t => t.AssignableTypes).Returns(new List<IType> { type.of<string>() });

        IConverter converter = BuildRoutine.Converter().ByCasting();

        Assert.That(converter.Convert(0, typeMock.Object, type.of<string>()), Is.EqualTo("success"));
    }

    [Test]
    public void Throws_CannotConvertException_when_given_type_cannot_be_converted_to_target_type()
    {
        var typeMock = new Mock<IType>();

        typeMock.Setup(t => t.AssignableTypes).Returns(new List<IType>());
        typeMock.Setup(t => t.Cast(It.IsAny<object>(), type.of<string>())).Throws<InvalidCastException>();

        IConverter converter = BuildRoutine.Converter().ByCasting();

        Assert.That(() => converter.Convert(0, typeMock.Object, type.of<string>()), Throws.TypeOf<CannotConvertException>());
    }

    [Test]
    public void View_types_can_be_filtered_so_that_only_appropriate_types_will_be_used()
    {
        IConverter converter = BuildRoutine.Converter().ByCasting(t => !t.CanBe<object>());

        Assert.That(() => converter.Convert("test", type.of<string>(), type.of<object>()), Throws.TypeOf<CannotConvertException>());
    }

    [Test]
    public void Throws_ArgumentNullException_when_given_view_type_predicate_is_null()
    {
        Assert.That(() => BuildRoutine.Converter().ByCasting(null), Throws.TypeOf<ArgumentNullException>());
    }
}
