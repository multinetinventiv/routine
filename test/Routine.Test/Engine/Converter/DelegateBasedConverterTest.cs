using NUnit.Framework;
using Routine.Engine.Converter;
using Routine.Engine;
using System.Collections.Generic;
using System;

namespace Routine.Test.Engine.Converter
{
    [TestFixture]
    public class DelegateBasedConverterTest
    {
        [Test]
        public void Inherites_ConverterBase()
        {
            Assert.IsTrue(typeof(ConverterBase<DelegateBasedConverter>).IsAssignableFrom(typeof(DelegateBasedConverter)));
        }

        [Test]
        public void Converts_object_to_target_type_using_given_delegate()
        {
            IConverter converter = BuildRoutine.Converter().By(() => new List<IType> { type.of<string>() }, (o, t) =>
            {
                Assert.AreEqual(0, o);
                Assert.AreEqual(type.of<string>(), t);

                return "success";
            });

            Assert.AreEqual("success", converter.Convert(0, type.of<int>(), type.of<string>()));
        }

        [Test]
        public void Throws_ArgumentNullException_when_any_of_the_given_delegates_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => BuildRoutine.Converter().By(() => new List<IType>(), null));
            Assert.Throws<ArgumentNullException>(() => BuildRoutine.Converter().By((Func<IType>)null, (_, _) => new object()));
        }

        [Test]
        public void Facade_By_IType__List_IType_and_Func_IType()
        {
            Assert.AreEqual("success", ((IConverter)BuildRoutine.Converter().By(() => type.of<string>(), (_, _) => "success")).Convert(0, type.of<int>(), type.of<string>()));
            Assert.AreEqual("success", ((IConverter)BuildRoutine.Converter().By(type.of<string>(), (_, _) => "success")).Convert(0, type.of<int>(), type.of<string>()));
            Assert.AreEqual("success", ((IConverter)BuildRoutine.Converter().By(new List<IType> { type.of<string>() }, (_, _) => "success")).Convert(0, type.of<int>(), type.of<string>()));
        }

        [Test]
        public void Facade_ToConstant()
        {
            Assert.AreEqual("success", ((IConverter)BuildRoutine.Converter().ToConstant("success")).Convert(0, type.of<int>(), type.of<string>()));
        }

        [Test]
        public void Facade_ToConstant_when_null_is_given_type_is_defined_as_null()
        {
            IConverter converter = BuildRoutine.Converter().ToConstant(null);

            Assert.IsNull(converter.GetTargetTypes(type.of<string>())[0]);
        }
    }
}
