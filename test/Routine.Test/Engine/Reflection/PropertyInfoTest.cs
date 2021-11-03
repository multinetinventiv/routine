using System;
using System.Linq;
using NUnit.Framework;
using Routine.Engine.Reflection;
using Routine.Test.Engine.Reflection.Domain;
using RoutineTest.OuterDomainNamespace;
using System.Diagnostics.CodeAnalysis;

namespace Routine.Test.Engine.Reflection
{
    [TestFixture]
    public class PropertyInfoTest : ReflectionTestBase
    {
        private System.Reflection.PropertyInfo propertyInfo;
        private PropertyInfo testing;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            propertyInfo = typeof(TestClass_OOP).GetProperty("PublicProperty");
            testing = type.of<TestClass_OOP>().GetProperty("PublicProperty");
        }

        [Test]
        public void System_PropertyInfo_is_wrapped_by_Routine_PropertyInfo()
        {
            Assert.AreEqual(propertyInfo.Name, testing.Name);
            Assert.AreEqual(propertyInfo.GetGetMethod()?.Name, testing.GetGetMethod().Name);
            Assert.AreEqual(propertyInfo.GetSetMethod()?.Name, testing.GetSetMethod().Name);
            Assert.AreSame(propertyInfo.DeclaringType, testing.DeclaringType.GetActualType());
            Assert.AreSame(propertyInfo.ReflectedType, testing.ReflectedType.GetActualType());
            Assert.AreSame(propertyInfo.PropertyType, testing.PropertyType.GetActualType());
        }

        [Test, SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public void System_PropertyInfo_GetIndexParameters_is_wrapped_by_Routine_PropertyInfo()
        {
            propertyInfo = typeof(TestClass_OOP).GetProperty("Item");
            testing = type.of<TestClass_OOP>().GetProperty("Item");

            var expected = propertyInfo.GetIndexParameters();
            var actual = testing.GetIndexParameters();

            foreach (var parameter in actual)
            {
                Assert.IsTrue(expected.Any(p => p.ParameterType == parameter.ParameterType.GetActualType()), $"{parameter.Name} was not expected in index parameters of {propertyInfo}");
            }

            foreach (var parameter in expected)
            {
                Assert.IsTrue(actual.Any(p => p.ParameterType.GetActualType() == parameter.ParameterType), $"{parameter.Name} was expected in index parameters of  {propertyInfo}");
            }
        }

        [Test]
        public void Routine_PropertyInfo_caches_wrapped_properties()
        {
            Assert.AreSame(testing.Name, testing.Name);
            Assert.AreSame(testing.GetGetMethod(), testing.GetGetMethod());
            Assert.AreSame(testing.GetSetMethod(), testing.GetSetMethod());
            Assert.AreSame(testing.DeclaringType, testing.DeclaringType);
            Assert.AreSame(testing.ReflectedType, testing.ReflectedType);
            Assert.AreSame(testing.PropertyType, testing.PropertyType);
            Assert.AreSame(testing.GetIndexParameters(), testing.GetIndexParameters());
            Assert.AreSame(Attribute_Property("Class").GetCustomAttributes(), Attribute_Property("Class").GetCustomAttributes());
        }

        [Test]
        public void Routine_PropertyInfo_can_get_value()
        {
            var obj = new TestClass_OOP
            {
                PublicProperty = "expected_get"
            };

            Assert.AreEqual("expected_get", testing.GetValue(obj));
        }

        [Test]
        public void Routine_PropertyInfo_can_get_static_value()
        {
            testing = OOP_StaticProperty("PublicStaticProperty");

            TestClass_OOP.PublicStaticProperty = "expected_get";

            Assert.AreEqual("expected_get", testing.GetStaticValue());
        }

        [Test]
        public void Routine_PropertyInfo_can_set_value()
        {
            var obj = new TestClass_OOP();

            testing.SetValue(obj, "expected_set");

            Assert.AreEqual("expected_set", obj.PublicProperty);
        }

        [Test]
        public void Routine_PropertyInfo_can_set_static_value()
        {
            testing = OOP_StaticProperty("PublicStaticProperty");

            testing.SetStaticValue("expected_set");

            Assert.AreEqual("expected_set", TestClass_OOP.PublicStaticProperty);
        }

        [Test]
        public void Routine_PropertyInfo_lists_custom_attributes_with_inherit_behaviour()
        {
            testing = Attribute_Property("Class");

            var actual = testing.GetCustomAttributes();

            Assert.AreEqual(1, actual.Length);
            Assert.IsInstanceOf<TestClassAttribute>(actual[0]);

            testing = Attribute_Property("Base");

            actual = testing.GetCustomAttributes();

            Assert.AreEqual(1, actual.Length);
            Assert.IsInstanceOf<TestBaseAttribute>(actual[0]);

            testing = Attribute_Property("Overridden");

            actual = testing.GetCustomAttributes();

            Assert.AreEqual(2, actual.Length);
            Assert.IsInstanceOf<TestClassAttribute>(actual[0]);
            Assert.IsInstanceOf<TestBaseAttribute>(actual[1]);

            testing = Attribute_InterfaceProperty("Interface");

            actual = testing.GetCustomAttributes();

            Assert.AreEqual(1, actual.Length);
            Assert.IsInstanceOf<TestInterfaceAttribute>(actual[0]);
        }

        [Test]
        public void Routine_PropertyInfo_lists_return_type_custom_attributes_of_its_get_method()
        {
            testing = Attribute_Property("Class");

            var actual = testing.GetReturnTypeCustomAttributes();

            Assert.AreEqual(1, actual.Length);
            Assert.IsInstanceOf<TestClassAttribute>(actual[0]);

            testing = Attribute_Property("WriteOnly");

            actual = testing.GetReturnTypeCustomAttributes();

            Assert.AreEqual(0, actual.Length);
        }

        [Test]
        public void When_exception_occurs_during_invocation__preloaded_and_reflected_implementations_behave_the_same()
        {
            var preloaded = type.of<TestClass_OOP>().GetProperty("ExceptionProperty");
            var reflected = type.of<TestOuterDomainType_OOP>().GetProperty("ExceptionProperty");

            Assert.IsInstanceOf<PreloadedPropertyInfo>(preloaded);
            Assert.IsInstanceOf<ReflectedPropertyInfo>(reflected);

            var expectedException = new Exception("expected");

            var testSubject1 = new TestClass_OOP
            {
                Exception = expectedException
            };

            try
            {
                preloaded.GetValue(testSubject1);
                Assert.Fail("exception not thrown");
            }
            catch (Exception ex)
            {
                Assert.AreSame(expectedException, ex);
            }

            try
            {
                preloaded.SetValue(testSubject1, string.Empty);
                Assert.Fail("exception not thrown");
            }
            catch (Exception ex)
            {
                Assert.AreSame(expectedException, ex);
            }

            var testSubject2 = new TestOuterDomainType_OOP
            {
                Exception = expectedException
            };

            try
            {
                reflected.GetValue(testSubject2);
                Assert.Fail("exception not thrown");
            }
            catch (Exception ex)
            {
                Assert.AreSame(expectedException, ex);
            }

            try
            {
                reflected.SetValue(testSubject2, string.Empty);
                Assert.Fail("exception not thrown");
            }
            catch (Exception ex)
            {
                Assert.AreSame(expectedException, ex);
            }
        }

        [Test]
        public void Extension_IsPubliclyReadable()
        {
            Assert.IsTrue(Members_Property("PublicReadOnly").IsPubliclyReadable);
            Assert.IsFalse(Members_Property("PublicWriteOnly").IsPubliclyReadable);
            Assert.IsFalse(Members_Property("PrivateGet").IsPubliclyReadable);
        }

        [Test]
        public void Extension_IsPubliclyWritable()
        {
            Assert.IsFalse(Members_Property("PublicReadOnly").IsPubliclyWritable);
            Assert.IsTrue(Members_Property("PublicWriteOnly").IsPubliclyWritable);
            Assert.IsFalse(Members_Property("PrivateSet").IsPubliclyWritable);
        }

        [Test]
        public void Extension_IsInherited()
        {
            Assert.IsFalse(OOP_Property("Public").IsInherited(false, false));
            Assert.IsFalse(OOP_Property("Overridden").IsInherited(false, false));
            Assert.IsFalse(OOP_Property("ImplicitInterface").IsInherited(false, false));
            Assert.IsTrue(OOP_Property("NotOverridden").IsInherited(false, false));

            Assert.IsFalse(OOP_Property("Public").IsInherited(false, true));
            Assert.IsTrue(OOP_Property("Overridden").IsInherited(false, true));
            Assert.IsTrue(OOP_Property("ImplicitInterface").IsInherited(false, true));
            Assert.IsTrue(OOP_Property("NotOverridden").IsInherited(false, true));

            Assert.IsFalse(OOP_Property("Public").IsInherited(true, false));
            Assert.IsFalse(OOP_Property("Overridden").IsInherited(true, false));
            Assert.IsFalse(OOP_Property("OtherNamespace").IsInherited(true, false));

            Assert.IsFalse(OOP_Property("Public").IsInherited(true, true));
            Assert.IsFalse(OOP_Property("Overridden").IsInherited(true, true));
            Assert.IsTrue(OOP_Property("OtherNamespace").IsInherited(true, true));
        }

        [Test]
        public void Extension_IsIndexer()
        {
            Assert.IsFalse(OOP_Property("Public").IsIndexer);
            Assert.IsTrue(OOP_Property("Item").IsIndexer);
        }

        [Test]
        public void Extension_Returns()
        {
            Assert.IsTrue(Members_Property("String").Returns(type.of<object>()));
            Assert.IsFalse(Members_Property("Int").Returns(type.of<string>()));

            Assert.IsTrue(Members_Property("StringList").ReturnsCollection());
            Assert.IsTrue(Members_Property("StringList").ReturnsCollection(type.of<object>()));
            Assert.IsFalse(Members_Property("NonGenericList").ReturnsCollection(type.of<string>()));

            //generics
            Assert.IsTrue(Members_Property("String").Returns<string>());
            Assert.IsTrue(Members_Property("StringList").ReturnsCollection<string>());

            //with name parameter
            Assert.IsFalse(Members_Property("String").Returns(type.of<string>(), "Wrong.*"));
            Assert.IsFalse(Members_Property("StringList").ReturnsCollection(type.of<string>(), "Wrong.*"));
        }

        [Test]
        public void Extension_Has()
        {
            Assert.IsTrue(Attribute_Property("Class").Has<TestClassAttribute>());
            Assert.IsTrue(Attribute_Property("Class").Has(type.of<TestClassAttribute>()));
        }
    }
}

