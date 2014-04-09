using System.Linq;
using NUnit.Framework;
using Routine.Core.Reflection;
using Routine.Test.Core.Reflection.Domain;

namespace Routine.Test.Core.Reflection
{
	[TestFixture]
	public class MethodInfoTest : ReflectionTestBase
	{
		private System.Reflection.MethodInfo methodInfo;
		private MethodInfo testing;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			methodInfo = typeof(TestClass_OOP).GetMethod("PublicMethod");
			testing = type.of<TestClass_OOP>().GetMethod("PublicMethod");
		}

		[Test]
		public void System_MethodInfo_is_wrapped_by_Routine_MethodInfo()
		{
			Assert.AreSame(methodInfo.Name, testing.Name);
			Assert.AreSame(methodInfo.DeclaringType, testing.DeclaringType.GetActualType());
			Assert.AreSame(methodInfo.ReflectedType, testing.ReflectedType.GetActualType());
			Assert.AreSame(methodInfo.ReturnType, testing.ReturnType.GetActualType());
		}

		[Test]
		public void System_MethodInfo_GetParameters_is_wrapped_by_Routine_MethodInfo()
		{
			methodInfo = typeof(TestClass_Members).GetMethod("FiveParameterMethod");
			testing = type.of<TestClass_Members>().GetMethod("FiveParameterMethod");

			var expected = methodInfo.GetParameters();
			var actual = testing.GetParameters();

			foreach(var parameter in actual)
			{
				Assert.IsTrue(expected.Any(p => p.ParameterType == parameter.ParameterType.GetActualType()), parameter.Name + " was not expected in parameters of " + methodInfo);
			}

			foreach(var parameter in expected)
			{
				Assert.IsTrue(actual.Any(p => p.ParameterType.GetActualType() == parameter.ParameterType), parameter.Name + " was expected in index parameters of " + methodInfo);
			}
		}

		[Test]
		public void Routine_MethodInfo_caches_wrapped_properties()
		{
			Assert.AreSame(testing.Name, testing.Name);
			Assert.AreSame(testing.DeclaringType, testing.DeclaringType);
			Assert.AreSame(testing.ReflectedType, testing.ReflectedType);
			Assert.AreSame(testing.ReturnType, testing.ReturnType);
			Assert.AreSame(testing.GetParameters(), testing.GetParameters());
			Assert.AreSame(Attribute_Method("Class").GetCustomAttributes(), Attribute_Method("Class").GetCustomAttributes());
		}

		[Test]
		public void Routine_MethodInfo_can_invoke_static_methods()
		{
			testing = OOP_StaticMethod("PublicStaticPingMethod");

			Assert.AreEqual("static test", testing.InvokeStatic("test"));
		}

		[Test]
		public void Routine_MethodInfo_can_invoke_instance_methods()
		{
			testing = OOP_Method("PublicPingMethod");

			var obj = new TestClass_OOP();

			Assert.AreEqual("instance test", testing.Invoke(obj, "test"));
		}

		[Test]
		public void Routine_MethodInfo_lists_custom_attributes_with_inherit_behaviour()
		{
			testing = Attribute_Method("Class");

			var actual = testing.GetCustomAttributes();

			Assert.AreEqual(1, actual.Length);
			Assert.IsInstanceOf<TestClassAttribute>(actual[0]);

			testing = Attribute_Method("Base");

			actual = testing.GetCustomAttributes();

			Assert.AreEqual(1, actual.Length);
			Assert.IsInstanceOf<TestBaseAttribute>(actual[0]);

			testing = Attribute_Method("Overridden");

			actual = testing.GetCustomAttributes();

			Assert.AreEqual(2, actual.Length);
			Assert.IsInstanceOf<TestClassAttribute>(actual[0]);
			Assert.IsInstanceOf<TestBaseAttribute>(actual[1]);

			testing = Attribute_InterfaceMethod("Interface");

			actual = testing.GetCustomAttributes();

			Assert.AreEqual(1, actual.Length);
			Assert.IsInstanceOf<TestInterfaceAttribute>(actual[0]);
		}

		[Test]
		public void Routine_MethodInfo_lists_return_type_custom_attributes_with_inherit_behaviour()
		{
			testing = Attribute_Method("Class");

			var actual = testing.GetReturnTypeCustomAttributes();
			
			Assert.AreEqual(1, actual.Length);
			Assert.IsInstanceOf<TestClassAttribute>(actual[0]);
		}

		[Test]
		public void Extension_IsOnReflected()
		{
			Assert.IsTrue(OOP_Method("Public").IsOnReflected());
			Assert.IsTrue(OOP_Method("Overridden").IsOnReflected());
			Assert.IsTrue(OOP_Method("ImplicitInterface").IsOnReflected());
			Assert.IsTrue(OOP_Method("ImplicitInterfaceWithParameter").IsOnReflected());
			Assert.IsFalse(OOP_Method("NotOverridden").IsOnReflected());

			Assert.IsTrue(OOP_Method("Public").IsOnReflected(true));
			Assert.IsFalse(OOP_Method("Overridden").IsOnReflected(true));
			Assert.IsFalse(OOP_Method("ImplicitInterface").IsOnReflected(true));
			Assert.IsFalse(OOP_Method("ImplicitInterfaceWithParameter").IsOnReflected(true));
			Assert.IsFalse(OOP_Method("NotOverridden").IsOnReflected(true));
		}

		[Test]
		public void Extension_IsWithinNamespace()
		{
			Assert.IsTrue(OOP_Method("Public").IsWithinRootNamespace());
			Assert.IsTrue(OOP_Method("Overridden").IsWithinRootNamespace());
			Assert.IsTrue(OOP_Method("OtherNamespace").IsWithinRootNamespace());
			Assert.IsTrue(OOP_Method("ToString").IsWithinRootNamespace());
			Assert.IsFalse(OOP_Method("GetHashCode").IsWithinRootNamespace());

			Assert.IsTrue(OOP_Method("Public").IsWithinRootNamespace(true));
			Assert.IsTrue(OOP_Method("Overridden").IsWithinRootNamespace(true));
			Assert.IsFalse(OOP_Method("OtherNamespace").IsWithinRootNamespace(true));
			Assert.IsFalse(OOP_Method("ToString").IsWithinRootNamespace(true));
			Assert.IsFalse(OOP_Method("GetHashCode").IsWithinRootNamespace(true));
		}

		[Test]
		public void Extension_HasParameters()
		{
			Assert.IsTrue(Members_Method("Parameterless").HasNoParameters());
			Assert.IsTrue(Members_Method("OneParameter").HasParameters<string>());
			Assert.IsTrue(Members_Method("TwoParameter").HasParameters<string, int>());
			Assert.IsTrue(Members_Method("ThreeParameter").HasParameters<string, int, double>());
			Assert.IsTrue(Members_Method("FourParameter").HasParameters<string, int, double, decimal>());

			Assert.IsFalse(Members_Method("ThreeParameter").HasParameters<string, int>());;
		}

		[Test]
		public void Extension_Returns()
		{
			Assert.IsTrue(Members_Method("Void").ReturnsVoid());
			Assert.IsFalse(Members_Method("String").ReturnsVoid());

			Assert.IsTrue(Members_Method("String").Returns(type.of<object>()));
			Assert.IsFalse(Members_Method("Int").Returns(type.of<string>()));

			Assert.IsTrue(Members_Method("StringList").ReturnsCollection());
			Assert.IsTrue(Members_Method("StringList").ReturnsCollection(type.of<object>()));
			Assert.IsFalse(Members_Method("NonGenericList").ReturnsCollection(type.of<string>()));

			//generics
			Assert.IsTrue(Members_Method("String").Returns<string>());
			Assert.IsTrue(Members_Method("StringList").ReturnsCollection<string>());

			//with name parameter
			Assert.IsFalse(Members_Method("String").Returns(type.of<string>(), "Wrong"));
			Assert.IsFalse(Members_Method("StringList").ReturnsCollection(type.of<string>(), "Wrong"));
		}

		[Test]
		public void Extension_Has()
		{
			Assert.IsTrue(Attribute_Method("Class").Has<TestClassAttribute>());
			Assert.IsTrue(Attribute_Method("Class").Has(type.of<TestClassAttribute>()));
		}

		[Test]
		public void Extension_ReturnTypeHas()
		{
			Assert.IsTrue(Attribute_Method("Class").ReturnTypeHas<TestClassAttribute>());
			Assert.IsTrue(Attribute_Method("Class").ReturnTypeHas(type.of<TestClassAttribute>()));
		}
	}
}

