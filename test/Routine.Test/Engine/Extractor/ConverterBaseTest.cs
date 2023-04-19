using Routine.Engine.Converter;
using Routine.Engine;
using Routine.Test.Core;

namespace Routine.Test.Engine.Extractor;

[TestFixture]
public class ConverterBaseTest : CoreTestBase
{
    #region SetUp & Helpers

    private class TestConverter : ConverterBase<TestConverter>
    {
        private readonly Func<object> _conversionDelegate;
        private readonly IType _theOnlyAcceptedTargetType;

        public TestConverter(Func<object> conversionDelegate, IType theOnlyAcceptedTargetType)
        {
            _conversionDelegate = conversionDelegate;
            _theOnlyAcceptedTargetType = theOnlyAcceptedTargetType;
        }

        protected override List<IType> GetTargetTypes(IType type) => new() { _theOnlyAcceptedTargetType };
        protected override object Convert(object @object, IType from, IType to) => _conversionDelegate();
    }

    #endregion

    [Test]
    public void When_exception_occurs_during_conversion_searches_target_type_in_target_types_list_and_wraps_the_exception_as_CannotConvertException()
    {
        var testing = new TestConverter(() => throw new Exception("inner"), type.of<string>()) as IConverter;

        try
        {
            testing.Convert(new object(), type.of<object>(), type.of<int>());
            Assert.Fail("exception not thrown");
        }
        catch (CannotConvertException ex)
        {
            Assert.That(ex.InnerException.Message, Is.EqualTo("inner"));
        }
    }

    [Test]
    public void When_a_successful_conversion_occurs_even_if_target_is_not_in_list__leaves_conversion_as_successful()
    {
        var testing = new TestConverter(() => "success", type.of<int>()) as IConverter;

        Assert.That(testing.Convert(new object(), type.of<object>(), type.of<string>()), Is.EqualTo("success"));
    }

    [Test]
    public void When_cannot_convert_exception_occurs__rethrows_the_exception()
    {
        var expected = new CannotConvertException(new object(), type.of<string>());
        var testing = new TestConverter(() => throw expected, type.of<string>()) as IConverter;

        try
        {
            testing.Convert(new object(), type.of<object>(), type.of<string>());
            Assert.Fail("exception not thrown");
        }
        catch (Exception actual)
        {
            Assert.That(actual, Is.SameAs(expected));
        }
    }

    [Test]
    public void When_an_exception_occurs_even_if_target_is_in_list__rethrows_the_exception()
    {
        var expected = new Exception();

        var testing = new TestConverter(() => throw expected, type.of<string>()) as IConverter;

        try
        {
            testing.Convert(new object(), type.of<object>(), type.of<string>());
            Assert.Fail("exception not thrown");
        }
        catch (Exception actual)
        {
            Assert.That(actual, Is.SameAs(expected));
        }
    }

    [Test]
    public void When_converting_to_a_target_type_within_the_target_types_list__allows_conversion()
    {
        var testing = new TestConverter(() => "converted", type.of<string>()) as IConverter;

        Assert.That(testing.Convert("original", type.of<string>(), type.of<string>()), Is.EqualTo("converted"));
    }
}
