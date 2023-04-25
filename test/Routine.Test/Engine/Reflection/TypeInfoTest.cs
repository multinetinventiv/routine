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
        Assert.That(_testing.Name, Is.EqualTo(_systemType.Name));
        Assert.That(_testing.FullName, Is.EqualTo(_systemType.FullName));
        Assert.That(_testing.Namespace, Is.EqualTo(_systemType.Namespace));
        Assert.That(_testing.BaseType.GetActualType(), Is.EqualTo(_systemType.BaseType));
        Assert.That(_testing.IsAbstract, Is.EqualTo(_systemType.IsAbstract));
        Assert.That(_testing.IsInterface, Is.EqualTo(_systemType.IsInterface));
        Assert.That(_testing.IsValueType, Is.EqualTo(_systemType.IsValueType));
        Assert.That(_testing.IsPrimitive, Is.EqualTo(_systemType.IsPrimitive));
        Assert.That(_testing.IsGenericType, Is.EqualTo(_systemType.IsGenericType));
        Assert.That(_testing.IsArray, Is.EqualTo(_systemType.IsArray));
        Assert.That(_testing.IsEnum, Is.EqualTo(_systemType.IsEnum));
        Assert.That(_testing.IsPublic, Is.EqualTo(_systemType.IsPublic));

        Assert.That(_testing.GetConstructor(type.of<string>()), Is.Not.Null);
        Assert.That(_testing.GetProperty("PublicProperty").Name, Is.EqualTo(_systemType.GetProperty("PublicProperty")?.Name));
        Assert.That(_testing.GetMethod("ToString").Name, Is.EqualTo(_systemType.GetMethod("ToString")?.Name));
    }

    [Test]
    public void When_a_previously_cached_type_is_being_optimized__previously_created_reflected_type_info_instance_is_invalidated()
    {
        var oldType = TypeInfo.Get<TestOuterDomainType_OOP>();
        Assert.That(oldType, Is.InstanceOf<ProxyTypeInfo>());

        var oldProxy = (ProxyTypeInfo)oldType;
        Assert.That(oldProxy.Real, Is.InstanceOf<ReflectedTypeInfo>());

        TypeInfo.Optimize(typeof(TestOuterDomainType_OOP));

        var newType = TypeInfo.Get<TestOuterDomainType_OOP>();
        Assert.That(newType, Is.InstanceOf<ProxyTypeInfo>());

        var newProxy = (ProxyTypeInfo)oldType;
        Assert.That(newProxy.Real, Is.InstanceOf<OptimizedTypeInfo>());

        Assert.That(newType, Is.SameAs(oldType));
        Assert.That(newProxy, Is.SameAs(oldProxy));
    }

    [Test]
    public void When_an_optimized_type_has_a_currently_non_optimized_type_reference__that_type_reference_should_be_invalidated_after_optimizing_it()
    {
        TypeInfo.Optimize(typeof(TestOuterDomainType_OOP));

        var property = TypeInfo.Get<TestOuterDomainType_OOP>().GetAllProperties().First(p => p.Returns<TestOuterLaterAddedDomainType_OOP>());
        var propertyType = (ProxyTypeInfo)property.PropertyType;

        Assert.That(propertyType.Real, Is.InstanceOf<ReflectedTypeInfo>());

        TypeInfo.Optimize(typeof(TestOuterLaterAddedDomainType_OOP));

        property = TypeInfo.Get<TestOuterDomainType_OOP>().GetAllProperties().First(p => p.Returns<TestOuterLaterAddedDomainType_OOP>());
        propertyType = (ProxyTypeInfo)property.PropertyType;

        Assert.That(propertyType.Real, Is.InstanceOf<OptimizedTypeInfo>());
    }

    [Test]
    public void When_a_type_is_loaded_in_two_different_load_contexts__old_one_should_be_matched_from_its_full_name_and_invalidated()
    {
        var typeFromDifferentLoadContext = Assembly.LoadFile(GetType().Assembly.Location).GetType(typeof(TestOuterDomainType_OOP).FullName);

        TypeInfo.Optimize(typeFromDifferentLoadContext);
        TypeInfo.Optimize(typeof(TestOuterDomainType_OOP));

        var actual = TypeInfo.Get(typeFromDifferentLoadContext);
        var expected = TypeInfo.Get<TestOuterDomainType_OOP>();

        Assert.That(actual, Is.SameAs(expected));
    }

    [Test]
    public void TypeInfo_has_the_same_behaviour_on_ToString__Equals_and_GetHashCode_methods()
    {
        Assert.That(TypeInfo.Get(typeof(string)).ToString(), Is.EqualTo(typeof(string).ToString()));
        Assert.That(TypeInfo.Get(typeof(string)).Equals(typeof(string)), Is.True);
        Assert.That(TypeInfo.Get(typeof(string)).GetHashCode(), Is.EqualTo(typeof(string).GetHashCode()));
    }

    [Test]
    public void Type_GetConstructors_is_wrapped_by_TypeInfo()
    {
        var actual = type.of<TestClass_OOP>().GetAllConstructors().ToList();

        Assert.That(actual.Any(c => c.HasNoParameters()), Is.True);
        Assert.That(actual.Any(c => c.HasParameters<string>()), Is.True);
        Assert.That(actual.Any(c => c.HasParameters<int>()), Is.True);
    }

    [Test]
    public void Type_GetProperties_is_wrapped_by_TypeInfo()
    {
        var actual = type.of<TestClass_OOP>().GetAllProperties().ToList();

        Assert.That(actual.Any(p => p.Name == "PublicProperty"), Is.True);
        Assert.That(actual.Any(p => p.Name == "PrivateProperty"), Is.True);
        Assert.That(actual.Any(p => p.Name == "ImplicitInterfaceProperty"), Is.True);
        Assert.That(actual.All(p => p.Name != "ExplicitInterfaceProperty"), Is.True);
        Assert.That(actual.All(p => p.Name != "PublicStaticProperty"), Is.True);
        Assert.That(actual.All(p => p.Name != "PrivateStaticProperty"), Is.True);

        actual = type.of<TestClass_OOP>().GetAllStaticProperties().ToList();

        Assert.That(actual.All(p => p.Name != "PublicProperty"), Is.True);
        Assert.That(actual.All(p => p.Name != "PrivateProperty"), Is.True);
        Assert.That(actual.All(p => p.Name != "ImplicitInterfaceProperty"), Is.True);
        Assert.That(actual.All(p => p.Name != "ExplicitInterfaceProperty"), Is.True);
        Assert.That(actual.Any(p => p.Name == "PublicStaticProperty"), Is.True);
        Assert.That(actual.Any(p => p.Name == "PrivateStaticProperty"), Is.True);
    }

    [Test]
    public void Type_GetMethods_is_wrapped_by_TypeInfo()
    {
        var actual = type.of<TestClass_OOP>().GetAllMethods().ToList();

        Assert.That(actual.Any(m => m.Name == "PublicMethod"), Is.True);
        Assert.That(actual.Any(m => m.Name == "PrivateMethod"), Is.True);
        Assert.That(actual.Any(m => m.Name == "ImplicitInterfaceMethod"), Is.True);
        Assert.That(actual.All(m => m.Name != "ExplicitInterfaceMethod"), Is.True);
        Assert.That(actual.All(m => m.Name != "PublicStaticMethod"), Is.True);
        Assert.That(actual.All(m => m.Name != "PrivateStaticMethod"), Is.True);


        actual = type.of<TestClass_OOP>().GetAllStaticMethods().ToList();

        Assert.That(actual.All(m => m.Name != "PublicMethod"), Is.True);
        Assert.That(actual.All(m => m.Name != "PrivateMethod"), Is.True);
        Assert.That(actual.All(m => m.Name != "ImplicitInterfaceMethod"), Is.True);
        Assert.That(actual.All(m => m.Name != "ExplicitInterfaceMethod"), Is.True);
        Assert.That(actual.Any(m => m.Name == "PublicStaticMethod"), Is.True);
        Assert.That(actual.Any(m => m.Name == "PrivateStaticMethod"), Is.True);
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
        Assert.That(_testing.Name, Is.SameAs(_testing.Name));
        Assert.That(_testing.FullName, Is.SameAs(_testing.FullName));
        Assert.That(_testing.Namespace, Is.SameAs(_testing.Namespace));
        Assert.That(_testing.BaseType, Is.SameAs(_testing.BaseType));
        Assert.That(_testing.GetAllConstructors(), Is.SameAs(_testing.GetAllConstructors()));
        Assert.That(_testing.GetAllProperties(), Is.SameAs(_testing.GetAllProperties()));
        Assert.That(_testing.GetAllMethods(), Is.SameAs(_testing.GetAllMethods()));
        Assert.That(_testing.GetAllStaticProperties(), Is.SameAs(_testing.GetAllStaticProperties()));
        Assert.That(_testing.GetAllStaticMethods(), Is.SameAs(_testing.GetAllStaticMethods()));
        Assert.That(TypeInfo.Get(typeof(TestClass_Attribute)).GetCustomAttributes(), Is.SameAs(TypeInfo.Get(typeof(TestClass_Attribute)).GetCustomAttributes()));
    }

    [Test]
    public void TypeInfo_has_one_instance_for_each_Type()
    {
        Assert.That(TypeInfo.Get(typeof(TestClass_OOP)), Is.SameAs(_testing));
    }

    [Test]
    public void TypeInfo_has_equals_operator_for_IType()
    {
        IType type = _testing;
        Assert.That(type == _testing, Is.True);
        Assert.That(type != _testing, Is.False);
        Assert.That(_testing == type, Is.True);
        Assert.That(_testing != type, Is.False);
    }

    [Test]
    public void TypeInfo_creates_instance_using_default_constructor()
    {
        var actual = (int)TypeInfo.Get(typeof(int)).CreateInstance();

        Assert.That(actual, Is.EqualTo(0));

        var actualObj = TypeInfo.Get(typeof(TestClass_OOP)).CreateInstance();

        Assert.That(actualObj, Is.InstanceOf<TestClass_OOP>());
    }

    [Test]
    public void TypeInfo_creates_instance_as_an_empty_list_or_array_when_type_is_list_or_array()
    {
        var list = (List<int>)TypeInfo.Get(typeof(List<int>)).CreateInstance();

        Assert.That(list.Count, Is.EqualTo(0));

        var array = (int[])TypeInfo.Get(typeof(int[])).CreateInstance();

        Assert.That(array.Length, Is.EqualTo(0));
    }

    [Test]
    public void TypeInfo_creates_list_instance_using_array_initializer_when_type_is_array()
    {
        var array = (int[])TypeInfo.Get(typeof(int[])).CreateListInstance(5);

        Assert.That(array.Length, Is.EqualTo(5));
    }

    [Test]
    public void TypeInfo_creates_list_instance_using_capacity_constructor_when_type_is_list()
    {
        var list = (List<int>)TypeInfo.Get(typeof(List<int>)).CreateListInstance(5);

        Assert.That(list.Count, Is.EqualTo(0));
        Assert.That(list.Capacity, Is.EqualTo(5));
    }

    [Test]
    public void TypeInfo_throws_exception_on_an_attempt_to_create_list_instance_on_a_non_list_type_()
    {
        Assert.That(() => TypeInfo.Get(typeof(int)).CreateListInstance(5), Throws.TypeOf<MissingMethodException>());
    }

    [Test]
    public void When_creating_instance_TypeInfo_throws_MissingMethodException_when_object_does_not_have_default_constructor()
    {
        Assert.That(() => TypeInfo.Get(typeof(TestClassWithoutDefaultConstructor_OOP)).CreateInstance(),
            Throws.TypeOf<MissingMethodException>()
        );
    }

    [Test]
    public void TypeInfo_lists_custom_attributes_with_inherit_behaviour()
    {
        var systemType = typeof(TestClass_Attribute);
        var testing = TypeInfo.Get(systemType);

        var actual = testing.GetCustomAttributes();
        Assert.That(actual.Length, Is.EqualTo(2));
        Assert.That(actual[0], Is.InstanceOf<TestClassAttribute>());
        Assert.That(actual[1], Is.InstanceOf<TestBaseAttribute>());

        systemType = typeof(TestInterface_Attribute);
        testing = TypeInfo.Get(systemType);

        actual = testing.GetCustomAttributes();

        Assert.That(actual.Length, Is.EqualTo(1));
        Assert.That(actual[0], Is.InstanceOf<TestInterfaceAttribute>());
    }

    [Test]
    public void TypeInfo_can_ignore_proxy_classes()
    {
        Assert.That(TypeInfo.Get(typeof(TestProxyClass_OOP)), Is.EqualTo(TypeInfo.Get(typeof(TestClass_OOP))),
            TypeInfo.Get(typeof(TestClass_OOP)) + "(" + TypeInfo.Get(typeof(TestClass_OOP)).GetType() + "), " +
            TypeInfo.Get(typeof(TestProxyClass_OOP)) + "(" + TypeInfo.Get(typeof(TestProxyClass_OOP)).GetType() + "), ");
    }

    [Test]
    public void TypeInfo_lists_assignable_types()
    {
        IType testClassOop = type.of<TestClass_OOP>();

        var actual = testClassOop.AssignableTypes;

        Assert.That(actual.Any(t => Equals(t, type.of<TestAbstractClass_OOP>())), Is.True, "TestClass_OOP is assignable to TestAbstractClass_OOP, but it is not found in assignable types");
        Assert.That(actual.Any(t => Equals(t, type.of<object>())), Is.True, "TestClass_OOP is assignable to object, but it is not found in assignable types");

        Assert.That(actual.Any(t => Equals(t, type.of<TestInterface_OOP>())), Is.True, "TestClass_OOP is assignable to TestInterface_OOP, but it is not found in assignable types");
        Assert.That(actual.Any(t => Equals(t, type.of<TestBaseInterface_OOP>())), Is.True, "TestClass_OOP is assignable to TestBaseInterface_OOP, but it is not found in assignable types");

        Assert.That(actual.Any(t => Equals(t, type.of<TestOuterInterface_OOP>())), Is.True, "TestClass_OOP is assignable to TestOuterInterface_OOP, but it is not found in assignable types");
    }

    [Test]
    public void Facade_type_of()
    {
        Assert.That(type.of<string>(), Is.SameAs(TypeInfo.Get<string>()));
    }

    [Test]
    public void Facade_Void()
    {
        Assert.That(TypeInfo.Void(), Is.SameAs(TypeInfo.Get(typeof(void))));
    }

    [Test]
    public void Facade_GetGeneric()
    {
        Assert.That(TypeInfo.Get<string>(), Is.SameAs(TypeInfo.Get(typeof(string))));
    }

    [Test]
    public void Facade_GetTypeInfo()
    {
        Assert.That("".GetTypeInfo(), Is.SameAs(TypeInfo.Get(typeof(string))));
    }

    [Test]
    public void Extension_IsVoid()
    {
        Assert.That(type.ofvoid().IsVoid, Is.True);
        Assert.That(type.of<string>().IsVoid, Is.False);
    }

    [Test]
    public void Parseable_types_are_represented_by_ParseableTypeInfo_even_if_they_are_added_for_optimization()
    {
        Assert.That(type.of<int>(), Is.InstanceOf<ParseableTypeInfo>());
        Assert.That(type.of<DateTime>(), Is.InstanceOf<ParseableTypeInfo>());

        Assert.That(type.of<TestClass_OOP>(), Is.Not.InstanceOf<ParseableTypeInfo>());
    }

    [Test]
    public void Extension_GetPublicConstructors()
    {
        var actual = type.of<TestClass_OOP>().GetPublicConstructors().ToList();

        Assert.That(actual.Any(c => c.HasNoParameters()), Is.True);
        Assert.That(actual.Any(c => c.HasParameters<string>()), Is.True);
        Assert.That(actual.Any(c => c.HasParameters<int>()), Is.False);
    }

    [Test]
    public void Extension_GetPublicProperties()
    {
        var actual = type.of<TestClass_OOP>().GetPublicProperties().ToList();

        Assert.That(actual.Any(p => p.Name == "PublicProperty"), Is.True);
        Assert.That(actual.Any(p => p.Name == "PublicGetPrivateSetProperty"), Is.True);
        Assert.That(actual.All(p => p.Name != "PrivateProperty"), Is.True);
        Assert.That(actual.All(p => p.Name != "PublicStaticProperty"), Is.True);

        actual = type.of<TestClass_OOP>().GetPublicProperties(true).ToList();

        Assert.That(actual.Any(p => p.Name == "PublicProperty"), Is.True);
        Assert.That(actual.All(p => p.Name != "PublicGetPrivateSetProperty"), Is.True);
        Assert.That(actual.All(p => p.Name != "PrivateProperty"), Is.True);
        Assert.That(actual.All(p => p.Name != "PublicStaticProperty"), Is.True);

        actual = type.of<TestInterface_OOP>().GetPublicProperties().ToList();

        Assert.That(actual.Any(p => p.Name == "ImplicitInterfaceProperty"), Is.True);
        Assert.That(actual.Any(p => p.Name == "ExplicitInterfaceProperty"), Is.True);
    }

    [Test]
    public void Extension_GetPublicStaticProperties()
    {
        var actual = type.of<TestClass_OOP>().GetPublicStaticProperties().ToList();

        Assert.That(actual.Any(p => p.Name == "PublicStaticProperty"), Is.True);
        Assert.That(actual.Any(p => p.Name == "PublicStaticGetPrivateSetProperty"), Is.True);
        Assert.That(actual.All(p => p.Name != "PrivateStaticProperty"), Is.True);
        Assert.That(actual.All(p => p.Name != "PublicProperty"), Is.True);

        actual = type.of<TestClass_OOP>().GetPublicStaticProperties(true).ToList();

        Assert.That(actual.Any(p => p.Name == "PublicStaticProperty"), Is.True);
        Assert.That(actual.All(p => p.Name != "PublicStaticGetPrivateSetProperty"), Is.True);
        Assert.That(actual.All(p => p.Name != "PrivateStaticProperty"), Is.True);
        Assert.That(actual.All(p => p.Name != "PublicProperty"), Is.True);
    }

    [Test]
    public void Extension_GetPublicMethods()
    {
        var actual = type.of<TestClass_OOP>().GetPublicMethods();

        Assert.That(actual.Any(m => m.Name == "PublicMethod"), Is.True);
        Assert.That(actual.All(m => m.Name != "PrivateMethod"), Is.True);
        Assert.That(actual.All(m => !m.Name.EndsWith("Property")), Is.True);
        Assert.That(actual.All(m => m.Name != "PublicStaticMethod"), Is.True);
    }

    [Test]
    public void Extension_GetPublicStaticMethods()
    {
        var actual = type.of<TestClass_OOP>().GetPublicStaticMethods();

        Assert.That(actual.Any(m => m.Name == "PublicStaticMethod"), Is.True);
        Assert.That(actual.All(m => m.Name != "PrivateStaticMethod"), Is.True);
        Assert.That(actual.All(m => !m.Name.EndsWith("Property")), Is.True);
        Assert.That(actual.All(m => m.Name != "PublicMethod"), Is.True);
    }

    [Test]
    public void Extension_CanBe()
    {
        Assert.That(type.of<string>().CanBe(type.of<object>()), Is.True);
        Assert.That(type.of<string>().CanBe<object>(), Is.True);
        Assert.That(type.of<object>().CanBe<string>(), Is.False);
        Assert.That(type.of<TestInterface_OOP>().CanBe<object>(), Is.True);
        Assert.That(type.of<TestInterface_OOP>().CanBe<TestBaseInterface_OOP>(), Is.True);

        Assert.That(type.of<string[]>().CanBe<ICollection>(), Is.True);
    }

    [Test]
    public void Extension_CanBeCollection()
    {
        var listType = new List<string>().GetTypeInfo();

        Assert.That(type.of<string[]>().CanBeCollection(), Is.True);
        Assert.That(listType.CanBeCollection(), Is.True);

        Assert.That(type.of<string[]>().CanBeCollection(type.of<string>()), Is.True);
        Assert.That(listType.CanBeCollection(type.of<string>()), Is.True);

        Assert.That(type.of<string[]>().CanBeCollection(type.of<object>()), Is.True);
        Assert.That(listType.CanBeCollection(type.of<object>()), Is.True);
        Assert.That(type.of<TestInterface_OOP[]>().CanBeCollection(type.of<object>()), Is.True);

        Assert.That(type.of<IList>().CanBeCollection(), Is.False);
        Assert.That(type.of<IList>().CanBeCollection(type.of<string>()), Is.False);

        //generics
        Assert.That(type.of<string[]>().CanBeCollection<string>(), Is.True);
    }

    [Test]
    public void Extension_GetItemType()
    {
        Assert.That(type.of<string[]>().GetItemType(), Is.EqualTo(type.of<string>()));

        var listType = new List<string>().GetTypeInfo();
        Assert.That(listType.GetItemType(), Is.EqualTo(type.of<string>()));
    }

    [Test]
    public void Extension_CanParse()
    {
        Assert.That(type.of<char>().CanParse(), Is.True);
        Assert.That(type.of<int>().CanParse(), Is.True);
        Assert.That(type.of<double>().CanParse(), Is.True);
        Assert.That(type.of<decimal>().CanParse(), Is.True);
        Assert.That(type.of<float>().CanParse(), Is.True);
        Assert.That(type.of<DateTime>().CanParse(), Is.True);
        Assert.That(type.of<TestClass_Parseable>().CanParse(), Is.True);
        Assert.That(type.of<TestClass_NotParseable>().CanParse(), Is.False);
    }

    [Test]
    public void Extension_Parse()
    {
        Assert.That(type.of<char>().Parse('c'.ToString()), Is.EqualTo('c'));
        Assert.That(type.of<int>().Parse(1.ToString()), Is.EqualTo(1));
        Assert.That(type.of<double>().Parse(1.0d.ToString()), Is.EqualTo(1.0d));
        Assert.That(type.of<decimal>().Parse(1.0.ToString()), Is.EqualTo(1.0));
        Assert.That(type.of<float>().Parse(1.0f.ToString()), Is.EqualTo(1.0f));
        Assert.That(type.of<DateTime>().Parse(new DateTime(2013, 7, 15, 11, 2, 10).ToString()), Is.EqualTo(new DateTime(2013, 7, 15, 11, 2, 10)));
        Assert.That(type.of<TestClass_Parseable>().Parse("dummy"), Is.EqualTo(TestClass_Parseable.ParsedResult));
    }

    [Test]
    public void Extension_Has()
    {
        Assert.That(type.of<TestClass_Attribute>().Has<TestClassAttribute>(), Is.True);
        Assert.That(type.of<TestClass_Attribute>().Has(type.of<TestClassAttribute>()), Is.True);
    }
}
