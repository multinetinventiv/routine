using Routine.Engine.Reflection;

namespace Routine.Test.Engine.Reflection;

[TestFixture]
public class ProxyTypeInfoTest : ReflectionTestBase
{
    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
    }

    [Test]
    public void Forwards_properties_to_the_real_object()
    {
        var @void = new ProxyTypeInfo(type.ofvoid());
        var @int = new ProxyTypeInfo(type.of<int>());
        var dayOfWeek = new ProxyTypeInfo(type.of<DayOfWeek>());
        var @string = new ProxyTypeInfo(type.of<string>());
        var attribute = new ProxyTypeInfo(type.of<Attribute>());
        var iList = new ProxyTypeInfo(type.of<IList>());
        var listString = new ProxyTypeInfo(type.of<List<string>>());
        var stringArray = new ProxyTypeInfo(type.of<string[]>());

        Assert.That(@string.GetActualType(), Is.EqualTo(typeof(string)));

        Assert.That(@string.IsPublic, Is.True);

        Assert.That(@string.IsAbstract, Is.False);
        Assert.That(attribute.IsAbstract, Is.True);

        Assert.That(@string.IsInterface, Is.False);
        Assert.That(attribute.IsInterface, Is.False);
        Assert.That(iList.IsInterface, Is.True);

        Assert.That(@string.IsValueType, Is.False);
        Assert.That(@int.IsValueType, Is.True);

        Assert.That(@string.IsGenericType, Is.False);
        Assert.That(listString.IsGenericType, Is.True);

        Assert.That(@string.IsPrimitive, Is.False);
        Assert.That(@int.IsPrimitive, Is.True);

        Assert.That(@void.IsVoid, Is.True);
        Assert.That(@string.IsVoid, Is.False);

        Assert.That(@string.IsEnum, Is.False);
        Assert.That(dayOfWeek.IsEnum, Is.True);

        Assert.That(@string.IsArray, Is.False);
        Assert.That(stringArray.IsArray, Is.True);

        Assert.That(@string.Name, Is.EqualTo(typeof(string).Name));
        Assert.That(@string.FullName, Is.EqualTo(typeof(string).FullName));
        Assert.That(@string.Namespace, Is.EqualTo(typeof(string).Namespace));
        Assert.That(@string.BaseType.GetActualType(), Is.EqualTo(typeof(object)));
    }

    [Test]
    public void Forwards_methods_to_the_real_object()
    {
        CallOnProxyAndVerifyOnMock(ti => ti.GetAllConstructors());
        CallOnProxyAndVerifyOnMock(ti => ti.GetAllProperties());
        CallOnProxyAndVerifyOnMock(ti => ti.GetAllStaticProperties());
        CallOnProxyAndVerifyOnMock(ti => ti.GetAllMethods());
        CallOnProxyAndVerifyOnMock(ti => ti.GetAllStaticMethods());
        CallOnProxyAndVerifyOnMock(ti => ti.GetCustomAttributes());
        CallOnProxyAndVerifyOnMock(ti => ti.GetGenericArguments());
        CallOnProxyAndVerifyOnMock(ti => ti.GetElementType());
        CallOnProxyAndVerifyOnMock(ti => ti.GetInterfaces());
        CallOnProxyAndVerifyOnMock(ti => ti.CanBe(It.IsAny<TypeInfo>()));
        CallOnProxyAndVerifyOnMock(ti => ti.GetEnumNames());
        CallOnProxyAndVerifyOnMock(ti => ti.GetEnumValues());
        CallOnProxyAndVerifyOnMock(ti => ti.GetEnumUnderlyingType());
        CallOnProxyAndVerifyOnMock(ti => ti.GetParseMethod());
        CallOnProxyAndVerifyOnMock(ti => ti.Load());
        CallOnProxyAndVerifyOnMock(ti => ti.CreateInstance());
        CallOnProxyAndVerifyOnMock(ti => ti.CreateListInstance(It.IsAny<int>()));
        CallOnProxyAndVerifyOnMock(ti => ti.GetAssignableTypes());
        CallOnProxyAndVerifyOnMock(ti => ti.GetPublicConstructors());
        CallOnProxyAndVerifyOnMock(ti => ti.GetConstructor(It.IsAny<TypeInfo[]>()));
        CallOnProxyAndVerifyOnMock(ti => ti.GetPublicProperties(It.IsAny<bool>()));
        CallOnProxyAndVerifyOnMock(ti => ti.GetPublicStaticProperties(It.IsAny<bool>()));
        CallOnProxyAndVerifyOnMock(ti => ti.GetProperty(It.IsAny<string>()));
        CallOnProxyAndVerifyOnMock(ti => ti.GetProperties(It.IsAny<string>()));
        CallOnProxyAndVerifyOnMock(ti => ti.GetStaticProperty(It.IsAny<string>()));
        CallOnProxyAndVerifyOnMock(ti => ti.GetStaticProperties(It.IsAny<string>()));
        CallOnProxyAndVerifyOnMock(ti => ti.GetPublicMethods());
        CallOnProxyAndVerifyOnMock(ti => ti.GetPublicStaticMethods());
        CallOnProxyAndVerifyOnMock(ti => ti.GetMethod(It.IsAny<string>()));
        CallOnProxyAndVerifyOnMock(ti => ti.GetMethods(It.IsAny<string>()));
        CallOnProxyAndVerifyOnMock(ti => ti.GetStaticMethod(It.IsAny<string>()));
        CallOnProxyAndVerifyOnMock(ti => ti.GetStaticMethods(It.IsAny<string>()));
    }

    [Test]
    public void Real_object_can_be_changed_later()
    {
        var testing = new ProxyTypeInfo(type.of<string>());

        testing.Real = type.of<int>();

        Assert.That(testing.GetActualType(), Is.EqualTo(typeof(int)));
    }
}
