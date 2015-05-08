using System;
using NUnit.Framework;
using Routine.Engine;

namespace Routine.Test.Engine.Converter
{
	[TestFixture]
	public class DelegateBasedConverterTest
	{
		[Test]
		public void Converts_object_to_target_type_using_given_delegate()
		{
			IConverter converter = BuildRoutine.Converter().By((o, t) =>
			{
				Assert.AreEqual(0, o);
				Assert.AreEqual(type.of<string>(), t);

				return "success";
			});

			Assert.AreEqual("success", converter.Convert(0, type.of<string>()));
		}

		[Test]
		public void Throws_ArgumentNullException_when_given_delegate_is_null()
		{
			Assert.Throws<ArgumentNullException>(() => BuildRoutine.Converter().By(null));
		}
	}
}