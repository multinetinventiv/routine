using System;
using NUnit.Framework;
using Routine.Engine.Converter;

namespace Routine.Test.Engine.Converter
{
	[TestFixture][Ignore]
	public class NullableConverterTest
	{
		[Test]
		public void Write_tests()
		{
			Assert.Fail();
		}

		[Test]
		public void Test_given_type_is_nullable()
		{
			Assert.DoesNotThrow(() => new NullableConverter(type.of<int?>()));
			Assert.Throws<ArgumentException>(() => new NullableConverter(type.of<int>()));
		}
	}
}