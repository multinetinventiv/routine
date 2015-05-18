using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Routine.Api.Template;

namespace Routine.Test.Api.Template
{
	[TestFixture]
	public class ClientApiFactoryFeatureTest : ClientApiFeatureTestBase
	{
		protected override string DefaultNamespace { get { return "Routine.Test.Api.Template.ClientApiFactoryFeatureTest_Generated"; } }

		[Test]
		public void For_initializable_models__a_factory_is_generated_and_registered_as_singleton()
		{
			ModelsAre(
				Model("TestClass").Name("TestClass")
				.Member("Name", "s-string")
				.Initializer(PModel("name", "s-string")),
				Model("s-string").IsValue()
			);

			var testing = Generator(c => c
				.Use(p => p.ReferencedTypeByShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern()));

			var assembly = testing.Generate(new ClientApiTemplate("TestApi"));

			var iTestClassFactory = GetRenderedType(assembly, "TestClassFactory");

			Assert.IsNotNull(iTestClassFactory);
			Assert.IsTrue(iTestClassFactory.IsPublic);
			Assert.IsTrue(iTestClassFactory.IsInterface);

			var testClassFactory = GetRenderedType(assembly, "TestClassFactoryImpl");
			Assert.IsNotNull(testClassFactory);
			Assert.IsTrue(iTestClassFactory.IsAssignableFrom(testClassFactory));
			Assert.IsTrue(testClassFactory.IsNotPublic);

			var get = GetRenderedType(assembly, "ITestApi").GetMethod("Get", new[] { typeof(Type) });

			var iTestApiObj = Activator.CreateInstance(GetRenderedType(assembly, "TestApi"), testingRapplication);

			Assert.IsInstanceOf(testClassFactory, get.Invoke(iTestApiObj, new object[] { iTestClassFactory }));
		}

		[Test]
		public void Initializers_are_rendered_as_factory_method()
		{
			ModelsAre(
				Model("TestClass").Name("TestClass")
				.Member("Name", "s-string")
				.Initializer(PModel("name", "s-string")),
				Model("s-string").IsValue()
			);

			var testing =
				Generator(c => c
					.Use(p => p.ReferencedTypeByShortModelIdPattern("System", "s"))
					.Use(p => p.ParseableValueTypePattern()));

			var assembly = testing.Generate(DefaultTestTemplate);
			var iTestClassFactory = GetRenderedType(assembly, "TestClassFactory");
			var iTestClass = GetRenderedType(assembly, "TestClass");

			var @new = iTestClassFactory.GetMethod("New");

			Assert.IsNotNull(@new);
			Assert.AreEqual(1, @new.GetParameters().Length);
			Assert.AreEqual(typeof(string), @new.GetParameters()[0].ParameterType);
			Assert.AreEqual("name", @new.GetParameters()[0].Name);
			Assert.AreEqual(iTestClass, @new.ReturnType);
		}

		[Test]
		public void Initializer_groups_are_rendered_as_overloads()
		{
			ModelsAre(
				Model("TestClass")
				.Member("Name", "s-string")
				.Name("TestClass")
				.Initializer(PModel("name", "s-string", 0, 1), PModel("surname", "s-string", 1)),
				Model("s-string").IsValue()
			);

			var testing =
				Generator(c => c
					.Use(p => p.ReferencedTypeByShortModelIdPattern("System", "s"))
					.Use(p => p.ParseableValueTypePattern()));

			var assembly = testing.Generate(DefaultTestTemplate);
			var iTestClassFactory = GetRenderedType(assembly, "TestClassFactory");

			var news = iTestClassFactory.GetMethods().Where(mi => mi.Name == "New").ToList();

			Assert.AreEqual(2, news.Count);

			Assert.AreEqual(1, news[0].GetParameters().Length);
			Assert.AreEqual(typeof(string), news[0].GetParameters()[0].ParameterType);
			Assert.AreEqual("name", news[0].GetParameters()[0].Name);

			Assert.AreEqual(2, news[1].GetParameters().Length);
			Assert.AreEqual(typeof(string), news[1].GetParameters()[0].ParameterType);
			Assert.AreEqual("name", news[1].GetParameters()[0].Name);
			Assert.AreEqual(typeof(string), news[1].GetParameters()[1].ParameterType);
			Assert.AreEqual("surname", news[1].GetParameters()[1].Name);
		}

		[Test]
		public void Factory_method_uses_client_api_to_initialize_a_robject()
		{
			ModelsAre(
				Model("TestClass").Name("TestClass")
				.Member("Name", "s-string")
				.Initializer(PModel("name", "s-string")),
				Model("TestClass2").Name("TestClass2")
				.Operation("InitializedParameter", "s-string", PModel("arg1", "TestClass", true)),
				Model("s-string").IsValue()
			);

			ObjectsAre(
				Object(Id("test2", "TestClass2"))
			);

			When(Id("test2", "TestClass2"))
				.Performs("InitializedParameter", p =>
					p["arg1"].Values[0].ObjectModelId == "TestClass" &&
					p["arg1"].Values[0].InitializationParameters["name"].Values[0].ReferenceId == "name1" &&
					p["arg1"].Values[0].InitializationParameters["name"].Values[0].ObjectModelId == "s-string" &&
					p["arg1"].Values[1].ObjectModelId == "TestClass" &&
					p["arg1"].Values[1].InitializationParameters["name"].Values[0].ReferenceId == "name2" &&
					p["arg1"].Values[1].InitializationParameters["name"].Values[0].ObjectModelId == "s-string"
				).Returns(Result(Id("success", "s-string")));

			var testing =
				Generator(c => c
					.Use(p => p.ReferencedTypeByShortModelIdPattern("System", "s"))
					.Use(p => p.ParseableValueTypePattern()));

			var assembly = testing.Generate(DefaultTestTemplate);
			var iTestClassFactory = GetRenderedType(assembly, "TestClassFactory");
			var iTestClass = GetRenderedType(assembly, "TestClass");
			var iTestClass2 = GetRenderedType(assembly, "TestClass2");

			var initializedParameter = iTestClass2.GetMethod("InitializedParameter");
			var @new = iTestClassFactory.GetMethod("New");

			var testClass2Obj = CreateInstance(GetRenderedType(assembly, "TestClass2Impl"), "test2", "TestClass2");
			var testClassFactoryObj = Activator.CreateInstance(GetRenderedType(assembly, "TestClassFactoryImpl"), testingRapplication);

			var name1 = @new.Invoke(testClassFactoryObj, new object[] { "name1" });
			var name2 = @new.Invoke(testClassFactoryObj, new object[] { "name2" });

			var arg1 = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(iTestClass));
			arg1.Add(name1);
			arg1.Add(name2);

			var actual = (string)initializedParameter.Invoke(testClass2Obj, new object[] { arg1 });

			Assert.AreEqual("success", actual);
		}

		protected override void Referenced_client_api_support_case()
		{
			Assert.Pass("Each client api assembly has its own factory classes");
		}

		protected override void List_input_and_output_case()
		{
			Assert.Pass("This feature has nothing to do with list input and output case");
		}

		public class CustomAttribute : Attribute { }

		protected override void Attribute_case()
		{
			ModelsAre(
				Model("TestClass").Name("TestClass")
				.Member("Name", "TestClass")
				.Initializer(PModel("parameter", "TestClass"))
			);

			var assembly =
				Generator(c => c
					.RenderedInitializerAttributes.Add(type.of<CustomAttribute>())
					.RenderedParameterAttributes.Add(type.of<CustomAttribute>()))
				.AddReference<CustomAttribute>()
				.Generate(DefaultTestTemplate);

			var testClassFactory = GetRenderedType(assembly, "TestClassFactory");

			var @new = testClassFactory.GetMethod("New");
			var parameter = @new.GetParameters().Single(p => p.Name == "parameter");

			Assert.IsTrue(Attribute.IsDefined(@new, typeof(CustomAttribute)));
			Assert.IsTrue(Attribute.IsDefined(parameter, typeof(CustomAttribute)));
		}
	}
}