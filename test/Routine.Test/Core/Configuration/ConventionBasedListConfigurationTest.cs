using Routine.Core.Configuration.Convention;
using Routine.Core.Configuration;
using Routine.Engine;

namespace Routine.Test.Core.Configuration;

[TestFixture]
public class ConventionBasedListConfigurationTest : CoreTestBase
{
    #region Setup & Helpers

    private Mock<ILayered> _layeredMock;
    private Mock<ILayered> _otherLayeredMock;

    private ConventionBasedListConfiguration<ILayered, IType, string> _testing;
    private ConventionBasedListConfiguration<ILayered, IType, string> _testingOther;
    private ConventionBasedListConfiguration<ILayered, IType, string> _testingOtherConfig;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        _layeredMock = new();
        _otherLayeredMock = new();

        _testing = new(_layeredMock.Object, "test");
        _testingOther = new(_layeredMock.Object, "test other");
        _testingOtherConfig = new(_otherLayeredMock.Object, "test other config");

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
    public void Returns_directly_given_convention_s_result()
    {
        _testing.Add(new[] { "result1", "result2" });

        var actual = _testing.Get(type.of<string>());

        Assert.That(actual.Count, Is.EqualTo(2));
        Assert.That(actual[0], Is.EqualTo("result1"));
        Assert.That(actual[1], Is.EqualTo("result2"));
    }

    [Test]
    public void Merges_conventions_s_results()
    {
        _testing.Add(new[] { "result1", "result2" });
        _testing.Add(new[] { "result3", "result4" });

        var actual = _testing.Get(type.of<string>());

        Assert.That(actual.Count, Is.EqualTo(4));
        Assert.That(actual[0], Is.EqualTo("result1"));
        Assert.That(actual[1], Is.EqualTo("result2"));
        Assert.That(actual[2], Is.EqualTo("result3"));
        Assert.That(actual[3], Is.EqualTo("result4"));
    }

    [Test]
    public void Returns_distinct_result()
    {
        _testing.Add(new[] { "result1", "result2" });
        _testing.Add(new[] { "result2", "result3" });

        var actual = _testing.Get(type.of<string>());

        Assert.That(actual.Count, Is.EqualTo(3));
        Assert.That(actual[0], Is.EqualTo("result1"));
        Assert.That(actual[1], Is.EqualTo("result2"));
        Assert.That(actual[2], Is.EqualTo("result3"));
    }

    [Test]
    public void Applies_only_applicable_conventions()
    {
        _testing.Add(new[] { "result1", "result2" }, _ => false);
        _testing.Add(new[] { "result3", "result4" });

        var actual = _testing.Get(type.of<string>());

        Assert.That(actual.Count, Is.EqualTo(2));
        Assert.That(actual[0], Is.EqualTo("result3"));
        Assert.That(actual[1], Is.EqualTo("result4"));
    }

    [Test]
    public void Merges_with_other_ConventionBasedListConfiguration_adding_other_s_conventions_to_the_end()
    {
        _testing.Add("result1");
        _testingOther.Add("result2");

        _testing.Merge(_testingOther);

        var actual = _testing.Get(type.of<string>());

        Assert.That(actual.Count, Is.EqualTo(2));
        Assert.That(actual[0], Is.EqualTo("result1"));
        Assert.That(actual[1], Is.EqualTo("result2"));
    }

    [Test]
    public void Can_override_empty_result_even_if_there_exists_other_applicable_conventions()
    {
        _testing.Add("result1");
        _testing.Add("result2");
        _testingOther.Add("result3");

        _testing.AddNoneWhen(type.of<string>());

        _testing.Merge(_testingOther);

        var actual = _testing.Get(type.of<string>());

        Assert.That(actual.Count, Is.EqualTo(0));
    }

    [Test]
    [Ignore("")]
    public void Can_override_certain_list_even_if_there_exists_other_applicable_conventions()
    {
    }

    [Test]
    public void When_set__convention_result_is_cached_for_a_given_input()
    {
        _testing = new ConventionBasedListConfiguration<ILayered, IType, string>(_layeredMock.Object, "test", true);

        var conventionMock = new Mock<IConvention<IType, List<string>>>();

        conventionMock.Setup(o => o.AppliesTo(It.IsAny<IType>())).Returns(true);
        conventionMock.Setup(o => o.Apply(It.IsAny<IType>())).Returns((IType s) => new List<string> { s.FullName });

        _testing.Add(conventionMock.Object);

        _testing.Get(type.of<string>());
        _testing.Get(type.of<string>());
        _testing.Get(type.of<int>());

        conventionMock.Verify(o => o.AppliesTo(type.of<string>()), Times.Once);
        conventionMock.Verify(o => o.Apply(type.of<string>()), Times.Once);
        conventionMock.Verify(o => o.AppliesTo(type.of<int>()), Times.Once);
        conventionMock.Verify(o => o.Apply(type.of<int>()), Times.Once);
    }

    [Test]
    public void When_an_exception_occurs__wraps_with_ConfigurationException()
    {
        var expected = new Exception("inner");
        _testing.Add(c => c.By(_ => throw expected));

        Assert.That(() => _testing.Get(type.of<string>()),
            Throws.TypeOf<ConfigurationException>().With.InnerException.SameAs(expected)
        );
    }

    [Test]
    public void When_a_ConfigurationException_occurs__simply_rethrows_it()
    {
        var expected = new ConfigurationException();
        _testing.Add(c => c.By(_ => throw expected));

        Assert.That(() => _testing.Get(type.of<string>()),
            Throws.TypeOf<ConfigurationException>().With.SameAs(expected)
        );
    }

    [Test]
    public void Applies_conventions_in_the_given_order()
    {
        _testing.Add(new[] { "result1", "result2" });
        _testing.Add(new[] { "result3", "result4" });

        var actual = _testing.Get(type.of<string>());

        Assert.That(actual.Count, Is.EqualTo(4));
        Assert.That(actual[0], Is.EqualTo("result1"));
        Assert.That(actual[1], Is.EqualTo("result2"));
        Assert.That(actual[2], Is.EqualTo("result3"));
        Assert.That(actual[3], Is.EqualTo("result4"));
    }

    [Test]
    public void When_layer_is_changed__conventions_are_ordered_first_by_their_layer_order_then_by_the_given_order()
    {
        _testing.Add(new[] { "result1", "result2" });

        SetMoreSpecificLayer();

        _testing.Add(new[] { "result3", "result4" });

        var actual = _testing.Get(type.of<string>());

        Assert.That(actual.Count, Is.EqualTo(4));
        Assert.That(actual[0], Is.EqualTo("result3"));
        Assert.That(actual[1], Is.EqualTo("result4"));
        Assert.That(actual[2], Is.EqualTo("result1"));
        Assert.That(actual[3], Is.EqualTo("result2"));
    }

    [Test]
    public void When_merging__conventions_remain_sorted_by_their_layer()
    {
        _testing.Add(new[] { "result1" });

        SetMoreSpecificLayer();

        _testingOther.Add(new[] { "result2" });

        _testing.Merge(_testingOther);

        var actual = _testing.Get(type.of<string>());

        Assert.That(actual.Count, Is.EqualTo(2));
        Assert.That(actual[0], Is.EqualTo("result2"));
        Assert.That(actual[1], Is.EqualTo("result1"));
    }

    [Test]
    public void When_merging_conventions_from_different_configuration__current_layer_is_added_to_given_conventions()
    {
        _testing.Add(new[] { "result1" });

        SetMoreSpecificLayer();

        _testingOtherConfig.Add(new[] { "result2" });

        _testing.Merge(_testingOtherConfig);

        var actual = _testing.Get(type.of<string>());

        Assert.That(actual.Count, Is.EqualTo(2));
        Assert.That(actual[0], Is.EqualTo("result2"));
        Assert.That(actual[1], Is.EqualTo("result1"));
    }

    [Test]
    public void When_merging_conventions_from_different_configuration__if_both_are_at_the_least_specific_layer__other_conventions_are_added_to_the_end()
    {
        _testing.Add(new[] { "result1" });
        _testingOtherConfig.Add(new[] { "result2" });

        _testing.Merge(_testingOtherConfig);

        var actual = _testing.Get(type.of<string>());

        Assert.That(actual.Count, Is.EqualTo(2));
        Assert.That(actual[0], Is.EqualTo("result1"));
        Assert.That(actual[1], Is.EqualTo("result2"));
    }

    [Test]
    public void When_merging__if_configuration_is_in_override_layer__other_conventions_added_in_override_layer_and_to_the_end()
    {
        _testing.Add(new[] { "result1" });

        SetOverrideMode();

        _testingOtherConfig.Add(new[] { "result2" });

        _testing.Merge(_testingOtherConfig);

        var actual = _testing.Get(type.of<string>());

        Assert.That(actual.Count, Is.EqualTo(2));
        Assert.That(actual[0], Is.EqualTo("result2"));
        Assert.That(actual[1], Is.EqualTo("result1"));
    }

    [Ignore("")]
    [Test]
    public void BUG_when_pattern_uses_override_throws_argument_out_of_range()
    {
        //BeginTest();
    }
}
