using Routine.Core.Configuration;
using Routine.Engine.Virtual;
using Routine.Engine;
using Routine.Test.Core;

namespace Routine.Test.Engine.Virtual;

[TestFixture]
public class VirtualMethodTest : CoreTestBase
{
    #region Setup & Helpers

    private Mock<IType> _parentTypeMock;
    private IType _parentType;

    public override void SetUp()
    {
        base.SetUp();

        _parentTypeMock = new();
        _parentType = _parentTypeMock.Object;
    }

    #endregion

    [Test]
    public void Parent_type_is_what_is_given_as_parent_type()
    {
        IMethod testing = new VirtualMethod(_parentType);

        Assert.That(testing.ParentType, Is.SameAs(_parentType));
    }

    [Test]
    public void Declaring_type_is_always_the_given_parent_type()
    {
        IMethod testing = new VirtualMethod(_parentType);

        Assert.That(testing.GetDeclaringType(false), Is.SameAs(_parentType));
        Assert.That(testing.GetDeclaringType(true), Is.SameAs(_parentType));
    }

    [Test]
    public void Virtual_methods_are_public()
    {
        IMethod testing = new VirtualMethod(_parentType);

        Assert.That(testing.IsPublic, Is.True);
    }

    [Test]
    public void Name_is_required()
    {
        IMethod testing = new VirtualMethod(_parentType)
            .Name.Set("virtual")
        ;

        Assert.That(testing.Name, Is.EqualTo("virtual"));

        testing = new VirtualMethod(_parentType);

        Assert.That(() => { var dummy = testing.Name; }, Throws.TypeOf<ConfigurationException>());
    }

    [Test]
    public void Return_type_is_required()
    {
        var typeMock = new Mock<IType>();

        IMethod testing = new VirtualMethod(_parentType)
            .ReturnType.Set(typeMock.Object)
        ;

        Assert.That(testing.ReturnType, Is.SameAs(typeMock.Object));

        testing = new VirtualMethod(_parentType);

        Assert.That(() => { var dummy = testing.ReturnType; }, Throws.TypeOf<ConfigurationException>());
    }

    [Test]
    public void When_performing_method__invokes_given_delegate()
    {
        IMethod testing = new VirtualMethod(type.of<string>())
            .ReturnType.Set(type.of<string>())
            .Body.Set((target, _) => $"virtual -> {target}")
        ;

        var actual = testing.PerformOn("test");

        Assert.That(actual, Is.EqualTo("virtual -> test"));
    }

    [Test]
    public void Before_performing__validates_target_object_against_given_parent_type()
    {
        IMethod testing = new VirtualMethod(type.of<string>())
            .Name.Set("virtual")
            .ReturnType.Set(type.of<string>())
            .Body.Set((target, _) => $"virtual -> {target}")
        ;

        Assert.That(() => testing.PerformOn(3), Throws.TypeOf<InvalidCastException>());
    }

    [Test]
    public void Target_validation_supports_inheritance()
    {
        IMethod testing = new VirtualMethod(type.of<object>())
            .Name.Set("virtual")
            .ReturnType.Set(type.of<string>())
            .Body.Set((target, _) => $"virtual -> {target}")
        ;

        var actual = testing.PerformOn("test");

        Assert.That(actual, Is.EqualTo("virtual -> test"));

        actual = testing.PerformOn(3);

        Assert.That(actual, Is.EqualTo("virtual -> 3"));
    }

    [Test]
    public void When_target_is_null__NullReferenceException_is_thrown()
    {
        IMethod testing = new VirtualMethod(type.of<string>())
            .Name.Set("virtual")
            .ReturnType.Set(type.of<string>())
            .Body.Set((target, _) => $"virtual -> {target}")
        ;

        Assert.That(() => testing.PerformOn(null), Throws.TypeOf<NullReferenceException>());
    }

    [Test]
    public void When_target_is_virtual_object__virtual_type_is_used_for_target_validation()
    {
        var vt = BuildRoutine.VirtualType().FromBasic()
            .Name.Set("VirtualType")
            .Namespace.Set("Virtual")
            .ToStringMethod.Set(o => o.Id)
        ;

        IMethod testing = new VirtualMethod(vt)
            .Name.Set("VirtualMethod")
            .ReturnType.Set(type.of<string>())
            .Body.Set((target, _) => $"virtual -> {target}")
        ;

        var actual = testing.PerformOn(new VirtualObject("test", vt));

        Assert.That(actual, Is.EqualTo("virtual -> test"));
        Assert.That(() => testing.PerformOn("dummy"), Throws.TypeOf<InvalidCastException>());
    }

    [Test]
    public void Before_returning_result_validates_returning_object_against_given_return_type()
    {
        IMethod testing = new VirtualMethod(type.of<string>())
            .Name.Set("virtual")
            .ReturnType.Set(type.of<string>())
            .Body.Set((_, _) => 3)
        ;

        Assert.That(() => testing.PerformOn("dummy"), Throws.TypeOf<InvalidCastException>());
    }

    [Test]
    public void Result_validation_supports_inheritance()
    {
        IMethod testing = new VirtualMethod(type.of<string>())
            .Name.Set("virtual")
            .ReturnType.Set(type.of<object>())
            .Body.Set((target, _) =>
            {
                if (Equals(target, "1"))
                {
                    return 1;
                }

                return target;
            })
        ;

        var actual = testing.PerformOn("1");

        Assert.That(actual, Is.EqualTo(1));

        actual = testing.PerformOn("test");

        Assert.That(actual, Is.EqualTo("test"));
    }

    [Test]
    public void When_result_is_null__result_validation_is_skipped()
    {
        IMethod testing = new VirtualMethod(type.of<string>())
            .Name.Set("virtual")
            .ReturnType.Set(type.of<string>())
            .Body.Set((_, _) => null)
        ;

        var actual = testing.PerformOn("test");

        Assert.That(actual, Is.Null);
    }

    [Test]
    public void When_return_type_is_void__null_result_is_expected()
    {
        IMethod testing = new VirtualMethod(type.of<string>())
            .Name.Set("virtual")
            .ReturnType.Set(type.ofvoid())
            .Body.Set((target, _) =>
            {
                if (Equals(target, "null"))
                {
                    return null;
                }

                return target;
            })
        ;

        var actual = testing.PerformOn("null");

        Assert.That(actual, Is.Null);
        Assert.That(() => testing.PerformOn("not null"), Throws.TypeOf<InvalidCastException>());
    }

    [Test]
    public void When_return_type_is_value_type__null_result_causes_NullReferenceException()
    {
        IMethod testing = new VirtualMethod(type.of<string>())
            .Name.Set("virtual")
            .ReturnType.Set(type.of<int>())
            .Body.Set((_, _) => null)
        ;

        Assert.That(() => testing.PerformOn("test"), Throws.TypeOf<NullReferenceException>());
    }

    [Test]
    public void When_return_type_is_virtual__virtual_type_is_used_for_result_validation()
    {
        var vt = BuildRoutine.VirtualType().FromBasic()
            .Name.Set("VirtualType")
            .Namespace.Set("Virtual")
            .ToStringMethod.Set(o => o.Id)
        ;

        IMethod testing = new VirtualMethod(type.of<string>())
            .Name.Set("VirtualMethod")
            .ReturnType.Set(vt)
            .Body.Set((target, _) =>
            {
                if (Equals(target, "test"))
                {
                    return new VirtualObject((string)target, vt);
                }

                return target;
            })
        ;

        var actual = testing.PerformOn("test");

        Assert.That(actual, Is.EqualTo(new VirtualObject("test", vt)));
        Assert.That(() => testing.PerformOn("dummy"), Throws.TypeOf<InvalidCastException>());
    }

    [Test]
    public void Virtual_parameters_can_be_added()
    {
        IMethod testing = new VirtualMethod(type.of<string>())
            .Name.Set("Concat")
            .Parameters.Add(p => p.Virtual()
                .Name.Set("param1")
                .Index.Set(0)
                .ParameterType.Set(type.of<string>())
            )
            .Parameters.Add(p => p.Virtual()
                .Name.Set("param2")
                .Index.Set(1)
                .ParameterType.Set(type.of<int>())
            )
            .Parameters.Add(p => p.Virtual()
                .Name.Set("param3")
                .Index.Set(2)
                .ParameterType.Set(type.of<int[]>())
            )
            .ReturnType.Set(type.of<string>())
            .Body.Set((target, parameters) =>
                $"{target}: {parameters[0]} {(int)parameters[1]} {((int[])parameters[2]).ToItemString()}")
        ;

        Assert.That(testing.Parameters.Count, Is.EqualTo(3));
        Assert.That(testing.Parameters[0].Name, Is.EqualTo("param1"));
        Assert.That(testing.Parameters[0].Index, Is.EqualTo(0));
        Assert.That(testing.Parameters[0].ParameterType, Is.EqualTo(type.of<string>()));
        Assert.That(testing.Parameters[1].Name, Is.EqualTo("param2"));
        Assert.That(testing.Parameters[1].Index, Is.EqualTo(1));
        Assert.That(testing.Parameters[1].ParameterType, Is.EqualTo(type.of<int>()));
        Assert.That(testing.Parameters[2].Name, Is.EqualTo("param3"));
        Assert.That(testing.Parameters[2].Index, Is.EqualTo(2));
        Assert.That(testing.Parameters[2].ParameterType, Is.EqualTo(type.of<int[]>()));

        var actual = testing.PerformOn("test", "arg1", 1, new[] { 2, 3 });

        Assert.That(actual, Is.EqualTo("test: arg1 1 [2,3]"));
    }

    [Test]
    public void Argument_count_cannot_be_less_or_more_than_parameter_count()
    {
        IMethod testing = new VirtualMethod(type.of<string>())
            .Name.Set("Concat")
            .Parameters.Add(p => p.Virtual()
                .Name.Set("param1")
                .Index.Set(0)
                .ParameterType.Set(type.of<string>())
            )
            .Parameters.Add(p => p.Virtual()
                .Name.Set("param2")
                .Index.Set(1)
                .ParameterType.Set(type.of<int>())
            )
            .ReturnType.Set(type.of<string>())
            .Body.Set((_, _) => "success")
        ;

        Assert.That(() => testing.PerformOn("test", "less"), Throws.TypeOf<InvalidOperationException>());
        Assert.That(() => testing.PerformOn("test", "arg1", 1, "more"), Throws.TypeOf<InvalidOperationException>());
    }

    [Test]
    public void Arguments_types_are_validated_against_parameter_types()
    {
        IMethod testing = new VirtualMethod(type.of<string>())
            .Name.Set("Concat")
            .Parameters.Add(p => p.Virtual()
                .Name.Set("param1")
                .Index.Set(0)
                .ParameterType.Set(type.of<string>())
            )
            .ReturnType.Set(type.of<string>())
            .Body.Set((_, _) => "success")
        ;

        Assert.That(() => testing.PerformOn("test", 1), Throws.TypeOf<InvalidCastException>());
        Assert.That(testing.PerformOn("test", "arg1"), Is.EqualTo("success"));
    }

    [Test]
    public void Null_arguments_skips_validation()
    {
        IMethod testing = new VirtualMethod(type.of<string>())
            .Name.Set("Concat")
            .Parameters.Add(p => p.Virtual()
                .Name.Set("param1")
                .Index.Set(0)
                .ParameterType.Set(type.of<string>())
            )
            .ReturnType.Set(type.of<string>())
            .Body.Set((_, _) => "success")
        ;

        Assert.That(testing.PerformOn("test", new object[] { null }), Is.EqualTo("success"));
    }

    [Test]
    public void When_a_parameter_type_is_value_type__null_causes_NullReferenceException()
    {
        IMethod testing = new VirtualMethod(type.of<string>())
            .Name.Set("Concat")
            .Parameters.Add(p => p.Virtual()
                .Name.Set("param1")
                .Index.Set(0)
                .ParameterType.Set(type.of<int>())
            )
            .ReturnType.Set(type.of<string>())
            .Body.Set((_, _) => "success")
        ;

        Assert.That(() => testing.PerformOn("test", new object[] { null }), Throws.TypeOf<NullReferenceException>());
    }

    [Test]
    public void Parameter_validation_supports_inheritance()
    {
        IMethod testing = new VirtualMethod(type.of<string>())
            .Name.Set("Concat")
            .Parameters.Add(p => p.Virtual()
                .Name.Set("param1")
                .Index.Set(0)
                .ParameterType.Set(type.of<object>())
            )
            .ReturnType.Set(type.of<string>())
            .Body.Set((_, _) => "success")
        ;

        Assert.That(testing.PerformOn("test", "arg1"), Is.EqualTo("success"));
        Assert.That(testing.PerformOn("test", 1), Is.EqualTo("success"));
    }

    [Test]
    public void Strategy_for_getting_type_of_an_object_can_be_altered_so_that_when_coding_style_is_configured_for_a_custom_type_getting_strategy__it_can_be_applied_to_virtual_methods()
    {
        _parentTypeMock.Setup(o => o.CanBe(_parentType)).Returns(true);

        IMethod testing = new VirtualMethod(_parentType)
            .Name.Set("VirtualMethod")
            .Parameters.Add(p => p.Virtual()
                .Name.Set("param1")
                .Index.Set(0)
                .ParameterType.Set(_parentType)
            )
            .ReturnType.Set(_parentType)
            .Body.Set((target, parameters) => $"virtual -> {target} {parameters[0]}")
            .TypeRetrieveStrategy.Set(_ => _parentType)
        ;

        var actual = testing.PerformOn("target", "arg1");

        Assert.That(actual, Is.EqualTo("virtual -> target arg1"));
    }

    [Test]
    public void Not_supported_features()
    {
        IMethod testing = new VirtualMethod(_parentType);

        Assert.That(testing.GetCustomAttributes().Length, Is.EqualTo(0));
        Assert.That(testing.GetReturnTypeCustomAttributes().Length, Is.EqualTo(0));
    }
}
