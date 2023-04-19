using Routine.Core.Configuration;
using Routine.Engine.Virtual;
using Routine.Engine;
using Routine.Test.Core;

namespace Routine.Test.Engine.Virtual;

[TestFixture]
public class VirtualTypeTest : CoreTestBase
{
    [Test]
    public void Virtual_types_are_public()
    {
        IType testing = BuildRoutine.VirtualType().FromBasic();

        Assert.That(testing.IsPublic, Is.True);
    }

    [Test]
    public void Virtual_types_cannot_be_array()
    {
        IType testing = BuildRoutine.VirtualType().FromBasic();

        Assert.That(testing.IsArray, Is.False);
    }

    [Test]
    public void Virtual_types_cannot_be_void()
    {
        IType testing = BuildRoutine.VirtualType().FromBasic();

        Assert.That(testing.IsVoid, Is.False);
    }

    [Test]
    public void Name_is_required()
    {
        IType testing = BuildRoutine.VirtualType().FromBasic()
            .Name.Set("Test")
        ;

        Assert.That(testing.Name, Is.EqualTo("Test"));

        testing = BuildRoutine.VirtualType().FromBasic();

        Assert.That(() => { var dummy = testing.Name; }, Throws.TypeOf<ConfigurationException>());
    }

    [Test]
    public void Namespace_is_required()
    {
        IType testing = BuildRoutine.VirtualType().FromBasic()
            .Namespace.Set("Routine")
        ;

        Assert.That(testing.Namespace, Is.EqualTo("Routine"));

        testing = BuildRoutine.VirtualType().FromBasic();

        Assert.That(() => { var dummy = testing.Namespace; }, Throws.TypeOf<ConfigurationException>());
    }

    [Test]
    public void FullName_is_built_using_namespace_and_name()
    {
        IType testing = BuildRoutine.VirtualType().FromBasic()
            .Name.Set("Test")
            .Namespace.Set("Routine")
        ;

        Assert.That(testing.FullName, Is.EqualTo("Routine.Test"));
    }

    [Test]
    public void IsInterface_is_optional__default_is_false()
    {
        IType testing = BuildRoutine.VirtualType().FromBasic()
            .IsInterface.Set(true)
        ;

        Assert.That(testing.IsInterface, Is.True);

        testing = BuildRoutine.VirtualType().FromBasic();

        Assert.That(testing.IsInterface, Is.False);
    }

    [Test]
    public void Creates_virtual_object()
    {
        IType testing = BuildRoutine.VirtualType().FromBasic()
            .DefaultInstanceId.Set("default")
        ;

        var actual = testing.CreateInstance();

        Assert.That(actual, Is.InstanceOf<VirtualObject>());
        Assert.That(((VirtualObject)actual).Id, Is.EqualTo("default"));
    }

    [Test]
    public void By_default__to_string_returns_Id_and_virtual_type_name()
    {
        IType testing = BuildRoutine.VirtualType().FromBasic()
            .DefaultInstanceId.Set("default")
            .Namespace.Set("Namespace")
            .Name.Set("Name")
        ;

        var actual = testing.CreateInstance();

        Assert.That(actual.ToString(), Is.EqualTo("default (Namespace.Name)"));
    }

    [Test]
    public void Methods_are_created_externally_and_added_to_type()
    {
        var mockMethod = new Mock<IMethod>();

        IType testing = BuildRoutine.VirtualType().FromBasic()
            .Methods.Add(mockMethod.Object)
        ;

        Assert.That(testing.Methods.Count, Is.EqualTo(1));
        Assert.That(testing.Methods[0], Is.SameAs(mockMethod.Object));
    }

    [Test]
    public void Can_be_itself_and_object()
    {
        IType testing = BuildRoutine.VirtualType().FromBasic();

        Assert.That(testing.CanBe(type.of<object>()), Is.True);
        Assert.That(testing.CanBe(type.of<string>()), Is.False);
    }

    [Test]
    public void Virtual_types_support_formatting_and_equality_members()
    {
        IType testing = BuildRoutine.VirtualType().FromBasic()
            .Name.Set("Virtual")
            .Namespace.Set("Routine")
        ;

        Assert.That(testing.ToString(), Is.EqualTo("Routine.Virtual"));

        IType clone = BuildRoutine.VirtualType().FromBasic()
            .Name.Set("Virtual")
            .Namespace.Set("Routine")
        ;

        Assert.That(clone.GetHashCode(), Is.EqualTo(testing.GetHashCode()));
        Assert.That(clone, Is.EqualTo(testing));
    }

    [Test]
    public void Virtual_types_have_assignable_virtual_types()
    {
        IType testing = BuildRoutine.VirtualType().FromBasic()
            .Name.Set("Virtual")
            .Namespace.Set("Routine")
            .AssignableTypes.Add(vt => vt.FromBasic()
                .Name.Set("IVirtual")
                .Namespace.Set("Routine")
                .IsInterface.Set(true)
            )
        ;

        Assert.That(testing.AssignableTypes.Count, Is.EqualTo(1));
        Assert.That(testing.AssignableTypes[0].Name, Is.EqualTo("IVirtual"));
    }

    [Test]
    public void Can_be_one_of_its_assignable_types()
    {
        var virtualInterface = BuildRoutine.VirtualType().FromBasic()
            .Name.Set("IVirtual")
            .Namespace.Set("Routine")
            .IsInterface.Set(true);

        IType testing = BuildRoutine.VirtualType().FromBasic()
            .Name.Set("Virtual")
            .Namespace.Set("Routine")
            .AssignableTypes.Add(virtualInterface)
        ;

        Assert.That(testing.CanBe(virtualInterface), Is.True);
        Assert.That(testing.CanBe(type.of<string>()), Is.False);
    }

    [Test]
    public void Casts_a_virtual_object_to_its_assignable_type()
    {
        var virtualInterface = BuildRoutine.VirtualType().FromBasic()
            .Name.Set("IVirtual")
            .Namespace.Set("Routine")
            .IsInterface.Set(true);

        IType testing = BuildRoutine.VirtualType().FromBasic()
            .DefaultInstanceId.Set("Id")
            .Name.Set("Virtual")
            .Namespace.Set("Routine")
            .AssignableTypes.Add(virtualInterface)
        ;

        var instance = testing.CreateInstance();

        Assert.That(testing.Cast(instance, virtualInterface), Is.SameAs(instance));
    }

    [Test]
    public void Casts_a_virtual_object_to_object()
    {
        IType testing = BuildRoutine.VirtualType().FromBasic()
            .DefaultInstanceId.Set("Id")
            .Name.Set("Virtual")
            .Namespace.Set("Routine")
        ;

        var instance = testing.CreateInstance();

        Assert.That(testing.Cast(instance, type.of<object>()), Is.SameAs(instance));
    }

    [Test]
    public void Cannot_cast_a_real_object()
    {
        var virtualInterface = BuildRoutine.VirtualType().FromBasic()
            .Name.Set("IVirtual")
            .Namespace.Set("Routine")
            .IsInterface.Set(true);

        IType testing = BuildRoutine.VirtualType().FromBasic()
            .DefaultInstanceId.Set("Id")
            .Name.Set("Virtual")
            .Namespace.Set("Routine")
            .AssignableTypes.Add(virtualInterface)
        ;

        Assert.That(() => testing.Cast("string", virtualInterface), Throws.TypeOf<InvalidCastException>());
    }

    [Test]
    public void Cannot_cast_to_a_real_type()
    {
        IType testing = BuildRoutine.VirtualType().FromBasic()
            .DefaultInstanceId.Set("Id")
            .Name.Set("Virtual")
            .Namespace.Set("Routine")
        ;

        Assert.That(() => testing.Cast(testing.CreateInstance(), type.of<string>()), Throws.TypeOf<InvalidCastException>());
    }

    [Test]
    public void Cannot_cast_to_a_virtual_type_that_is_not_in_assignable_types()
    {
        var virtualInterface = BuildRoutine.VirtualType().FromBasic()
            .Name.Set("IVirtual")
            .Namespace.Set("Routine")
            .IsInterface.Set(true);

        IType testing = BuildRoutine.VirtualType().FromBasic()
            .DefaultInstanceId.Set("Id")
            .Name.Set("Virtual")
            .Namespace.Set("Routine")
        ;

        Assert.That(() => testing.Cast(testing.CreateInstance(), virtualInterface), Throws.TypeOf<InvalidCastException>());
    }

    [Test]
    public void Not_supported_features()
    {
        IType testing = BuildRoutine.VirtualType().FromBasic();

        Assert.That(testing.ParentType, Is.Null);
        Assert.That(testing.GetCustomAttributes().Length, Is.EqualTo(0));
        Assert.That(testing.IsAbstract, Is.False);
        Assert.That(testing.IsEnum, Is.False);
        Assert.That(testing.IsGenericType, Is.False);
        Assert.That(testing.IsPrimitive, Is.False);
        Assert.That(testing.IsValueType, Is.False);
        Assert.That(testing.BaseType, Is.EqualTo(type.of<object>()));
        Assert.That(testing.GetGenericArguments().Count, Is.EqualTo(0));
        Assert.That(testing.GetElementType(), Is.Null);
        Assert.That(testing.GetParseMethod(), Is.Null);
        Assert.That(testing.GetEnumNames().Count, Is.EqualTo(0));
        Assert.That(testing.GetEnumValues().Count, Is.EqualTo(0));
        Assert.That(testing.GetEnumUnderlyingType(), Is.Null);
        Assert.That(() => testing.CreateListInstance(10), Throws.TypeOf<NotSupportedException>());
        Assert.That(testing.Constructors.Count, Is.EqualTo(0));
        Assert.That(testing.Properties.Count, Is.EqualTo(0));
    }
}
