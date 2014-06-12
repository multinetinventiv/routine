using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Routine.Core.Reflection;
using Routine.Test.Core.Reflection.Domain;

namespace Routine.Test.Core.Reflection
{
	[TestFixture]
	public class ConstructorInfoTest : ReflectionTestBase
	{
		private System.Reflection.ConstructorInfo constructorInfo;
		private ConstructorInfo testing;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			constructorInfo = typeof(TestClass_OOP).GetConstructor(new[] { typeof(string) });
			testing = type.of<TestClass_OOP>().GetConstructor(new[] { type.of<string>() });
		}

		[Test]
		public void System_ConstructorInfo_is_wrapped_by_Routine_ConstructorInfo()
		{
			Assert.AreSame(constructorInfo.DeclaringType, testing.DeclaringType.GetActualType());
		}

		[Test]
		public void System_ConstructorInfo_GetParameters_is_wrapped_by_Routine_MethodInfo()
		{
			constructorInfo = typeof(TestClass_Members).GetConstructor(new[] { typeof(string), typeof(int) });
			testing = type.of<TestClass_Members>().GetConstructor(new[] { type.of<string>(), type.of<int>() });

			var expected = constructorInfo.GetParameters();
			var actual = testing.GetParameters();

			foreach (var parameter in actual)
			{
				Assert.IsTrue(expected.Any(p => p.ParameterType == parameter.ParameterType.GetActualType()), parameter.Name + " was not expected in parameters of " + constructorInfo);
			}

			foreach (var parameter in expected)
			{
				Assert.IsTrue(actual.Any(p => p.ParameterType.GetActualType() == parameter.ParameterType), parameter.Name + " was expected in index parameters of " + constructorInfo);
			}
		}

		[Test]
		public void Routine_ConstructorInfo_caches_wrapped_properties()
		{
			Assert.AreSame(testing.DeclaringType, testing.DeclaringType);
			Assert.AreSame(testing.GetParameters(), testing.GetParameters());
			Assert.AreSame(Attribute_Constructor().GetCustomAttributes(), Attribute_Constructor().GetCustomAttributes());
		}

		[Test]
		public void Routine_ConstructorInfo_can_be_invoked()
		{
			testing = Members_Constructor(type.of<string>(), type.of<int>());

			var actual = testing.Invoke("test", 1) as TestClass_Members;
			
			Assert.AreEqual("test", actual.StringProperty);
			Assert.AreEqual(1, actual.IntProperty);
		}

		[Test]
		public void Routine_ConstructorInfo_lists_custom_attributes()
		{
			testing = Attribute_Constructor();

			var actual = testing.GetCustomAttributes();

			Assert.AreEqual(1, actual.Length);
			Assert.IsInstanceOf<TestClassAttribute>(actual[0]);

			testing = Attribute_Constructor(type.of<int>());

			actual = testing.GetCustomAttributes();

			Assert.AreEqual(1, actual.Length);
			Assert.IsInstanceOf<TestClassAttribute>(actual[0]);
		}
	}
}
