using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Routine.Engine;
using Routine.Engine.Reflection;
using Routine.Test.Engine.Reflection.Domain;
using RoutineTest.OuterDomainNamespace;
using RoutineTest.OuterNamespace;
using System.Diagnostics.CodeAnalysis;

namespace Routine.Test.Engine.Reflection
{
    [TestFixture]
    public class TypeInfoTest : ReflectionTestBase
    {
        private Type systemType;
        private TypeInfo testing;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            systemType = typeof(TestClass_OOP);
            testing = TypeInfo.Get(systemType);
        }

        [Test]
        public void Type_is_wrapped_by_TypeInfo()
        {
            Assert.AreEqual(systemType.Name, testing.Name);
            Assert.AreEqual(systemType.FullName, testing.FullName);
            Assert.AreEqual(systemType.Namespace, testing.Namespace);
            Assert.AreEqual(systemType.BaseType, testing.BaseType.GetActualType());
            Assert.AreEqual(systemType.IsAbstract, testing.IsAbstract);
            Assert.AreEqual(systemType.IsInterface, testing.IsInterface);
            Assert.AreEqual(systemType.IsValueType, testing.IsValueType);
            Assert.AreEqual(systemType.IsPrimitive, testing.IsPrimitive);
            Assert.AreEqual(systemType.IsGenericType, testing.IsGenericType);
            Assert.AreEqual(systemType.IsArray, testing.IsArray);
            Assert.AreEqual(systemType.IsEnum, testing.IsEnum);
            Assert.AreEqual(systemType.IsPublic, testing.IsPublic);

            Assert.IsNotNull(testing.GetConstructor(type.of<string>()));
            Assert.AreEqual(systemType.GetProperty("PublicProperty")?.Name, testing.GetProperty("PublicProperty").Name);
            Assert.AreEqual(systemType.GetMethod("ToString")?.Name, testing.GetMethod("ToString").Name);
        }

        [Test]
        public void When_a_previously_cached_type_is_being_optimized__previously_created_reflected_type_info_instance_is_invalidated()
        {
            Assert.IsInstanceOf<ReflectedTypeInfo>(TypeInfo.Get<TestOuterDomainType_OOP>());

            TypeInfo.Optimize(typeof(TestOuterDomainType_OOP));

            Assert.IsInstanceOf<OptimizedTypeInfo>(TypeInfo.Get<TestOuterDomainType_OOP>());
        }

        [Test]
        public void When_an_optimized_type_has_a_currently_non_optimized_type_reference__that_type_reference_should_be_invalidated_after_optimizing_it()
        {
            TypeInfo.Optimize(typeof(TestOuterDomainType_OOP));

            var property = TypeInfo.Get<TestOuterDomainType_OOP>().GetAllProperties().First(p => p.Returns<TestOuterLaterAddedDomainType_OOP>());

            Assert.IsInstanceOf<ReflectedTypeInfo>(property.PropertyType);

            TypeInfo.Optimize(typeof(TestOuterLaterAddedDomainType_OOP));

            property = TypeInfo.Get<TestOuterDomainType_OOP>().GetAllProperties().First(p => p.Returns<TestOuterLaterAddedDomainType_OOP>());

            Assert.IsInstanceOf<OptimizedTypeInfo>(property.PropertyType);
        }

        [Test]
        public void TypeInfo_has_the_same_behaviour_on_ToString__Equals_and_GetHashCode_methods()
        {
            Assert.AreEqual(typeof(string).ToString(), TypeInfo.Get(typeof(string)).ToString());
            Assert.IsTrue(TypeInfo.Get(typeof(string)).Equals(typeof(string)));
            Assert.AreEqual(typeof(string).GetHashCode(), TypeInfo.Get(typeof(string)).GetHashCode());
        }

        [Test]
        public void Type_GetConstructors_is_wrapped_by_TypeInfo()
        {
            var actual = type.of<TestClass_OOP>().GetAllConstructors().ToList();

            Assert.IsTrue(actual.Any(c => c.HasNoParameters()));
            Assert.IsTrue(actual.Any(c => c.HasParameters<string>()));
            Assert.IsTrue(actual.Any(c => c.HasParameters<int>()));
        }

        [Test]
        public void Type_GetProperties_is_wrapped_by_TypeInfo()
        {
            var actual = type.of<TestClass_OOP>().GetAllProperties().ToList();

            Assert.IsTrue(actual.Any(p => p.Name == "PublicProperty"));
            Assert.IsTrue(actual.Any(p => p.Name == "PrivateProperty"));
            Assert.IsTrue(actual.Any(p => p.Name == "ImplicitInterfaceProperty"));
            Assert.IsTrue(actual.All(p => p.Name != "ExplicitInterfaceProperty"));
            Assert.IsTrue(actual.All(p => p.Name != "PublicStaticProperty"));
            Assert.IsTrue(actual.All(p => p.Name != "PrivateStaticProperty"));

            actual = type.of<TestClass_OOP>().GetAllStaticProperties().ToList();

            Assert.IsTrue(actual.All(p => p.Name != "PublicProperty"));
            Assert.IsTrue(actual.All(p => p.Name != "PrivateProperty"));
            Assert.IsTrue(actual.All(p => p.Name != "ImplicitInterfaceProperty"));
            Assert.IsTrue(actual.All(p => p.Name != "ExplicitInterfaceProperty"));
            Assert.IsTrue(actual.Any(p => p.Name == "PublicStaticProperty"));
            Assert.IsTrue(actual.Any(p => p.Name == "PrivateStaticProperty"));
        }

        [Test]
        public void Type_GetMethods_is_wrapped_by_TypeInfo()
        {
            var actual = type.of<TestClass_OOP>().GetAllMethods().ToList();

            Assert.IsTrue(actual.Any(m => m.Name == "PublicMethod"));
            Assert.IsTrue(actual.Any(m => m.Name == "PrivateMethod"));
            Assert.IsTrue(actual.Any(m => m.Name == "ImplicitInterfaceMethod"));
            Assert.IsTrue(actual.All(m => m.Name != "ExplicitInterfaceMethod"));
            Assert.IsTrue(actual.All(m => m.Name != "PublicStaticMethod"));
            Assert.IsTrue(actual.All(m => m.Name != "PrivateStaticMethod"));


            actual = type.of<TestClass_OOP>().GetAllStaticMethods().ToList();

            Assert.IsTrue(actual.All(m => m.Name != "PublicMethod"));
            Assert.IsTrue(actual.All(m => m.Name != "PrivateMethod"));
            Assert.IsTrue(actual.All(m => m.Name != "ImplicitInterfaceMethod"));
            Assert.IsTrue(actual.All(m => m.Name != "ExplicitInterfaceMethod"));
            Assert.IsTrue(actual.Any(m => m.Name == "PublicStaticMethod"));
            Assert.IsTrue(actual.Any(m => m.Name == "PrivateStaticMethod"));
        }

        [Test]
        public void TypeInfo_caches_wrapped_properties()
        {
            Assert.AreSame(testing.Name, testing.Name);
            Assert.AreSame(testing.FullName, testing.FullName);
            Assert.AreSame(testing.Namespace, testing.Namespace);
            Assert.AreSame(testing.BaseType, testing.BaseType);
            Assert.AreSame(testing.GetAllConstructors(), testing.GetAllConstructors());
            Assert.AreSame(testing.GetAllProperties(), testing.GetAllProperties());
            Assert.AreSame(testing.GetAllMethods(), testing.GetAllMethods());
            Assert.AreSame(testing.GetAllStaticProperties(), testing.GetAllStaticProperties());
            Assert.AreSame(testing.GetAllStaticMethods(), testing.GetAllStaticMethods());
            Assert.AreSame(TypeInfo.Get(typeof(TestClass_Attribute)).GetCustomAttributes(), TypeInfo.Get(typeof(TestClass_Attribute)).GetCustomAttributes());
        }

        [Test]
        public void TypeInfo_has_one_instance_for_each_Type()
        {
            Assert.AreSame(testing, TypeInfo.Get(typeof(TestClass_OOP)));
        }

        [Test]
        public void TypeInfo_has_equals_operator_for_IType()
        {
            IType type = testing;
            Assert.IsTrue(type == testing);
            Assert.IsFalse(type != testing);
            Assert.IsTrue(testing == type);
            Assert.IsFalse(testing != type);
        }

        [Test]
        public void TypeInfo_creates_instance_using_default_constructor()
        {
            var actual = (int)TypeInfo.Get(typeof(int)).CreateInstance();

            Assert.AreEqual(0, actual);

            var actualObj = TypeInfo.Get(typeof(TestClass_OOP)).CreateInstance();

            Assert.IsInstanceOf<TestClass_OOP>(actualObj);
        }

        [Test]
        public void TypeInfo_creates_instance_as_an_empty_list_or_array_when_type_is_list_or_array()
        {
            var list = (List<int>)TypeInfo.Get(typeof(List<int>)).CreateInstance();

            Assert.AreEqual(0, list.Count);

            var array = (int[])TypeInfo.Get(typeof(int[])).CreateInstance();

            Assert.AreEqual(0, array.Length);
        }

        [Test]
        public void TypeInfo_creates_list_instance_using_array_initializer_when_type_is_array()
        {
            var array = (int[])TypeInfo.Get(typeof(int[])).CreateListInstance(5);

            Assert.AreEqual(5, array.Length);
        }

        [Test]
        public void TypeInfo_creates_list_instance_using_capacity_constructor_when_type_is_list()
        {
            var list = (List<int>)TypeInfo.Get(typeof(List<int>)).CreateListInstance(5);

            Assert.AreEqual(0, list.Count);
            Assert.AreEqual(5, list.Capacity);
        }

        [Test]
        public void TypeInfo_throws_exception_on_an_attempt_to_create_list_instance_on_a_non_list_type_()
        {
            Assert.Throws<MissingMethodException>(() => TypeInfo.Get(typeof(int)).CreateListInstance(5));
        }

        [Test]
        public void When_creating_instance_TypeInfo_throws_MissingMethodException_when_object_does_not_have_default_constructor()
        {
            try
            {
                TypeInfo.Get(typeof(TestClassWithoutDefaultConstructor_OOP)).CreateInstance();
                Assert.Fail("exception not thrown");
            }
            catch (MissingMethodException) { }
        }

        [Test, SuppressMessage("ReSharper", "LocalVariableHidesMember")]
        public void TypeInfo_lists_custom_attributes_with_inherit_behaviour()
        {
            var systemType = typeof(TestClass_Attribute);
            var testing = TypeInfo.Get(systemType);

            var actual = testing.GetCustomAttributes();
            Assert.AreEqual(2, actual.Length);
            Assert.IsInstanceOf<TestClassAttribute>(actual[0]);
            Assert.IsInstanceOf<TestBaseAttribute>(actual[1]);

            systemType = typeof(TestInterface_Attribute);
            testing = TypeInfo.Get(systemType);

            actual = testing.GetCustomAttributes();

            Assert.AreEqual(1, actual.Length);
            Assert.IsInstanceOf<TestInterfaceAttribute>(actual[0]);
        }

        [Test]
        public void TypeInfo_can_ignore_proxy_classes()
        {
            Assert.AreEqual(TypeInfo.Get(typeof(TestClass_OOP)), TypeInfo.Get(typeof(TestProxyClass_OOP)),
                TypeInfo.Get(typeof(TestClass_OOP)) + "(" + TypeInfo.Get(typeof(TestClass_OOP)).GetType() + "), " +
                TypeInfo.Get(typeof(TestProxyClass_OOP)) + "(" + TypeInfo.Get(typeof(TestProxyClass_OOP)).GetType() + "), ");
        }

        [Test]
        public void TypeInfo_lists_assignable_types()
        {
            IType testClassOop = type.of<TestClass_OOP>();

            var actual = testClassOop.AssignableTypes;

            Assert.IsTrue(actual.Any(t => Equals(t, type.of<TestAbstractClass_OOP>())), "TestClass_OOP is assignable to TestAbstractClass_OOP, but it is not found in assignable types");
            Assert.IsTrue(actual.Any(t => Equals(t, type.of<object>())), "TestClass_OOP is assignable to object, but it is not found in assignable types");

            Assert.IsTrue(actual.Any(t => Equals(t, type.of<TestInterface_OOP>())), "TestClass_OOP is assignable to TestInterface_OOP, but it is not found in assignable types");
            Assert.IsTrue(actual.Any(t => Equals(t, type.of<TestBaseInterface_OOP>())), "TestClass_OOP is assignable to TestBaseInterface_OOP, but it is not found in assignable types");

            Assert.IsTrue(actual.Any(t => Equals(t, type.of<TestOuterInterface_OOP>())), "TestClass_OOP is assignable to TestOuterInterface_OOP, but it is not found in assignable types");
        }

        [Test]
        public void Facade_type_of()
        {
            Assert.AreSame(TypeInfo.Get<string>(), type.of<string>());
        }

        [Test]
        public void Facade_Void()
        {
            Assert.AreSame(TypeInfo.Get(typeof(void)), TypeInfo.Void());
        }

        [Test]
        public void Facade_GetGeneric()
        {
            Assert.AreSame(TypeInfo.Get(typeof(string)), TypeInfo.Get<string>());
        }

        [Test]
        public void Facade_GetTypeInfo()
        {
            Assert.AreSame(TypeInfo.Get(typeof(string)), "".GetTypeInfo());
        }

        [Test]
        public void Extension_IsVoid()
        {
            Assert.IsTrue(type.ofvoid().IsVoid);
            Assert.IsFalse(type.of<string>().IsVoid);
        }

        [Test]
        public void Parseable_types_are_represented_by_ParseableTypeInfo_even_if_they_are_added_for_optimization()
        {
            Assert.IsFalse(type.of<int>() is OptimizedTypeInfo);
            Assert.IsFalse(type.of<string>() is OptimizedTypeInfo);
            Assert.IsFalse(type.of<DateTime>() is OptimizedTypeInfo);

            Assert.IsTrue(type.of<TestClass_OOP>() is OptimizedTypeInfo);
        }

        [Test]
        public void Extension_GetPublicConstructors()
        {
            var actual = type.of<TestClass_OOP>().GetPublicConstructors().ToList();

            Assert.IsTrue(actual.Any(c => c.HasNoParameters()));
            Assert.IsTrue(actual.Any(c => c.HasParameters<string>()));
            Assert.IsFalse(actual.Any(c => c.HasParameters<int>()));
        }

        [Test]
        public void Extension_GetPublicProperties()
        {
            var actual = type.of<TestClass_OOP>().GetPublicProperties().ToList();

            Assert.IsTrue(actual.Any(p => p.Name == "PublicProperty"));
            Assert.IsTrue(actual.Any(p => p.Name == "PublicGetPrivateSetProperty"));
            Assert.IsTrue(actual.All(p => p.Name != "PrivateProperty"));
            Assert.IsTrue(actual.All(p => p.Name != "PublicStaticProperty"));

            actual = type.of<TestClass_OOP>().GetPublicProperties(true).ToList();

            Assert.IsTrue(actual.Any(p => p.Name == "PublicProperty"));
            Assert.IsTrue(actual.All(p => p.Name != "PublicGetPrivateSetProperty"));
            Assert.IsTrue(actual.All(p => p.Name != "PrivateProperty"));
            Assert.IsTrue(actual.All(p => p.Name != "PublicStaticProperty"));

            actual = type.of<TestInterface_OOP>().GetPublicProperties().ToList();

            Assert.IsTrue(actual.Any(p => p.Name == "ImplicitInterfaceProperty"));
            Assert.IsTrue(actual.Any(p => p.Name == "ExplicitInterfaceProperty"));
        }

        [Test]
        public void Extension_GetPublicStaticProperties()
        {
            var actual = type.of<TestClass_OOP>().GetPublicStaticProperties().ToList();

            Assert.IsTrue(actual.Any(p => p.Name == "PublicStaticProperty"));
            Assert.IsTrue(actual.Any(p => p.Name == "PublicStaticGetPrivateSetProperty"));
            Assert.IsTrue(actual.All(p => p.Name != "PrivateStaticProperty"));
            Assert.IsTrue(actual.All(p => p.Name != "PublicProperty"));

            actual = type.of<TestClass_OOP>().GetPublicStaticProperties(true).ToList();

            Assert.IsTrue(actual.Any(p => p.Name == "PublicStaticProperty"));
            Assert.IsTrue(actual.All(p => p.Name != "PublicStaticGetPrivateSetProperty"));
            Assert.IsTrue(actual.All(p => p.Name != "PrivateStaticProperty"));
            Assert.IsTrue(actual.All(p => p.Name != "PublicProperty"));
        }

        [Test]
        public void Extension_GetPublicMethods()
        {
            var actual = type.of<TestClass_OOP>().GetPublicMethods();

            Assert.IsTrue(actual.Any(m => m.Name == "PublicMethod"));
            Assert.IsTrue(actual.All(m => m.Name != "PrivateMethod"));
            Assert.IsTrue(actual.All(m => !m.Name.EndsWith("Property")));
            Assert.IsTrue(actual.All(m => m.Name != "PublicStaticMethod"));
        }

        [Test]
        public void Extension_GetPublicStaticMethods()
        {
            var actual = type.of<TestClass_OOP>().GetPublicStaticMethods();

            Assert.IsTrue(actual.Any(m => m.Name == "PublicStaticMethod"));
            Assert.IsTrue(actual.All(m => m.Name != "PrivateStaticMethod"));
            Assert.IsTrue(actual.All(m => !m.Name.EndsWith("Property")));
            Assert.IsTrue(actual.All(m => m.Name != "PublicMethod"));
        }

        [Test]
        public void Extension_CanBe()
        {
            Assert.IsTrue(type.of<string>().CanBe(type.of<object>()));
            Assert.IsTrue(type.of<string>().CanBe<object>());
            Assert.IsFalse(type.of<object>().CanBe<string>());
            Assert.IsTrue(type.of<TestInterface_OOP>().CanBe<object>());
            Assert.IsTrue(type.of<TestInterface_OOP>().CanBe<TestBaseInterface_OOP>());

            Assert.IsTrue(type.of<string[]>().CanBe<ICollection>());
        }

        [Test]
        public void Extension_CanBeCollection()
        {
            var listType = new List<string>().GetTypeInfo();

            Assert.IsTrue(type.of<string[]>().CanBeCollection());
            Assert.IsTrue(listType.CanBeCollection());

            Assert.IsTrue(type.of<string[]>().CanBeCollection(type.of<string>()));
            Assert.IsTrue(listType.CanBeCollection(type.of<string>()));

            Assert.IsTrue(type.of<string[]>().CanBeCollection(type.of<object>()));
            Assert.IsTrue(listType.CanBeCollection(type.of<object>()));
            Assert.IsTrue(type.of<TestInterface_OOP[]>().CanBeCollection(type.of<object>()));

            Assert.IsFalse(type.of<IList>().CanBeCollection());
            Assert.IsFalse(type.of<IList>().CanBeCollection(type.of<string>()));

            //generics
            Assert.IsTrue(type.of<string[]>().CanBeCollection<string>());
        }

        [Test]
        public void Extension_GetItemType()
        {
            Assert.AreEqual(type.of<string>(), type.of<string[]>().GetItemType());

            var listType = new List<string>().GetTypeInfo();
            Assert.AreEqual(type.of<string>(), listType.GetItemType());
        }

        [Test]
        public void Extension_CanParse()
        {
            Assert.IsTrue(type.of<char>().CanParse());
            Assert.IsTrue(type.of<int>().CanParse());
            Assert.IsTrue(type.of<double>().CanParse());
            Assert.IsTrue(type.of<decimal>().CanParse());
            Assert.IsTrue(type.of<float>().CanParse());
            Assert.IsTrue(type.of<DateTime>().CanParse());
            Assert.IsTrue(type.of<TestClass_Parseable>().CanParse());
            Assert.IsFalse(type.of<TestClass_NotParseable>().CanParse());
        }

        [Test, SuppressMessage("ReSharper", "SpecifyACultureInStringConversionExplicitly")]
        public void Extension_Parse()
        {
            Assert.AreEqual('c', type.of<char>().Parse('c'.ToString()));
            Assert.AreEqual(1, type.of<int>().Parse(1.ToString()));
            Assert.AreEqual(1.0d, type.of<double>().Parse(1.0d.ToString()));
            Assert.AreEqual(1.0, type.of<decimal>().Parse(1.0.ToString()));
            Assert.AreEqual(1.0f, type.of<float>().Parse(1.0f.ToString()));
            Assert.AreEqual(new DateTime(2013, 7, 15, 11, 2, 10), type.of<DateTime>().Parse(new DateTime(2013, 7, 15, 11, 2, 10).ToString()));
            Assert.AreEqual(TestClass_Parseable.ParsedResult, type.of<TestClass_Parseable>().Parse("dummy"));
        }

        [Test]
        public void Extension_Has()
        {
            Assert.IsTrue(type.of<TestClass_Attribute>().Has<TestClassAttribute>());
            Assert.IsTrue(type.of<TestClass_Attribute>().Has(type.of<TestClassAttribute>()));
        }
    }
}

