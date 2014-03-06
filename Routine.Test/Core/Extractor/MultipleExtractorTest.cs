using System;
using Moq;
using NUnit.Framework;
using Routine.Core;
using Routine.Core.CodingStyle;
using Routine.Core.Extractor;

namespace Routine.Test.Core.Extractor
{
	[TestFixture]
	public class MultipleExtractorTest : CoreTestBase
	{
		public override string[] DomainTypeRootNamespaces{get{return new[]{"Routine.Test.Core.Extractor"};}}

		private Mock<IOptionalExtractor<string, string>> extractorMock1;
		private Mock<IOptionalExtractor<string, string>> extractorMock2;
		private Mock<IOptionalExtractor<string, string>> extractorMock3;

		private MultipleExtractor<GenericCodingStyle, string, string> testing;
		private IExtractor<string, string> testingInterface;
		private MultipleExtractor<GenericCodingStyle, string, string> testingOther;
		//private IExtractor<string, string> testingOtherInterface;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			extractorMock1 = new Mock<IOptionalExtractor<string, string>>();
			extractorMock2 = new Mock<IOptionalExtractor<string, string>>();
			extractorMock3 = new Mock<IOptionalExtractor<string, string>>();

			testingInterface = testing = new MultipleExtractor<GenericCodingStyle, string, string>(new GenericCodingStyle(), "test");
			/*testingOtherInterface =*/ testingOther = new MultipleExtractor<GenericCodingStyle, string, string>(new GenericCodingStyle(), "test other");

			testing.Add(extractorMock1.Object)
				   .Done(extractorMock2.Object);

			testingOther.Done(extractorMock3.Object);
		}

		private void ExtractorMockReturns(Mock<IOptionalExtractor<string, string>> extractorMock, string result)
		{
			extractorMock.Setup(o => o.CanExtract(It.IsAny<string>())).Returns(true);
			extractorMock.Setup(o => o.Extract(It.IsAny<string>())).Returns(result);

			string outparam = result;
			extractorMock.Setup(o => o.TryExtract(It.IsAny<string>(), out outparam)).Returns(true);
		}

		private void ExtractorMockThrows(Mock<IOptionalExtractor<string, string>> extractorMock, Exception exception)
		{
			extractorMock.Setup(o => o.CanExtract(It.IsAny<string>())).Returns(true);
			extractorMock.Setup(o => o.Extract(It.IsAny<string>())).Throws(exception);

			string outparam = null;
			extractorMock.Setup(o => o.TryExtract(It.IsAny<string>(), out outparam)).Throws(exception);
		}

		[Test]
		public void Extract_AltExtractorlardanIlkUygunOlanKullanilarakExtractYapilir()
		{
			ExtractorMockReturns(extractorMock2, "extractor2");

			Assert.AreEqual("extractor2", testingInterface.Extract("dummy"));
		}

		[Test]
		public void Extract_HicbirAltExtractorUygunDegilseHataFirlatilir()
		{
			try
			{
				testingInterface.Extract("dummy");
				Assert.Fail("exception not thrown");
			}
			catch(CannotExtractException ex)
			{
				Assert.IsTrue(ex.Message.Contains("test") && ex.Message.Contains("dummy") && ex.Message.Contains(typeof(string).Name), ex.Message);
			}
		}

		[Test]
		public void Extract_BasarisizExtractionIcinDefaultDegerSetEdilebilir()
		{
			testing.OnFailReturn("default");

			Assert.AreEqual("default", testingInterface.Extract("dummy"));
		}

		[Test]
		public void Extract_BasarisizExtractionIcinFirlatilacakHataDegistirilebilir()
		{
			var expected = new CannotExtractException();
			testing.OnFailThrow(expected);

			try
			{
				testingInterface.Extract("dummy");
				Assert.Fail("exception not thrown");
			}
			catch(CannotExtractException actual)
			{
				Assert.AreSame(expected, actual);
			}
		}

		[Test]
		public void Extract_BasarisizExtractionIcinFirlatilacakHataNesneKullanilarakOlusturulabilir()
		{
			testing.OnFailThrow(o => new CannotExtractException("!!test fail!!", o));

			try
			{
				testingInterface.Extract("dummy");
				Assert.Fail("exception not thrown");
			}
			catch(CannotExtractException ex)
			{
				Assert.IsTrue(ex.Message.Contains("!!test fail!!"), ex.Message);
			}
		}

		[Test]
		public void Extract_UygunOlanBirExtractorHataFirlatirsaHataPaketlenerekTekrarFirlatilir()
		{
			ExtractorMockThrows(extractorMock1, new Exception("inner exception"));

			try
			{
				testingInterface.Extract("dummy");
				Assert.Fail("exception not thrown");
			}
			catch(CannotExtractException ex)
			{
				Assert.AreEqual("inner exception", ex.InnerException.Message);
			}
		}

		[Test]
		public void Extract_UygunOlanBirExtractorCannotExtractHatasiFirlatirsaHataPaketlenmedenTekrarFirlatilir()
		{
			var expected = new CannotExtractException();

			ExtractorMockThrows(extractorMock1, expected);

			try
			{
				testingInterface.Extract("dummy");
				Assert.Fail("exception not thrown");
			}
			catch(CannotExtractException ex)
			{
				Assert.AreSame(expected, ex);
			}
		}

		[Test]
		public void Merge_BaskaBirMultipleExtractorIcerisindekiExtractorlariGelisSirasinaGoreSonaEkler()
		{
			ExtractorMockReturns(extractorMock3, "extractor3");

			testing.Merge(testingOther);

			Assert.AreEqual("extractor3", testingInterface.Extract("dummy"));
		}
	}
}

