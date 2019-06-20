using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Routine.Core.Configuration;
using Routine.Core.Configuration.Convention;
using Routine.Engine;

namespace Routine.Test.Core.Configuration
{
	[TestFixture]
	public class ConventionBasedListConfigurationTest : CoreTestBase
	{
		#region Setup & Helpers

		private Mock<ILayered> layeredMock;
		private Mock<ILayered> otherLayeredMock;

		private ConventionBasedListConfiguration<ILayered, IType, string> testing;
		private ConventionBasedListConfiguration<ILayered, IType, string> testingOther;
		private ConventionBasedListConfiguration<ILayered, IType, string> testingOtherConfig;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			layeredMock = new Mock<ILayered>();
			otherLayeredMock = new Mock<ILayered>();

			testing = new ConventionBasedListConfiguration<ILayered, IType, string>(layeredMock.Object, "test");
			testingOther = new ConventionBasedListConfiguration<ILayered, IType, string>(layeredMock.Object, "test other");
			testingOtherConfig = new ConventionBasedListConfiguration<ILayered, IType, string>(otherLayeredMock.Object, "test other config");

			layeredMock.Setup(o => o.CurrentLayer).Returns(Layer.LeastSpecific);
			otherLayeredMock.Setup(o => o.CurrentLayer).Returns(Layer.LeastSpecific);
		}

		private void SetMoreSpecificLayer()
		{
			var currentLayer = layeredMock.Object.CurrentLayer;
			layeredMock.Setup(o => o.CurrentLayer).Returns(currentLayer.MoreSpecific());
		}

		private void SetOverrideMode()
		{
			layeredMock.Setup(o => o.CurrentLayer).Returns(Layer.MostSpecific);
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
		public void Merges_with_other_ConventionBasedListConfiguration_adding_other_s_conventions_to_the_end()
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
		public void Can_override_empty_result_even_if_there_exists_other_applicable_conventions()
		{
			testing.Add("result1");
			testing.Add("result2");
			testingOther.Add("result3");

			testing.AddNoneWhen(type.of<string>());

			testing.Merge(testingOther);

			var actual = testing.Get(type.of<string>());

			Assert.AreEqual(0, actual.Count);
		}

		[Test][Ignore]
		public void Can_override_certain_list_even_if_there_exists_other_applicable_conventions()
		{
			Assert.Fail();
		}

		[Test]
		public void When_set__convention_result_is_cached_for_a_given_input()
		{
			testing = new ConventionBasedListConfiguration<ILayered, IType, string>(layeredMock.Object, "test", true);

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

		[Test]
		public void Applies_conventions_in_the_given_order()
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
		public void When_layer_is_changed__conventions_are_ordered_first_by_their_layer_order_then_by_the_given_order()
		{
			testing.Add(new[] { "result1", "result2" });

			SetMoreSpecificLayer();

			testing.Add(new[] { "result3", "result4" });

			var actual = testing.Get(type.of<string>());

			Assert.AreEqual(4, actual.Count);
			Assert.AreEqual("result3", actual[0]);
			Assert.AreEqual("result4", actual[1]);
			Assert.AreEqual("result1", actual[2]);
			Assert.AreEqual("result2", actual[3]);
		}

		[Test]
		public void When_merging__conventions_remain_sorted_by_their_layer()
		{
			testing.Add(new []{"result1"});

			SetMoreSpecificLayer();

			testingOther.Add(new []{"result2"});

			testing.Merge(testingOther);

			var actual = testing.Get(type.of<string>());

			Assert.AreEqual(2, actual.Count);
			Assert.AreEqual("result2", actual[0]);
			Assert.AreEqual("result1", actual[1]);
		}

		[Test]
		public void When_merging_conventions_from_different_configuration__current_layer_is_added_to_given_conventions()
		{
			testing.Add(new[] { "result1" });

			SetMoreSpecificLayer();

			testingOtherConfig.Add(new[] { "result2" });

			testing.Merge(testingOtherConfig);

			var actual = testing.Get(type.of<string>());

			Assert.AreEqual(2, actual.Count);
			Assert.AreEqual("result2", actual[0]);
			Assert.AreEqual("result1", actual[1]);
		}

		[Test]
		public void When_merging_conventions_from_different_configuration__if_both_are_at_the_least_specific_layer__other_conventions_are_added_to_the_end()
		{
			testing.Add(new[] { "result1" });
			testingOtherConfig.Add(new[] { "result2" });

			testing.Merge(testingOtherConfig);

			var actual = testing.Get(type.of<string>());

			Assert.AreEqual(2, actual.Count);
			Assert.AreEqual("result1", actual[0]);
			Assert.AreEqual("result2", actual[1]);
		}

		[Test]
		public void When_merging__if_configuration_is_in_override_layer__other_conventions_added_in_override_layer_and_to_the_end()
		{
			testing.Add(new[] { "result1" });

			SetOverrideMode();

			testingOtherConfig.Add(new[] { "result2" });

			testing.Merge(testingOtherConfig);

			var actual = testing.Get(type.of<string>());

			Assert.AreEqual(2, actual.Count);
			Assert.AreEqual("result2", actual[0]);
			Assert.AreEqual("result1", actual[1]);
		}

        [Ignore]
		[Test]
		public void BUG_when_pattern_uses_override_throws_argument_out_of_range()
		{
			//BeginTest();
			Assert.Fail();
		}
	}
}