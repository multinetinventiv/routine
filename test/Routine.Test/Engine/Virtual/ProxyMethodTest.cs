using Moq;
using NUnit.Framework;
using Routine.Engine;
using Routine.Engine.Virtual;
using Routine.Test.Core;

namespace Routine.Test.Engine.Virtual
{
    [TestFixture]
    public class ProxyMethodTest : CoreTestBase
    {
        [Test]
        public void Proxy_methods_inherit_ispublic__name__parameters__return_type_and_custom_attributes_from_given_method()
        {
            //[SecuritySafeCritical] string Insert(int startIndex, string value)
            IMethod real = type.of<string>().GetMethod("Insert");
            IMethod proxy = new ProxyMethod(type.of<string>(), real);

            Assert.AreEqual(real.IsPublic, proxy.IsPublic);
            Assert.AreEqual(real.Name, proxy.Name);

            Assert.AreEqual(real.Parameters.Count, proxy.Parameters.Count);
            for (int i = 0; i < real.Parameters.Count; i++)
            {
                Assert.AreEqual(real.Parameters[i].Name, proxy.Parameters[i].Name);
                Assert.AreEqual(real.Parameters[i].Index, proxy.Parameters[i].Index);
                Assert.AreEqual(real.Parameters[i].ParameterType, proxy.Parameters[i].ParameterType);
            }

            Assert.AreEqual(real.ReturnType, proxy.ReturnType);

            Assert.AreEqual(real.GetCustomAttributes().Length, proxy.GetCustomAttributes().Length);
            for (int i = 0; i < real.GetCustomAttributes().Length; i++)
            {
                var realAttr = real.GetCustomAttributes()[i];
                var proxyAttr = proxy.GetCustomAttributes()[i];

                Assert.AreEqual(realAttr, proxyAttr);
            }
        }

        [Test]
        public void Proxy_method_name_can_be_altered()
        {
            //[SecuritySafeCritical] string Insert(int startIndex, string value)
            IMethod real = type.of<string>().GetMethod("Insert");
            IMethod proxy = new ProxyMethod(type.of<string>(), real).Name.Set("Overridden");

            Assert.AreEqual("Overridden", proxy.Name);
        }

        [Test]
        public void Parent_type_is_what_is_given_as_parent_type()
        {
            var typeMock = new Mock<IType>();
            var expected = typeMock.Object;
            IMethod real = type.of<string>().GetMethod("Insert");
            IMethod proxy = new ProxyMethod(expected, real);

            Assert.AreSame(expected, proxy.ParentType);
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
                Assert.AreSame(expected, parameter.ParentType);
            }
        }

        [Test]
        public void Declaring_type_is_always_the_given_parent_type()
        {
            var typeMock = new Mock<IType>();
            var expected = typeMock.Object;
            IMethod real = type.of<string>().GetMethod("Insert");
            IMethod proxy = new ProxyMethod(expected, real);

            Assert.AreSame(expected, proxy.GetDeclaringType(false));
            Assert.AreSame(expected, proxy.GetDeclaringType(true));
        }

        [Test]
        public void Delegates_perform_action_to_target()
        {
            //[SecuritySafeCritical] string Insert(int startIndex, string value)
            IMethod real = type.of<string>().GetMethod("Insert");
            IMethod proxy = new ProxyMethod(type.of<string>(), real);

            Assert.AreEqual("inserttest", proxy.PerformOn("test", 0, "insert"));
        }

        [Test]
        public void Delegates_perform_action_to_the_target_obtained_via_target_delegate()
        {
            IMethod real = type.of<string>().GetMethod("Insert");
            IMethod proxy = new ProxyMethod(type.of<char>(), real, (o, _) => o.ToString());

            Assert.AreEqual("insertt", proxy.PerformOn('t', 0, "insert"));
        }

        [Test]
        public void Additional_parameter_shifts_index_of_the_real_parameters()
        {
            var parameterMock = new Mock<IParameter>();
            parameterMock.Setup(o => o.Name).Returns("additional");
            parameterMock.Setup(o => o.ParameterType).Returns(type.of<string>());

            IMethod real = type.of<string>().GetMethod("Insert");
            IMethod proxy = new ProxyMethod(type.of<char>(), real, (o, _) => o.ToString(), parameterMock.Object);

            Assert.AreEqual(real.Parameters.Count + 1, proxy.Parameters.Count);
            Assert.AreEqual("additional", proxy.Parameters[0].Name);
            Assert.AreEqual(0, proxy.Parameters[0].Index);
            Assert.AreEqual(type.of<string>(), proxy.Parameters[0].ParameterType);
            for (int i = 0; i < real.Parameters.Count; i++)
            {
                Assert.AreEqual(real.Parameters[i].Name, proxy.Parameters[i + 1].Name);
                Assert.AreEqual(real.Parameters[i].Index + 1, proxy.Parameters[i + 1].Index);
                Assert.AreEqual(real.Parameters[i].ParameterType, proxy.Parameters[i + 1].ParameterType);
            }
        }

        [Test]
        public void When_performing_on_real__additional_parameters_are_skipped()
        {
            var parameterMock = new Mock<IParameter>();

            IMethod real = type.of<string>().GetMethod("Insert");
            IMethod proxy = new ProxyMethod(type.of<char>(), real, (o, _) => o.ToString(), parameterMock.Object);

            Assert.AreEqual("insertt", proxy.PerformOn('t', "dummy", 0, "insert"));
        }

        [Test]
        public void Target_can_be_obtained_from_an_additional_parameter()
        {
            var parameterMock = new Mock<IParameter>();

            IMethod real = type.of<string>().GetMethod("Insert");
            IMethod proxy = new ProxyMethod(type.of<char>(), real, (_, p) => p[0], parameterMock.Object);

            Assert.AreEqual("inserttest", proxy.PerformOn('t', "test", 0, "insert"));
        }
    }
}