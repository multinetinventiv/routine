using NUnit.Framework;
using Routine.Core;
using Routine.Core.Extractor;

namespace Routine.Test.Core.Extractor
{
	[TestFixture]
	public class BaseOptionalExtractorTest : CoreTestBase
	{
		private class TestExtractor<T> : BaseOptionalExtractor<TestExtractor<T>, T, string>
		{
			protected override string Extract(T obj)
			{
				return obj.ToString();
			}
		}

		public override string[] DomainTypeRootNamespaces{get{return new[]{"Routine.Test.Core.Extractor"};}}

		[Test]
		public void When_no_condition_is_given__CanExtract_always_returns_true()
		{
			IOptionalExtractor<string, string> testing = new TestExtractor<string>();

			Assert.IsTrue(testing.CanExtract("any"));
		}

		[Test]
		public void When_given__CanExtract_returns_condition_s_output()
		{
			IOptionalExtractor<string, string> testing = new TestExtractor<string>().When(o => o == "valid");

			Assert.IsTrue(testing.CanExtract("valid"));
			Assert.IsFalse(testing.CanExtract("invalid"));
		}

		[Test]
		public void When_it_can_t_extract_from_given_object__Extract_throws_CannotExtractException()
		{
			IOptionalExtractor<string, string> testing = new TestExtractor<string>().When(o => o == "valid");

			try
			{
				testing.Extract(null);
				Assert.Fail("exception not thrown");
			}
			catch(CannotExtractException) {}
		}

		[Test]
		public void Facade_When()
		{
			IOptionalExtractor<string, string> testing = new TestExtractor<string>().When("match");

			Assert.IsTrue(testing.CanExtract("match"));
			Assert.IsFalse(testing.CanExtract("nomatch"));
		}

		[Test]
		public void Facade_WhenDefault()
		{
			IOptionalExtractor<string, string> testing = new TestExtractor<string>().WhenDefault();

			Assert.IsTrue(testing.CanExtract(null));
			Assert.IsFalse(testing.CanExtract("string"));
		}

		[Test]
		public void Facade_WhenType()
		{
			IOptionalExtractor<object, string> testing = new TestExtractor<object>().WhenType(t => t.IsPrimitive);

			Assert.IsTrue(testing.CanExtract(1));
			Assert.IsFalse(testing.CanExtract("string"));
		}
	}
}
