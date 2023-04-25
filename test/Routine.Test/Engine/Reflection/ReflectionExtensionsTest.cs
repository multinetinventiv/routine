using Routine.Engine;
using Routine.Test.Core;

namespace Routine.Test.Engine.Reflection;

[TestFixture]
public class ReflectionExtensionsTest : CoreTestBase
{
    #region Setup & Helpers

    // ReSharper disable UnusedMember.Local
    // ReSharper disable UnusedParameter.Local
    private class NotParseableBecauseMethodDoesNotReturnItsOwnInstance
    {
        public static int Parse(string text) => 0;
    }

    private class NotParseableBecauseMethodDoesNotAcceptStringParameter
    {
        public static NotParseableBecauseMethodDoesNotAcceptStringParameter Parse(int i) => null;
    }

    private class NotParseableBecauseMethodIsNotStatic
    {
        public NotParseableBecauseMethodIsNotStatic Parse(string text) => null;
    }

    private class NotParseableBecauseMethodDoesNotHaveOneParameter
    {
        public static NotParseableBecauseMethodDoesNotHaveOneParameter Parse() => null;
        public static NotParseableBecauseMethodDoesNotHaveOneParameter Parse(string text, string text2) => null;
    }

    private class ParseableEvenIfThereAreOverloads
    {
        public static ParseableEvenIfThereAreOverloads Parse() => null;
        public static ParseableEvenIfThereAreOverloads Parse(int i) => null;
        public static ParseableEvenIfThereAreOverloads Parse(string text) => null;
    }
    // ReSharper restore UnusedParameter.Local
    // ReSharper restore UnusedMember.Local

    public override void SetUp()
    {
        base.SetUp();

        TypeInfo.Optimize(
            GetType().Assembly
                .GetTypes()
                .Where(t => t.Namespace != null && t.Namespace.StartsWith("Routine.Test"))
                .ToArray()
        );
    }

    private ITypeComponent TypeComponent(params object[] customAttributes)
    {
        var result = new Mock<ITypeComponent>();

        result.Setup(o => o.GetCustomAttributes()).Returns(customAttributes);

        return result.Object;
    }

    private IParameter Parameter(TypeInfo parameter)
    {
        var result = new Mock<IParameter>();

        result.Setup(o => o.ParameterType).Returns(parameter);

        return result.Object;
    }

    private IMethod Method(TypeInfo returnType, params TypeInfo[] parameterTypes) => Method("Dummy", returnType, parameterTypes);
    private IMethod Method(string name, TypeInfo returnType, params TypeInfo[] parameterTypes)
    {
        var result = new Mock<IMethod>();

        var parameters = parameterTypes.Select(Parameter).ToList();
        result.Setup(o => o.Parameters).Returns(parameters);
        result.Setup(o => o.ReturnType).Returns(returnType);
        result.Setup(o => o.Name).Returns(name);

        return result.Object;
    }

    #endregion

    [Test]
    public void Test_ToCSharpString()
    {
        Assert.That(typeof(int?).ToCSharpString(), Is.EqualTo("global::System.Nullable<global::System.Int32>"));
        Assert.That(typeof(int?).ToCSharpString(false), Is.EqualTo("Nullable<Int32>"));
    }

    [Test]
    public void Test_CanParse()
    {
        Assert.That(typeof(int).CanParse(), Is.True);
        Assert.That(typeof(ParseableEvenIfThereAreOverloads).CanParse(), Is.True);

        Assert.That(typeof(NotParseableBecauseMethodDoesNotReturnItsOwnInstance).CanParse(), Is.False);
        Assert.That(typeof(NotParseableBecauseMethodDoesNotAcceptStringParameter).CanParse(), Is.False);
        Assert.That(typeof(NotParseableBecauseMethodIsNotStatic).CanParse(), Is.False);
        Assert.That(typeof(NotParseableBecauseMethodDoesNotHaveOneParameter).CanParse(), Is.False);
    }

    [Test]
    public void Test_IsNullable()
    {
        Assert.That(typeof(int?).IsNullable(), Is.True);
        Assert.That(typeof(int).IsNullable(), Is.False);
        Assert.That(typeof(List<int>).IsNullable(), Is.False);
    }

    [Test]
    public void Test_IParametric_HasParameters()
    {
        Assert.That(Method(type.ofvoid()).HasNoParameters(), Is.True);
        Assert.That(Method(type.ofvoid(), type.of<string>()).HasParameters<string>(), Is.True);
        Assert.That(Method(type.ofvoid(), type.of<string>(), type.of<int>()).HasParameters<string, int>(), Is.True);
        Assert.That(Method(type.ofvoid(), type.of<string>(), type.of<int>(), type.of<double>()).HasParameters<string, int, double>(), Is.True);
        Assert.That(Method(type.ofvoid(), type.of<string>(), type.of<int>(), type.of<double>(), type.of<decimal>()).HasParameters<string, int, double, decimal>(), Is.True);

        Assert.That(Method(type.ofvoid(), type.of<string>(), type.of<int>(), type.of<double>()).HasParameters<string, int>(), Is.False);
    }

    [Test]
    public void Test_IParametric_ReturnsVoid()
    {
        Assert.That(Method(type.ofvoid()).ReturnsVoid(), Is.True);
        Assert.That(Method(type.of<string>()).ReturnsVoid(), Is.False);
    }

    [Test]
    public void Test_IReturnable_Returns()
    {
        Assert.That(Method(type.of<string>()).Returns(type.of<object>()), Is.True);
        Assert.That(Method(type.of<int>()).Returns(type.of<string>()), Is.False);

        Assert.That(Method(type.of<List<string>>()).ReturnsCollection(), Is.True);
        Assert.That(Method(type.of<List<string>>()).ReturnsCollection(type.of<object>()), Is.True);
        Assert.That(Method(type.of<IList>()).ReturnsCollection(type.of<string>()), Is.False);

        //generic
        Assert.That(Method(type.of<string>()).Returns<string>(), Is.True);
        Assert.That(Method(type.of<List<string>>()).ReturnsCollection<string>(), Is.True);

        //with name parameter
        Assert.That(Method("Right", type.of<string>()).Returns(type.of<string>(), "Wrong"), Is.False);
        Assert.That(Method("Right", type.of<List<string>>()).ReturnsCollection(type.of<string>(), "Wrong"), Is.False);
    }

    [Test]
    public void Test_ITypeComponent_Has()
    {
        Assert.That(TypeComponent(new AttributeUsageAttribute(AttributeTargets.Method)).Has<AttributeUsageAttribute>(), Is.True);
        Assert.That(TypeComponent(new AttributeUsageAttribute(AttributeTargets.Method)).Has(type.of<AttributeUsageAttribute>()), Is.True);

        Assert.That(TypeComponent().Has<AttributeUsageAttribute>(), Is.False);
        Assert.That(TypeComponent().Has(type.of<AttributeUsageAttribute>()), Is.False);
    }
}
