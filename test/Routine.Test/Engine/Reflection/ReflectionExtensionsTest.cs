using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Routine.Engine;
using Routine.Test.Core;

namespace Routine.Test.Engine.Reflection
{
	[TestFixture]
	public class ReflectionExtensionsTest : CoreTestBase
	{
		#region Setup & Helpers

		private class NotParseableBecauseMethodDoesNotReturnItsOwnInstance
		{
			public static int Parse(string text) { return 0; }
		}

		private class NotParseableBecauseMethodDoesNotAcceptStringParameter
		{
			public static NotParseableBecauseMethodDoesNotAcceptStringParameter Parse(int i) { return null; }
		}

		private class NotParseableBecauseMethodIsNotStatic
		{
			public NotParseableBecauseMethodIsNotStatic Parse(string text) { return null; }
		}

		private class NotParseableBecauseMethodDoesNotHaveOneParameter
		{
			public static NotParseableBecauseMethodDoesNotHaveOneParameter Parse() { return null; }
			public static NotParseableBecauseMethodDoesNotHaveOneParameter Parse(string text, string text2) { return null; }
		}

		private class ParseableEvenIfThereAreOverloads
		{
			public static ParseableEvenIfThereAreOverloads Parse() { return null; }
			public static ParseableEvenIfThereAreOverloads Parse(int i) { return null; }
			public static ParseableEvenIfThereAreOverloads Parse(string text) { return null; }
		}

		public override void SetUp()
		{
			base.SetUp();

			TypeInfo.Optimize(GetType().Assembly.GetTypes().Where(t => t.Namespace.StartsWith("Routine.Test")).ToArray());
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

		private IMethod Method(string name, params TypeInfo[] parameterTypes)
		{
			return Method(name, type.ofvoid(), parameterTypes);
		}

		private IMethod Method(TypeInfo returnType, params TypeInfo[] parameterTypes)
		{
			return Method("Dummy", returnType, parameterTypes);
		}

		private IMethod Method(string name, TypeInfo returnType, params TypeInfo[] parameterTypes)
		{
			var result = new Mock<IMethod>();

			var parameters = parameterTypes.Select(p => Parameter(p)).ToList();
			result.Setup(o => o.Parameters).Returns(parameters);
			result.Setup(o => o.ReturnType).Returns(returnType);
			result.Setup(o => o.Name).Returns(name);

			return result.Object;
		}

		#endregion

		[Test]
		public void Test_ToCSharpString()
		{
			Assert.AreEqual("global::System.Nullable<global::System.Int32>", typeof(int?).ToCSharpString());
			Assert.AreEqual("Nullable<Int32>", typeof(int?).ToCSharpString(false));
		}

		[Test]
		public void Test_CanParse()
		{
			Assert.IsTrue(typeof(int).CanParse());
			Assert.IsTrue(typeof(ParseableEvenIfThereAreOverloads).CanParse());

			Assert.IsFalse(typeof(NotParseableBecauseMethodDoesNotReturnItsOwnInstance).CanParse());
			Assert.IsFalse(typeof(NotParseableBecauseMethodDoesNotAcceptStringParameter).CanParse());
			Assert.IsFalse(typeof(NotParseableBecauseMethodIsNotStatic).CanParse());
			Assert.IsFalse(typeof(NotParseableBecauseMethodDoesNotHaveOneParameter).CanParse());
		}

		[Test]
		public void Test_IsNullable()
		{
			Assert.IsTrue(typeof(int?).IsNullable());
			Assert.IsFalse(typeof(int).IsNullable());
			Assert.IsFalse(typeof(List<int>).IsNullable());
		}

		[Test]
		public void Test_IParametric_HasParameters()
		{
			Assert.IsTrue(Method(type.ofvoid()).HasNoParameters());
			Assert.IsTrue(Method(type.ofvoid(), type.of<string>()).HasParameters<string>());
			Assert.IsTrue(Method(type.ofvoid(), type.of<string>(), type.of<int>()).HasParameters<string, int>());
			Assert.IsTrue(Method(type.ofvoid(), type.of<string>(), type.of<int>(), type.of<double>()).HasParameters<string, int, double>());
			Assert.IsTrue(Method(type.ofvoid(), type.of<string>(), type.of<int>(), type.of<double>(), type.of<decimal>()).HasParameters<string, int, double, decimal>());

			Assert.IsFalse(Method(type.ofvoid(), type.of<string>(), type.of<int>(), type.of<double>()).HasParameters<string, int>());;
		}

		[Test]
		public void Test_IParametric_ReturnsVoid()
		{
			Assert.IsTrue(Method(type.ofvoid()).ReturnsVoid());
			Assert.IsFalse(Method(type.of<string>()).ReturnsVoid());
		}

		[Test]
		public void Test_IReturnable_Returns()
		{
			Assert.IsTrue(Method(type.of<string>()).Returns(type.of<object>()));
			Assert.IsFalse(Method(type.of<int>()).Returns(type.of<string>()));

			Assert.IsTrue(Method(type.of<List<string>>()).ReturnsCollection());
			Assert.IsTrue(Method(type.of<List<string>>()).ReturnsCollection(type.of<object>()));
			Assert.IsFalse(Method(type.of<IList>()).ReturnsCollection(type.of<string>()));

			//generic
			Assert.IsTrue(Method(type.of<string>()).Returns<string>());
			Assert.IsTrue(Method(type.of<List<string>>()).ReturnsCollection<string>());

			//with name parameter
			Assert.IsFalse(Method("Right", type.of<string>()).Returns(type.of<string>(), "Wrong"));
			Assert.IsFalse(Method("Right", type.of<List<string>>()).ReturnsCollection(type.of<string>(), "Wrong"));
		}

		[Test]
		public void Test_ITypeComponent_Has()
		{
			Assert.IsTrue(TypeComponent(new AttributeUsageAttribute(AttributeTargets.Method)).Has<AttributeUsageAttribute>());
			Assert.IsTrue(TypeComponent(new AttributeUsageAttribute(AttributeTargets.Method)).Has(type.of<AttributeUsageAttribute>()));

			Assert.IsFalse(TypeComponent().Has<AttributeUsageAttribute>());
			Assert.IsFalse(TypeComponent().Has(type.of<AttributeUsageAttribute>()));
		}
	}
}

