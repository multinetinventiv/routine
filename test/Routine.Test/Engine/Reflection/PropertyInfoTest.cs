using Routine.Engine.Reflection;
using Routine.Test.Engine.Reflection.Domain;
using RoutineTest.OuterDomainNamespace;
using System.Diagnostics.CodeAnalysis;

namespace Routine.Test.Engine.Reflection;

[TestFixture]
public class PropertyInfoTest : ReflectionTestBase
{
    private System.Reflection.PropertyInfo _propertyInfo;
    private PropertyInfo _testing;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        _propertyInfo = typeof(TestClass_OOP).GetProperty("PublicProperty");
        _testing = type.of<TestClass_OOP>().GetProperty("PublicProperty");
    }

    [Test]
    public void System_PropertyInfo_is_wrapped_by_Routine_PropertyInfo()
    {
        Assert.That(_testing.Name, Is.EqualTo(_propertyInfo.Name));
        Assert.That(_testing.GetGetMethod().Name, Is.EqualTo(_propertyInfo.GetGetMethod()?.Name));
        Assert.That(_testing.GetSetMethod().Name, Is.EqualTo(_propertyInfo.GetSetMethod()?.Name));
        Assert.That(_testing.DeclaringType.GetActualType(), Is.SameAs(_propertyInfo.DeclaringType));
        Assert.That(_testing.ReflectedType.GetActualType(), Is.SameAs(_propertyInfo.ReflectedType));
        Assert.That(_testing.PropertyType.GetActualType(), Is.SameAs(_propertyInfo.PropertyType));
    }

    [Test, SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public void System_PropertyInfo_GetIndexParameters_is_wrapped_by_Routine_PropertyInfo()
    {
        _propertyInfo = typeof(TestClass_OOP).GetProperty("Item");
        _testing = type.of<TestClass_OOP>().GetProperty("Item");

        var expected = _propertyInfo.GetIndexParameters();
        var actual = _testing.GetIndexParameters();

        foreach (var parameter in actual)
        {
            Assert.That(expected.Any(p => p.ParameterType == parameter.ParameterType.GetActualType()), Is.True, $"{parameter.Name} was not expected in index parameters of {_propertyInfo}");
        }

        foreach (var parameter in expected)
        {
            Assert.That(actual.Any(p => p.ParameterType.GetActualType() == parameter.ParameterType), Is.True, $"{parameter.Name} was expected in index parameters of  {_propertyInfo}");
        }
    }

    [Test]
    public void Routine_PropertyInfo_caches_wrapped_properties()
    {
        Assert.That(_testing.Name, Is.SameAs(_testing.Name));
        Assert.That(_testing.GetGetMethod(), Is.SameAs(_testing.GetGetMethod()));
        Assert.That(_testing.GetSetMethod(), Is.SameAs(_testing.GetSetMethod()));
        Assert.That(_testing.DeclaringType, Is.SameAs(_testing.DeclaringType));
        Assert.That(_testing.ReflectedType, Is.SameAs(_testing.ReflectedType));
        Assert.That(_testing.PropertyType, Is.SameAs(_testing.PropertyType));
        Assert.That(_testing.GetIndexParameters(), Is.SameAs(_testing.GetIndexParameters()));
        Assert.That(Attribute_Property("Class").GetCustomAttributes(), Is.SameAs(Attribute_Property("Class").GetCustomAttributes()));
    }

    [Test]
    public void Routine_PropertyInfo_can_get_value()
    {
        var obj = new TestClass_OOP
        {
            PublicProperty = "expected_get"
        };

        Assert.That(_testing.GetValue(obj), Is.EqualTo("expected_get"));
    }

    [Test]
    public void Routine_PropertyInfo_can_get_static_value()
    {
        _testing = OOP_StaticProperty("PublicStaticProperty");

        TestClass_OOP.PublicStaticProperty = "expected_get";

        Assert.That(_testing.GetStaticValue(), Is.EqualTo("expected_get"));
    }

    [Test]
    public void Routine_PropertyInfo_can_set_value()
    {
        var obj = new TestClass_OOP();

        _testing.SetValue(obj, "expected_set");

        Assert.That(obj.PublicProperty, Is.EqualTo("expected_set"));
    }

    [Test]
    public void Routine_PropertyInfo_can_set_static_value()
    {
        _testing = OOP_StaticProperty("PublicStaticProperty");

        _testing.SetStaticValue("expected_set");

        Assert.That(TestClass_OOP.PublicStaticProperty, Is.EqualTo("expected_set"));
    }

    [Test]
    public void Routine_PropertyInfo_lists_custom_attributes_with_inherit_behaviour()
    {
        _testing = Attribute_Property("Class");

        var actual = _testing.GetCustomAttributes();

        Assert.That(actual.Length, Is.EqualTo(1));
        Assert.That(actual[0], Is.InstanceOf<TestClassAttribute>());

        _testing = Attribute_Property("Base");

        actual = _testing.GetCustomAttributes();

        Assert.That(actual.Length, Is.EqualTo(1));
        Assert.That(actual[0], Is.InstanceOf<TestBaseAttribute>());

        _testing = Attribute_Property("Overridden");

        actual = _testing.GetCustomAttributes();

        Assert.That(actual.Length, Is.EqualTo(2));
        Assert.That(actual[0], Is.InstanceOf<TestClassAttribute>());
        Assert.That(actual[1], Is.InstanceOf<TestBaseAttribute>());

        _testing = Attribute_InterfaceProperty("Interface");

        actual = _testing.GetCustomAttributes();

        Assert.That(actual.Length, Is.EqualTo(1));
        Assert.That(actual[0], Is.InstanceOf<TestInterfaceAttribute>());
    }

    [Test]
    public void Routine_PropertyInfo_lists_return_type_custom_attributes_of_its_get_method()
    {
        _testing = Attribute_Property("Class");

        var actual = _testing.GetReturnTypeCustomAttributes();

        Assert.That(actual.Length, Is.EqualTo(1));
        Assert.That(actual[0], Is.InstanceOf<TestClassAttribute>());

        _testing = Attribute_Property("WriteOnly");

        actual = _testing.GetReturnTypeCustomAttributes();

        Assert.That(actual.Length, Is.EqualTo(0));
    }

    [Test]
    public void When_exception_occurs_during_invocation__preloaded_and_reflected_implementations_behave_the_same()
    {
        var preloaded = type.of<TestClass_OOP>().GetProperty("ExceptionProperty");
        var reflected = type.of<TestOuterDomainType_OOP>().GetProperty("ExceptionProperty");

        Assert.That(preloaded, Is.InstanceOf<PreloadedPropertyInfo>());
        Assert.That(reflected, Is.InstanceOf<ReflectedPropertyInfo>());

        var expectedException = new Exception("expected");

        var testSubject1 = new TestClass_OOP
        {
            Exception = expectedException
        };

        Assert.That(() => preloaded.GetValue(testSubject1),
            Throws.Exception.SameAs(expectedException)
        );
        Assert.That(() => preloaded.SetValue(testSubject1, string.Empty),
            Throws.Exception.SameAs(expectedException)
        );

        var testSubject2 = new TestOuterDomainType_OOP
        {
            Exception = expectedException
        };

        Assert.That(() => reflected.GetValue(testSubject2),
            Throws.Exception.SameAs(expectedException)
        );
        Assert.That(() => reflected.SetValue(testSubject2, string.Empty),
            Throws.Exception.SameAs(expectedException)
        );
    }

    [Test]
    public void Extension_IsPubliclyReadable()
    {
        Assert.That(Members_Property("PublicReadOnly").IsPubliclyReadable, Is.True);
        Assert.That(Members_Property("PublicWriteOnly").IsPubliclyReadable, Is.False);
        Assert.That(Members_Property("PrivateGet").IsPubliclyReadable, Is.False);
    }

    [Test]
    public void Extension_IsPubliclyWritable()
    {
        Assert.That(Members_Property("PublicReadOnly").IsPubliclyWritable, Is.False);
        Assert.That(Members_Property("PublicWriteOnly").IsPubliclyWritable, Is.True);
        Assert.That(Members_Property("PrivateSet").IsPubliclyWritable, Is.False);
    }

    [Test]
    public void Extension_IsInherited()
    {
        Assert.That(OOP_Property("Public").IsInherited(false, false), Is.False);
        Assert.That(OOP_Property("Overridden").IsInherited(false, false), Is.False);
        Assert.That(OOP_Property("ImplicitInterface").IsInherited(false, false), Is.False);
        Assert.That(OOP_Property("NotOverridden").IsInherited(false, false), Is.True);

        Assert.That(OOP_Property("Public").IsInherited(false, true), Is.False);
        Assert.That(OOP_Property("Overridden").IsInherited(false, true), Is.True);
        Assert.That(OOP_Property("ImplicitInterface").IsInherited(false, true), Is.True);
        Assert.That(OOP_Property("NotOverridden").IsInherited(false, true), Is.True);

        Assert.That(OOP_Property("Public").IsInherited(true, false), Is.False);
        Assert.That(OOP_Property("Overridden").IsInherited(true, false), Is.False);
        Assert.That(OOP_Property("OtherNamespace").IsInherited(true, false), Is.False);

        Assert.That(OOP_Property("Public").IsInherited(true, true), Is.False);
        Assert.That(OOP_Property("Overridden").IsInherited(true, true), Is.False);
        Assert.That(OOP_Property("OtherNamespace").IsInherited(true, true), Is.True);
    }

    [Test]
    public void Extension_IsIndexer()
    {
        Assert.That(OOP_Property("Public").IsIndexer, Is.False);
        Assert.That(OOP_Property("Item").IsIndexer, Is.True);
    }

    [Test]
    public void Extension_Returns()
    {
        Assert.That(Members_Property("String").Returns(type.of<object>()), Is.True);
        Assert.That(Members_Property("Int").Returns(type.of<string>()), Is.False);

        Assert.That(Members_Property("StringList").ReturnsCollection(), Is.True);
        Assert.That(Members_Property("StringList").ReturnsCollection(type.of<object>()), Is.True);
        Assert.That(Members_Property("NonGenericList").ReturnsCollection(type.of<string>()), Is.False);

        //generics
        Assert.That(Members_Property("String").Returns<string>(), Is.True);
        Assert.That(Members_Property("StringList").ReturnsCollection<string>(), Is.True);

        //with name parameter
        Assert.That(Members_Property("String").Returns(type.of<string>(), "Wrong.*"), Is.False);
        Assert.That(Members_Property("StringList").ReturnsCollection(type.of<string>(), "Wrong.*"), Is.False);
    }

    [Test]
    public void Extension_Has()
    {
        Assert.That(Attribute_Property("Class").Has<TestClassAttribute>(), Is.True);
        Assert.That(Attribute_Property("Class").Has(type.of<TestClassAttribute>()), Is.True);
    }
}
