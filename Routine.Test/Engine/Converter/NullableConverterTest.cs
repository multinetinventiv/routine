using NUnit.Framework;
using Routine.Engine;

namespace Routine.Test.Engine.Converter
{
	[TestFixture]
	public class NullableConverterTest
	{
		[Test]
		public void Converts_value_types_to_their_nullable_types()
		{
			IConverter converter = BuildRoutine.Converter().ToNullable();

			Assert.AreEqual(type.of<int?>(), converter.GetTargetTypes(type.of<int>())[0]);
			
			var actual = converter.Convert(3, type.of<int>(), type.of<int?>());

			int? expected = 3;

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Does_not_cover_void__non_value_types_and_generic_types()
		{
			IConverter converter = BuildRoutine.Converter().ToNullable();

			Assert.IsEmpty(converter.GetTargetTypes(type.ofvoid()));
			Assert.IsEmpty(converter.GetTargetTypes(type.of<string>()));
			Assert.IsEmpty(converter.GetTargetTypes(type.of<int?>()));
		}
	}
}