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
        Assert.That(_testing.DeclaringType.GetActualType(), Is.SameAs(_constructorInfo.DeclaringType));
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
            Assert.That(expected.Any(p => p.ParameterType == parameter.ParameterType.GetActualType()), Is.True, parameter.Name + " was not expected in parameters of " + _constructorInfo);
        }

        foreach (var parameter in expected)
        {
            Assert.That(actual.Any(p => p.ParameterType.GetActualType() == parameter.ParameterType), Is.True, parameter.Name + " was expected in index parameters of " + _constructorInfo);
        }
    }

    [Test]
    public void Routine_ConstructorInfo_caches_wrapped_properties()
    {
        Assert.That(_testing.DeclaringType, Is.SameAs(_testing.DeclaringType));
        Assert.That(_testing.GetParameters(), Is.SameAs(_testing.GetParameters()));
        Assert.That(Attribute_Constructor().GetCustomAttributes(), Is.SameAs(Attribute_Constructor().GetCustomAttributes()));
    }

    [Test, SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public void Routine_ConstructorInfo_can_be_invoked()
    {
        _testing = Members_Constructor(type.of<string>(), type.of<int>());

        var actual = _testing.Invoke("test", 1) as TestClass_Members;

        Assert.That(actual.StringProperty, Is.EqualTo("test"));
        Assert.That(actual.IntProperty, Is.EqualTo(1));
    }

    [Test]
    public void Routine_ConstructorInfo_lists_custom_attributes()
    {
        _testing = Attribute_Constructor();

        var actual = _testing.GetCustomAttributes();

        Assert.That(actual.Length, Is.EqualTo(1));
        Assert.That(actual[0], Is.InstanceOf<TestClassAttribute>());

        _testing = Attribute_Constructor(type.of<int>());

        actual = _testing.GetCustomAttributes();

        Assert.That(actual.Length, Is.EqualTo(1));
        Assert.That(actual[0], Is.InstanceOf<TestClassAttribute>());
    }

    [Test]
    public void When_exception_occurs_during_invocation__preloaded_and_reflected_implementations_behave_the_same()
    {
        var preloaded = type.of<TestClass_OOP>().GetConstructor(type.of<Exception>());
        var reflected = type.of<TestOuterDomainType_OOP>().GetConstructor(type.of<Exception>());

        Assert.That(preloaded, Is.InstanceOf<PreloadedConstructorInfo>());
        Assert.That(reflected, Is.InstanceOf<ReflectedConstructorInfo>());

        var expectedException = new Exception("expected");

        Assert.That(() => preloaded.Invoke(expectedException),
            Throws.Exception.SameAs(expectedException)
        );
        Assert.That(() => reflected.Invoke(expectedException),
            Throws.Exception.SameAs(expectedException)
        );
    }
}
