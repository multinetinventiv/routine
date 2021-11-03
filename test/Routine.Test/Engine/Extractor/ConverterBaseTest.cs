using System;
using System.Collections.Generic;
using NUnit.Framework;
using Routine.Engine;
using Routine.Engine.Converter;
using Routine.Test.Core;

namespace Routine.Test.Engine.Extractor
{
    [TestFixture]
    public class ConverterBaseTest : CoreTestBase
    {
        #region SetUp & Helpers

        private class TestConverter : ConverterBase<TestConverter>
        {
            private readonly Func<object> conversionDelegate;
            private readonly IType theOnlyAcceptedTargetType;

            public TestConverter(Func<object> conversionDelegate, IType theOnlyAcceptedTargetType)
            {
                this.conversionDelegate = conversionDelegate;
                this.theOnlyAcceptedTargetType = theOnlyAcceptedTargetType;
            }

            protected override List<IType> GetTargetTypes(IType type) => new() { theOnlyAcceptedTargetType };
            protected override object Convert(object @object, IType from, IType to) => conversionDelegate();
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
                Assert.AreEqual("inner", ex.InnerException.Message);
            }
        }

        [Test]
        public void When_a_successful_conversion_occurs_even_if_target_is_not_in_list__leaves_conversion_as_successful()
        {
            var testing = new TestConverter(() => "success", type.of<int>()) as IConverter;

            Assert.AreEqual("success", testing.Convert(new object(), type.of<object>(), type.of<string>()));
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
                Assert.AreSame(expected, actual);
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
                Assert.AreSame(expected, actual);
            }
        }

        [Test]
        public void When_converting_to_a_target_type_within_the_target_types_list__allows_conversion()
        {
            var testing = new TestConverter(() => "converted", type.of<string>()) as IConverter;

            Assert.AreEqual("converted", testing.Convert("original", type.of<string>(), type.of<string>()));
        }
    }
}