using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Routine.Core.Configuration;
using Routine.Core.Configuration.Convention;
using Routine.Engine;
using Routine.Engine.Configuration.Conventional;

namespace Routine.Test.Core.Configuration
{
	[TestFixture]
	public class ConventionalListConfigurationTest : CoreTestBase
	{
		#region Setup & Helpers

		private Mock<IConvention<IType, List<string>>> conventionMock1;
		private Mock<IConvention<IType, List<string>>> conventionMock2;
		private Mock<IConvention<IType, List<string>>> conventionMock3;

		private ConventionalListConfiguration<ConventionalCodingStyle, IType, string> testing;
		private ConventionalListConfiguration<ConventionalCodingStyle, IType, string> testingOther;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			conventionMock1 = new Mock<IConvention<IType, List<string>>>();
			conventionMock2 = new Mock<IConvention<IType, List<string>>>();
			conventionMock3 = new Mock<IConvention<IType, List<string>>>();

			testing = new ConventionalListConfiguration<ConventionalCodingStyle, IType, string>(new ConventionalCodingStyle(), "test");
			testingOther = new ConventionalListConfiguration<ConventionalCodingStyle, IType, string>(new ConventionalCodingStyle(), "test other");

			testing.Add(conventionMock1.Object);
			testing.Add(conventionMock2.Object);

			testingOther.Add(conventionMock3.Object);

			SetUpConvention(conventionMock1);
			SetUpConvention(conventionMock2);
			SetUpConvention(conventionMock3);
		}

		private void SetUpConvention(Mock<IConvention<IType, List<string>>> conventionMock, params string[] returnList)
		{
			SetUpConvention(conventionMock, true, returnList);
		}

		private void SetUpConvention(Mock<IConvention<IType, List<string>>> conventionMock, bool appliesTo, params string[] returnList)
		{
			conventionMock.Setup(o => o.AppliesTo(It.IsAny<IType>())).Returns(appliesTo);
			conventionMock.Setup(o => o.Apply(It.IsAny<IType>())).Returns(returnList.ToList());
		}

		private void SetUpConvention(Mock<IConvention<IType, List<string>>> conventionMock, Exception exception)
		{
			conventionMock.Setup(o => o.AppliesTo(It.IsAny<IType>())).Returns(true);
			conventionMock.Setup(o => o.Apply(It.IsAny<IType>())).Throws(exception);
		}

		#endregion

		[Test]
		public void Returns_directly_given_convention_s_result()
		{
			SetUpConvention(conventionMock1, "result1", "result2");

			var actual = testing.Get(type.of<string>());

			Assert.AreEqual(2, actual.Count);
			Assert.AreEqual("result1", actual[0]);
			Assert.AreEqual("result2", actual[1]);
		}

		[Test]
		public void Merges_conventions_s_results()
		{
			SetUpConvention(conventionMock1, "result1", "result2");
			SetUpConvention(conventionMock2, "result3", "result4");

			var actual = testing.Get(type.of<string>());

			Assert.AreEqual(4, actual.Count);
			Assert.AreEqual("result1", actual[0]);
			Assert.AreEqual("result2", actual[1]);
			Assert.AreEqual("result3", actual[2]);
			Assert.AreEqual("result4", actual[3]);
		}

		[Test]
		public void Returns_distinct_result()
		{
			SetUpConvention(conventionMock1, "result1", "result2");
			SetUpConvention(conventionMock2, "result2", "result3");

			var actual = testing.Get(type.of<string>());

			Assert.AreEqual(3, actual.Count);
			Assert.AreEqual("result1", actual[0]);
			Assert.AreEqual("result2", actual[1]);
			Assert.AreEqual("result3", actual[2]);
		}

		[Test]
		public void Applies_only_applicable_conventions()
		{
			SetUpConvention(conventionMock1, false, "result1", "result2");
			SetUpConvention(conventionMock2, "result3", "result4");

			var actual = testing.Get(type.of<string>());

			Assert.AreEqual(2, actual.Count);
			Assert.AreEqual("result3", actual[0]);
			Assert.AreEqual("result4", actual[1]);
		}

		[Test]
		public void Can_override_empty_result_even_if_there_exist_other_applicable_conventions()
		{
			SetUpConvention(conventionMock1, "result1");
			SetUpConvention(conventionMock2, "result2");
			SetUpConvention(conventionMock3, "result3");

			testing.AddNoneWhen(type.of<string>());

			testing.Merge(testingOther);

			var actual = testing.Get(type.of<string>());

			Assert.AreEqual(0, actual.Count);
		}

		[Test]
		public void Merges_with_other_ConventionalListConfiguration_adding_other_s_conventions_to_the_end()
		{
			SetUpConvention(conventionMock1, "result1");
			SetUpConvention(conventionMock3, "result2");

			testing.Merge(testingOther);

			var actual = testing.Get(type.of<string>());

			Assert.AreEqual(2, actual.Count);
			Assert.AreEqual("result1", actual[0]);
			Assert.AreEqual("result2", actual[1]);
		}

		[Test]
		public void Cache_feature()
		{
			Assert.Fail();
		}

		[Test]
		public void Wraps_any_exception_with_ConfigurationException()
		{
			Assert.Fail();
		}

		[Test]
		public void Rethrows_ConfigurationException()
		{
			Assert.Fail();
		}
	}
}