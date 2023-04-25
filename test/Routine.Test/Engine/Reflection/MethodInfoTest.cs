using Routine.Engine.Reflection;
using Routine.Test.Engine.Reflection.Domain;
using RoutineTest.OuterDomainNamespace;
using System.Diagnostics.CodeAnalysis;

namespace Routine.Test.Engine.Reflection;

[TestFixture]
public class MethodInfoTest : ReflectionTestBase
{
    private System.Reflection.MethodInfo _methodInfo;
    private MethodInfo _testing;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        _methodInfo = typeof(TestClass_OOP).GetMethod("PublicMethod");
        _testing = type.of<TestClass_OOP>().GetMethod("PublicMethod");
    }

    [Test]
    public void System_MethodInfo_is_wrapped_by_Routine_MethodInfo()
    {
        Assert.That(_testing.Name, Is.SameAs(_methodInfo.Name));
        Assert.That(_testing.DeclaringType.GetActualType(), Is.SameAs(_methodInfo.DeclaringType));
        Assert.That(_testing.ReflectedType.GetActualType(), Is.SameAs(_methodInfo.ReflectedType));
        Assert.That(_testing.ReturnType.GetActualType(), Is.SameAs(_methodInfo.ReturnType));
    }

    [TestCase("PublicMethod", "PublicMethodAsync")]
    [TestCase("PrivateMethod", "PrivateMethodAsync")]
    [TestCase("PublicStaticMethod", "PublicStaticMethodAsync")]
    [TestCase("PublicPingMethod", "PublicPingMethodAsync")]
    [TestCase("PublicStaticPingMethod", "PublicStaticPingMethodAsync")]
    public void Given_an_async_method__task_is_ignored_and_return_type_becomes_void(string sync, string async)
    {
        Assert.That(
            (type.of<TestClass_OOP>().GetMethod(sync) ?? type.of<TestClass_OOP>().GetStaticMethod(sync)).ReturnType,
            Is.SameAs((type.of<TestClass_OOP>().GetMethod(async) ?? type.of<TestClass_OOP>().GetStaticMethod(async)).ReturnType)
        );
    }

    [Test, SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public void System_MethodInfo_GetParameters_is_wrapped_by_Routine_MethodInfo()
    {
        _methodInfo = typeof(TestClass_Members).GetMethod("FiveParameterMethod");
        _testing = type.of<TestClass_Members>().GetMethod("FiveParameterMethod");

        var expected = _methodInfo.GetParameters();
        var actual = _testing.GetParameters();

        foreach (var parameter in actual)
        {
            Assert.That(expected.Any(p => p.ParameterType == parameter.ParameterType.GetActualType()), Is.True, parameter.Name + " was not expected in parameters of " + _methodInfo);
        }

        foreach (var parameter in expected)
        {
            Assert.That(actual.Any(p => p.ParameterType.GetActualType() == parameter.ParameterType), Is.True, parameter.Name + " was expected in index parameters of " + _methodInfo);
        }
    }

    [Test]
    public void Routine_MethodInfo_caches_wrapped_properties()
    {
        Assert.That(_testing.Name, Is.SameAs(_testing.Name));
        Assert.That(_testing.DeclaringType, Is.SameAs(_testing.DeclaringType));
        Assert.That(_testing.ReflectedType, Is.SameAs(_testing.ReflectedType));
        Assert.That(_testing.ReturnType, Is.SameAs(_testing.ReturnType));
        Assert.That(_testing.GetParameters(), Is.SameAs(_testing.GetParameters()));
        Assert.That(Attribute_Method("Class").GetCustomAttributes(), Is.SameAs(Attribute_Method("Class").GetCustomAttributes()));
    }

    [Test]
    public void Routine_MethodInfo_can_invoke_static_methods()
    {
        _testing = OOP_StaticMethod("PublicStaticPingMethod");

        Assert.That(_testing.InvokeStatic("test"), Is.EqualTo("static test"));
    }

    [Test]
    public void Routine_MethodInfo_can_invoke_instance_methods()
    {
        _testing = OOP_Method("PublicPingMethod");

        var obj = new TestClass_OOP();

        Assert.That(_testing.Invoke(obj, "test"), Is.EqualTo("instance test"));
    }

    [Test]
    public void Routine_MethodInfo_can_invoke_default_interface_methods()
    {
        _testing = OOP_InterfaceMethod("DefaultInterfaceMethod");

        var obj = new TestClass_OOP();

        Assert.That(_testing.Invoke(obj, "test"), Is.EqualTo("default interface test"));
    }

    [Test]
    public void Routine_MethodInfo_throws_null_exception_when_target_is_null()
    {
        _testing = OOP_Method("PublicPingMethod");

        Assert.That(() => _testing.Invoke(null, "test"), Throws.TypeOf<NullReferenceException>());

        _testing = OOP_Method("PrivateMethod");

        Assert.That(() => _testing.Invoke(null, "test"), Throws.TypeOf<NullReferenceException>());
    }

    [Test]
    public void Routine_MethodInfo_lists_custom_attributes_with_inherit_behaviour()
    {
        _testing = Attribute_Method("Class");

        var actual = _testing.GetCustomAttributes();

        Assert.That(actual.Length, Is.EqualTo(1));
        Assert.That(actual[0], Is.InstanceOf<TestClassAttribute>());

        _testing = Attribute_Method("Base");

        actual = _testing.GetCustomAttributes();

        Assert.That(actual.Length, Is.EqualTo(1));
        Assert.That(actual[0], Is.InstanceOf<TestBaseAttribute>());

        _testing = Attribute_Method("Overridden");

        actual = _testing.GetCustomAttributes();

        Assert.That(actual.Length, Is.EqualTo(2));
        Assert.That(actual[0], Is.InstanceOf<TestClassAttribute>());
        Assert.That(actual[1], Is.InstanceOf<TestBaseAttribute>());

        _testing = Attribute_InterfaceMethod("Interface");

        actual = _testing.GetCustomAttributes();

        Assert.That(actual.Length, Is.EqualTo(1));
        Assert.That(actual[0], Is.InstanceOf<TestInterfaceAttribute>());
    }

    [Test]
    public void Routine_MethodInfo_lists_return_type_custom_attributes_with_inherit_behaviour()
    {
        _testing = Attribute_Method("Class");

        var actual = _testing.GetReturnTypeCustomAttributes();

        Assert.That(actual.Length, Is.EqualTo(1));
        Assert.That(actual[0], Is.InstanceOf<TestClassAttribute>());
    }

    [Test]
    public void When_exception_occurs_during_invocation__preloaded_and_reflected_implementations_behave_the_same()
    {
        var preloaded = type.of<TestClass_OOP>().GetMethod("ExceptionMethod");
        var reflected = type.of<TestOuterDomainType_OOP>().GetMethod("ExceptionMethod");

        Assert.That(preloaded, Is.InstanceOf<PreloadedMethodInfo>());
        Assert.That(reflected, Is.InstanceOf<ReflectedMethodInfo>());

        var expectedException = new Exception("expected");

        Assert.That(() => preloaded.Invoke(new TestClass_OOP(), expectedException),
            Throws.Exception.SameAs(expectedException)
        );
        Assert.That(() => reflected.Invoke(new TestOuterDomainType_OOP(), expectedException),
            Throws.Exception.SameAs(expectedException)
        );
    }

    [Test]
    public void Extension_IsInherited()
    {
        Assert.That(OOP_Method("Public").IsInherited(false, false), Is.False);
        Assert.That(OOP_Method("Overridden").IsInherited(false, false), Is.False);
        Assert.That(OOP_Method("ImplicitInterface").IsInherited(false, false), Is.False);
        Assert.That(OOP_Method("ImplicitInterfaceWithParameter").IsInherited(false, false), Is.False);
        Assert.That(OOP_Method("NotOverridden").IsInherited(false, false), Is.True);

        Assert.That(OOP_Method("Public").IsInherited(false, true), Is.False);
        Assert.That(OOP_Method("Overridden").IsInherited(false, true), Is.True);
        Assert.That(OOP_Method("ImplicitInterface").IsInherited(false, true), Is.True);
        Assert.That(OOP_Method("ImplicitInterfaceWithParameter").IsInherited(false, true), Is.True);
        Assert.That(OOP_Method("NotOverridden").IsInherited(false, true), Is.True);

        Assert.That(OOP_Method("Public").IsInherited(true, false), Is.False);
        Assert.That(OOP_Method("Overridden").IsInherited(true, false), Is.False);
        Assert.That(OOP_Method("NotOverridden").IsInherited(true, false), Is.False);
        Assert.That(OOP_Method("OtherNamespace").IsInherited(true, false), Is.False);
        Assert.That(OOP_Method("ToString").IsInherited(true, false), Is.False);
        Assert.That(OOP_Method("GetHashCode").IsInherited(true, false), Is.True);

        Assert.That(OOP_Method("Public").IsInherited(true, true), Is.False);
        Assert.That(OOP_Method("Overridden").IsInherited(true, true), Is.False);
        Assert.That(OOP_Method("NotOverridden").IsInherited(true, true), Is.False);
        Assert.That(OOP_Method("OtherNamespace").IsInherited(true, true), Is.True);
        Assert.That(OOP_Method("ToString").IsInherited(true, true), Is.True);
        Assert.That(OOP_Method("GetHashCode").IsInherited(true, true), Is.True);
    }

    [Test]
    public void Extension_HasParameters()
    {
        Assert.That(Members_Method("Parameterless").HasNoParameters(), Is.True);
        Assert.That(Members_Method("OneParameter").HasParameters<string>(), Is.True);
        Assert.That(Members_Method("TwoParameter").HasParameters<string, int>(), Is.True);
        Assert.That(Members_Method("ThreeParameter").HasParameters<string, int, double>(), Is.True);
        Assert.That(Members_Method("FourParameter").HasParameters<string, int, double, decimal>(), Is.True);

        Assert.That(Members_Method("ThreeParameter").HasParameters<string, int>(), Is.False);
    }

    [Test]
    public void Extension_Returns()
    {
        Assert.That(Members_Method("Void").ReturnsVoid(), Is.True);
        Assert.That(Members_Method("String").ReturnsVoid(), Is.False);

        Assert.That(Members_Method("String").Returns(type.of<object>()), Is.True);
        Assert.That(Members_Method("Int").Returns(type.of<string>()), Is.False);

        Assert.That(Members_Method("StringList").ReturnsCollection(), Is.True);
        Assert.That(Members_Method("StringList").ReturnsCollection(type.of<object>()), Is.True);
        Assert.That(Members_Method("NonGenericList").ReturnsCollection(type.of<string>()), Is.False);

        //generics
        Assert.That(Members_Method("String").Returns<string>(), Is.True);
        Assert.That(Members_Method("StringList").ReturnsCollection<string>(), Is.True);

        //with name parameter
        Assert.That(Members_Method("String").Returns(type.of<string>(), "Wrong"), Is.False);
        Assert.That(Members_Method("StringList").ReturnsCollection(type.of<string>(), "Wrong"), Is.False);
    }

    [Test]
    public void Extension_Has()
    {
        Assert.That(Attribute_Method("Class").Has<TestClassAttribute>(), Is.True);
        Assert.That(Attribute_Method("Class").Has(type.of<TestClassAttribute>()), Is.True);
    }

    [Test]
    public void Extension_ReturnTypeHas()
    {
        Assert.That(Attribute_Method("Class").ReturnTypeHas<TestClassAttribute>(), Is.True);
        Assert.That(Attribute_Method("Class").ReturnTypeHas(type.of<TestClassAttribute>()), Is.True);
    }
}
