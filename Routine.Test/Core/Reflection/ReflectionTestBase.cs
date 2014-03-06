using Routine.Core.Reflection;
using RoutineTest.OuterNamespace;
using System.Collections.Generic;
using System.Collections;
using Routine.Test.Core.Reflection.Domain;

#region Outer Namespace Model
namespace RoutineTest.OuterNamespace
{
	public interface TestOuterInterface_OOP
	{
		string OtherNamespaceProperty{get;}
		void OtherNamespaceMethod();
	}
}
#endregion

namespace Routine.Test.Core.Reflection.Domain
{
	#region OOP Model
	public class TestClassWithoutDefaultConstructor_OOP
	{
		public TestClassWithoutDefaultConstructor_OOP(string dummy){}
	}

	public interface TestBaseInterface_OOP{}
	public interface TestInterface_OOP : TestBaseInterface_OOP
	{
		string ImplicitInterfaceProperty{get;}
		string ExplicitInterfaceProperty{get;}

		void ImplicitInterfaceMethod();
		void ImplicitInterfaceWithParameterMethod(string str);
		void ExplicitInterfaceMethod();
	}

	public abstract class TestAbstractClass_OOP
	{
		public virtual string OverriddenProperty{get{return "";}}
		public virtual string NotOverriddenProperty{get{return "";}}
		public abstract string AbstractProperty{get;}

		public virtual void OverriddenMethod(){}
		public virtual void NotOverriddenMethod(){}
		public abstract void AbstractMethod();
	}

	public class TestClass_OOP : TestAbstractClass_OOP, TestInterface_OOP, TestOuterInterface_OOP
	{
		public string this[int i, string str] {get{return null;}}

		public string PublicProperty {get;set;}
		private string PrivateProperty {get;set;}
		public string PublicGetPrivateSetProperty{get;private set;}

		public static string PublicStaticProperty{get;set;}
		private static string PrivateStaticProperty{get;set;}
		public static string PublicStaticGetPrivateSetProperty{get;private set;}

		public string OtherNamespaceProperty{get{return "";}}

		public string ImplicitInterfaceProperty{get{return "";}}
		string TestInterface_OOP.ExplicitInterfaceProperty{get{return"";}}

		public override string OverriddenProperty{get{return"";}}
		public override string AbstractProperty{get{return "";}}

		public void PublicMethod(){}
		private void PrivateMethod(){}

		public override void AbstractMethod(){}
		public override void OverriddenMethod(){}

		public void ImplicitInterfaceMethod(){}
		public void ImplicitInterfaceWithParameterMethod(string str){}
		void TestInterface_OOP.ExplicitInterfaceMethod(){}

		public void OtherNamespaceMethod() {}
		public override string ToString(){return "TestClass_OOP";}

		public static void PublicStaticMethod(){}
		private static void PrivateStaticMethod(){}

		public static string PublicStaticPingMethod(string message){return "static " + message;}
		public string PublicPingMethod(string message){return "instance " + message;}
	}
	#endregion

	#region Parseable Model
	public class TestClass_Parseable
	{
		public static readonly TestClass_Parseable ParsedResult = new TestClass_Parseable();

		public static TestClass_Parseable Parse(string value){return ParsedResult;}
	}

	public class TestClass_NotParseable
	{
		public static void Parse(string value){}
		public static TestClass_NotParseable Parse(){return null;}
	}
	#endregion

	#region Members Model
	public class TestClass_Members
	{
		public int IntProperty{get{return 0;}}
		public string StringProperty{get{return null;}}
		public List<string> StringListProperty{get{return null;}}
		public IList NonGenericListProperty{get{return null;}}
		public string PublicAutoProperty{ get; set;}
		public string PublicProperty{get{return null;} set{}}
		public string PublicReadOnlyProperty{ get{return null;}}
		public string PublicWriteOnlyProperty{set{}}
		public string PrivateGetProperty{private get; set;}
		public string PrivateSetProperty{get; private set;}

		public List<string> StringListMethod(){return null;}
		public IList NonGenericListMethod(){return null;}

		public string StringMethod(){return null;}
		public void VoidMethod(){}
		public int IntMethod(){return 0;}

		public void ParameterlessMethod(){}
		public void OneParameterMethod(string p1){}
		public void TwoParameterMethod(string p1, int p2){}
		public void ThreeParameterMethod(string p1, int p2, double p3){}
		public void FourParameterMethod(string p1, int p2, double p3, decimal p4){}
		public void FiveParameterMethod(string p1, int p2, double p3, decimal p4, float p5){}
	}
	#endregion
}

namespace Routine.Test.Core.Reflection
{
	public abstract class ReflectionTestBase : CoreTestBase
	{
		public override string[] DomainTypeRootNamespaces{get{return new[]{"Routine.Test.Core.Reflection.Domain"};}}

		protected MethodInfo OOP_Method(string prefixOrFullName)
		{
			return 	type.of<TestClass_OOP>().GetMethod(prefixOrFullName + "Method") ??
				type.of<TestClass_OOP>().GetMethod(prefixOrFullName);
		}

		protected MethodInfo OOP_StaticMethod(string prefixOrFullName)
		{
			return 	type.of<TestClass_OOP>().GetStaticMethod(prefixOrFullName + "Method") ??
				type.of<TestClass_OOP>().GetStaticMethod(prefixOrFullName);
		}

		protected PropertyInfo OOP_Property(string prefixOrFullName)
		{
			return 	type.of<TestClass_OOP>().GetProperty(prefixOrFullName + "Property") ??
				type.of<TestClass_OOP>().GetProperty(prefixOrFullName);
		}

		protected PropertyInfo OOP_StaticProperty(string prefixOrFullName)
		{
			return 	type.of<TestClass_OOP>().GetStaticProperty(prefixOrFullName + "Property") ??
				type.of<TestClass_OOP>().GetStaticProperty(prefixOrFullName);
		}

		protected MethodInfo Members_Method(string prefixOrFullName)
		{
			return 	type.of<TestClass_Members>().GetMethod(prefixOrFullName + "Method") ??
				type.of<TestClass_Members>().GetMethod(prefixOrFullName);
		}

		protected PropertyInfo Members_Property(string prefixOrFullName)
		{
			return	type.of<TestClass_Members>().GetProperty(prefixOrFullName + "Property") ??
				type.of<TestClass_Members>().GetProperty(prefixOrFullName);
		}
	}
}

