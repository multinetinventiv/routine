using Routine.Engine;

namespace Routine.Test.Engine.Converter;

[TestFixture]
public class NullableConverterTest
{
    [Test]
    public void Converts_value_types_to_their_nullable_types()
    {
        IConverter converter = BuildRoutine.Converter().ToNullable();

        Assert.That(converter.GetTargetTypes(type.of<int>())[0], Is.EqualTo(type.of<int?>()));

        var actual = converter.Convert(3, type.of<int>(), type.of<int?>());

        int? expected = 3;

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Does_not_cover_void__non_value_types_and_generic_types()
    {
        IConverter converter = BuildRoutine.Converter().ToNullable();

        Assert.That(converter.GetTargetTypes(type.ofvoid()), Is.Empty);
        Assert.That(converter.GetTargetTypes(type.of<string>()), Is.Empty);
        Assert.That(converter.GetTargetTypes(type.of<int?>()), Is.Empty);
    }
}
