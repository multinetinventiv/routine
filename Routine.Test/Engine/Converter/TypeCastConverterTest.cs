using System;
using Moq;
using NUnit.Framework;
using Routine.Engine;

namespace Routine.Test.Engine.Converter
{
	[TestFixture]
	public class TypeCastConverterTest
	{
		[Test]
		public void Converts_object_to_target_type_using_cast_method_of_IType()
		{
			var typeMock = new Mock<IType>();

			typeMock.Setup(t => t.Cast(It.IsAny<object>(), It.IsAny<IType>())).Returns("success");
			typeMock.Setup(t => t.CanBe(It.IsAny<IType>())).Returns(true);

			IConverter converter = BuildRoutine.Converter().ByCastingFrom(typeMock.Object);

			Assert.AreEqual("success", converter.Convert(0, type.of<string>()));
		}

		[Test]
		public void Throws_ArgumentNullException_when_given_type_is_null()
		{
			Assert.Throws<ArgumentNullException>(() => BuildRoutine.Converter().ByCastingFrom(null));
		}

		[Test]
		public void Throws_CannotConvertException_when_given_type_cannot_be_converted_to_target_type()
		{
			var typeMock = new Mock<IType>();

			typeMock.Setup(t => t.CanBe(It.IsAny<IType>())).Returns(false);

			IConverter converter = BuildRoutine.Converter().ByCastingFrom(typeMock.Object);

			Assert.Throws<CannotConvertException>(() => converter.Convert(0, type.of<string>()));
		}
	}
}