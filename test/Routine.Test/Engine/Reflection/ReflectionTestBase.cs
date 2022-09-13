using Routine.Engine.Reflection;
using Routine.Test.Core;
using Routine.Test.Engine.Reflection.Domain;

namespace Routine.Test.Engine.Reflection;

public abstract class ReflectionTestBase : CoreTestBase
{
    public override void SetUp()
    {
        base.SetUp();

        TypeInfo.Optimize(GetType().Assembly
            .GetTypes()
            .Where(t =>
                t.Namespace != null && (
                    t.Namespace.StartsWith("Routine.Test.Engine.Reflection.Domain") ||
                    t.Namespace.StartsWith("RoutineTest.OuterNamespace")
                )
            )
            .ToArray()
        );
    }

    public override void TearDown()
    {
        base.TearDown();

        TypeInfo.Clear();
    }

    protected ConstructorInfo OOP_Constructor(params TypeInfo[] typeInfos) =>
        type.of<TestClass_OOP>().GetConstructor(typeInfos);

    protected MethodInfo OOP_Method(string prefixOrFullName) =>
        type.of<TestClass_OOP>().GetMethod($"{prefixOrFullName}Method") ??
        type.of<TestClass_OOP>().GetMethod(prefixOrFullName);

    protected MethodInfo OOP_StaticMethod(string prefixOrFullName) =>
        type.of<TestClass_OOP>().GetStaticMethod($"{prefixOrFullName}Method") ??
        type.of<TestClass_OOP>().GetStaticMethod(prefixOrFullName);

    protected MethodInfo OOP_InterfaceMethod(string prefixOrFullName) =>
        type.of<TestInterface_OOP>().GetMethod($"{prefixOrFullName}Method") ??
        type.of<TestInterface_OOP>().GetMethod(prefixOrFullName);

    protected PropertyInfo OOP_Property(string prefixOrFullName) =>
        type.of<TestClass_OOP>().GetProperty($"{prefixOrFullName}Property") ??
        type.of<TestClass_OOP>().GetProperty(prefixOrFullName);

    protected PropertyInfo OOP_StaticProperty(string prefixOrFullName) =>
        type.of<TestClass_OOP>().GetStaticProperty($"{prefixOrFullName}Property") ??
        type.of<TestClass_OOP>().GetStaticProperty(prefixOrFullName);

    protected ConstructorInfo Members_Constructor(params TypeInfo[] typeInfos) =>
        type.of<TestClass_Members>().GetConstructor(typeInfos);

    protected MethodInfo Members_Method(string prefixOrFullName) =>
        type.of<TestClass_Members>().GetMethod($"{prefixOrFullName}Method") ??
        type.of<TestClass_Members>().GetMethod(prefixOrFullName);

    protected PropertyInfo Members_Property(string prefixOrFullName) =>
        type.of<TestClass_Members>().GetProperty($"{prefixOrFullName}Property") ??
        type.of<TestClass_Members>().GetProperty(prefixOrFullName);

    protected ConstructorInfo Attribute_Constructor(params TypeInfo[] typeInfos) =>
        type.of<TestClass_Attribute>().GetConstructor(typeInfos);

    protected MethodInfo Attribute_Method(string prefixOrFullName) =>
        type.of<TestClass_Attribute>().GetMethod($"{prefixOrFullName}Method") ??
        type.of<TestClass_Attribute>().GetMethod(prefixOrFullName);

    protected PropertyInfo Attribute_Property(string prefixOrFullName) =>
        type.of<TestClass_Attribute>().GetProperty($"{prefixOrFullName}Property") ??
        type.of<TestClass_Attribute>().GetProperty(prefixOrFullName);

    protected MethodInfo Attribute_InterfaceMethod(string prefixOrFullName) =>
        type.of<TestInterface_Attribute>().GetMethod($"{prefixOrFullName}Method") ??
        type.of<TestInterface_Attribute>().GetMethod(prefixOrFullName);

    protected PropertyInfo Attribute_InterfaceProperty(string prefixOrFullName) =>
        type.of<TestInterface_Attribute>().GetProperty($"{prefixOrFullName}Property") ??
        type.of<TestInterface_Attribute>().GetProperty(prefixOrFullName);
}
