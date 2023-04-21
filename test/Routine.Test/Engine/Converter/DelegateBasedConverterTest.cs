using Routine.Engine.Converter;
using Routine.Engine;

namespace Routine.Test.Engine.Converter;

[TestFixture]
public class DelegateBasedConverterTest
{
    [Test]
    public void Inherites_ConverterBase()
    {
        Assert.That(typeof(ConverterBase<DelegateBasedConverter>).IsAssignableFrom(typeof(DelegateBasedConverter)), Is.True);
    }

    [Test]
    public void Converts_object_to_target_type_using_given_delegate()
    {
        IConverter converter = BuildRoutine.Converter().By(() => new List<IType> { type.of<string>() }, (o, t) =>
        {
            Assert.That(o, Is.EqualTo(0));
            Assert.That(t, Is.EqualTo(type.of<string>()));

            return "success";
        });

        Assert.That(converter.Convert(0, type.of<int>(), type.of<string>()), Is.EqualTo("success"));
    }

    [Test]
    public void Throws_ArgumentNullException_when_any_of_the_given_delegates_is_null()
    {
        Assert.That(() => BuildRoutine.Converter().By(() => new List<IType>(), null), Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => BuildRoutine.Converter().By((Func<IType>)null, (_, _) => new object()), Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    public void Facade_By_IType__List_IType_and_Func_IType()
    {
        Assert.That(((IConverter)BuildRoutine.Converter().By(() => type.of<string>(), (_, _) => "success")).Convert(0, type.of<int>(), type.of<string>()), Is.EqualTo("success"));
        Assert.That(((IConverter)BuildRoutine.Converter().By(type.of<string>(), (_, _) => "success")).Convert(0, type.of<int>(), type.of<string>()), Is.EqualTo("success"));
        Assert.That(((IConverter)BuildRoutine.Converter().By(new List<IType> { type.of<string>() }, (_, _) => "success")).Convert(0, type.of<int>(), type.of<string>()), Is.EqualTo("success"));
    }

    [Test]
    public void Facade_ToConstant()
    {
        Assert.That(((IConverter)BuildRoutine.Converter().ToConstant("success")).Convert(0, type.of<int>(), type.of<string>()), Is.EqualTo("success"));
    }

    [Test]
    public void Facade_ToConstant_when_null_is_given_type_is_defined_as_null()
    {
        IConverter converter = BuildRoutine.Converter().ToConstant(null);

        Assert.That(converter.GetTargetTypes(type.of<string>())[0], Is.Null);
    }
}
