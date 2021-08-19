using System;
using Moq;
using NUnit.Framework;
using Routine.Core.Configuration;
using Routine.Core.Configuration.Convention;

namespace Routine.Test.Core.Configuration
{
    [TestFixture]
    public class ConventionBasedConfigurationTest : CoreTestBase
    {
        #region Setup & Helpers

        private Mock<ILayered> layeredMock;
        private Mock<ILayered> otherLayeredMock;

        private ConventionBasedConfiguration<ILayered, string, string> testing;
        private ConventionBasedConfiguration<ILayered, string, string> testingOther;
        private ConventionBasedConfiguration<ILayered, string, string> testingOtherConfig;

        [SetUp]
        public override void SetUp()
        {
            layeredMock = new Mock<ILayered>();
            otherLayeredMock = new Mock<ILayered>();

            testing = new ConventionBasedConfiguration<ILayered, string, string>(layeredMock.Object, "test");
            testingOther = new ConventionBasedConfiguration<ILayered, string, string>(layeredMock.Object, "test other");
            testingOtherConfig = new ConventionBasedConfiguration<ILayered, string, string>(otherLayeredMock.Object, "test other configuration");

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
        public void Gets_using_first_appropriate_convention()
        {
            testing.Set("convention2");

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
        public void Merges_with_other_ConventionBasedConfiguration_adding_other_s_conventions_to_the_end()
        {
            testing.Set("out1", "in1");
            testingOther.Set("out2");

            testing.Merge(testingOther);

            Assert.AreEqual("out1", testing.Get("in1"));
            Assert.AreEqual("out2", testing.Get("dummy"));
        }

        [Test]
        public void When_an_exception_occurs__wraps_with_ConfigurationException()
        {
            var expected = new Exception("inner");
            testing.Set(c => c.By(_ => { throw expected; }));

            try
            {
                testing.Get("dummy");

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
            testing.Set(c => c.By(_ => { throw expected; }));

            try
            {
                testing.Get("dummy");

                Assert.Fail("Exception not thrown");
            }
            catch (ConfigurationException ex)
            {
                Assert.AreSame(expected, ex);
            }
        }

        [Test]
        public void When_set__convention_result_is_cached_for_a_given_input()
        {
            testing = new ConventionBasedConfiguration<ILayered, string, string>(layeredMock.Object, "test", true);

            var conventionMock = new Mock<IConvention<string, string>>();

            conventionMock.Setup(o => o.AppliesTo(It.IsAny<string>())).Returns(true);
            conventionMock.Setup(o => o.Apply(It.IsAny<string>())).Returns((string s) => s);

            testing.Set(conventionMock.Object);

            testing.Get("test");
            testing.Get("test");
            testing.Get("test2");

            conventionMock.Verify(o => o.AppliesTo("test"), Times.Once);
            conventionMock.Verify(o => o.Apply("test"), Times.Once);
            conventionMock.Verify(o => o.AppliesTo("test2"), Times.Once);
            conventionMock.Verify(o => o.Apply("test2"), Times.Once);
        }

        [Test]
        public void Conventions_are_applied_within_the_given_order()
        {
            testing.Set("out1", "in1");
            testing.Set("out2", "in1");
            testing.Set("out3", "in2");

            Assert.AreEqual("out1", testing.Get("in1"));
            Assert.AreEqual("out3", testing.Get("in2"));
        }

        [Test]
        public void When_layer_is_changed__conventions_are_ordered_first_by_their_layer_order_then_by_the_given_order()
        {
            testing.Set("out1", "in1");

            SetMoreSpecificLayer();

            testing.Set("out2", "in1");

            Assert.AreEqual("out2", testing.Get("in1"));
        }

        [Test]
        public void When_merging__conventions_remain_sorted_by_their_layer()
        {
            testing.Set("out1", "in1");

            SetMoreSpecificLayer();

            testingOther.Set("out2", "in1");

            testing.Merge(testingOther);

            Assert.AreEqual("out2", testing.Get("in1"));
        }

        [Test]
        public void When_merging_conventions_from_different_configuration__current_layer_is_added_to_given_conventions()
        {
            testing.Set("out1", "in1");

            SetMoreSpecificLayer();

            testingOtherConfig.Set("out2", "in1");

            testing.Merge(testingOtherConfig);

            Assert.AreEqual("out2", testing.Get("in1"));
        }

        [Test]
        public void When_merging_conventions_from_different_configuration__if_both_are_at_the_least_specific_layer__other_conventions_are_added_to_the_end()
        {
            testing.Set("out1", "in1");
            testingOtherConfig.Set("out2", "in1");

            testing.Merge(testingOtherConfig);

            Assert.AreEqual("out1", testing.Get("in1"));
        }

        [Test]
        public void When_merging__if_configuration_is_in_override_layer__other_conventions_added_in_override_layer_and_to_the_end()
        {
            testing.Set("out1", "in1");

            SetOverrideMode();

            testingOtherConfig.Set("out2", "in1");

            testing.Merge(testingOtherConfig);

            Assert.AreEqual("out2", testing.Get("in1"));
        }

        [Test]
        public void A_default_fail_result_can_be_set_to_be_returned_in_case_all_conventions_fail()
        {
            testing.Set("default");

            SetMoreSpecificLayer();

            testing.Set("specific", "in");

            Assert.AreEqual("specific", testing.Get("in"));
            Assert.AreEqual("default", testing.Get("dummy"));
        }

        [Ignore("")]
        [Test]
        public void BUG_when_pattern_uses_override_throws_argument_out_of_range()
        {
            //BeginTest();
            Assert.Fail();
        }
    }
}