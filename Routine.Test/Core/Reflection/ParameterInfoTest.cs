using System;
using NUnit.Framework;
using Routine.Core.Reflection;
using Routine.Test.Core.Reflection.Domain;

namespace Routine.Test.Core.Reflection.Domain
{
	public class ReflectedParameter
	{
		public void AMethod(string theParameter){}
	}
}
namespace Routine.Test.Core.Reflection
{
	[TestFixture]
	public class ParameterInfoTest : CoreTestBase
	{
		public override string[] DomainTypeRootNamespaces{get{return new[]{"Routine.Test.Core.Reflection.Domain"};}}

		private System.Reflection.ParameterInfo parameterInfo;
		private ParameterInfo testing;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			parameterInfo = typeof(ReflectedParameter).GetMethod("AMethod").GetParameters()[0];
			testing = type.of<ReflectedParameter>().GetMethod("AMethod").GetParameters()[0];
		}

		[Test]
		public void SystemParameterInfoIsWrappedByRoutineParameterInfo()
		{
			Assert.AreEqual(parameterInfo.Name, testing.Name);
			Assert.AreSame(parameterInfo.ParameterType, testing.ParameterType.GetActualType());
			Assert.AreEqual(parameterInfo.Position, testing.Position);
		}

		[Test]
		public void RoutineParameterInfoCachesWrappedProperties()
		{
			Assert.AreSame(testing.Name, testing.Name);
			Assert.AreSame(testing.ParameterType, testing.ParameterType);
		}
	}
}

