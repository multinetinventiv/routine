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
		public void SystemMethodInfoIsWrappedByRoutineMethodInfo()
		{
			Assert.AreSame(methodInfo.Name, testing.Name);
			Assert.AreSame(methodInfo.DeclaringType, testing.DeclaringType.GetActualType());
			Assert.AreSame(methodInfo.ReflectedType, testing.ReflectedType.GetActualType());
			Assert.AreSame(methodInfo.ReturnType, testing.ReturnType.GetActualType());
		}

		[Test]
		public void SystemMethodInfoGetParametersIsWrappedByRoutineMethodInfo()
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
		public void RoutineMethodInfoCachesWrappedProperties()
		{
			Assert.AreSame(testing.Name, testing.Name);
			Assert.AreSame(testing.DeclaringType, testing.DeclaringType);
			Assert.AreSame(testing.ReflectedType, testing.ReflectedType);
			Assert.AreSame(testing.ReturnType, testing.ReturnType);
			Assert.AreSame(testing.GetParameters(), testing.GetParameters());
		}

		[Test]
		public void RoutineMethodInfoCanInvokeStaticMethods()
		{
			testing = OOP_StaticMethod("PublicStaticPingMethod");

			Assert.AreEqual("static test", testing.InvokeStatic("test"));
		}

		[Test]
		public void RoutineMethodInfoCanInvokeInstanceMethods()
		{
			testing = OOP_Method("PublicPingMethod");

			var obj = new TestClass_OOP();

			Assert.AreEqual("instance test", testing.Invoke(obj, "test"));
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
	}
}

