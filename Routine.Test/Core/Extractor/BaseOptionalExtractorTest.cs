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
		public void CanExtract_DurumBelirtilmemisseTrueDoner()
		{
			IOptionalExtractor<string, string> testing = new TestExtractor<string>();

			Assert.IsTrue(testing.CanExtract("any"));
		}

		[Test]
		public void CanExtract_NesneBelirtilmisDurumaUygunsaTrueDoner()
		{
			IOptionalExtractor<string, string> testing = new TestExtractor<string>().When(o => o == "valid");

			Assert.IsTrue(testing.CanExtract("valid"));
		}

		[Test]
		public void CanExtract_NesneBelirtilmisDurumaUygunDegilseFalseDoner()
		{
			IOptionalExtractor<string, string> testing = new TestExtractor<string>().When(o => o == "valid");

			Assert.IsFalse(testing.CanExtract("invalid"));
		}

		[Test]
		public void Extract_ThrowsCannotExtractExceptionWhenItCantExtractFromGivenObject()
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
		public void Facade_WhenNull()
		{
			IOptionalExtractor<string, string> testing = new TestExtractor<string>().WhenNull();

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
