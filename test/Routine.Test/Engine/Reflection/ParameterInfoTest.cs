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
        Assert.AreEqual(_parameterInfo.Name, _testing.Name);
        Assert.AreSame(_parameterInfo.ParameterType, _testing.ParameterType.GetActualType());
        Assert.AreEqual(_parameterInfo.Position, _testing.Position);
        Assert.AreEqual(_parameterInfo.IsOptional, _testing.IsOptional);
        Assert.AreEqual(_parameterInfo.HasDefaultValue, _testing.HasDefaultValue);
        Assert.AreEqual(_parameterInfo.DefaultValue, _testing.DefaultValue);
    }

    [Test]
    public void Routine_ParameterInfo_caches_wrapped_properties()
    {
        Assert.AreSame(_testing.Name, _testing.Name);
        Assert.AreSame(_testing.ParameterType, _testing.ParameterType);
        Assert.AreSame(_testing.GetCustomAttributes(), _testing.GetCustomAttributes());
        Assert.AreSame(_testing.DefaultValue, _testing.DefaultValue);
    }

    [Test]
    public void Routine_ParameterInfo_lists_custom_attributes_with_inherit_behaviour()
    {
        var actual = _testing.GetCustomAttributes();

        Assert.AreEqual(2, actual.Length);
        Assert.IsInstanceOf<TestClassAttribute>(actual[0]);
        Assert.IsInstanceOf<OptionalAttribute>(actual[1]); // added to optional parameters automatically by compiler
    }

    [Test]
    public void Extension_Has()
    {
        Assert.IsTrue(_testing.Has<TestClassAttribute>());
        Assert.IsTrue(_testing.Has(type.of<TestClassAttribute>()));
    }
}
