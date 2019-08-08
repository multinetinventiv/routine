using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Routine.Api.Template;
using Routine.Client;

namespace Routine.Test.Api.Template
{
	[TestFixture]
	public class ClientApiApiClassFeatureTest : ClientApiFeatureTestBase
	{
		protected override string DefaultNamespace { get { return "Routine.Test.Api.Template.ClientApiApiClassFeatureTest_Generated"; } }

		[Test]
		public void An_api_type_and_interface_is_rendered_by_default()
		{
			var assembly = Generator().Generate(new ClientApiTemplate("TestApi"));

			var testApi = GetRenderedType(assembly, "TestApi");
			var iTestApi = GetRenderedType(assembly, "ITestApi");

			Assert.IsNotNull(testApi);
			Assert.IsNotNull(iTestApi);
			Assert.IsTrue(iTestApi.IsAssignableFrom(testApi));
			Assert.AreEqual(DefaultNamespace, testApi.Namespace);
			Assert.AreEqual(DefaultNamespace, iTestApi.Namespace);
			Assert.IsNotNull(testApi.GetConstructor(new[] { typeof(Rapplication) }));
		}

		[Test]
		public void Api_interface_gives_access_to_instances_of_operational_types()
		{
			ModelsAre(
				Model("TestClass1").Name("TestClass1")
				.Data("Name", "TestClass2"),
				Model("TestClass2").Name("TestClass2").Data("Self", "TestClass2")
			);

			ObjectsAre(
				Object(Id("instance1", "TestClass1")).Display("instance1_value"),
				Object(Id("instance2", "TestClass1")).Display("instance2_value")
			);

			var assembly = Generator().Generate(new ClientApiTemplate("TestApi"));

			var testApi = GetRenderedType(assembly, "TestApi");
			var iTestApi = GetRenderedType(assembly, "ITestApi");

			Assert.IsNull(testApi.GetMethod("Get", new[] { typeof(string) }));

			var get = iTestApi.GetMethod("Get", new[] { typeof(string) });

			Assert.IsNotNull(get);
			Assert.IsTrue(get.IsGenericMethod);
			Assert.AreEqual(1, get.GetParameters().Length);
			Assert.IsTrue(get.GetParameters().Any(p => p.Name == "instanceId" && p.ParameterType == typeof(string)));

			var apiObj = Activator.CreateInstance(testApi, testingRapplication);

			var testClass1 = GetRenderedType(assembly, "TestClass1Impl");
			var iTestClass1 = GetRenderedType(assembly, "TestClass1");

			var getTestClass1 = get.MakeGenericMethod(iTestClass1);
			var instance1 = getTestClass1.Invoke(apiObj, new object[] { "instance1" });
			var instance2 = getTestClass1.Invoke(apiObj, new object[] { "instance2" });

			Assert.IsInstanceOf(testClass1, instance1);
			Assert.IsInstanceOf(testClass1, instance2);

			Assert.AreEqual("instance1_value", instance1.ToString());
			Assert.AreEqual("instance2_value", instance2.ToString());
		}

		[Test]
		public void When_a_rendered_type_has_only_one_static_instance_id__that_type_is_treated_as_singleton()
		{
			ModelsAre(
				Model("TestClass1").Name("TestClass1")
				.StaticInstanceIds("instance1")
				.Data("Name", "TestClass2"),
				Model("TestClass2").Name("TestClass2").Data("Self", "TestClass2")
				.StaticInstanceIds("instance2", "instance3")
			);

			ObjectsAre(
				Object(Id("instance1", "TestClass1")).Display("instance1_value"),
				Object(Id("instance2", "TestClass2")).Display("instance2_value"),
				Object(Id("instance3", "TestClass2")).Display("instance3_value")
			);

			var testing = Generator();

			var assembly = testing.Generate(new ClientApiTemplate("TestApi"));

			var testApi = GetRenderedType(assembly, "TestApi");
			var iTestApi = GetRenderedType(assembly, "ITestApi");

			var apiObj = Activator.CreateInstance(testApi, testingRapplication);

			var testClass1 = GetRenderedType(assembly, "TestClass1");
			var testClass2 = GetRenderedType(assembly, "TestClass2");

			var get = iTestApi.GetMethod("Get", new Type[0]);
			var getTestClass1 = get.MakeGenericMethod(testClass1);

			var instance1 = getTestClass1.Invoke(apiObj, new object[0]);

			Assert.AreEqual("instance1_value", instance1.ToString());

			var getTestClass2 = get.MakeGenericMethod(testClass2);
			try
			{
				getTestClass2.Invoke(apiObj, new object[0]);
				Assert.Fail("exception not thrown");
			}
			catch (TargetInvocationException ex)
			{
				Assert.AreEqual("SingletonException", ex.InnerException.GetType().Name);
				Assert.IsTrue(ex.InnerException.Message.Contains(testClass2.FullName));
			}
		}

		protected override void Referenced_client_api_support_case()
		{
			Assert.Pass("Each client api assembly has its own api class");
		}

		protected override void List_input_and_output_case()
		{
			Assert.Pass("This feature has nothing to do with list input and output case");
		}

		protected override void Attribute_case()
		{
			Assert.Pass("This feature has nothing to do with attribute case");
		}
	}
}