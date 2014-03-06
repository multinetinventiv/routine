using NUnit.Framework;
using Routine.Core;
using Routine.Core.Extractor;

namespace Routine.Test.Core.Extractor
{
	[TestFixture]
	public class ConverterExtractorTest : CoreTestBase
	{
		public override string[] DomainTypeRootNamespaces{get{return new[]{"Routine.Test.Core.Extractor"};}}

		[Test]
		public void Extract_VerilenNesneConverterMetodundanGecirilerekSonucEldeEdilir()
		{
			IExtractor<object, string> testing = 
				new ConverterExtractor<object, string>()
					.Return(o => o.ToString());

			Assert.AreEqual("1", testing.Extract(1));
		}

		[Test]
		public void Extract_ConverterTanimlanmazsaSonucunDefaultDegeriDonulur()
		{
			IExtractor<int, string> testing1 = new ConverterExtractor<int, string>();
			Assert.AreEqual(null, testing1.Extract(1));

			IExtractor<string, int> testing2 = new ConverterExtractor<string, int>();
			Assert.AreEqual(0, testing2.Extract("string"));
		}
	}
}

