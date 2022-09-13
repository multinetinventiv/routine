using Routine.Engine.Reflection;
using Routine.Test.Engine.Reflection.Domain;
using System.Runtime.InteropServices;

namespace Routine.Test.Engine.Reflection;

[TestFixture]
public class ParameterInfoTest : ReflectionTestBase
{
    private System.Reflection.ParameterInfo parameterInfo;
    private ParameterInfo testing;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        parameterInfo = typeof(ReflectedParameter).GetMethod("AMethod")?.GetParameters()[0];
        testing = type.of<ReflectedParameter>().GetMethod("AMethod").GetParameters()[0];
    }

    [Test]
    public void System_ParameterInfo_is_wrapped_by_Routine_ParameterInfo()
    {
        Assert.AreEqual(parameterInfo.Name, testing.Name);
        Assert.AreSame(parameterInfo.ParameterType, testing.ParameterType.GetActualType());
        Assert.AreEqual(parameterInfo.Position, testing.Position);
        Assert.AreEqual(parameterInfo.IsOptional, testing.IsOptional);
        Assert.AreEqual(parameterInfo.HasDefaultValue, testing.HasDefaultValue);
        Assert.AreEqual(parameterInfo.DefaultValue, testing.DefaultValue);
    }

    [Test]
    public void Routine_ParameterInfo_caches_wrapped_properties()
    {
        Assert.AreSame(testing.Name, testing.Name);
        Assert.AreSame(testing.ParameterType, testing.ParameterType);
        Assert.AreSame(testing.GetCustomAttributes(), testing.GetCustomAttributes());
        Assert.AreSame(testing.DefaultValue, testing.DefaultValue);
    }

    [Test]
    public void Routine_ParameterInfo_lists_custom_attributes_with_inherit_behaviour()
    {
        var actual = testing.GetCustomAttributes();

        Assert.AreEqual(2, actual.Length);
        Assert.IsInstanceOf<TestClassAttribute>(actual[0]);
        Assert.IsInstanceOf<OptionalAttribute>(actual[1]); // added to optional parameters automatically by compiler
    }

    [Test]
    public void Extension_Has()
    {
        Assert.IsTrue(testing.Has<TestClassAttribute>());
        Assert.IsTrue(testing.Has(type.of<TestClassAttribute>()));
    }
}
