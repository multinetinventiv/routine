using Routine.Engine.Reflection;
using Routine.Test.Engine.Reflection.Domain;
using RoutineTest.OuterDomainNamespace;
using System.Diagnostics.CodeAnalysis;

namespace Routine.Test.Engine.Reflection;

[TestFixture]
public class ConstructorInfoTest : ReflectionTestBase
{
    private System.Reflection.ConstructorInfo _constructorInfo;
    private ConstructorInfo _testing;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        _constructorInfo = typeof(TestClass_OOP).GetConstructor(new[] { typeof(string) });
        _testing = type.of<TestClass_OOP>().GetConstructor(type.of<string>());
    }

    [Test]
    public void System_ConstructorInfo_is_wrapped_by_Routine_ConstructorInfo()
    {
        Assert.AreSame(_constructorInfo.DeclaringType, _testing.DeclaringType.GetActualType());
    }

    [Test, SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public void System_ConstructorInfo_GetParameters_is_wrapped_by_Routine_MethodInfo()
    {
        _constructorInfo = typeof(TestClass_Members).GetConstructor(new[] { typeof(string), typeof(int) });
        _testing = type.of<TestClass_Members>().GetConstructor(type.of<string>(), type.of<int>());

        var expected = _constructorInfo.GetParameters();
        var actual = _testing.GetParameters();

        foreach (var parameter in actual)
        {
            Assert.IsTrue(expected.Any(p => p.ParameterType == parameter.ParameterType.GetActualType()), parameter.Name + " was not expected in parameters of " + _constructorInfo);
        }

        foreach (var parameter in expected)
        {
            Assert.IsTrue(actual.Any(p => p.ParameterType.GetActualType() == parameter.ParameterType), parameter.Name + " was expected in index parameters of " + _constructorInfo);
        }
    }

    [Test]
    public void Routine_ConstructorInfo_caches_wrapped_properties()
    {
        Assert.AreSame(_testing.DeclaringType, _testing.DeclaringType);
        Assert.AreSame(_testing.GetParameters(), _testing.GetParameters());
        Assert.AreSame(Attribute_Constructor().GetCustomAttributes(), Attribute_Constructor().GetCustomAttributes());
    }

    [Test, SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public void Routine_ConstructorInfo_can_be_invoked()
    {
        _testing = Members_Constructor(type.of<string>(), type.of<int>());

        var actual = _testing.Invoke("test", 1) as TestClass_Members;

        Assert.AreEqual("test", actual.StringProperty);
        Assert.AreEqual(1, actual.IntProperty);
    }

    [Test]
    public void Routine_ConstructorInfo_lists_custom_attributes()
    {
        _testing = Attribute_Constructor();

        var actual = _testing.GetCustomAttributes();

        Assert.AreEqual(1, actual.Length);
        Assert.IsInstanceOf<TestClassAttribute>(actual[0]);

        _testing = Attribute_Constructor(type.of<int>());

        actual = _testing.GetCustomAttributes();

        Assert.AreEqual(1, actual.Length);
        Assert.IsInstanceOf<TestClassAttribute>(actual[0]);
    }

    [Test]
    public void When_exception_occurs_during_invocation__preloaded_and_reflected_implementations_behave_the_same()
    {
        var preloaded = type.of<TestClass_OOP>().GetConstructor(type.of<Exception>());
        var reflected = type.of<TestOuterDomainType_OOP>().GetConstructor(type.of<Exception>());

        Assert.IsInstanceOf<PreloadedConstructorInfo>(preloaded);
        Assert.IsInstanceOf<ReflectedConstructorInfo>(reflected);

        var expectedException = new Exception("expected");

        try
        {
            preloaded.Invoke(expectedException);
            Assert.Fail("exception not thrown");
        }
        catch (Exception ex)
        {
            Assert.AreSame(expectedException, ex);
        }

        try
        {
            reflected.Invoke(expectedException);
            Assert.Fail("exception not thrown");
        }
        catch (Exception ex)
        {
            Assert.AreSame(expectedException, ex);
        }
    }
}
