using System;
using NUnit.Framework;
using Routine.Engine;

namespace Routine.Test.Engine.Configuration
{
	[TestFixture]
	public class ConventionalCodingStyleTest
	{
		[Test]
		public void Short_model_id_pattern__shortens_a_type_name_with_given_short_prefix()
		{
			var testing = BuildRoutine.CodingStyle().FromBasic()
				.Use(p => p.ShortModelIdPattern("System", "s"))
			as ICodingStyle;

			Assert.AreEqual("s-int-32", testing.GetTypeId(type.of<int>()));
		}

		[Test]
		public void Short_model_id_pattern__nullable_support()
		{
			var testing = BuildRoutine.CodingStyle().FromBasic()
				.Use(p => p.ShortModelIdPattern("System", "s"))
			as ICodingStyle;

			Assert.AreEqual("s-int-32?", testing.GetTypeId(type.of<int?>()));
			Assert.AreEqual("s-date-time?", testing.GetTypeId(type.of<DateTime?>()));
		}
	}
}