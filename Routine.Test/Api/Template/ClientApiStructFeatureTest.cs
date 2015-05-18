using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Routine.Api.Template;

namespace Routine.Test.Api.Template
{
	[TestFixture]
	public class ClientApiStructFeatureTest : ClientApiFeatureTestBase
	{
		protected override string DefaultNamespace { get { return "Routine.Test.Api.Template.ClientApiStructFeatureTest_Generated"; } }

		[Test]
		public void Models_with_only_initializers_are_rendered_as_structs()
		{
			ModelsAre(
				Model("TestClass").Name("TestClass")
				.Initializer(PModel("name", "s-string")),
				Model("s-string").IsValue()
			);

			var testing = Generator(c => c
				.Use(p => p.ReferencedTypeByShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern()));

			var assembly = testing.Generate(DefaultTestTemplate);

			var testClass = GetRenderedType(assembly, "TestClass");

			Assert.IsNotNull(testClass);
			Assert.IsTrue(testClass.IsPublic);
			Assert.IsTrue(testClass.IsValueType);
		}

		[Test]
		public void Structs_have_constructor_for_each_initializer_group_and_a_readonly_property_for_each_parameter()
		{
			ModelsAre(
				Model("TestClass").Name("TestClass")
				.Initializer(PModel("name", "s-string", 0, 1), PModel("surname", "s-string", 1)),
				Model("s-string").IsValue()
			);

			var testing =
				Generator(c => c
					.Use(p => p.ReferencedTypeByShortModelIdPattern("System", "s"))
					.Use(p => p.ParseableValueTypePattern()));

			var assembly = testing.Generate(DefaultTestTemplate);
			var testClass = GetRenderedType(assembly, "TestClass");

			var ctors = testClass.GetConstructors();

			Assert.AreEqual(2, ctors.Length);

			Assert.AreEqual(1, ctors[0].GetParameters().Length);
			Assert.AreEqual(typeof(string), ctors[0].GetParameters()[0].ParameterType);
			Assert.AreEqual("name", ctors[0].GetParameters()[0].Name);

			Assert.AreEqual(2, ctors[1].GetParameters().Length);
			Assert.AreEqual(typeof(string), ctors[1].GetParameters()[0].ParameterType);
			Assert.AreEqual("name", ctors[1].GetParameters()[0].Name);
			Assert.AreEqual(typeof(string), ctors[1].GetParameters()[1].ParameterType);
			Assert.AreEqual("surname", ctors[1].GetParameters()[1].Name);

			var name = testClass.GetProperty("Name");

			Assert.IsNotNull(name);
			Assert.AreEqual(typeof(string), name.PropertyType);
			Assert.IsTrue(name.CanRead);
			Assert.IsTrue(name.GetSetMethod(true).IsPrivate);

			var surname = testClass.GetProperty("Surname");

			Assert.IsNotNull(surname);
			Assert.AreEqual(typeof(string), surname.PropertyType);
			Assert.IsTrue(surname.CanRead);
			Assert.IsTrue(surname.GetSetMethod(true).IsPrivate);

			var testClassObj = Activator.CreateInstance(testClass, "name", "surname");
			Assert.AreEqual("name", name.GetValue(testClassObj, new object[0]));
			Assert.AreEqual("surname", surname.GetValue(testClassObj, new object[0]));
		}

		[Test]
		public void Structs_are_rendered_as_public_so_they_dont_have_interface_and_are_used_directly_as_struct_when_used_in_parameters()
		{
			ModelsAre(
				   Model("TestClass").Name("TestClass")
				   .Initializer(PModel("name", "s-string")),
				   Model("TestClass2").Name("TestClass2")
				   .Operation("StructInput", "s-string", PModel("arg1", "TestClass")),
				   Model("s-string").IsValue()
			   );

			var testing =
				Generator(c => c
					.Use(p => p.ReferencedTypeByShortModelIdPattern("System", "s"))
					.Use(p => p.ParseableValueTypePattern()));

			var assembly = testing.Generate(DefaultTestTemplate);
			var testClass = GetRenderedType(assembly, "TestClass");
			var iTestClass2 = GetRenderedType(assembly, "TestClass2");
			var structInput = iTestClass2.GetMethod("StructInput");

			Assert.AreEqual(testClass, structInput.GetParameters()[0].ParameterType);
		}

		[Test]
		public void Structs_are_converted_to_robject_via_internal_methods()
		{
			ModelsAre(
				Model("TestClass").Name("TestClass")
				.Initializer(PModel("name", "s-string"), PModel("surname", "s-string")),
				Model("TestClass2").Name("TestClass2")
				.Operation("StructInput", "s-string", PModel("arg1", "TestClass")),
				Model("s-string").IsValue()
			);

			ObjectsAre(
				Object(Id("test2", "TestClass2"))
			);

			When(Id("test2", "TestClass2"))
				.Performs("StructInput", p =>
					p["arg1"].Values[0].ObjectModelId == "TestClass" &&
					p["arg1"].Values[0].InitializationParameters["name"].Values[0].ObjectModelId == "s-string" &&
					p["arg1"].Values[0].InitializationParameters["name"].Values[0].ReferenceId == "name" &&
					p["arg1"].Values[0].InitializationParameters["surname"].Values[0].ObjectModelId == "s-string" &&
					p["arg1"].Values[0].InitializationParameters["surname"].Values[0].ReferenceId == "surname"
				).Returns(Result(Id("success", "s-string")));


			var testing =
				Generator(c => c
					.Use(p => p.ReferencedTypeByShortModelIdPattern("System", "s"))
					.Use(p => p.ParseableValueTypePattern()));

			var assembly = testing.Generate(DefaultTestTemplate);
			var testClass = GetRenderedType(assembly, "TestClass");
			var testClassObj = Activator.CreateInstance(testClass, "name", "surname");

			var iTestClass2 = GetRenderedType(assembly, "TestClass2");
			var structInput = iTestClass2.GetMethod("StructInput");
			var test2 = CreateInstance(GetRenderedType(assembly, "TestClass2Impl"), "test2", "TestClass2");

			var actual = (string)structInput.Invoke(test2, new[] { testClassObj });

			Assert.AreEqual("success", actual);
		}

		[Test]
		public void Structs_have_constructor_overloads_for_each_initializer_group()
		{
			ModelsAre(
				Model("TestClass").Name("TestClass")
				.Initializer(PModel("name", "s-string", 0, 1), PModel("surname", "s-string", 1)),
				Model("s-string").IsValue()
			);

			var testing =
				Generator(c => c
					.Use(p => p.ReferencedTypeByShortModelIdPattern("System", "s"))
					.Use(p => p.ParseableValueTypePattern()));

			var assembly = testing.Generate(DefaultTestTemplate);
			var testClass = GetRenderedType(assembly, "TestClass");

			var ctors = testClass.GetConstructors();

			Assert.AreEqual(2, ctors.Length);

			var ctor1 = ctors.Single(ctor => ctor.GetParameters().Length == 2);
			Assert.AreEqual(typeof(string), ctor1.GetParameters()[0].ParameterType);
			Assert.AreEqual("name", ctor1.GetParameters()[0].Name);
			Assert.AreEqual(typeof(string), ctor1.GetParameters()[1].ParameterType);
			Assert.AreEqual("surname", ctor1.GetParameters()[1].Name);

			var ctor2 = ctors.Single(ctor => ctor.GetParameters().Length == 1);
			Assert.AreEqual(typeof(string), ctor2.GetParameters()[0].ParameterType);
			Assert.AreEqual("name", ctor2.GetParameters()[0].Name);
		}

		[Test]
		public void Structs_remember_from_which_of_its_constructor_it_is_created__and__uses_only_parameters_of_that_group_when_initializing_robject()
		{
			ModelsAre(
				Model("TestClass").Name("TestClass")
				.Initializer(PModel("name", "s-string", 0, 1), PModel("surname", "s-string", 1)),
				Model("TestClass2").Name("TestClass2")
				.Operation("StructInput", "s-string", PModel("arg1", "TestClass")),
				Model("s-string").IsValue()
			);

			ObjectsAre(
				Object(Id("test2", "TestClass2"))
			);

			When(Id("test2", "TestClass2"))
				.Performs("StructInput", p =>
					p["arg1"].Values[0].ObjectModelId == "TestClass" &&
					p["arg1"].Values[0].InitializationParameters["name"].Values[0].ObjectModelId == "s-string" &&
					p["arg1"].Values[0].InitializationParameters["name"].Values[0].ReferenceId == "name" &&
					p["arg1"].Values[0].InitializationParameters.ContainsKey("surname") == false
				).Returns(Result(Id("success", "s-string")));

			var testing =
				Generator(c => c
					.Use(p => p.ReferencedTypeByShortModelIdPattern("System", "s"))
					.Use(p => p.ParseableValueTypePattern()));

			var assembly = testing.Generate(DefaultTestTemplate);
			var testClass = GetRenderedType(assembly, "TestClass");
			var testClassObj = Activator.CreateInstance(testClass, "name");

			var iTestClass2 = GetRenderedType(assembly, "TestClass2");
			var structInput = iTestClass2.GetMethod("StructInput");
			var test2 = CreateInstance(GetRenderedType(assembly, "TestClass2Impl"), "test2", "TestClass2");

			var actual = (string)structInput.Invoke(test2, new[] { testClassObj });

			Assert.AreEqual("success", actual);
		}

		protected override void Referenced_client_api_support_case()
		{
			ModelsAre(
				Model("Module2-TestClass2").Module("Module2").Name("TestClass2").Member("Self", "Module2-TestClass2"),
				Model("Module2-Struct").Module("Module2").Name("Struct").Initializer(PModel("arg1", "Module2-TestClass2")),
				Model("Module1-TestClass1").Module("Module1").Name("TestClass1")
				.Operation("StructOperation", "Module2-TestClass2", PModel("arg1", "Module2-Struct"))
			);

			ObjectsAre(
				Object(Id("test2_1", "Module2-TestClass2")).Value("test2_1_value"),
				Object(Id("test2_2", "Module2-TestClass2")).Value("test2_2_value")
			);
			ObjectsAre(
				Object(Id("test1", "Module1-TestClass1"))
			);

			When(Id("test1", "Module1-TestClass1"))
				.Performs("StructOperation", p =>
					p["arg1"].Values[0].ObjectModelId == "Module2-Struct" &&
					p["arg1"].Values[0].InitializationParameters["arg1"].Values[0].ObjectModelId == "Module2-TestClass2" &&
					p["arg1"].Values[0].InitializationParameters["arg1"].Values[0].ReferenceId == "test2_1"
				).Returns(Result(Id("test2_2", "Module2-TestClass2"))
			);

			var otherApiGenerator = Generator(c => c
				.FriendlyAssemblyNames.Add(DefaultNamespace + ".Module1")
				.DefaultNamespace.Set(DefaultNamespace + ".Module2")
				.InMemory.Set(false)
				.TypeIsRendered.Set(true, m => m.Module == "Module2")
				.TypeIsRendered.Set(false)
			);

			var otherAssembly = otherApiGenerator.Generate(DefaultTestTemplate);

			var testing = Generator(c => c
				.DefaultNamespace.Set(DefaultNamespace + ".Module1")
				.TypeIsRendered.Set(true, m => m.Module == "Module1")
				.TypeIsRendered.Set(false)
				.InMemory.Set(false)
				.Use(p => p.ReferenceOtherClientApiPattern(otherAssembly, otherApiGenerator.Context))
			).AddReference(otherAssembly);

			var assembly = testing.Generate(DefaultTestTemplate);

			var iTestClass1 = GetRenderedType(assembly, "TestClass1");
			var @struct = GetRenderedType(otherAssembly, "Struct");

			var structOperation = iTestClass1.GetMethod("StructOperation");

			var testObj1 = CreateInstance(GetRenderedType(assembly, "TestClass1Impl"), "test1", "Module1-TestClass1");
			var testObj2_1 = CreateInstance(GetRenderedType(otherAssembly, "TestClass2Impl"), "test2_1", "Module2-TestClass2");
			var testObj2_2 = CreateInstance(GetRenderedType(otherAssembly, "TestClass2Impl"), "test2_2", "Module2-TestClass2");

			var structOperationArg1 = Activator.CreateInstance(@struct, testObj2_1);
			var structOperationResult = structOperation.Invoke(testObj1, new[] { structOperationArg1 });
			Assert.AreEqual(testObj2_2, structOperationResult);
		}

		protected override void List_input_and_output_case()
		{
			ModelsAre(
				Model("TestClass").Name("TestClass")
				.Operation("StructListOperation", "s-string", PModel("arg1", "TestStruct", true)),
				Model("TestStruct").Name("TestStruct")
				.Initializer(PModel("name", "s-string")),
				Model("s-string").IsValue()
			);

			ObjectsAre(
				Object(Id("obj", "TestClass"))
			);

			When(Id("obj", "TestClass")).Performs("StructListOperation", p =>
				p["arg1"].IsList &&
				p["arg1"].Values[0].ObjectModelId == "TestStruct" &&
				p["arg1"].Values[0].InitializationParameters["name"].Values[0].ObjectModelId == "s-string" &&
				p["arg1"].Values[0].InitializationParameters["name"].Values[0].ReferenceId == "name1" &&
				p["arg1"].Values[1].InitializationParameters["name"].Values[0].ObjectModelId == "s-string" &&
				p["arg1"].Values[1].InitializationParameters["name"].Values[0].ReferenceId == "name2"
			).Returns(Result(Id("success", "s-string")));

			var assembly = Generator(c => c
				.Use(p => p.ReferencedTypeByShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern())).Generate(DefaultTestTemplate);

			var iTestClass = GetRenderedType(assembly, "TestClass");
			var testStruct = GetRenderedType(assembly, "TestStruct");

			var structListOperation = iTestClass.GetMethod("StructListOperation");

			var name1 = Activator.CreateInstance(testStruct, "name1");
			var name2 = Activator.CreateInstance(testStruct, "name2");

			var obj = CreateInstance(GetRenderedType(assembly, "TestClassImpl"), "obj", "TestClass");

			var arg1 = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(testStruct));
			arg1.Add(name1);
			arg1.Add(name2);

			var actual = structListOperation.Invoke(obj, new object[] { arg1 });

			Assert.AreEqual("success", actual);
		}

		public class CustomAttribute : Attribute { }

		protected override void Attribute_case()
		{
			ModelsAre(
				Model("TestClass").Name("TestClass")
				.Initializer(PModel("parameter", "s-string")),
				Model("s-string").IsValue()
			);

			var assembly =
				Generator(c => c
					.Use(p => p.ReferencedTypeByShortModelIdPattern("System", "s"))
					.Use(p => p.ParseableValueTypePattern())
					.RenderedTypeAttributes.Add(type.of<CustomAttribute>())
					.RenderedInitializerAttributes.Add(type.of<CustomAttribute>())
					.RenderedParameterAttributes.Add(type.of<CustomAttribute>()))
					.AddReference<CustomAttribute>()
					.Generate(DefaultTestTemplate)
				;

			var testClass = GetRenderedType(assembly, "TestClass");
			var initializer = testClass.GetConstructors().Single(ci => ci.GetParameters().Any(p => p.Name == "parameter"));
			var parameter = initializer.GetParameters().Single(p => p.Name == "parameter");
			var parameterProperty = testClass.GetProperty("Parameter");

			Assert.IsTrue(Attribute.IsDefined(testClass, typeof(CustomAttribute)));
			Assert.IsTrue(Attribute.IsDefined(initializer, typeof(CustomAttribute)));
			Assert.IsTrue(Attribute.IsDefined(parameter, typeof(CustomAttribute)));
			Assert.IsTrue(Attribute.IsDefined(parameterProperty, typeof(CustomAttribute)));
		}
	}
}