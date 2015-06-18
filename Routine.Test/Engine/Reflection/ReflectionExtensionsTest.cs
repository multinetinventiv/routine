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

		public override void SetUp()
		{
			base.SetUp();

			TypeInfo.AddDomainTypes(GetType().Assembly.GetTypes().Where(t => t.Namespace.StartsWith("Routine.Test")).ToArray());
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

		private IOperation Operation(string name, params TypeInfo[] parameterTypes)
		{
			return Operation(name, type.ofvoid(), parameterTypes);
		}

		private IOperation Operation(TypeInfo returnType, params TypeInfo[] parameterTypes)
		{
			return Operation("Dummy", returnType, parameterTypes);
		}

		private IOperation Operation(string name, TypeInfo returnType, params TypeInfo[] parameterTypes)
		{
			var result = new Mock<IOperation>();

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
			Assert.AreEqual("System.Nullable<System.Int32>", typeof(int?).ToCSharpString());
			Assert.AreEqual("Nullable<Int32>", typeof(int?).ToCSharpString(false));
		}

		[Test]
		public void Test_IParametric_HasParameters()
		{
			Assert.IsTrue(Operation(type.ofvoid()).HasNoParameters());
			Assert.IsTrue(Operation(type.ofvoid(), type.of<string>()).HasParameters<string>());
			Assert.IsTrue(Operation(type.ofvoid(), type.of<string>(), type.of<int>()).HasParameters<string, int>());
			Assert.IsTrue(Operation(type.ofvoid(), type.of<string>(), type.of<int>(), type.of<double>()).HasParameters<string, int, double>());
			Assert.IsTrue(Operation(type.ofvoid(), type.of<string>(), type.of<int>(), type.of<double>(), type.of<decimal>()).HasParameters<string, int, double, decimal>());

			Assert.IsFalse(Operation(type.ofvoid(), type.of<string>(), type.of<int>(), type.of<double>()).HasParameters<string, int>());;
		}

		[Test]
		public void Test_IParametric_ReturnsVoid()
		{
			Assert.IsTrue(Operation(type.ofvoid()).ReturnsVoid());
			Assert.IsFalse(Operation(type.of<string>()).ReturnsVoid());
		}

		[Test]
		public void Test_IReturnable_Returns()
		{
			Assert.IsTrue(Operation(type.of<string>()).Returns(type.of<object>()));
			Assert.IsFalse(Operation(type.of<int>()).Returns(type.of<string>()));

			Assert.IsTrue(Operation(type.of<List<string>>()).ReturnsCollection());
			Assert.IsTrue(Operation(type.of<List<string>>()).ReturnsCollection(type.of<object>()));
			Assert.IsFalse(Operation(type.of<IList>()).ReturnsCollection(type.of<string>()));

			//generic
			Assert.IsTrue(Operation(type.of<string>()).Returns<string>());
			Assert.IsTrue(Operation(type.of<List<string>>()).ReturnsCollection<string>());

			//with name parameter
			Assert.IsFalse(Operation("Right", type.of<string>()).Returns(type.of<string>(), "Wrong"));
			Assert.IsFalse(Operation("Right", type.of<List<string>>()).ReturnsCollection(type.of<string>(), "Wrong"));
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

