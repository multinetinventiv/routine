using System;
using Moq;
using NUnit.Framework;
using Routine.Core.Configuration;
using Routine.Engine;
using Routine.Engine.Virtual;
using Routine.Test.Core;

namespace Routine.Test.Engine.Virtual
{
	[TestFixture]
	public class VirtualTypeTest : CoreTestBase
	{
		[Test]
		public void Virtual_types_are_public()
		{
			IType testing = BuildRoutine.VirtualType().FromBasic();

			Assert.IsTrue(testing.IsPublic);
		}

		[Test]
		public void Virtual_types_are_domain_type()
		{
			IType testing = BuildRoutine.VirtualType().FromBasic();

			Assert.IsTrue(testing.IsDomainType);
		}

		[Test]
		public void Virtual_types_cannot_be_array()
		{
			IType testing = BuildRoutine.VirtualType().FromBasic();

			Assert.IsFalse(testing.IsArray);
		}

		[Test]
		public void Virtual_types_cannot_be_void()
		{
			IType testing = BuildRoutine.VirtualType().FromBasic();

			Assert.IsFalse(testing.IsVoid);
		}

		[Test]
		public void Name_is_required()
		{
			IType testing = BuildRoutine.VirtualType().FromBasic()
				.Name.Set("Test")
			;

			Assert.AreEqual("Test", testing.Name);

			testing = BuildRoutine.VirtualType().FromBasic();

			Assert.Throws<ConfigurationException>(() => { var dummy = testing.Name; });
		}

		[Test]
		public void Namespace_is_required()
		{
			IType testing = BuildRoutine.VirtualType().FromBasic()
				.Namespace.Set("Routine")
			;

			Assert.AreEqual("Routine", testing.Namespace);

			testing = BuildRoutine.VirtualType().FromBasic();

			Assert.Throws<ConfigurationException>(() => { var dummy = testing.Namespace; });
		}

		[Test]
		public void FullName_is_built_using_namespace_and_name()
		{
			IType testing = BuildRoutine.VirtualType().FromBasic()
				.Name.Set("Test")
				.Namespace.Set("Routine")
			;

			Assert.AreEqual("Routine.Test", testing.FullName);
		}

		[Test]
		public void IsInterface_is_optional__default_is_false()
		{
			IType testing = BuildRoutine.VirtualType().FromBasic()
				.IsInterface.Set(true)
			;

			Assert.IsTrue(testing.IsInterface);
			
			testing = BuildRoutine.VirtualType().FromBasic();

			Assert.IsFalse(testing.IsInterface);
		}

		[Test]
		public void Creates_virtual_object()
		{
			IType testing = BuildRoutine.VirtualType().FromBasic()
				.DefaultInstanceId.Set("default")
			;

			var actual = testing.CreateInstance();

			Assert.IsInstanceOf<VirtualObject>(actual);
			Assert.AreEqual("default", (actual as VirtualObject).Id);
		}

		[Test]
		public void To_string_returns_VirtualType_full_name_by_default()
		{
			IType testing = BuildRoutine.VirtualType().FromBasic()
				.DefaultInstanceId.Set("default")
			;

			var actual = testing.CreateInstance();

			Assert.AreEqual(typeof(VirtualType).FullName, actual.ToString());
		}

		[Test]
		public void Operations_are_created_externally_and_added_to_type()
		{
			var operationMock = new Mock<IOperation>();

			IType testing = BuildRoutine.VirtualType().FromBasic()
				.Operations.Add(operationMock.Object)
			;

			Assert.AreEqual(1, testing.Operations.Count);
			Assert.AreSame(operationMock.Object, testing.Operations[0]);
		}

		[Test]
		public void Can_be_itself_and_object()
		{
			IType testing = BuildRoutine.VirtualType().FromBasic();

			Assert.IsTrue(testing.CanBe(type.of<object>()));
			Assert.IsFalse(testing.CanBe(type.of<string>()));
		}

		[Test]
		public void Virtual_types_support_formatting_and_equality_members()
		{
			IType testing = BuildRoutine.VirtualType().FromBasic()
				.Name.Set("Virtual")
				.Namespace.Set("Routine")
			;

			Assert.AreEqual("v-Routine.Virtual", testing.ToString());

			IType clone = BuildRoutine.VirtualType().FromBasic()
				.Name.Set("Virtual")
				.Namespace.Set("Routine")
			;

			Assert.AreEqual(testing.GetHashCode(), clone.GetHashCode());
			Assert.AreEqual(testing, clone);
		}

		[Test]
		public void Not_supported_features()
		{
			IType testing = BuildRoutine.VirtualType().FromBasic();

			Assert.IsNull(testing.ParentType);
			Assert.AreEqual(0, testing.GetCustomAttributes().Length);
			Assert.IsFalse(testing.IsAbstract);
			Assert.IsFalse(testing.IsEnum);
			Assert.IsFalse(testing.IsGenericType);
			Assert.IsFalse(testing.IsPrimitive);
			Assert.IsFalse(testing.IsValueType);
			Assert.AreEqual(type.of<object>(), testing.BaseType);
			Assert.AreEqual(0, testing.ConvertibleTypes.Count);
			Assert.AreEqual(0, testing.GetGenericArguments().Count);
			Assert.IsNull(testing.GetElementType());
			Assert.IsNull(testing.GetParseOperation());
			Assert.AreEqual(0, testing.GetEnumNames().Count);
			Assert.AreEqual(0, testing.GetEnumValues().Count);
			Assert.IsNull(testing.GetEnumUnderlyingType());
			Assert.IsTrue(testing.CanBe(testing));
			Assert.AreEqual("test", testing.Convert("test", type.of<string>()));
			Assert.Throws<NotSupportedException>(() => testing.CreateListInstance(10));
			Assert.AreEqual(0, testing.Initializers.Count);
			Assert.AreEqual(0, testing.Members.Count);
		}
	}
}