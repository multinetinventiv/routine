using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Routine.Engine.Reflection;
using Routine.Test.Core;
using Routine.Test.Engine.Reflection.Domain;
using RoutineTest.OuterNamespace;

#region Test Model

namespace Routine.Test.Engine.Reflection.Domain
{
    #region OOP Model

    public class TestClassWithoutDefaultConstructor_OOP
    {
        public TestClassWithoutDefaultConstructor_OOP(string dummy) { }
    }

    public interface TestBaseInterface_OOP
    {
        string ExplicitBaseInterfaceProperty { get; }
    }

    public interface TestInterface_OOP : TestBaseInterface_OOP
    {
        string ImplicitInterfaceProperty { get; }
        string ExplicitInterfaceProperty { get; }

        void ImplicitInterfaceMethod();
        void ImplicitInterfaceWithParameterMethod(string str);
        void ExplicitInterfaceMethod();
    }

    public abstract class TestAbstractClass_OOP
    {
        public virtual string OverriddenProperty => "";
        public virtual string NotOverriddenProperty => "";
        public abstract string AbstractProperty { get; }

        public virtual void OverriddenMethod() { }
        public virtual void NotOverriddenMethod() { }
        public abstract void AbstractMethod();
    }

    public class TestClass_OOP : TestAbstractClass_OOP, TestInterface_OOP, TestOuterInterface_OOP
    {
        public TestClass_OOP() { }
        public TestClass_OOP(string str) { }
        public TestClass_OOP(Exception ex) => throw ex;
        private TestClass_OOP(int i) { }

        public string this[int i, string str] { get => null; set { } }

        public string PublicProperty { get; set; }
        private string PrivateProperty { get; set; }
        public string PublicGetPrivateSetProperty { get; private set; }

        public static string PublicStaticProperty { get; set; }
        private static string PrivateStaticProperty { get; set; }
        public static string PublicStaticGetPrivateSetProperty { get; private set; }

        public string OtherNamespaceProperty => "";

        string TestBaseInterface_OOP.ExplicitBaseInterfaceProperty => "";
        public string ImplicitInterfaceProperty => "";
        string TestInterface_OOP.ExplicitInterfaceProperty => "";

        public override string OverriddenProperty => "";
        public override string AbstractProperty => "";

        public void PublicMethod() { }
        private void PrivateMethod() { }

        public override void AbstractMethod() { }
        public override void OverriddenMethod() { }

        public void ImplicitInterfaceMethod() { }
        public void ImplicitInterfaceWithParameterMethod(string str) { }
        void TestInterface_OOP.ExplicitInterfaceMethod() { }

        public void OtherNamespaceMethod() { }
        public override string ToString() => "TestClass_OOP";

        public static void PublicStaticMethod() { }
        private static void PrivateStaticMethod() { }

        public static string PublicStaticPingMethod(string message) => $"static {message}";
        public string PublicPingMethod(string message) => $"instance {message}";

        public void ExceptionMethod(Exception ex) => throw ex;
        public Exception Exception;
        public string ExceptionProperty
        {
            get => throw Exception; set => throw Exception;
        }
    }

    public class TestProxyClass_OOP : TestClass_OOP { }

    #endregion

    #region Parseable Model

    public class TestClass_Parseable
    {
        public static readonly TestClass_Parseable ParsedResult = new();

        public static TestClass_Parseable Parse(string value) => ParsedResult;
    }

    public class TestClass_NotParseable
    {
        public static void Parse(string value) { }
        public static TestClass_NotParseable Parse() => null;
    }

    #endregion

    #region Members Model

    public class TestClass_Members
    {
        public TestClass_Members() : this(null) { }
        public TestClass_Members(string str) : this(str, 0) { }
        public TestClass_Members(int i) : this(null, i) { }
        public TestClass_Members(string str, int i) { IntProperty = i; StringProperty = str; }

        public int IntProperty { get; set; }
        public string StringProperty { get; set; }
        public List<string> StringListProperty => null;
        public IList NonGenericListProperty => null;
        public string PublicAutoProperty { get; set; }
        public string PublicProperty
        {
            get => null;
            set { }
        }
        public string PublicReadOnlyProperty => null;
        public string PublicWriteOnlyProperty { set { } }
        public string PrivateGetProperty { private get; set; }
        public string PrivateSetProperty { get; private set; }

        public List<string> StringListMethod() => null;
        public IList NonGenericListMethod() => null;

        public string StringMethod() => null;
        public void VoidMethod() { }
        public int IntMethod() => 0;

        public void ParameterlessMethod() { }
        public void OneParameterMethod(string p1) { }
        public void TwoParameterMethod(string p1, int p2) { }
        public void ThreeParameterMethod(string p1, int p2, double p3) { }
        public void FourParameterMethod(string p1, int p2, double p3, decimal p4) { }
        public void FiveParameterMethod(string p1, int p2, double p3, decimal p4, float p5) { }
    }

    #endregion

    #region Attribute Model

    public class TestInterfaceAttribute : Attribute { }

    [TestInterface]
    public interface TestInterface_Attribute
    {
        [TestInterface]
        string InterfaceProperty { get; set; }

        [TestInterface]
        void InterfaceMethod();
    }

    public class TestBaseAttribute : Attribute { }

    [TestBase]
    public abstract class TestBase_Attribute
    {
        [TestBase]
        public string BaseProperty { get; set; }

        [TestBase]
        public virtual string OverriddenProperty { get; set; }

        [TestBase]
        public void BaseMethod() { }

        [TestBase]
        public virtual void OverriddenMethod() { }
    }

    public class TestClassAttribute : Attribute { }

    [TestClass]
    public class TestClass_Attribute : TestBase_Attribute, TestInterface_Attribute
    {
        [TestClass]
        public TestClass_Attribute() { }

        [TestClass]
        public TestClass_Attribute([TestClass] int i) { }

        [TestClass]
        public string ClassProperty { [return: TestClass] get; set; }

        public string WriteOnlyProperty { set { } }

        [TestClass]
        public override string OverriddenProperty { get; set; }

        [TestClass]
        public string InterfaceProperty { get; set; }

        [TestClass]
        [return: TestClass]
        public string ClassMethod([TestClass] string parameter) => null;

        [TestClass]
        public override void OverriddenMethod() { base.OverriddenMethod(); }

        [TestClass]
        public void InterfaceMethod() { }
    }

    #endregion

    #region Parameter Model

    public class ReflectedParameter
    {
        public void AMethod([TestClass] string theParameter) { }
    }

    #endregion
}

#endregion

#region Test Model (Outer Namespace)

namespace RoutineTest.OuterNamespace
{
    public interface TestOuterInterface_OOP
    {
        string OtherNamespaceProperty { get; }
        void OtherNamespaceMethod();
    }
}

namespace RoutineTest.OuterDomainNamespace
{
    public class TestOuterDomainType_OOP
    {
        public TestOuterDomainType_OOP() { }
        public TestOuterDomainType_OOP(Exception ex) { throw ex; }
        public void ExceptionMethod(Exception ex) { throw ex; }

        public Exception Exception;
        public string ExceptionProperty
        {
            get => throw Exception;
            set => throw Exception;
        }

        public TestOuterLaterAddedDomainType_OOP LaterAddedDomainTypeProperty { get; set; }
    }

    public class TestOuterLaterAddedDomainType_OOP
    {
    }
}

#endregion

namespace Routine.Test.Engine.Reflection
{
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
}