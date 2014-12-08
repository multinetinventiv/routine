using System;
using System.Collections.Generic;
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

		private ConventionalListConfiguration<ConventionalCodingStyle, IType, string> testing;
		private ConventionalListConfiguration<ConventionalCodingStyle, IType, string> testingOther;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			testing = new ConventionalListConfiguration<ConventionalCodingStyle, IType, string>(new ConventionalCodingStyle(), "test");
			testingOther = new ConventionalListConfiguration<ConventionalCodingStyle, IType, string>(new ConventionalCodingStyle(), "test other");
		}


		#endregion

		[Test]
		public void Returns_directly_given_convention_s_result()
		{
			testing.Add(new[] { "result1", "result2" });

			var actual = testing.Get(type.of<string>());

			Assert.AreEqual(2, actual.Count);
			Assert.AreEqual("result1", actual[0]);
			Assert.AreEqual("result2", actual[1]);
		}

		[Test]
		public void Merges_conventions_s_results()
		{
			testing.Add(new[] { "result1", "result2" });
			testing.Add(new[] { "result3", "result4" });

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
			testing.Add(new[] { "result1", "result2" });
			testing.Add(new[] { "result2", "result3" });

			var actual = testing.Get(type.of<string>());

			Assert.AreEqual(3, actual.Count);
			Assert.AreEqual("result1", actual[0]);
			Assert.AreEqual("result2", actual[1]);
			Assert.AreEqual("result3", actual[2]);
		}

		[Test]
		public void Applies_only_applicable_conventions()
		{
			testing.Add(new[] { "result1", "result2" }, t => false);
			testing.Add(new[] { "result3", "result4" });

			var actual = testing.Get(type.of<string>());

			Assert.AreEqual(2, actual.Count);
			Assert.AreEqual("result3", actual[0]);
			Assert.AreEqual("result4", actual[1]);
		}

		[Test]
		public void Merges_with_other_ConventionalListConfiguration_adding_other_s_conventions_to_the_end()
		{
			testing.Add("result1");
			testingOther.Add("result2");

			testing.Merge(testingOther);

			var actual = testing.Get(type.of<string>());

			Assert.AreEqual(2, actual.Count);
			Assert.AreEqual("result1", actual[0]);
			Assert.AreEqual("result2", actual[1]);
		}

		[Test]
		public void Can_override_empty_result_even_if_there_exist_other_applicable_conventions()
		{
			testing.Add("result1");
			testing.Add("result2");
			testingOther.Add("result3");

			testing.AddNoneWhen(type.of<string>());

			testing.Merge(testingOther);

			var actual = testing.Get(type.of<string>());

			Assert.AreEqual(0, actual.Count);
		}

		[Test]
		public void When_set__convention_result_is_cached_for_a_given_input()
		{
			testing = new ConventionalListConfiguration<ConventionalCodingStyle, IType, string>(new ConventionalCodingStyle(), "test", true);

			var conventionMock = new Mock<IConvention<IType, List<string>>>();

			conventionMock.Setup(o => o.AppliesTo(It.IsAny<IType>())).Returns(true);
			conventionMock.Setup(o => o.Apply(It.IsAny<IType>())).Returns((IType s) => new List<string> { s.FullName });

			testing.Add(conventionMock.Object);

			testing.Get(type.of<string>());
			testing.Get(type.of<string>());
			testing.Get(type.of<int>());

			conventionMock.Verify(o => o.AppliesTo(type.of<string>()), Times.Once);
			conventionMock.Verify(o => o.Apply(type.of<string>()), Times.Once);
			conventionMock.Verify(o => o.AppliesTo(type.of<int>()), Times.Once);
			conventionMock.Verify(o => o.Apply(type.of<int>()), Times.Once);
		}

		[Test]
		public void When_an_exception_occurs__wraps_with_ConfigurationException()
		{
			var expected = new Exception("inner");
			testing.Add(c => c.By(s => { throw expected; }));

			try
			{
				testing.Get(type.of<string>());

				Assert.Fail("Exception not thrown");
			}
			catch (ConfigurationException ex)
			{
				Assert.AreSame(expected, ex.InnerException);
			}
		}

		[Test]
		public void When_a_ConfigurationException_occurs__simply_rethrows_it()
		{
			var expected = new ConfigurationException();
			testing.Add(c => c.By(s => { throw expected; }));

			try
			{
				testing.Get(type.of<string>());

				Assert.Fail("Exception not thrown");
			}
			catch (ConfigurationException ex)
			{
				Assert.AreSame(expected, ex);
			}
		}
	}
}