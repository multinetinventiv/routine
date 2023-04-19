using Routine.Core.Configuration;
using Routine.Core.Configuration.Convention;
using Routine.Engine.Configuration.ConventionBased;

namespace Routine.Test.Core.Configuration;

[TestFixture]
public class ConventionBasedConfigurationTest : CoreTestBase
{
    #region Setup & Helpers

    private Mock<ILayered> _layeredMock;
    private Mock<ILayered> _otherLayeredMock;

    private ConventionBasedConfiguration<ILayered, string, string> _testing;
    private ConventionBasedConfiguration<ILayered, string, string> _testingOther;
    private ConventionBasedConfiguration<ILayered, string, string> _testingOtherConfig;

    [SetUp]
    public override void SetUp()
    {
        _layeredMock = new();
        _otherLayeredMock = new();

        _testing = new(_layeredMock.Object, "test");
        _testingOther = new(_layeredMock.Object, "test other");
        _testingOtherConfig = new(_otherLayeredMock.Object, "test other configuration");

        _layeredMock.Setup(o => o.CurrentLayer).Returns(Layer.LeastSpecific);
        _otherLayeredMock.Setup(o => o.CurrentLayer).Returns(Layer.LeastSpecific);
    }

    private void SetMoreSpecificLayer()
    {
        var currentLayer = _layeredMock.Object.CurrentLayer;
        _layeredMock.Setup(o => o.CurrentLayer).Returns(currentLayer.MoreSpecific());
    }

    private void SetOverrideMode()
    {
        _layeredMock.Setup(o => o.CurrentLayer).Returns(Layer.MostSpecific);
    }

    #endregion

    [Test]
    public void Gets_using_first_appropriate_convention()
    {
        _testing.Set("convention2");

        Assert.That(_testing.Get("dummy"), Is.EqualTo("convention2"));
    }

    [Test]
    public void Throws_ConfigurationException_when_none_of_the_conventions_is_applicable_for_given_input()
    {
        try
        {
            _testing.Get("dummy");
            Assert.Fail("exception not thrown");
        }
        catch (ConfigurationException ex)
        {
            Assert.That(ex.Message.Contains("test") && ex.Message.Contains("dummy") && ex.Message.Contains(typeof(string).Name), Is.True, ex.Message);
        }
    }

    [Test]
    public void Fail_exception_can_be_overridden()
    {
        var expected = new ConfigurationException();
        _testing.OnFailThrow(expected);

        try
        {
            _testing.Get("dummy");
            Assert.Fail("exception not thrown");
        }
        catch (ConfigurationException actual)
        {
            Assert.That(actual, Is.SameAs(expected));
        }
    }

    [Test]
    public void Overridden_fail_exception_can_be_built_using_given_input()
    {
        _testing.OnFailThrow(o => new ConfigurationException("!!test fail!!", o));

        try
        {
            _testing.Get("dummy");
            Assert.Fail("exception not thrown");
        }
        catch (ConfigurationException ex)
        {
            Assert.That(ex.Message.Contains("!!test fail!!"), Is.True, ex.Message);
        }
    }

    [Test]
    public void Merges_with_other_ConventionBasedConfiguration_adding_other_s_conventions_to_the_end()
    {
        _testing.Set("out1", "in1");
        _testingOther.Set("out2");

        _testing.Merge(_testingOther);

        Assert.That(_testing.Get("in1"), Is.EqualTo("out1"));
        Assert.That(_testing.Get("dummy"), Is.EqualTo("out2"));
    }

    [Test]
    public void When_an_exception_occurs__wraps_with_ConfigurationException()
    {
        var expected = new Exception("inner");
        _testing.Set(c => c.By(_ => throw expected));

        try
        {
            _testing.Get("dummy");

            Assert.Fail("Exception not thrown");
        }
        catch (ConfigurationException ex)
        {
            Assert.That(ex.InnerException, Is.SameAs(expected));
        }
    }

    [Test]
    public void When_a_ConfigurationException_occurs__simply_rethrows_it()
    {
        var expected = new ConfigurationException();
        _testing.Set(c => c.By(_ => throw expected));

        try
        {
            _testing.Get("dummy");

            Assert.Fail("Exception not thrown");
        }
        catch (ConfigurationException ex)
        {
            Assert.That(ex, Is.SameAs(expected));
        }
    }

    [Test]
    public void When_set__convention_result_is_cached_for_a_given_input()
    {
        _testing = new ConventionBasedConfiguration<ILayered, string, string>(_layeredMock.Object, "test", true);

        var conventionMock = new Mock<IConvention<string, string>>();

        conventionMock.Setup(o => o.AppliesTo(It.IsAny<string>())).Returns(true);
        conventionMock.Setup(o => o.Apply(It.IsAny<string>())).Returns((string s) => s);

        _testing.Set(conventionMock.Object);

        _testing.Get("test");
        _testing.Get("test");
        _testing.Get("test2");

        conventionMock.Verify(o => o.AppliesTo("test"), Times.Once);
        conventionMock.Verify(o => o.Apply("test"), Times.Once);
        conventionMock.Verify(o => o.AppliesTo("test2"), Times.Once);
        conventionMock.Verify(o => o.Apply("test2"), Times.Once);
    }

    [Test]
    public void Conventions_are_applied_within_the_given_order()
    {
        _testing.Set("out1", "in1");
        _testing.Set("out2", "in1");
        _testing.Set("out3", "in2");

        Assert.That(_testing.Get("in1"), Is.EqualTo("out1"));
        Assert.That(_testing.Get("in2"), Is.EqualTo("out3"));
    }

    [Test]
    public void When_layer_is_changed__conventions_are_ordered_first_by_their_layer_order_then_by_the_given_order()
    {
        _testing.Set("out1", "in1");

        SetMoreSpecificLayer();

        _testing.Set("out2", "in1");

        Assert.That(_testing.Get("in1"), Is.EqualTo("out2"));
    }

    [Test]
    public void When_merging__conventions_remain_sorted_by_their_layer()
    {
        _testing.Set("out1", "in1");

        SetMoreSpecificLayer();

        _testingOther.Set("out2", "in1");

        _testing.Merge(_testingOther);

        Assert.That(_testing.Get("in1"), Is.EqualTo("out2"));
    }

    [Test]
    public void When_merging_conventions_from_different_configuration__current_layer_is_added_to_given_conventions()
    {
        _testing.Set("out1", "in1");

        SetMoreSpecificLayer();

        _testingOtherConfig.Set("out2", "in1");

        _testing.Merge(_testingOtherConfig);

        Assert.That(_testing.Get("in1"), Is.EqualTo("out2"));
    }

    [Test]
    public void When_merging_conventions_from_different_configuration__if_both_are_at_the_least_specific_layer__other_conventions_are_added_to_the_end()
    {
        _testing.Set("out1", "in1");
        _testingOtherConfig.Set("out2", "in1");

        _testing.Merge(_testingOtherConfig);

        Assert.That(_testing.Get("in1"), Is.EqualTo("out1"));
    }

    [Test]
    public void When_merging__if_configuration_is_in_override_layer__other_conventions_added_in_override_layer_and_to_the_end()
    {
        _testing.Set("out1", "in1");

        SetOverrideMode();

        _testingOtherConfig.Set("out2", "in1");

        _testing.Merge(_testingOtherConfig);

        Assert.That(_testing.Get("in1"), Is.EqualTo("out2"));
    }

    [Test]
    public void A_default_fail_result_can_be_set_to_be_returned_in_case_all_conventions_fail()
    {
        _testing.Set("default");

        SetMoreSpecificLayer();

        _testing.Set("specific", "in");

        Assert.That(_testing.Get("in"), Is.EqualTo("specific"));
        Assert.That(_testing.Get("dummy"), Is.EqualTo("default"));
    }

    [Ignore("")]
    [Test]
    public void BUG_when_pattern_uses_override_throws_argument_out_of_range()
    {
        //BeginTest();
        Assert.Fail();
    }

    [Test]
    public void BUG_pattern_builder_causes_reset_in_proxy_matcher()
    {
        var conventionBasedCodingStyle = new ConventionBasedCodingStyle();
        conventionBasedCodingStyle.RecognizeProxyTypesBy(t => true, t => typeof(object));

        BuildRoutine.CodingStylePattern().FromEmpty();
        var typeAfter = type.of<string>();

        Assert.That(typeAfter, Is.EqualTo(type.of<object>()));
    }
}
