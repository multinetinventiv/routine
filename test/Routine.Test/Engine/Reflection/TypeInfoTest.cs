using Routine.Engine.Reflection;
using Routine.Engine;
using Routine.Test.Engine.Reflection.Domain;
using RoutineTest.OuterDomainNamespace;
using RoutineTest.OuterNamespace;
using System.Reflection;

namespace Routine.Test.Engine.Reflection;

[TestFixture]
public class TypeInfoTest : ReflectionTestBase
{
    private Type _systemType;
    private TypeInfo _testing;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        _systemType = typeof(TestClass_OOP);
        _testing = TypeInfo.Get(_systemType);
    }

    [Test]
    public void Type_is_wrapped_by_TypeInfo()
    {
        Assert.AreEqual(_systemType.Name, _testing.Name);
        Assert.AreEqual(_systemType.FullName, _testing.FullName);
        Assert.AreEqual(_systemType.Namespace, _testing.Namespace);
        Assert.AreEqual(_systemType.BaseType, _testing.BaseType.GetActualType());
        Assert.AreEqual(_systemType.IsAbstract, _testing.IsAbstract);
        Assert.AreEqual(_systemType.IsInterface, _testing.IsInterface);
        Assert.AreEqual(_systemType.IsValueType, _testing.IsValueType);
        Assert.AreEqual(_systemType.IsPrimitive, _testing.IsPrimitive);
        Assert.AreEqual(_systemType.IsGenericType, _testing.IsGenericType);
        Assert.AreEqual(_systemType.IsArray, _testing.IsArray);
        Assert.AreEqual(_systemType.IsEnum, _testing.IsEnum);
        Assert.AreEqual(_systemType.IsPublic, _testing.IsPublic);

        Assert.IsNotNull(_testing.GetConstructor(type.of<string>()));
        Assert.AreEqual(_systemType.GetProperty("PublicProperty")?.Name, _testing.GetProperty("PublicProperty").Name);
        Assert.AreEqual(_systemType.GetMethod("ToString")?.Name, _testing.GetMethod("ToString").Name);
    }

    [Test]
    public void When_a_previously_cached_type_is_being_optimized__previously_created_reflected_type_info_instance_is_invalidated()
    {
        var oldType = TypeInfo.Get<TestOuterDomainType_OOP>();
        Assert.IsInstanceOf<ProxyTypeInfo>(oldType);

        var oldProxy = (ProxyTypeInfo)oldType;
        Assert.IsInstanceOf<ReflectedTypeInfo>(oldProxy.Real);

        TypeInfo.Optimize(typeof(TestOuterDomainType_OOP));

        var newType = TypeInfo.Get<TestOuterDomainType_OOP>();
        Assert.IsInstanceOf<ProxyTypeInfo>(newType);

        var newProxy = (ProxyTypeInfo)oldType;
        Assert.IsInstanceOf<OptimizedTypeInfo>(newProxy.Real);

        Assert.AreSame(oldType, newType);
        Assert.AreSame(oldProxy, newProxy);
    }

    [Test]
    public void When_an_optimized_type_has_a_currently_non_optimized_type_reference__that_type_reference_should_be_invalidated_after_optimizing_it()
    {
        TypeInfo.Optimize(typeof(TestOuterDomainType_OOP));

        var property = TypeInfo.Get<TestOuterDomainType_OOP>().GetAllProperties().First(p => p.Returns<TestOuterLaterAddedDomainType_OOP>());
        var propertyType = (ProxyTypeInfo)property.PropertyType;

        Assert.IsInstanceOf<ReflectedTypeInfo>(propertyType.Real);

        TypeInfo.Optimize(typeof(TestOuterLaterAddedDomainType_OOP));

        property = TypeInfo.Get<TestOuterDomainType_OOP>().GetAllProperties().First(p => p.Returns<TestOuterLaterAddedDomainType_OOP>());
        propertyType = (ProxyTypeInfo)property.PropertyType;

        Assert.IsInstanceOf<OptimizedTypeInfo>(propertyType.Real);
    }

    [Test]
    public void When_a_type_is_loaded_in_two_different_load_contexts__old_one_should_be_matched_from_its_full_name_and_invalidated()
    {
        var typeFromDifferentLoadContext = Assembly.LoadFile(GetType().Assembly.Location).GetType(typeof(TestOuterDomainType_OOP).FullName);

        TypeInfo.Optimize(typeFromDifferentLoadContext);
        TypeInfo.Optimize(typeof(TestOuterDomainType_OOP));

        var actual = TypeInfo.Get(typeFromDifferentLoadContext);
        var expected = TypeInfo.Get<TestOuterDomainType_OOP>();

        Assert.AreSame(expected, actual);
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
    public void Type_GetGenericArguments_is_wrapped_by_TypeInfo()
    {
        var actual = TypeInfo.Get(typeof(TestGenericClass_OOP<>)).GetGenericArguments();
        var expected = typeof(TestGenericClass_OOP<>).GetGenericArguments();

        Assert.That(actual.Length, Is.EqualTo(expected.Length));
        Assert.That(actual[0].Name, Is.EqualTo(expected[0].Name));
    }

    [Test]
    public void BUG_Second_generic_arguments_access_causes_wrong_type_info_to_be_returned()
    {
        TypeInfo.Get(typeof(IEnumerable<>)).GetGenericArguments();

        var actual = TypeInfo.Get(typeof(TestGenericClass_OOP<>)).GetGenericArguments()[0];
        var expected = typeof(TestGenericClass_OOP<>).GetGenericArguments()[0];

        Assert.That(actual.Name, Is.EqualTo(expected.Name));
    }

    [Test]
    public void TypeInfo_caches_wrapped_properties()
    {
        Assert.AreSame(_testing.Name, _testing.Name);
        Assert.AreSame(_testing.FullName, _testing.FullName);
        Assert.AreSame(_testing.Namespace, _testing.Namespace);
        Assert.AreSame(_testing.BaseType, _testing.BaseType);
        Assert.AreSame(_testing.GetAllConstructors(), _testing.GetAllConstructors());
        Assert.AreSame(_testing.GetAllProperties(), _testing.GetAllProperties());
        Assert.AreSame(_testing.GetAllMethods(), _testing.GetAllMethods());
        Assert.AreSame(_testing.GetAllStaticProperties(), _testing.GetAllStaticProperties());
        Assert.AreSame(_testing.GetAllStaticMethods(), _testing.GetAllStaticMethods());
        Assert.AreSame(TypeInfo.Get(typeof(TestClass_Attribute)).GetCustomAttributes(), TypeInfo.Get(typeof(TestClass_Attribute)).GetCustomAttributes());
    }

    [Test]
    public void TypeInfo_has_one_instance_for_each_Type()
    {
        Assert.AreSame(_testing, TypeInfo.Get(typeof(TestClass_OOP)));
    }

    [Test]
    public void TypeInfo_has_equals_operator_for_IType()
    {
        IType type = _testing;
        Assert.IsTrue(type == _testing);
        Assert.IsFalse(type != _testing);
        Assert.IsTrue(_testing == type);
        Assert.IsFalse(_testing != type);
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

    [Test]
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
        Assert.IsInstanceOf<ParseableTypeInfo>(type.of<int>());
        Assert.IsInstanceOf<ParseableTypeInfo>(type.of<DateTime>());

        Assert.IsNotInstanceOf<ParseableTypeInfo>(type.of<TestClass_OOP>());
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

    [Test]
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
