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
        Assert.AreSame(_methodInfo.Name, _testing.Name);
        Assert.AreSame(_methodInfo.DeclaringType, _testing.DeclaringType.GetActualType());
        Assert.AreSame(_methodInfo.ReflectedType, _testing.ReflectedType.GetActualType());
        Assert.AreSame(_methodInfo.ReturnType, _testing.ReturnType.GetActualType());
    }

    [TestCase("PublicMethod", "PublicMethodAsync")]
    [TestCase("PrivateMethod", "PrivateMethodAsync")]
    [TestCase("PublicStaticMethod", "PublicStaticMethodAsync")]
    [TestCase("PublicPingMethod", "PublicPingMethodAsync")]
    [TestCase("PublicStaticPingMethod", "PublicStaticPingMethodAsync")]
    public void Given_an_async_method__task_is_ignored_and_return_type_becomes_void(string sync, string async)
    {
        Assert.AreSame(
            (type.of<TestClass_OOP>().GetMethod(sync) ?? type.of<TestClass_OOP>().GetStaticMethod(sync)).ReturnType,
            (type.of<TestClass_OOP>().GetMethod(async) ?? type.of<TestClass_OOP>().GetStaticMethod(async)).ReturnType
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
            Assert.IsTrue(expected.Any(p => p.ParameterType == parameter.ParameterType.GetActualType()), parameter.Name + " was not expected in parameters of " + _methodInfo);
        }

        foreach (var parameter in expected)
        {
            Assert.IsTrue(actual.Any(p => p.ParameterType.GetActualType() == parameter.ParameterType), parameter.Name + " was expected in index parameters of " + _methodInfo);
        }
    }

    [Test]
    public void Routine_MethodInfo_caches_wrapped_properties()
    {
        Assert.AreSame(_testing.Name, _testing.Name);
        Assert.AreSame(_testing.DeclaringType, _testing.DeclaringType);
        Assert.AreSame(_testing.ReflectedType, _testing.ReflectedType);
        Assert.AreSame(_testing.ReturnType, _testing.ReturnType);
        Assert.AreSame(_testing.GetParameters(), _testing.GetParameters());
        Assert.AreSame(Attribute_Method("Class").GetCustomAttributes(), Attribute_Method("Class").GetCustomAttributes());
    }

    [Test]
    public void Routine_MethodInfo_can_invoke_static_methods()
    {
        _testing = OOP_StaticMethod("PublicStaticPingMethod");

        Assert.AreEqual("static test", _testing.InvokeStatic("test"));
    }

    [Test]
    public void Routine_MethodInfo_can_invoke_instance_methods()
    {
        _testing = OOP_Method("PublicPingMethod");

        var obj = new TestClass_OOP();

        Assert.AreEqual("instance test", _testing.Invoke(obj, "test"));
    }

    [Test]
    public void Routine_MethodInfo_can_invoke_default_interface_methods()
    {
        _testing = OOP_InterfaceMethod("DefaultInterfaceMethod");

        var obj = new TestClass_OOP();

        Assert.AreEqual("default interface test", _testing.Invoke(obj, "test"));
    }

    [Test]
    public void Routine_MethodInfo_throws_null_exception_when_target_is_null()
    {
        _testing = OOP_Method("PublicPingMethod");

        Assert.Throws<NullReferenceException>(() => _testing.Invoke(null, "test"));

        _testing = OOP_Method("PrivateMethod");

        Assert.Throws<NullReferenceException>(() => _testing.Invoke(null, "test"));
    }

    [Test]
    public void Routine_MethodInfo_lists_custom_attributes_with_inherit_behaviour()
    {
        _testing = Attribute_Method("Class");

        var actual = _testing.GetCustomAttributes();

        Assert.AreEqual(1, actual.Length);
        Assert.IsInstanceOf<TestClassAttribute>(actual[0]);

        _testing = Attribute_Method("Base");

        actual = _testing.GetCustomAttributes();

        Assert.AreEqual(1, actual.Length);
        Assert.IsInstanceOf<TestBaseAttribute>(actual[0]);

        _testing = Attribute_Method("Overridden");

        actual = _testing.GetCustomAttributes();

        Assert.AreEqual(2, actual.Length);
        Assert.IsInstanceOf<TestClassAttribute>(actual[0]);
        Assert.IsInstanceOf<TestBaseAttribute>(actual[1]);

        _testing = Attribute_InterfaceMethod("Interface");

        actual = _testing.GetCustomAttributes();

        Assert.AreEqual(1, actual.Length);
        Assert.IsInstanceOf<TestInterfaceAttribute>(actual[0]);
    }

    [Test]
    public void Routine_MethodInfo_lists_return_type_custom_attributes_with_inherit_behaviour()
    {
        _testing = Attribute_Method("Class");

        var actual = _testing.GetReturnTypeCustomAttributes();

        Assert.AreEqual(1, actual.Length);
        Assert.IsInstanceOf<TestClassAttribute>(actual[0]);
    }

    [Test]
    public void When_exception_occurs_during_invocation__preloaded_and_reflected_implementations_behave_the_same()
    {
        var preloaded = type.of<TestClass_OOP>().GetMethod("ExceptionMethod");
        var reflected = type.of<TestOuterDomainType_OOP>().GetMethod("ExceptionMethod");

        Assert.IsInstanceOf<PreloadedMethodInfo>(preloaded);
        Assert.IsInstanceOf<ReflectedMethodInfo>(reflected);

        var expectedException = new Exception("expected");

        try
        {
            preloaded.Invoke(new TestClass_OOP(), expectedException);
            Assert.Fail("exception not thrown");
        }
        catch (Exception ex)
        {
            Assert.AreSame(expectedException, ex);
        }

        try
        {
            reflected.Invoke(new TestOuterDomainType_OOP(), expectedException);
            Assert.Fail("exception not thrown");
        }
        catch (Exception ex)
        {
            Assert.AreSame(expectedException, ex);
        }
    }

    [Test]
    public void Extension_IsInherited()
    {
        Assert.IsFalse(OOP_Method("Public").IsInherited(false, false));
        Assert.IsFalse(OOP_Method("Overridden").IsInherited(false, false));
        Assert.IsFalse(OOP_Method("ImplicitInterface").IsInherited(false, false));
        Assert.IsFalse(OOP_Method("ImplicitInterfaceWithParameter").IsInherited(false, false));
        Assert.IsTrue(OOP_Method("NotOverridden").IsInherited(false, false));

        Assert.IsFalse(OOP_Method("Public").IsInherited(false, true));
        Assert.IsTrue(OOP_Method("Overridden").IsInherited(false, true));
        Assert.IsTrue(OOP_Method("ImplicitInterface").IsInherited(false, true));
        Assert.IsTrue(OOP_Method("ImplicitInterfaceWithParameter").IsInherited(false, true));
        Assert.IsTrue(OOP_Method("NotOverridden").IsInherited(false, true));

        Assert.IsFalse(OOP_Method("Public").IsInherited(true, false));
        Assert.IsFalse(OOP_Method("Overridden").IsInherited(true, false));
        Assert.IsFalse(OOP_Method("NotOverridden").IsInherited(true, false));
        Assert.IsFalse(OOP_Method("OtherNamespace").IsInherited(true, false));
        Assert.IsFalse(OOP_Method("ToString").IsInherited(true, false));
        Assert.IsTrue(OOP_Method("GetHashCode").IsInherited(true, false));

        Assert.IsFalse(OOP_Method("Public").IsInherited(true, true));
        Assert.IsFalse(OOP_Method("Overridden").IsInherited(true, true));
        Assert.IsFalse(OOP_Method("NotOverridden").IsInherited(true, true));
        Assert.IsTrue(OOP_Method("OtherNamespace").IsInherited(true, true));
        Assert.IsTrue(OOP_Method("ToString").IsInherited(true, true));
        Assert.IsTrue(OOP_Method("GetHashCode").IsInherited(true, true));
    }

    [Test]
    public void Extension_HasParameters()
    {
        Assert.IsTrue(Members_Method("Parameterless").HasNoParameters());
        Assert.IsTrue(Members_Method("OneParameter").HasParameters<string>());
        Assert.IsTrue(Members_Method("TwoParameter").HasParameters<string, int>());
        Assert.IsTrue(Members_Method("ThreeParameter").HasParameters<string, int, double>());
        Assert.IsTrue(Members_Method("FourParameter").HasParameters<string, int, double, decimal>());

        Assert.IsFalse(Members_Method("ThreeParameter").HasParameters<string, int>());
    }

    [Test]
    public void Extension_Returns()
    {
        Assert.IsTrue(Members_Method("Void").ReturnsVoid());
        Assert.IsFalse(Members_Method("String").ReturnsVoid());

        Assert.IsTrue(Members_Method("String").Returns(type.of<object>()));
        Assert.IsFalse(Members_Method("Int").Returns(type.of<string>()));

        Assert.IsTrue(Members_Method("StringList").ReturnsCollection());
        Assert.IsTrue(Members_Method("StringList").ReturnsCollection(type.of<object>()));
        Assert.IsFalse(Members_Method("NonGenericList").ReturnsCollection(type.of<string>()));

        //generics
        Assert.IsTrue(Members_Method("String").Returns<string>());
        Assert.IsTrue(Members_Method("StringList").ReturnsCollection<string>());

        //with name parameter
        Assert.IsFalse(Members_Method("String").Returns(type.of<string>(), "Wrong"));
        Assert.IsFalse(Members_Method("StringList").ReturnsCollection(type.of<string>(), "Wrong"));
    }

    [Test]
    public void Extension_Has()
    {
        Assert.IsTrue(Attribute_Method("Class").Has<TestClassAttribute>());
        Assert.IsTrue(Attribute_Method("Class").Has(type.of<TestClassAttribute>()));
    }

    [Test]
    public void Extension_ReturnTypeHas()
    {
        Assert.IsTrue(Attribute_Method("Class").ReturnTypeHas<TestClassAttribute>());
        Assert.IsTrue(Attribute_Method("Class").ReturnTypeHas(type.of<TestClassAttribute>()));
    }
}
