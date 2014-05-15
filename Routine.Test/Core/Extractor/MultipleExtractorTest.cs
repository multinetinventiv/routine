using System;
using Moq;
using NUnit.Framework;
using Routine.Core;
using Routine.Core.Configuration;
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
		public void Extracts_using_first_appropriate_sub_extractor()
		{
			ExtractorMockReturns(extractorMock2, "extractor2");

			Assert.AreEqual("extractor2", testingInterface.Extract("dummy"));
		}

		[Test]
		public void Throws_CannotExtractException_when_none_of_the_sub_extractors_can_extract_given_input()
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
		public void Fail_exception_can_be_overridden()
		{
			var expected = new CannotExtractException();
			testing.OnFailThrow(expected);

			try
			{
				testingInterface.Extract("dummy");
				Assert.Fail("exception not thrown");
			}
			catch (CannotExtractException actual)
			{
				Assert.AreSame(expected, actual);
			}
		}

		[Test]
		public void Overridden_fail_exception_can_be_built_using_given_input()
		{
			testing.OnFailThrow(o => new CannotExtractException("!!test fail!!", o));

			try
			{
				testingInterface.Extract("dummy");
				Assert.Fail("exception not thrown");
			}
			catch (CannotExtractException ex)
			{
				Assert.IsTrue(ex.Message.Contains("!!test fail!!"), ex.Message);
			}
		}

		[Test]
		public void A_default_fail_result_can_be_set_to_be_returned_when_extraction_fails()
		{
			testing.OnFailReturn("default");

			Assert.AreEqual("default", testingInterface.Extract("dummy"));
		}

		[Test]
		public void Merges_with_other_MultipleExtractor_adding_other_s_extractors_to_the_end()
		{
			ExtractorMockReturns(extractorMock3, "extractor3");

			testing.Merge(testingOther);

			Assert.AreEqual("extractor3", testingInterface.Extract("dummy"));
		}
	}
}

