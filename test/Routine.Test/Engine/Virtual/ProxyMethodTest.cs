using Routine.Engine.Virtual;
using Routine.Engine;
using Routine.Test.Core;

namespace Routine.Test.Engine.Virtual;

[TestFixture]
public class ProxyMethodTest : CoreTestBase
{
    [Test]
    public void Proxy_methods_inherit_ispublic__name__parameters__return_type_and_custom_attributes_from_given_method()
    {
        //[SecuritySafeCritical] string Insert(int startIndex, string value)
        IMethod real = type.of<string>().GetMethod("Insert");
        IMethod proxy = new ProxyMethod(type.of<string>(), real);

        Assert.That(proxy.IsPublic, Is.EqualTo(real.IsPublic));
        Assert.That(proxy.Name, Is.EqualTo(real.Name));

        Assert.That(proxy.Parameters.Count, Is.EqualTo(real.Parameters.Count));
        for (int i = 0; i < real.Parameters.Count; i++)
        {
            Assert.That(proxy.Parameters[i].Name, Is.EqualTo(real.Parameters[i].Name));
            Assert.That(proxy.Parameters[i].Index, Is.EqualTo(real.Parameters[i].Index));
            Assert.That(proxy.Parameters[i].ParameterType, Is.EqualTo(real.Parameters[i].ParameterType));
        }

        Assert.That(proxy.ReturnType, Is.EqualTo(real.ReturnType));

        Assert.That(proxy.GetCustomAttributes().Length, Is.EqualTo(real.GetCustomAttributes().Length));
        for (int i = 0; i < real.GetCustomAttributes().Length; i++)
        {
            var realAttr = real.GetCustomAttributes()[i];
            var proxyAttr = proxy.GetCustomAttributes()[i];

            Assert.That(proxyAttr, Is.EqualTo(realAttr));
        }
    }

    [Test]
    public void Proxy_method_name_can_be_altered()
    {
        //[SecuritySafeCritical] string Insert(int startIndex, string value)
        IMethod real = type.of<string>().GetMethod("Insert");
        IMethod proxy = new ProxyMethod(type.of<string>(), real).Name.Set("Overridden");

        Assert.That(proxy.Name, Is.EqualTo("Overridden"));
    }

    [Test]
    public void Parent_type_is_what_is_given_as_parent_type()
    {
        var typeMock = new Mock<IType>();
        var expected = typeMock.Object;
        IMethod real = type.of<string>().GetMethod("Insert");
        IMethod proxy = new ProxyMethod(expected, real);

        Assert.That(proxy.ParentType, Is.SameAs(expected));
    }

    [Test]
    public void Parent_type_for_parameters_is_what_is_given_as_parent_type()
    {
        var typeMock = new Mock<IType>();
        var expected = typeMock.Object;
        IMethod real = type.of<string>().GetMethod("Insert");
        IMethod proxy = new ProxyMethod(expected, real);

        foreach (var parameter in proxy.Parameters)
        {
            Assert.That(parameter.ParentType, Is.SameAs(expected));
        }
    }

    [Test]
    public void Declaring_type_is_always_the_given_parent_type()
    {
        var typeMock = new Mock<IType>();
        var expected = typeMock.Object;
        IMethod real = type.of<string>().GetMethod("Insert");
        IMethod proxy = new ProxyMethod(expected, real);

        Assert.That(proxy.GetDeclaringType(false), Is.SameAs(expected));
        Assert.That(proxy.GetDeclaringType(true), Is.SameAs(expected));
    }

    [Test]
    public void Delegates_perform_action_to_target()
    {
        //[SecuritySafeCritical] string Insert(int startIndex, string value)
        IMethod real = type.of<string>().GetMethod("Insert");
        IMethod proxy = new ProxyMethod(type.of<string>(), real);

        Assert.That(proxy.PerformOn("test", 0, "insert"), Is.EqualTo("inserttest"));
    }

    [Test]
    public void Delegates_perform_action_to_the_target_obtained_via_target_delegate()
    {
        IMethod real = type.of<string>().GetMethod("Insert");
        IMethod proxy = new ProxyMethod(type.of<char>(), real, (o, _) => o.ToString());

        Assert.That(proxy.PerformOn('t', 0, "insert"), Is.EqualTo("insertt"));
    }

    [Test]
    public void Additional_parameter_shifts_index_of_the_real_parameters()
    {
        var parameterMock = new Mock<IParameter>();
        parameterMock.Setup(o => o.Name).Returns("additional");
        parameterMock.Setup(o => o.ParameterType).Returns(type.of<string>());

        IMethod real = type.of<string>().GetMethod("Insert");
        IMethod proxy = new ProxyMethod(type.of<char>(), real, (o, _) => o.ToString(), parameterMock.Object);

        Assert.That(proxy.Parameters.Count, Is.EqualTo(real.Parameters.Count + 1));
        Assert.That(proxy.Parameters[0].Name, Is.EqualTo("additional"));
        Assert.That(proxy.Parameters[0].Index, Is.EqualTo(0));
        Assert.That(proxy.Parameters[0].ParameterType, Is.EqualTo(type.of<string>()));
        for (int i = 0; i < real.Parameters.Count; i++)
        {
            Assert.That(proxy.Parameters[i + 1].Name, Is.EqualTo(real.Parameters[i].Name));
            Assert.That(proxy.Parameters[i + 1].Index, Is.EqualTo(real.Parameters[i].Index + 1));
            Assert.That(proxy.Parameters[i + 1].ParameterType, Is.EqualTo(real.Parameters[i].ParameterType));
        }
    }

    [Test]
    public void When_performing_on_real__additional_parameters_are_skipped()
    {
        var parameterMock = new Mock<IParameter>();

        IMethod real = type.of<string>().GetMethod("Insert");
        IMethod proxy = new ProxyMethod(type.of<char>(), real, (o, _) => o.ToString(), parameterMock.Object);

        Assert.That(proxy.PerformOn('t', "dummy", 0, "insert"), Is.EqualTo("insertt"));
    }

    [Test]
    public void Target_can_be_obtained_from_an_additional_parameter()
    {
        var parameterMock = new Mock<IParameter>();

        IMethod real = type.of<string>().GetMethod("Insert");
        IMethod proxy = new ProxyMethod(type.of<char>(), real, (_, p) => p[0], parameterMock.Object);

        Assert.That(proxy.PerformOn('t', "test", 0, "insert"), Is.EqualTo("inserttest"));
    }
}
