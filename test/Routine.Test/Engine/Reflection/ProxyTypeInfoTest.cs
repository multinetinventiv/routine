using System.Linq.Expressions;
using Routine.Engine.Reflection;

namespace Routine.Test.Engine.Reflection;

[TestFixture]
public class ProxyTypeInfoTest
{
    #region Helpers

    private void TestProxy(Expression<Action<TypeInfo>> expression)
    {
        var mock = new Mock<TypeInfo>();
        var testing = new ProxyTypeInfo(mock.Object);

        expression.Compile()(testing);
        mock.Verify(expression);
    }

    private void TestProxy<T>(Expression<Func<TypeInfo, T>> expression)
    {
        var mock = new Mock<TypeInfo>();
        var testing = new ProxyTypeInfo(mock.Object);

        expression.Compile()(testing);
        mock.Verify(expression);
    }

    #endregion

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

        Assert.That(@string.Name, Is.EqualTo("String"));
        Assert.That(@string.FullName, Is.EqualTo("System.String"));
        Assert.That(@string.Namespace, Is.EqualTo("System"));
        Assert.That(@string.BaseType.GetActualType(), Is.EqualTo(typeof(object)));
    }

    [Test]
    public void Forwards_methods_to_the_real_object()
    {
        TestProxy(ti => ti.GetAllConstructors());
        TestProxy(ti => ti.GetAllProperties());
        TestProxy(ti => ti.GetAllStaticProperties());
        TestProxy(ti => ti.GetAllMethods());
        TestProxy(ti => ti.GetAllStaticMethods());
        TestProxy(ti => ti.GetCustomAttributes());
        TestProxy(ti => ti.GetGenericArguments());
        TestProxy(ti => ti.GetElementType());
        TestProxy(ti => ti.GetInterfaces());
        TestProxy(ti => ti.CanBe(It.IsAny<TypeInfo>()));
        TestProxy(ti => ti.GetEnumNames());
        TestProxy(ti => ti.GetEnumValues());
        TestProxy(ti => ti.GetEnumUnderlyingType());
        TestProxy(ti => ti.GetParseMethod());
        TestProxy(ti => ti.Load());
        TestProxy(ti => ti.CreateInstance());
        TestProxy(ti => ti.CreateListInstance(It.IsAny<int>()));
        TestProxy(ti => ti.GetAssignableTypes());
        TestProxy(ti => ti.GetPublicConstructors());
        TestProxy(ti => ti.GetConstructor(It.IsAny<TypeInfo[]>()));
        TestProxy(ti => ti.GetPublicProperties(It.IsAny<bool>()));
        TestProxy(ti => ti.GetPublicStaticProperties(It.IsAny<bool>()));
        TestProxy(ti => ti.GetProperty(It.IsAny<string>()));
        TestProxy(ti => ti.GetProperties(It.IsAny<string>()));
        TestProxy(ti => ti.GetStaticProperty(It.IsAny<string>()));
        TestProxy(ti => ti.GetStaticProperties(It.IsAny<string>()));
        TestProxy(ti => ti.GetPublicMethods());
        TestProxy(ti => ti.GetPublicStaticMethods());
        TestProxy(ti => ti.GetMethod(It.IsAny<string>()));
        TestProxy(ti => ti.GetMethods(It.IsAny<string>()));
        TestProxy(ti => ti.GetStaticMethod(It.IsAny<string>()));
        TestProxy(ti => ti.GetStaticMethods(It.IsAny<string>()));
    }

    [Test]
    public void Real_object_can_be_changed_later()
    {
        var testing = new ProxyTypeInfo(type.of<string>());

        testing.SetReal(type.of<int>());

        Assert.That(testing.GetActualType(), Is.EqualTo(typeof(int)));
    }
}
