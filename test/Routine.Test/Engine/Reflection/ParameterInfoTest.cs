using Routine.Engine.Reflection;
using Routine.Test.Engine.Reflection.Domain;
using System.Runtime.InteropServices;

namespace Routine.Test.Engine.Reflection;

[TestFixture]
public class ParameterInfoTest : ReflectionTestBase
{
    private System.Reflection.ParameterInfo _parameterInfo;
    private ParameterInfo _testing;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        _parameterInfo = typeof(ReflectedParameter).GetMethod("AMethod")?.GetParameters()[0];
        _testing = type.of<ReflectedParameter>().GetMethod("AMethod").GetParameters()[0];
    }

    [Test]
    public void System_ParameterInfo_is_wrapped_by_Routine_ParameterInfo()
    {
        Assert.That(_testing.Name, Is.EqualTo(_parameterInfo.Name));
        Assert.That(_testing.ParameterType.GetActualType(), Is.SameAs(_parameterInfo.ParameterType));
        Assert.That(_testing.Position, Is.EqualTo(_parameterInfo.Position));
        Assert.That(_testing.IsOptional, Is.EqualTo(_parameterInfo.IsOptional));
        Assert.That(_testing.HasDefaultValue, Is.EqualTo(_parameterInfo.HasDefaultValue));
        Assert.That(_testing.DefaultValue, Is.EqualTo(_parameterInfo.DefaultValue));
    }

    [Test]
    public void Routine_ParameterInfo_caches_wrapped_properties()
    {
        Assert.That(_testing.Name, Is.SameAs(_testing.Name));
        Assert.That(_testing.ParameterType, Is.SameAs(_testing.ParameterType));
        Assert.That(_testing.GetCustomAttributes(), Is.SameAs(_testing.GetCustomAttributes()));
        Assert.That(_testing.DefaultValue, Is.SameAs(_testing.DefaultValue));
    }

    [Test]
    public void Routine_ParameterInfo_lists_custom_attributes_with_inherit_behaviour()
    {
        var actual = _testing.GetCustomAttributes();

        Assert.That(actual.Length, Is.EqualTo(2));
        Assert.That(actual[0], Is.InstanceOf<TestClassAttribute>());
        Assert.That(actual[1], Is.InstanceOf<OptionalAttribute>()); // added to optional parameters automatically by compiler
    }

    [Test]
    public void Extension_Has()
    {
        Assert.That(_testing.Has<TestClassAttribute>(), Is.True);
        Assert.That(_testing.Has(type.of<TestClassAttribute>()), Is.True);
    }
}
