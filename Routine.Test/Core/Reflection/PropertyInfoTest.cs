using System;
using System.Linq;
using NUnit.Framework;
using Routine.Core.Reflection;
using Routine.Test.Core.Reflection.Domain;

namespace Routine.Test.Core.Reflection
{
	[TestFixture]
	public class PropertyInfoTest : ReflectionTestBase
	{
		private System.Reflection.PropertyInfo propertyInfo;
		private PropertyInfo testing;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			propertyInfo = typeof(TestClass_OOP).GetProperty("PublicProperty");
			testing = type.of<TestClass_OOP>().GetProperty("PublicProperty");
		}

		[Test]
		public void SystemPropertyInfoIsWrappedByRoutinePropertyInfo()
		{
			Assert.AreEqual(propertyInfo.Name, testing.Name);
			Assert.AreEqual(propertyInfo.GetGetMethod().Name, testing.GetGetMethod().Name);
			Assert.AreEqual(propertyInfo.GetSetMethod().Name, testing.GetSetMethod().Name);
			Assert.AreSame(propertyInfo.DeclaringType, testing.DeclaringType.GetActualType());
			Assert.AreSame(propertyInfo.ReflectedType, testing.ReflectedType.GetActualType());
			Assert.AreSame(propertyInfo.PropertyType, testing.PropertyType.GetActualType());
		}

		[Test]
		public void SystemPropertyInfoGetIndexParametersIsWrappedByRoutinePropertyInfo()
		{
			propertyInfo = typeof(TestClass_OOP).GetProperty("Item");
			testing = type.of<TestClass_OOP>().GetProperty("Item");

			var expected = propertyInfo.GetIndexParameters();
			var actual = testing.GetIndexParameters();

			foreach(var parameter in actual)
			{
				Assert.IsTrue(expected.Any(p => p.ParameterType == parameter.ParameterType.GetActualType()), parameter.Name + " was not expected in index parameters of " + propertyInfo);
			}

			foreach(var parameter in expected)
			{
				Assert.IsTrue(actual.Any(p => p.ParameterType.GetActualType() == parameter.ParameterType), parameter.Name + " was expected in index parameters of " + propertyInfo);
			}
		}

		[Test]
		public void RoutinePropertyInfoCachesWrappedProperties()
		{
			Assert.AreSame(testing.Name, testing.Name);
			Assert.AreSame(testing.GetGetMethod(), testing.GetGetMethod());
			Assert.AreSame(testing.GetSetMethod(), testing.GetSetMethod());
			Assert.AreSame(testing.DeclaringType, testing.DeclaringType);
			Assert.AreSame(testing.ReflectedType, testing.ReflectedType);
			Assert.AreSame(testing.PropertyType, testing.PropertyType);
			Assert.AreSame(testing.GetIndexParameters(), testing.GetIndexParameters());
		}

		[Test]
		public void RoutinePropertyInfoCanGetValue()
		{
			var obj = new TestClass_OOP {
				PublicProperty = "expected_get"
			};

			Assert.AreEqual("expected_get", testing.GetValue(obj));
		}

		[Test]
		public void RoutinePropertyInfoCanGetStaticValue()
		{
			testing = OOP_StaticProperty("PublicStaticProperty");

			TestClass_OOP.PublicStaticProperty = "expected_get";

			Assert.AreEqual("expected_get", testing.GetStaticValue());
		}

		[Test]
		public void RoutinePropertyInfoCanSetValue()
		{
			var obj = new TestClass_OOP();

			testing.SetValue(obj, "expected_set");

			Assert.AreEqual("expected_set", obj.PublicProperty);
		}

		[Test]
		public void RoutinePropertyInfoCanSetStaticValue()
		{
			testing = OOP_StaticProperty("PublicStaticProperty");

			testing.SetStaticValue("expected_set");

			Assert.AreEqual("expected_set", TestClass_OOP.PublicStaticProperty);
		}

		[Test]
		public void Extension_IsPubliclyReadable()
		{
			Assert.IsTrue(Members_Property("PublicReadOnly").IsPubliclyReadable);
			Assert.IsFalse(Members_Property("PublicWriteOnly").IsPubliclyReadable);
			Assert.IsFalse(Members_Property("PrivateGet").IsPubliclyReadable);
		}

		[Test]
		public void Extension_IsPubliclyWritable()
		{
			Assert.IsFalse(Members_Property("PublicReadOnly").IsPubliclyWritable);
			Assert.IsTrue(Members_Property("PublicWriteOnly").IsPubliclyWritable);
			Assert.IsFalse(Members_Property("PrivateSet").IsPubliclyWritable);
		}

		[Test]
		public void Extension_IsOnReflected()
		{
			Assert.IsTrue(OOP_Property("Public").IsOnReflected());
			Assert.IsTrue(OOP_Property("Overridden").IsOnReflected());
			Assert.IsTrue(OOP_Property("ImplicitInterface").IsOnReflected());
			Assert.IsFalse(OOP_Property("NotOverridden").IsOnReflected());

			Assert.IsTrue(OOP_Property("Public").IsOnReflected(true));
			Assert.IsFalse(OOP_Property("Overridden").IsOnReflected(true));
			Assert.IsFalse(OOP_Property("ImplicitInterface").IsOnReflected(true));
			Assert.IsFalse(OOP_Property("NotOverridden").IsOnReflected(true));
		}

		[Test]
		public void Extension_IsWithinRootNamespace()
		{
			Assert.IsTrue(OOP_Property("Public").IsWithinRootNamespace());
			Assert.IsTrue(OOP_Property("Overridden").IsWithinRootNamespace());
			Assert.IsTrue(OOP_Property("OtherNamespace").IsWithinRootNamespace());

			Assert.IsTrue(OOP_Property("Public").IsWithinRootNamespace(true));
			Assert.IsTrue(OOP_Property("Overridden").IsWithinRootNamespace(true));
			Assert.IsFalse(OOP_Property("OtherNamespace").IsWithinRootNamespace(true));
		}

		[Test]
		public void Extension_IsIndexer()
		{
			Assert.IsFalse(OOP_Property("Public").IsIndexer);
			Assert.IsTrue(OOP_Property("Item").IsIndexer);
		}

		[Test]
		public void Extension_Returns()
		{
			Assert.IsTrue(Members_Property("String").Returns(type.of<object>()));
			Assert.IsFalse(Members_Property("Int").Returns(type.of<string>()));

			Assert.IsTrue(Members_Property("StringList").ReturnsCollection());
			Assert.IsTrue(Members_Property("StringList").ReturnsCollection(type.of<object>()));
			Assert.IsFalse(Members_Property("NonGenericList").ReturnsCollection(type.of<string>()));

			//generics
			Assert.IsTrue(Members_Property("String").Returns<string>());
			Assert.IsTrue(Members_Property("StringList").ReturnsCollection<string>());

			//with name parameter
			Assert.IsFalse(Members_Property("String").Returns(type.of<string>(), "Wrong.*"));
			Assert.IsFalse(Members_Property("StringList").ReturnsCollection(type.of<string>(), "Wrong.*"));		
		}
	}
}

