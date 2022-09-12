using Moq;
using NUnit.Framework;
using Routine.Core.Configuration;
using Routine.Engine.Virtual;
using Routine.Engine;
using Routine.Test.Core;
using System;

namespace Routine.Test.Engine.Virtual;

[TestFixture]
public class VirtualMethodTest : CoreTestBase
{
    #region Setup & Helpers

    private Mock<IType> parentTypeMock;
    private IType parentType;

    public override void SetUp()
    {
        base.SetUp();

        parentTypeMock = new Mock<IType>();
        parentType = parentTypeMock.Object;
    }

    #endregion

    [Test]
    public void Parent_type_is_what_is_given_as_parent_type()
    {
        IMethod testing = new VirtualMethod(parentType);

        Assert.AreSame(parentType, testing.ParentType);
    }

    [Test]
    public void Declaring_type_is_always_the_given_parent_type()
    {
        IMethod testing = new VirtualMethod(parentType);

        Assert.AreSame(parentType, testing.GetDeclaringType(false));
        Assert.AreSame(parentType, testing.GetDeclaringType(true));
    }

    [Test]
    public void Virtual_methods_are_public()
    {
        IMethod testing = new VirtualMethod(parentType);

        Assert.IsTrue(testing.IsPublic);
    }

    [Test]
    public void Name_is_required()
    {
        IMethod testing = new VirtualMethod(parentType)
            .Name.Set("virtual")
        ;

        Assert.AreEqual("virtual", testing.Name);

        testing = new VirtualMethod(parentType);

        Assert.Throws<ConfigurationException>(() => { var dummy = testing.Name; });
    }

    [Test]
    public void Return_type_is_required()
    {
        var typeMock = new Mock<IType>();

        IMethod testing = new VirtualMethod(parentType)
            .ReturnType.Set(typeMock.Object)
        ;

        Assert.AreSame(typeMock.Object, testing.ReturnType);

        testing = new VirtualMethod(parentType);

        Assert.Throws<ConfigurationException>(() => { var dummy = testing.ReturnType; });
    }

    [Test]
    public void When_performing_method__invokes_given_delegate()
    {
        IMethod testing = new VirtualMethod(type.of<string>())
            .ReturnType.Set(type.of<string>())
            .Body.Set((target, _) => $"virtual -> {target}")
        ;

        var actual = testing.PerformOn("test");

        Assert.AreEqual("virtual -> test", actual);
    }

    [Test]
    public void Before_performing__validates_target_object_against_given_parent_type()
    {
        IMethod testing = new VirtualMethod(type.of<string>())
            .Name.Set("virtual")
            .ReturnType.Set(type.of<string>())
            .Body.Set((target, _) => $"virtual -> {target}")
        ;

        Assert.Throws<InvalidCastException>(() => testing.PerformOn(3));
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

        Assert.AreEqual("virtual -> test", actual);

        actual = testing.PerformOn(3);

        Assert.AreEqual("virtual -> 3", actual);
    }

    [Test]
    public void When_target_is_null__NullReferenceException_is_thrown()
    {
        IMethod testing = new VirtualMethod(type.of<string>())
            .Name.Set("virtual")
            .ReturnType.Set(type.of<string>())
            .Body.Set((target, _) => $"virtual -> {target}")
        ;

        Assert.Throws<NullReferenceException>(() => testing.PerformOn(null));
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

        Assert.AreEqual("virtual -> test", actual);
        Assert.Throws<InvalidCastException>(() => testing.PerformOn("dummy"));
    }

    [Test]
    public void Before_returning_result_validates_returning_object_against_given_return_type()
    {
        IMethod testing = new VirtualMethod(type.of<string>())
            .Name.Set("virtual")
            .ReturnType.Set(type.of<string>())
            .Body.Set((_, _) => 3)
        ;

        Assert.Throws<InvalidCastException>(() => testing.PerformOn("dummy"));
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

        Assert.AreEqual(1, actual);

        actual = testing.PerformOn("test");

        Assert.AreEqual("test", actual);
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

        Assert.IsNull(actual);
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

        Assert.IsNull(actual);
        Assert.Throws<InvalidCastException>(() => testing.PerformOn("not null"));
    }

    [Test]
    public void When_return_type_is_value_type__null_result_causes_NullReferenceException()
    {
        IMethod testing = new VirtualMethod(type.of<string>())
            .Name.Set("virtual")
            .ReturnType.Set(type.of<int>())
            .Body.Set((_, _) => null)
        ;

        Assert.Throws<NullReferenceException>(() => testing.PerformOn("test"));
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

        Assert.AreEqual(new VirtualObject("test", vt), actual);
        Assert.Throws<InvalidCastException>(() => testing.PerformOn("dummy"));
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

        Assert.AreEqual(3, testing.Parameters.Count);
        Assert.AreEqual("param1", testing.Parameters[0].Name);
        Assert.AreEqual(0, testing.Parameters[0].Index);
        Assert.AreEqual(type.of<string>(), testing.Parameters[0].ParameterType);
        Assert.AreEqual("param2", testing.Parameters[1].Name);
        Assert.AreEqual(1, testing.Parameters[1].Index);
        Assert.AreEqual(type.of<int>(), testing.Parameters[1].ParameterType);
        Assert.AreEqual("param3", testing.Parameters[2].Name);
        Assert.AreEqual(2, testing.Parameters[2].Index);
        Assert.AreEqual(type.of<int[]>(), testing.Parameters[2].ParameterType);

        var actual = testing.PerformOn("test", "arg1", 1, new[] { 2, 3 });

        Assert.AreEqual("test: arg1 1 [2,3]", actual);
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

        Assert.Throws<InvalidOperationException>(() => testing.PerformOn("test", "less"));
        Assert.Throws<InvalidOperationException>(() => testing.PerformOn("test", "arg1", 1, "more"));
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

        Assert.Throws<InvalidCastException>(() => testing.PerformOn("test", 1));
        Assert.AreEqual("success", testing.PerformOn("test", "arg1"));
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

        Assert.AreEqual("success", testing.PerformOn("test", new object[] { null }));
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

        Assert.Throws<NullReferenceException>(() => testing.PerformOn("test", new object[] { null }));
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

        Assert.AreEqual("success", testing.PerformOn("test", "arg1"));
        Assert.AreEqual("success", testing.PerformOn("test", 1));
    }

    [Test]
    public void Strategy_for_getting_type_of_an_object_can_be_altered_so_that_when_coding_style_is_configured_for_a_custom_type_getting_strategy__it_can_be_applied_to_virtual_methods()
    {
        parentTypeMock.Setup(o => o.CanBe(parentType)).Returns(true);

        IMethod testing = new VirtualMethod(parentType)
            .Name.Set("VirtualMethod")
            .Parameters.Add(p => p.Virtual()
                .Name.Set("param1")
                .Index.Set(0)
                .ParameterType.Set(parentType)
            )
            .ReturnType.Set(parentType)
            .Body.Set((target, parameters) => $"virtual -> {target} {parameters[0]}")
            .TypeRetrieveStrategy.Set(_ => parentType)
        ;

        var actual = testing.PerformOn("target", "arg1");

        Assert.AreEqual("virtual -> target arg1", actual);
    }

    [Test]
    public void Not_supported_features()
    {
        IMethod testing = new VirtualMethod(parentType);

        Assert.AreEqual(0, testing.GetCustomAttributes().Length);
        Assert.AreEqual(0, testing.GetReturnTypeCustomAttributes().Length);
    }
}
