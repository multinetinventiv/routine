using System;
using Moq;
using NUnit.Framework;
using Routine.Core.Configuration;
using Routine.Core.Configuration.Convention;
using Routine.Engine.Configuration.Conventional;

namespace Routine.Test.Core.Configuration
{
	[TestFixture]
	public class ConventionalConfigurationTest : CoreTestBase
	{
		#region Setup & Helpers

		private Mock<IConvention<string, string>> conventionMock1;
		private Mock<IConvention<string, string>> conventionMock2;
		private Mock<IConvention<string, string>> conventionMock3;

		private ConventionalConfiguration<ConventionalCodingStyle, string, string> testing;
		private ConventionalConfiguration<ConventionalCodingStyle, string, string> testingOther;

		[SetUp]
		public override void SetUp()
		{
			conventionMock1 = new Mock<IConvention<string, string>>();
			conventionMock2 = new Mock<IConvention<string, string>>();
			conventionMock3 = new Mock<IConvention<string, string>>();

			testing = new ConventionalConfiguration<ConventionalCodingStyle, string, string>(new ConventionalCodingStyle(), "test");
			testingOther = new ConventionalConfiguration<ConventionalCodingStyle, string, string>(new ConventionalCodingStyle(), "test other");

			testing.Set(conventionMock1.Object);
			testing.Set(conventionMock2.Object);

			testingOther.Set(conventionMock3.Object);
		}

		private void SetUpConvention(Mock<IConvention<string, string>> conventionMock, string result)
		{
			conventionMock.Setup(o => o.AppliesTo(It.IsAny<string>())).Returns(true);
			conventionMock.Setup(o => o.Apply(It.IsAny<string>())).Returns(result);
		}

		private void SetUpConvention(Mock<IConvention<string, string>> conventionMock, Exception exception)
		{
			conventionMock.Setup(o => o.AppliesTo(It.IsAny<string>())).Returns(true);
			conventionMock.Setup(o => o.Apply(It.IsAny<string>())).Throws(exception);
		} 

		#endregion

		[Test]
		public void Gets_using_first_appropriate_convention()
		{
			SetUpConvention(conventionMock2, "convention2");

			Assert.AreEqual("convention2", testing.Get("dummy"));
		}

		[Test]
		public void Throws_ConfigurationException_when_none_of_the_conventions_is_applicable_for_given_input()
		{
			try
			{
				testing.Get("dummy");
				Assert.Fail("exception not thrown");
			}
			catch (ConfigurationException ex)
			{
				Assert.IsTrue(ex.Message.Contains("test") && ex.Message.Contains("dummy") && ex.Message.Contains(typeof(string).Name), ex.Message);
			}
		}

		[Test]
		public void Fail_exception_can_be_overridden()
		{
			var expected = new ConfigurationException();
			testing.OnFailThrow(expected);

			try
			{
				testing.Get("dummy");
				Assert.Fail("exception not thrown");
			}
			catch (ConfigurationException actual)
			{
				Assert.AreSame(expected, actual);
			}
		}

		[Test]
		public void Overridden_fail_exception_can_be_built_using_given_input()
		{
			testing.OnFailThrow(o => new ConfigurationException("!!test fail!!", o));

			try
			{
				testing.Get("dummy");
				Assert.Fail("exception not thrown");
			}
			catch (ConfigurationException ex)
			{
				Assert.IsTrue(ex.Message.Contains("!!test fail!!"), ex.Message);
			}
		}

		[Test]
		public void A_default_fail_result_can_be_set_to_be_returned_in_case_all_conventions_fail()
		{
			testing.OnFailReturn("default");

			Assert.AreEqual("default", testing.Get("dummy"));
		}

		[Test]
		public void Merges_with_other_ConventionalConfiguration_adding_other_s_conventions_to_the_end()
		{
			SetUpConvention(conventionMock3, "convention3");

			testing.Merge(testingOther);

			Assert.AreEqual("convention3", testing.Get("dummy"));
		}

		[Test]
		public void When_an_exception_occurs__wraps_with_ConfigurationException()
		{
			Assert.Fail();
		}

		[Test]
		public void When_a_ConfigurationException_occurs__simply_rethrows_it()
		{
			Assert.Fail();
		}

		[Test]
		public void Test_cache_feature()
		{
			Assert.Fail();
		}

		[Test][Ignore]
		public void Convention_orders()
		{
			Assert.Fail();
		}

		[Test][Ignore]
		public void Specific_configuration()
		{
			Assert.Fail();
		}
	}
}