using NUnit.Framework;
using Routine.Core;
using Routine.Core.Extractor;

namespace Routine.Test.Core.Extractor
{
	[TestFixture]
	public class StaticExtractorTest : CoreTestBase
	{
		public override string[] DomainTypeRootNamespaces{get{return new[]{"Routine.Test.Core.Extractor"};}}

		[Test]
		public void Extract_VerilenNesneNeOlursaOlsunSabitSonucuDoner()
		{
			IExtractor<string, string> testing = new StaticExtractor<string, string>("static_result");

			Assert.AreEqual("static_result", testing.Extract("test1"));
			Assert.AreEqual("static_result", testing.Extract("test2"));
		}
	}
}

