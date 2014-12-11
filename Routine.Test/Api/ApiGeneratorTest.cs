using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Moq;
using NUnit.Framework;
using Routine.Api;
using Routine.Api.Configuration;
using Routine.Api.Context;
using Routine.Api.Template.T4;
using Routine.Client;
using Routine.Core;
using Routine.Core.Configuration;
using Routine.Test.Client;
using Routine.Test.Common;

namespace Routine.Test.Api
{
	[TestFixture]
	public class ApiGeneratorTest : ClientTestBase
	{
		#region SetUp & Helpers

		private const string TEST_NAMESPACE = "Routine.Test.ApiGen.Client";

		private ApiGenerator Generator() { return Generator(c => c); }
		private ApiGenerator Generator(Func<ConventionalApiGenerationConfiguration, IApiGenerationConfiguration> config)
		{
			var cfg = config(
				BuildRoutine.ApiGenerationConfig()
					.FromBasic()
					.InMemory.Set(true)
					.DefaultNamespace.Set(TEST_NAMESPACE)
				);

			return new ApiGenerator(new DefaultApiGenerationContext(cfg, new ApplicationCodeModel(testingRapplication, cfg)));
		}

		public Type GenerateAndGetClientClass(ApiGenerator generator, string name)
		{
			return generator.GenerateClientApi().GetTypes().Single(t => t.Name == name);
		}

		public object GenerateAndGetClientInstance(ApiGenerator generator, string id, string name)
		{
			return Activator.CreateInstance(GenerateAndGetClientClass(generator, name), Robj(id, name));
		}

		#endregion

		[Test]
		public void Generates_assembly_with_given_default_namespace_when_in_memory_is_set_to_false()
		{
			var testing = Generator(c => c.InMemory.Set(false).DefaultNamespace.Set("Default.Namespace"));

			Assembly assembly = testing.GenerateClientApi();

			Assert.AreEqual("Default.Namespace", assembly.GetName().Name);
		}

		[Test]
		public void Class_is_directly_the_given_model_name()
		{
			ModelsAre(Model("TestClassId").Name("TestClass"));

			var types = Generator().GenerateClientApi().GetTypes();

			Assert.IsTrue(types.Any(t => t.Name == "TestClass"), "TestClass was not found");
		}

		[Test]
		public void Must_have_default_namespace()
		{
			ModelsAre(Model("TestClassId").Name("TestClass"));

			var testing = Generator(c => c.DefaultNamespace.Set("Default.Namespace"));

			var testClass = GenerateAndGetClientClass(testing, "TestClass");

			Assert.IsTrue(testClass.Namespace.StartsWith("Default.Namespace"), "Namespace should start with Default.Namespace, but it is -> " + testClass.Namespace);

			testing = Generator(c => c.DefaultNamespace.SetDefault());

			try
			{
				testing.GenerateClientApi();
				Assert.Fail("exception not thrown");
			}
			catch (InvalidOperationException) { }
		}

		[Test]
		public void Appends_module_to_default_namespace_when_module_is_not_empty()
		{
			ModelsAre(
				Model("TestClassId1").Module("Module").Name("TestClass1"),
				Model("TestClassId2").Name("TestClass2"));

			var types = Generator().GenerateClientApi().GetTypes();

			var testClass1 = types.Single(t => t.Name == "TestClass1");
			var testClass2 = types.Single(t => t.Name == "TestClass2");

			Assert.IsTrue(testClass1.Namespace.EndsWith("Module"), "Namespace should end with Module, but it is -> " + testClass1.Namespace);
			Assert.AreEqual(TEST_NAMESPACE, testClass2.Namespace);
		}

		[Test]
		public void Domain_types_are_rendered_using_generated_client_types()
		{
			ModelsAre(
				Model("TestClass").Name("TestClass").Member("Sub", "TestClass2"),
				Model("TestClass2").Name("TestClass2"));

			var types = Generator().GenerateClientApi().GetTypes();
			var testClass = types.Single(t => t.Name == "TestClass");
			var testClass2 = types.Single(t => t.Name == "TestClass2");

			var actual = testClass.GetProperties().Single(p => p.Name == "Sub").PropertyType;

			Assert.AreEqual(testClass2, actual);
		}

		[Test]
		public void Value_types_are_not_rendered_but_their_assemblies_should_be_referenced()
		{
			ModelsAre(
				Model("TestClass").Name("TestClass")
				.Member("Price", "c-price")
				.Member("Address", "c-fat-string"),
				Model("c-fat-string").IsValue(),
				Model("c-price").IsValue()
			);

			var testing =
				Generator(c => c
					.Use(p => p.ShortModelIdPattern("Routine.Test.Common", "c"))
					.Use(p => p.ParseableValueTypePattern()))
				.AddReference<FatString>();

			var assembly = testing.GenerateClientApi();

			Assert.AreEqual(1, assembly.GetTypes().Count());

			var testClass = assembly.GetTypes().Single(t => t.Name == "TestClass");

			var actual = testClass.GetProperties().Single(p => p.Name == "Price").PropertyType;
			Assert.AreEqual(typeof(Price), actual);

			actual = testClass.GetProperties().Single(p => p.Name == "Address").PropertyType;
			Assert.AreEqual(typeof(FatString), actual);
		}

		[Test]
		public void When_the_type_of_a_member__operation_or_parameter_is_not_rendered_and_is_not_configured_as_a_referenced_type__ConfigurationException_is_thrown()
		{
			ModelsAre(
				Model("TestClass")
				.Name("TestClass")
				.Member("Member", "s-string"),
				Model("s-string").IsValue()
			);

			try
			{
				Generator().GenerateClientApi();
				Assert.Fail("exception not thrown");
			}
			catch (ConfigurationException ex)
			{
				Assert.IsTrue(ex.Message.Contains("s-string"), "Exception message does not contain s-string");
			}

			ModelsAre(true, Model("TestClass").Name("TestClass").Initializer(PModel("notReferenced", "s-string")));

			try
			{
				Generator().GenerateClientApi();
				Assert.Fail("exception not thrown");
			}
			catch (ConfigurationException ex)
			{
				Assert.IsTrue(ex.Message.Contains("s-string"), "Exception message does not contain s-string");
			}

			ModelsAre(true, Model("TestClass").Name("TestClass").Operation("Operation", PModel("notReferenced", "s-string")));

			try
			{
				Generator().GenerateClientApi();
				Assert.Fail("exception not thrown");
			}
			catch (ConfigurationException ex)
			{
				Assert.IsTrue(ex.Message.Contains("s-string"), "Exception message does not contain s-string");
			}

			ModelsAre(true, Model("TestClass").Name("TestClass").Operation("Operation", "s-string"));

			try
			{
				Generator().GenerateClientApi();
				Assert.Fail("exception not thrown");
			}
			catch (ConfigurationException ex)
			{
				Assert.IsTrue(ex.Message.Contains("s-string"), "Exception message does not contain s-string");
			}
		}

		[Test]
		public void Types_of_members__operations_and_parameters_that_are_neither_rendered_nor_configured_as_a_referenced_type__can_be_configured_to_be_skipped()
		{
			ModelsAre(
				Model("TestClass")
				.Name("TestClass")
				.Initializer(PModel("notReferenced", "s-string"))
				.Member("Member", "s-string")
				.Operation("Operation", "s-string")
				.Operation("Operation2", PModel("notReferenced", "s-string")),
				Model("s-string").IsValue()
			);

			var clientType = GenerateAndGetClientClass(Generator(c => c.IgnoreReferencedTypeNotFound.Set(true)), "TestClass");

			Assert.IsFalse(clientType.GetConstructors().Any(c => c.GetParameters().Any(p => p.Name == "notReferenced")));
			Assert.IsFalse(clientType.GetProperties().Any(c => c.Name == "Member"));
			Assert.IsFalse(clientType.GetMethods().Any(c => c.Name == "Operation"));
			Assert.IsFalse(clientType.GetMethods().Any(c => c.GetParameters().Any(p => p.Name == "notReferenced")));
		}

		[Test]
		public void Initializer_is_rendered_as_constructor()
		{
			ModelsAre(
				Model("TestClass")
				.Name("TestClass")
				.Initializer(PModel("name", "s-string")),
				Model("s-string").IsValue()
			);

			var testing =
				Generator(c => c
					.Use(p => p.ShortModelIdPattern("System", "s"))
					.Use(p => p.ParseableValueTypePattern()));

			var constructor = GenerateAndGetClientClass(testing, "TestClass")
				.GetConstructors().Single(c => c.GetParameters().Any(p => p.ParameterType == typeof(string)));

			Assert.AreEqual(2, constructor.GetParameters().Length);
			Assert.AreEqual(typeof(Rapplication), constructor.GetParameters()[0].ParameterType);
			Assert.AreEqual(typeof(string), constructor.GetParameters()[1].ParameterType);
			Assert.AreEqual("name", constructor.GetParameters()[1].Name);
		}

		[Test]
		public void Initializer_groups_are_rendered_as_constructor_overloads()
		{
			ModelsAre(
				Model("TestClass")
				.Name("TestClass")
				.Initializer(PModel("name", "s-string", 0, 1), PModel("surname", "s-string", 1)),
				Model("s-string").IsValue()
			);

			var testing =
				Generator(c => c
					.Use(p => p.ShortModelIdPattern("System", "s"))
					.Use(p => p.ParseableValueTypePattern()));

			var constructors = GenerateAndGetClientClass(testing, "TestClass")
				.GetConstructors().Where(c => c.GetParameters().Any(p => p.ParameterType == typeof(string))).ToList();

			Assert.AreEqual(2, constructors.Count);

			Assert.AreEqual(2, constructors[0].GetParameters().Length);
			Assert.AreEqual(typeof(Rapplication), constructors[0].GetParameters()[0].ParameterType);
			Assert.AreEqual(typeof(string), constructors[0].GetParameters()[1].ParameterType);
			Assert.AreEqual("name", constructors[0].GetParameters()[1].Name);

			Assert.AreEqual(3, constructors[1].GetParameters().Length);
			Assert.AreEqual(typeof(Rapplication), constructors[1].GetParameters()[0].ParameterType);
			Assert.AreEqual(typeof(string), constructors[1].GetParameters()[1].ParameterType);
			Assert.AreEqual("name", constructors[1].GetParameters()[1].Name);
			Assert.AreEqual(typeof(string), constructors[1].GetParameters()[2].ParameterType);
			Assert.AreEqual("surname", constructors[1].GetParameters()[2].Name);
		}

		[Test]
		public void Members_are_rendered_as_read_only_properties()
		{
			ModelsAre(
				Model("TestClass").Name("TestClass").Member("Name", "s-string"),
				Model("s-string").IsValue()
			);

			var testing = Generator(c => c
				.Use(p => p.ShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern()));

			var properties = GenerateAndGetClientClass(testing, "TestClass").GetProperties();

			Assert.IsTrue(properties.Any(p => p.Name == "Name"), "Name property was not found");

			var nameProperty = properties.Single(p => p.Name == "Name");

			Assert.AreEqual(typeof(string), nameProperty.PropertyType);
			Assert.IsTrue(nameProperty.CanRead);
			Assert.IsFalse(nameProperty.CanWrite);
		}

		[Test]
		public void List_member_support()
		{
			ModelsAre(
				Model("TestClass").Name("TestClass").Member("OrderIds", "s-string", true),
				Model("s-string").IsValue()
			);

			var testing = Generator(c => c
				.Use(p => p.ShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern()));

			var properties = GenerateAndGetClientClass(testing, "TestClass").GetProperties();

			var nameProperty = properties.Single(p => p.Name == "OrderIds");

			Assert.AreEqual(typeof(List<string>), nameProperty.PropertyType);
		}

		[Test]
		public void Operations_are_rendered_as_methods()
		{
			ModelsAre(Model("TestClass").Name("TestClass").Operation("VoidMethod", true));

			var methods = GenerateAndGetClientClass(Generator(), "TestClass").GetMethods();

			Assert.IsTrue(methods.Any(m => m.Name == "VoidMethod"), "VoidMethod method was not found");

			var voidMethod = methods.Single(m => m.Name == "VoidMethod");

			Assert.IsTrue(voidMethod.ReturnType == typeof(void));
			Assert.AreEqual(0, voidMethod.GetParameters().Length);
		}

		[Test]
		public void Operation_results_are_method_return_types()
		{
			ModelsAre(
				Model("TestClass").Name("TestClass").Operation("StringMethod", "s-string"),
				Model("s-string").IsValue()
			);

			var testing = Generator(c => c
				.Use(p => p.ShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern()));

			var methods = GenerateAndGetClientClass(testing, "TestClass").GetMethods();

			var stringMethod = methods.Single(m => m.Name == "StringMethod");

			Assert.IsTrue(stringMethod.ReturnType == typeof(string));
		}

		[Test]
		public void List_operation_result_support()
		{
			ModelsAre(
				Model("TestClass").Name("TestClass").Operation("StringListMethod", "s-string", true),
				Model("s-string").IsValue()
			);
			var testing = Generator(c => c
				.Use(p => p.ShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern()));

			var methods = GenerateAndGetClientClass(testing, "TestClass").GetMethods();

			var stringMethod = methods.Single(m => m.Name == "StringListMethod");

			Assert.IsTrue(stringMethod.ReturnType == typeof(List<string>));
		}

		[Test]
		public void Operation_parameters_are_method_parameters()
		{
			ModelsAre(
				Model("TestClass")
				.Name("TestClass")
				.Operation("ParameterMethod", true, PModel("arg1", "s-string"), PModel("arg2", "s-string", true)),
				Model("s-string").IsValue()
			);

			var testing = Generator(c => c
				.Use(p => p.ShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern()));

			var methods = GenerateAndGetClientClass(testing, "TestClass").GetMethods();

			var parameterMethod = methods.Single(m => m.Name == "ParameterMethod");

			Assert.AreEqual(2, parameterMethod.GetParameters().Length);
			Assert.AreEqual("arg1", parameterMethod.GetParameters()[0].Name);
			Assert.AreEqual(typeof(string), parameterMethod.GetParameters()[0].ParameterType);
			Assert.AreEqual("arg2", parameterMethod.GetParameters()[1].Name);
			Assert.AreEqual(typeof(List<string>), parameterMethod.GetParameters()[1].ParameterType);
		}

		[Test]
		public void Operation_groups_are_rendered_as_method_overloads()
		{
			ModelsAre(
				Model("TestClass")
				.Name("TestClass")
				.Operation("OverloadMethod", true, PModel("param1", "s-string", 0, 1), PModel("param2", "s-int-32", 1, 2)),
				Model("s-string").IsValue(),
				Model("s-int-32").IsValue()
			);

			var testing = Generator(c => c
				.Use(p => p.ShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern()));

			var methods = GenerateAndGetClientClass(testing, "TestClass").GetMethods();

			var overloadMethods = methods.Where(m => m.Name == "OverloadMethod").ToList();

			Assert.AreEqual(3, overloadMethods.Count);

			Assert.AreEqual(1, overloadMethods[0].GetParameters().Length);
			Assert.AreEqual("param1", overloadMethods[0].GetParameters()[0].Name);
			Assert.AreEqual(typeof(string), overloadMethods[0].GetParameters()[0].ParameterType);

			Assert.AreEqual(2, overloadMethods[1].GetParameters().Length);
			Assert.AreEqual("param1", overloadMethods[1].GetParameters()[0].Name);
			Assert.AreEqual(typeof(string), overloadMethods[1].GetParameters()[0].ParameterType);
			Assert.AreEqual("param2", overloadMethods[1].GetParameters()[1].Name);
			Assert.AreEqual(typeof(int), overloadMethods[1].GetParameters()[1].ParameterType);

			Assert.AreEqual(1, overloadMethods[2].GetParameters().Length);
			Assert.AreEqual("param2", overloadMethods[2].GetParameters()[0].Name);
			Assert.AreEqual(typeof(int), overloadMethods[2].GetParameters()[0].ParameterType);
		}

		[Test][Ignore]
		public void Client_types_have_a_conversion_method_for_each_of_their_view_model()
		{
			Assert.Fail("to be designed");
		}

		[Test]
		public void Can_have_a_module_filter_to_generate_assembly_for_only_specified_modules()
		{
			ModelsAre(
				Model("Included1-TestClass1").Module("Included1").Name("TestClass1"),
				Model("Included2-TestClass2").Module("Included2").Name("TestClass2"),
				Model("Excluded1-TestClass3").Module("Excluded1").Name("TestClass3"));

			var testing = Generator(c => c
				.TypeIsRendered.Set(false, m => !m.Module.Matches("Included.*"))
			);

			var types = testing.GenerateClientApi().GetTypes();

			Assert.IsTrue(types.Any(t => t.Name == "TestClass1"), "TestClass1 was not found");
			Assert.IsTrue(types.Any(t => t.Name == "TestClass2"), "TestClass2 was not found");
			Assert.IsTrue(types.All(t => t.Name != "TestClass3"), "TestClass2 was found");
		}

		[Test]
		public void Automatically_excludes_members_and_operations_needing_a_not_rendered_model()
		{
			ModelsAre(
				Model("Included-TestClass1").Module("Included").Name("TestClass1")
				.Initializer(PModel("parameterExcluded", "Excluded-TestClass2"))
				.Member("PropertyExcluded", "Excluded-TestClass2")
				.Member("PropertyIncluded", "s-string")
				.Operation("MethodExcludedBecauseOfReturnType", "Excluded-TestClass2")
				.Operation("MethodExcludedBecauseOfParameter", "s-string", PModel("excludeReason", "Excluded-TestClass2"))
				.Operation("MethodIncluded", "s-string"),
				Model("Excluded-TestClass2").Module("Excluded").Name("TestClass2"),
				Model("s-string").IsValue()
			);

			var testing = Generator(c => c
				.Use(p => p.ShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern())
				.TypeIsRendered.Set(false, m => !m.Module.Matches("Included.*")));

			var types = testing.GenerateClientApi().GetTypes();

			Assert.IsFalse(types.Any(t => t.Name == "TestClass2"), "TestClass2 was found");

			var testClass1 = types.Single(t => t.Name == "TestClass1");

			Assert.IsFalse(testClass1.GetConstructors().Any(c => c.GetParameters().Any(p => p.Name == "parameterExcluded")), "Initializer should be excluded but was found");

			Assert.IsNull(testClass1.GetProperty("PropertyExcluded"), "PropertyExcluded was found");
			Assert.IsNull(testClass1.GetMethod("MethodExcludedBecauseOfReturnType"), "MethodExcludedBecauseOfReturnType was found");
			Assert.IsNull(testClass1.GetMethod("MethodExcludedBecauseOfParameter"), "MethodExcludedBecauseOfParameter was found");

			Assert.IsNotNull(testClass1.GetProperty("PropertyIncluded"), "PropertyIncluded was not found");
			Assert.IsNotNull(testClass1.GetMethod("MethodIncluded"), "MethodIncluded was not found");
		}

		[Test]
		public void Property_with_type_from_an_excluded_module_class_is_rendered_when_excluded_module_class_is_referenced()
		{
			ModelsAre(
				Model("Module1-TestClass1").Module("Module1").Name("TestClass1"),
				Model("Module2-TestClass2").Module("Module2").Name("TestClass2")
				.Member("DependentProperty", "Module1-TestClass1"));

			var otherApiGenerator = Generator(c => c
				//RoutineTest is used instead of Routine.Test
				//when running all tests together, it conflicts with another test
				.DefaultNamespace.Set("RoutineTest.ApiGen.Client.Module1")
				.InMemory.Set(false)
				.TypeIsRendered.Set(false, m => m.Module != "Module1"));

			var otherApi = otherApiGenerator.GenerateClientApi();

			var testClass1 = otherApi.GetTypes().Single(t => t.Name == "TestClass1");

			var testing = Generator(c => c
				.DefaultNamespace.Set("RoutineTest.ApiGen.Client.Module2")
				.TypeIsRendered.Set(false, m => m.Module != "Module2")
				.ReferencedType.Set(s => s
					.By(t => t.Id.Replace("-", ".").Prepend("RoutineTest.ApiGen.Client.").ToTypeInfo(true))
					.When(t => t.Id.StartsWith("Module1")))
				)
				.AddReference(otherApi);

			var testClass2 = GenerateAndGetClientClass(testing, "TestClass2");

			Assert.IsNotNull(testClass2.GetProperty("DependentProperty"), "DependentProperty should have been rendered!");
			Assert.AreEqual(testClass1, testClass2.GetProperty("DependentProperty").PropertyType);
		}

		[Test]
		public void Module_is_not_appended_to_default_namespace_when_default_namespace_has_it_already()
		{
			ModelsAre(
				Model("Module1-TestClass1").Module("Module1").Name("TestClass1"));

			var testing = Generator(c => c.DefaultNamespace.Set("Routine.Test.ApiGen.Client.Module1"));

			var testClass1 = GenerateAndGetClientClass(testing, "TestClass1");

			Assert.AreEqual("Routine.Test.ApiGen.Client.Module1", testClass1.Namespace);
		}

		[Test]
		public void Property_values_are_fetched_via_client_api()
		{
			ModelsAre(
				Model("s-string").IsValue(),
				Model("TestClass").Name("TestClass")
				.Member("Name", "s-string"));

			ObjectsAre(
				Object(Id("test_id", "TestClass"))
				.Value("test_value")
				.Member("Name", Id("test_name", "s-string")));

			var testing = Generator(c => c
				.Use(p => p.ShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern()));

			var testObj = GenerateAndGetClientInstance(testing, "test_id", "TestClass");

			var actual = testObj.GetType().GetProperty("Name").GetValue(testObj, new object[0]) as string;
			Assert.AreEqual("test_name", actual);
		}

		[Test]
		public void Methods_are_called_via_client_api()
		{
			ModelsAre(
				Model("s-string").IsValue(),
				Model("TestClass").Name("TestClass")
				.Operation("Operation", "s-string", PModel("arg1", "s-string"), PModel("arg2", "s-string")));

			ObjectsAre(
				Object(Id("test_id", "TestClass"))
				.Value("test_value"));

			When(Id("test_id", "TestClass"))
				.Performs("Operation", p =>
					p["arg1"].Values[0].ReferenceId == "arg1_test" &&
					p["arg2"].Values[0].ReferenceId == "arg2_test")
				.Returns(Result(Id("result_test", "s-string")));

			var testing = Generator(c => c
				.Use(p => p.ShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern()));

			var testObj = GenerateAndGetClientInstance(testing, "test_id", "TestClass");

			var actual = testObj.GetType().GetMethod("Operation").Invoke(testObj, new object[] { "arg1_test", "arg2_test" }) as string;

			Assert.AreEqual("result_test", actual);
		}

		[Test]
		public void Value_types_other_than_string_are_converted_using_given_converter_function()
		{
			var expectedGuid = Guid.NewGuid();
			const int expectedInt = 12;

			ModelsAre(
				Model("s-int-32").IsValue(),
				Model("s-guid").IsValue(),
				Model("TestClass").Name("TestClass")
				.Member("Uid", "s-guid")
				.Operation("Operation", "s-int-32", PModel("arg1", "s-guid")));

			ObjectsAre(
				Object(Id("test_id", "TestClass"))
				.Value("test_value")
				.Member("Uid", Id(expectedGuid.ToString(), "s-guid")));

			When(Id("test_id", "TestClass"))
				.Performs("Operation", p =>
					p["arg1"].Values[0].ReferenceId == expectedGuid.ToString())
				.Returns(Result(Id(expectedInt.ToString(CultureInfo.InvariantCulture), "s-int-32")));

			var testing = Generator(c => c
				.Use(p => p.ShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern()));

			var testObj = GenerateAndGetClientInstance(testing, "test_id", "TestClass");

			var actualGuid = (Guid)testObj.GetType().GetProperty("Uid").GetValue(testObj, new object[0]);
			Assert.AreEqual(expectedGuid, actualGuid);

			var actualInt = (int)testObj.GetType().GetMethod("Operation").Invoke(testObj, new object[] { expectedGuid });
			Assert.AreEqual(expectedInt, actualInt);
		}

		[Test]
		public void Value_types_can_be_rendered_directly_as_string()
		{
			const string expectedIntString = "12";

			ModelsAre(
				Model("s-int-32").IsValue(),
				Model("TestClass").Name("TestClass")
				.Member("Id", "s-int-32")
				.Operation("Operation", "s-int-32", PModel("arg1", "s-int-32")));

			ObjectsAre(
				Object(Id("test_id", "TestClass"))
				.Value("test_value")
				.Member("Id", Id(expectedIntString, "s-int-32")));

			When(Id("test_id", "TestClass"))
				.Performs("Operation", p =>
					p["arg1"].Values[0].ReferenceId == expectedIntString)
				.Returns(Result(Id(expectedIntString, "s-int-32")));

			var testing = Generator(c => c
				.Use(p => p.ShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern())
				.TargetValueType.Set(e => e.Constant(type.of<string>()).When(type.of<int>()))
				);

			var testObj = GenerateAndGetClientInstance(testing, "test_id", "TestClass");

			var actualIntString = (string)testObj.GetType().GetProperty("Id").GetValue(testObj, new object[0]);
			Assert.AreEqual(expectedIntString, actualIntString);

			actualIntString = (string)testObj.GetType().GetMethod("Operation").Invoke(testObj, new object[] { expectedIntString });
			Assert.AreEqual(expectedIntString, actualIntString);
		}

		[Test]
		public void Value_types_can_be_rendered_as_another_value_type()
		{
			const string expectedString = "test string";
			var expectedFatString = new FatString(expectedString);

			ModelsAre(
				Model("s-string").IsValue(),
				Model("s-fat-string").IsValue(),
				Model("TestClass").Name("TestClass")
				.Member("Id", "s-string")
				.Operation("Operation", "s-string", PModel("arg1", "s-string")));

			ObjectsAre(
				Object(Id("test_id", "TestClass"))
				.Value("test_value")
				.Member("Id", Id(expectedString, "s-string")));

			When(Id("test_id", "TestClass"))
				.Performs("Operation", p =>
					p["arg1"].Values[0].ReferenceId == expectedString)
				.Returns(Result(Id(expectedString, "s-string")));

			var testing = Generator(c => c
				.Use(p => p.ShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern())
				.TargetValueType.Set(type.of<FatString>(), type.of<string>())
			).AddReference<FatString>();

			var testObj = GenerateAndGetClientInstance(testing, "test_id", "TestClass");

			var actualFatString = (FatString)testObj.GetType().GetProperty("Id").GetValue(testObj, new object[0]);
			Assert.AreEqual(expectedFatString, actualFatString);

			actualFatString = (FatString)testObj.GetType().GetMethod("Operation").Invoke(testObj, new object[] { expectedFatString });
			Assert.AreEqual(expectedFatString, actualFatString);
		}

		[Test]
		public void Domain_type_instances_are_created_using_client_class_constructors()
		{
			ModelsAre(
				Model("TestClass2").Name("TestClass2"),
				Model("TestClass1").Name("TestClass1")
				.Member("Sub", "TestClass2")
				.Operation("Operation", "TestClass2", PModel("arg1", "TestClass2")));

			ObjectsAre(
				Object(Id("test2", "TestClass2"))
				.Value("test2_value"),
				Object(Id("test1", "TestClass1"))
				.Value("test1_value")
				.Member("Sub", Id("test2", "TestClass2")));

			When(Id("test1", "TestClass1"))
				.Performs("Operation", p =>
					p["arg1"].Values[0].ObjectModelId == "TestClass2" &&
					p["arg1"].Values[0].ReferenceId == "test2")
				.Returns(Result(Id("test2", "TestClass2")));

			var types = Generator().GenerateClientApi().GetTypes();
			var testClass1 = types.Single(t => t.Name == "TestClass1");
			var testClass2 = types.Single(t => t.Name == "TestClass2");

			var subProperty = testClass1.GetProperty("Sub");
			var operationMethod = testClass1.GetMethod("Operation");

			var testObj1 = Activator.CreateInstance(testClass1, Robj("test1", "TestClass1"));
			var testObj2 = Activator.CreateInstance(testClass2, Robj("test2", "TestClass2"));

			var subObj = subProperty.GetValue(testObj1, new object[0]);
			Assert.AreEqual(testObj2, subObj);

			var operationResult = operationMethod.Invoke(testObj1, new[] { testObj2 });
			Assert.AreEqual(testObj2, operationResult);
		}

		[Test]
		public void Referenced_domain_type_instances_are_also_created_using_client_class_constructors()
		{
			ModelsAre(
				Model("Module2-TestClass2").Module("Module2").Name("TestClass2"),
				Model("Module1-TestClass1").Module("Module1").Name("TestClass1")
				.Member("Sub", "Module2-TestClass2")
				.Operation("Operation", "Module2-TestClass2", PModel("arg1", "Module2-TestClass2")));

			ObjectsAre(
				Object(Id("test2", "Module2-TestClass2"))
				.Value("test2_value"),
				Object(Id("test1", "Module1-TestClass1"))
				.Value("test1_value")
				.Member("Sub", Id("test2", "Module2-TestClass2")));

			When(Id("test1", "Module1-TestClass1"))
				.Performs("Operation", p =>
					p["arg1"].Values[0].ObjectModelId == "Module2-TestClass2" &&
					p["arg1"].Values[0].ReferenceId == "test2")
				.Returns(Result(Id("test2", "Module2-TestClass2")));

			var otherApiGenerator = Generator(c => c
				//RoutineTest is used instead of Routine.Test, 
				//this is because when running all tests together, it conflicts with another test
				.DefaultNamespace.Set("RoutineTest.ApiGen.Client.Module2")
				.InMemory.Set(false)
				.TypeIsRendered.Set(false, m => m.Module != "Module2")
			);

			var otherApi = otherApiGenerator.GenerateClientApi();

			var testClass2 = otherApi.GetTypes().Single(t => t.Name == "TestClass2");

			var testing = Generator(c => c
					.DefaultNamespace.Set("RoutineTest.ApiGen.Client.Module1")
				.TypeIsRendered.Set(false, m => m.Module != "Module1")
					.ReferencedType.Set(s => s
						.By(t => t.Id.Replace("-", ".").Prepend("RoutineTest.ApiGen.Client.").ToTypeInfo(true))
						.When(t => t.Id.StartsWith("Module2")))
				)
				.AddReference(otherApi);

			var testingApi = testing.GenerateClientApi();

			var testClass1 = testingApi.GetTypes().Single(t => t.Name == "TestClass1");

			var subProperty = testClass1.GetProperty("Sub");
			var operationMethod = testClass1.GetMethod("Operation");

			var testObj1 = Activator.CreateInstance(testClass1, Robj("test1", "Module1-TestClass1"));
			var testObj2 = Activator.CreateInstance(testClass2, Robj("test2", "Module2-TestClass2"));

			var subObj = subProperty.GetValue(testObj1, new object[0]);
			Assert.AreEqual(testObj2, subObj);

			var operationResult = operationMethod.Invoke(testObj1, new[] { testObj2 });
			Assert.AreEqual(testObj2, operationResult);
		}

		[Test]
		public void List_member__parameter_and_operation_result_support()
		{
			ModelsAre(
				Model("s-string").IsValue(),
				Model("TestClass2").Name("TestClass2"),
				Model("TestClass1").Name("TestClass1")
				.Member("Subs", "TestClass2", true)
				.Member("Names", "s-string", true)
				.Operation("SubListOperation", "TestClass2", true, PModel("arg1", "TestClass2", true))
				.Operation("NameListOperation", "s-string", true, PModel("arg1", "s-string", true))
			);

			ObjectsAre(
				Object(Id("test2.1", "TestClass2")).Value("test2.1_value"),
				Object(Id("test2.2", "TestClass2")).Value("test2.2_value"),
				Object(Id("test2.3", "TestClass2")).Value("test2.3_value"),
				Object(Id("test2.4", "TestClass2")).Value("test2.4_value"),
				Object(Id("test1", "TestClass1"))
				.Value("test1_value")
				.Member("Subs", Id("test2.1", "TestClass2"), Id("test2.2", "TestClass2"))
				.Member("Names", Id("name1", "s-string"), Id("name2", "s-string"))
			);

			When(Id("test1", "TestClass1"))
				.Performs("SubListOperation", p =>
					p["arg1"].Values[0].ObjectModelId == "TestClass2" &&
					p["arg1"].Values[0].ReferenceId == "test2.1" &&
					p["arg1"].Values[1].ObjectModelId == "TestClass2" &&
					p["arg1"].Values[1].ReferenceId == "test2.2")
				.Returns(Result(Id("test2.3", "TestClass2"), Id("test2.4", "TestClass2")));

			When(Id("test1", "TestClass1"))
				.Performs("NameListOperation", p =>
					p["arg1"].Values[0].ObjectModelId == "s-string" &&
					p["arg1"].Values[0].ReferenceId == "name1" &&
					p["arg1"].Values[1].ObjectModelId == "s-string" &&
					p["arg1"].Values[1].ReferenceId == "name2")
				.Returns(Result(Id("name3", "s-string"), Id("name4", "s-string")));

			var testing = Generator(c => c
				.Use(p => p.ShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern()));

			var types = testing.GenerateClientApi().GetTypes();
			var testClass1 = types.Single(t => t.Name == "TestClass1");
			var testClass2 = types.Single(t => t.Name == "TestClass2");

			var subsProperty = testClass1.GetProperty("Subs");
			var namesProperty = testClass1.GetProperty("Names");

			var subListOperation = testClass1.GetMethod("SubListOperation");
			var nameListOperation = testClass1.GetMethod("NameListOperation");

			var testObj1 = Activator.CreateInstance(testClass1, Robj("test1", "TestClass1"));
			var testObj2_1 = Activator.CreateInstance(testClass2, Robj("test2.1", "TestClass2"));
			var testObj2_2 = Activator.CreateInstance(testClass2, Robj("test2.2", "TestClass2"));
			var testObj2_3 = Activator.CreateInstance(testClass2, Robj("test2.3", "TestClass2"));
			var testObj2_4 = Activator.CreateInstance(testClass2, Robj("test2.4", "TestClass2"));

			var subObjs = subsProperty.GetValue(testObj1, new object[0]) as IList;
			Assert.AreEqual(testObj2_1, subObjs[0]);
			Assert.AreEqual(testObj2_2, subObjs[1]);

			var subListResult = subListOperation.Invoke(testObj1, new object[] { subObjs }) as IList;
			Assert.AreEqual(testObj2_3, subListResult[0]);
			Assert.AreEqual(testObj2_4, subListResult[1]);

			var names = namesProperty.GetValue(testObj1, new object[0]) as IList;
			Assert.AreEqual("name1", names[0]);
			Assert.AreEqual("name2", names[1]);

			var nameListResult = nameListOperation.Invoke(testObj1, new object[] { names }) as IList;
			Assert.AreEqual("name3", nameListResult[0]);
			Assert.AreEqual("name4", nameListResult[1]);
		}

		[Test]
		public void Null_member_value_support()
		{
			ModelsAre(
				Model("s-string").IsValue(),
				Model("TestClass1").Name("TestClass1")
				.Member("Name", "s-string")
			);

			ObjectsAre(
				Object(Id("test1", "TestClass1"))
				.Value("test1_value")
				.Member("Name", Null("s-string"))
			);

			var testing = Generator(c => c
				.Use(p => p.ShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern()));

			var types = testing.GenerateClientApi().GetTypes();
			var testClass1 = types.Single(t => t.Name == "TestClass1");

			var nameProperty = testClass1.GetProperty("Name");

			var testObj1 = Activator.CreateInstance(testClass1, Robj("test1", "TestClass1"));

			var name = nameProperty.GetValue(testObj1, new object[0]);
			Assert.IsNull(name);
		}

		[Test]
		public void Null_parameter_value_support()
		{
			ModelsAre(
				Model("s-string").IsValue(),
				Model("TestClass1").Name("TestClass1")
				.Operation("Operation", "s-string", PModel("arg1", "s-string"))
			);

			ObjectsAre(Object(Id("test1", "TestClass1")).Value("test1_value"));

			When(Id("test1", "TestClass1"))
				.Performs("Operation", p =>
					p["arg1"].Values[0].IsNull)
				.Returns(Result(Id("resultForNull", "s-string")));

			var testing = Generator(c => c
				.Use(p => p.ShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern()));

			var types = testing.GenerateClientApi().GetTypes();
			var testClass1 = types.Single(t => t.Name == "TestClass1");

			var operation = testClass1.GetMethod("Operation");

			var testObj1 = Activator.CreateInstance(testClass1, Robj("test1", "TestClass1"));

			var actual = operation.Invoke(testObj1, new object[] { null });
			Assert.AreEqual("resultForNull", actual);
		}

		[Test]
		public void Null_operation_result_support()
		{
			ModelsAre(
				Model("s-string").IsValue(),
				Model("TestClass1").Name("TestClass1")
				.Operation("Operation", "s-string")
			);

			ObjectsAre(Object(Id("test1", "TestClass1")).Value("test1_value"));

			When(Id("test1", "TestClass1"))
				.Performs("Operation")
				.Returns(Result(Null("s-string")));

			var testing = Generator(c => c
				.Use(p => p.ShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern()));

			var types = testing.GenerateClientApi().GetTypes();
			var testClass1 = types.Single(t => t.Name == "TestClass1");

			var operation = testClass1.GetMethod("Operation");

			var testObj1 = Activator.CreateInstance(testClass1, Robj("test1", "TestClass1"));

			var actual = operation.Invoke(testObj1, new object[0]);
			Assert.IsNull(actual);
		}

		[Test]
		public void Client_types_have_ToString_method_which_returns_value()
		{
			ModelsAre(Model("TestClass1").Name("TestClass1"));

			ObjectsAre(Object(Id("test1", "TestClass1")).Value("test1_value"));

			var types = Generator().GenerateClientApi().GetTypes();
			var testClass1 = types.Single(t => t.Name == "TestClass1");

			var testObj1 = Activator.CreateInstance(testClass1, Robj("test1", "TestClass1"));

			Assert.AreEqual("test1_value", testObj1.ToString());
		}

		[Test]
		public void Client_types_have_ToString_overload_to_include_Id_for_debugging_purposes()
		{
			ModelsAre(Model("TestClass1").Name("TestClass1"));

			ObjectsAre(Object(Id("test1", "TestClass1")).Value("test1_value"));

			var types = Generator().GenerateClientApi().GetTypes();
			var testClass1 = types.Single(t => t.Name == "TestClass1");

			var testObj1 = Activator.CreateInstance(testClass1, Robj("test1", "TestClass1"));
			var toString = testClass1.GetMethod("ToString", new[] { typeof(bool) });

			var debugMode = toString.Invoke(testObj1, new object[] { true }) as string;
			Assert.AreEqual("[Id: test1, Value: test1_value]", debugMode);

			var releaseMode = toString.Invoke(testObj1, new object[] { false }) as string;
			Assert.AreEqual("test1_value", releaseMode);
		}

		[Test]
		public void Client_types_have_Invalidate_method_which_invalidates_member_values()
		{
			ModelsAre(
				Model("s-string").IsValue(),
				Model("TestClass1").Name("TestClass1")
				.Member("Name", "s-string")
			);

			ObjectsAre(
				Object(Id("test1", "TestClass1"))
				.Value("test1_value")
				.Member("Name", Id("name", "s-string"))
			);

			var testing = Generator(c => c
				.Use(p => p.ShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern()));

			var types = testing.GenerateClientApi().GetTypes();
			var testClass1 = types.Single(t => t.Name == "TestClass1");

			var nameProperty = testClass1.GetProperty("Name");

			var testObj1 = Activator.CreateInstance(testClass1, Robj("test1", "TestClass1"));

			var name = nameProperty.GetValue(testObj1, new object[0]);
			Assert.AreEqual("name", name);

			testClass1.GetMethod("Invalidate").Invoke(testObj1, new object[0]);

			name = nameProperty.GetValue(testObj1, new object[0]);
			Assert.AreEqual("name", name);

			objectServiceMock.Verify(o => o.Get(It.IsAny<ObjectReferenceData>()), Times.Exactly(2));
		}

		[Test]
		public void Generator_can_create_api_class_to_access_singleton_instances()
		{
			ModelsAre(
				Model("TestClass1").Name("TestClass1").Member("Name", "TestClass2"),
				Model("TestClass2").Name("TestClass2")
			);

			ObjectsAre(
				Object(Id("instance1", "TestClass1")).Value("instance1_value"),
				Object(Id("instance2", "TestClass1")).Value("instance2_value")
			);

			var testing = Generator(c => c
				.ApiName.Set("TestApi")
				.StaticInstanceIds.Add(s => s
					.Constant("instance1", "instance2")
					.When(ocm => ocm.Id.EndsWith("Class1"))));

			var testApi = testing.GenerateClientApi();

			var testClass2 = testApi.GetTypes().Single(t => t.Name == "TestClass2");
			var apiClass = testApi.GetTypes().Single(t => t.Name == "TestApi");

			Assert.AreEqual(TEST_NAMESPACE, apiClass.Namespace);
			Assert.IsNotNull(apiClass.GetConstructor(new[] { typeof(Rapplication) }));
			Assert.IsFalse(apiClass.GetMethods().Any(m => m.ReturnType == testClass2));

			var apiObj = Activator.CreateInstance(apiClass, testingRapplication);

			var instance1 = apiClass.GetMethod("GetTestClass1Instance1", new Type[0]).Invoke(apiObj, new object[0]);
			var instance2 = apiClass.GetMethod("GetTestClass1Instance2", new Type[0]).Invoke(apiObj, new object[0]);

			Assert.AreEqual("instance1_value", instance1.ToString());
			Assert.AreEqual("instance2_value", instance2.ToString());
		}

		[Test]
		public void Api_class_can_initialize_initializable_models()
		{
			ModelsAre(
				Model("TestClass").Name("TestClass").Initializer(PModel("name", "s-string")),
				Model("s-string").IsValue()
			);

			var testing = Generator(c => c
				.ApiName.Set("TestApi")
				.Use(p => p.ShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern()));

			var testApi = testing.GenerateClientApi();

			var apiClass = testApi.GetTypes().Single(t => t.Name == "TestApi");
			var testClass = testApi.GetTypes().Single(t => t.Name == "TestClass");

			var initializationMethod = apiClass.GetMethod("NewTestClass");

			Assert.IsNotNull(initializationMethod);
			Assert.AreEqual(1, initializationMethod.GetParameters().Length);
			Assert.AreEqual(typeof(string), initializationMethod.GetParameters()[0].ParameterType);
			Assert.AreEqual("name", initializationMethod.GetParameters()[0].Name);

			var apiObj = Activator.CreateInstance(apiClass, testingRapplication);

			Assert.IsInstanceOf(testClass, initializationMethod.Invoke(apiObj, new object[] { "test" }));
		}

		[Test]
		public void When_a_model_has_only_one_singleton_id__access_method_does_not_include_id_as_suffix()
		{
			ModelsAre(
				Model("TestClass1").Name("TestClass1").Member("Name", "TestClass2"),
				Model("TestClass2").Name("TestClass2")
			);

			ObjectsAre(
				Object(Id("instance1", "TestClass1")).Value("instance1_value")
			);

			var testing = Generator(c => c
				.ApiName.Set("TestApi")
				.StaticInstanceIds.Add(s => s
					.Constant("instance1")
					.When(ocm => ocm.Id.EndsWith("Class1"))));

			var testApi = testing.GenerateClientApi();

			var apiClass = testApi.GetTypes().Single(t => t.Name == "TestApi");

			var apiObj = Activator.CreateInstance(apiClass, testingRapplication);

			var instance1 = apiClass.GetMethod("GetTestClass1", new Type[0]).Invoke(apiObj, new object[0]);

			Assert.AreEqual("instance1_value", instance1.ToString());
		}

		[Test]
		public void When_a_model_without_member_has_a_singleton_id_its_access_member_is_rendered_as_property()
		{
			ModelsAre(
				Model("TestClass1").Name("TestClass1"),
				Model("TestClass2").Name("TestClass2")
			);

			ObjectsAre(
				Object(Id("instance1", "TestClass1")).Value("instance1_value"),
				Object(Id("instance2", "TestClass2")).Value("instance2_value"),
				Object(Id("instance3", "TestClass2")).Value("instance3_value")
			);

			var testing = Generator(c => c
				.ApiName.Set("TestApi")
				.StaticInstanceIds.Add(s => s.Constant("instance1").When(ocm => ocm.Id.EndsWith("Class1")))
				.StaticInstanceIds.Add(s => s.Constant("instance2", "instance3").When(ocm => ocm.Id.EndsWith("Class2"))));

			var testApi = testing.GenerateClientApi();

			var apiClass = testApi.GetTypes().Single(t => t.Name == "TestApi");

			Assert.AreEqual(TEST_NAMESPACE, apiClass.Namespace);
			Assert.IsNotNull(apiClass.GetConstructor(new[] { typeof(Rapplication) }));

			var apiObj = Activator.CreateInstance(apiClass, testingRapplication);

			var instance1 = apiClass.GetProperty("TestClass1").GetValue(apiObj, new object[0]);
			var instance2 = apiClass.GetProperty("TestClass2Instance2").GetValue(apiObj, new object[0]);
			var instance3 = apiClass.GetProperty("TestClass2Instance3").GetValue(apiObj, new object[0]);

			Assert.AreEqual("instance1_value", instance1.ToString());
			Assert.AreEqual("instance2_value", instance2.ToString());
			Assert.AreEqual("instance3_value", instance3.ToString());
		}

		[Test]
		public void When_api_class_is_generated__Robject_property_and_constructors_are_rendered_as_internal()
		{
			ModelsAre(
				Model("TestClass1").Name("TestClass1").Initializer(PModel("name", "s-string")),
				Model("s-string").IsValue()
			);

			ObjectsAre(
				Object(Id("instance1", "TestClass1")).Value("instance1_value")
			);

			var testing = Generator(c => c
				.ApiName.Set("TestApi")
				.Use(p => p.ShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern())
				.StaticInstanceIds.Add(s => s
					.Constant("instance1")
					.When(ocm => ocm.Id.EndsWith("Class1"))));

			var testApi = testing.GenerateClientApi();

			var testClass1 = testApi.GetTypes().Single(t => t.Name == "TestClass1");

			Assert.AreEqual(0, testClass1.GetConstructors().Length);
			Assert.IsNull(testClass1.GetProperty("Robject"));
		}

		[Test]
		public void Client_assemblies_can_be_marked_as_friend_to_enable_inter_client_assembly_dependency()
		{
			ModelsAre(
				Model("Module2-TestClass2").Module("Module2").Name("TestClass2"),
				Model("Module1-TestClass1").Module("Module1").Name("TestClass1")
				.Member("Sub", "Module2-TestClass2")
			);

			var otherApiGenerator = Generator(c => c
					.ApiName.Set("Module2Api")
					.DefaultNamespace.Set("Routine.Test.ApiGen.Client.Module2")
					.InMemory.Set(false)
					.FriendlyAssemblyNames.Add("Routine.Test.ApiGen.Client.Module1")
					.TypeIsRendered.Set(false, m => m.Module != "Module2")
				);

			var otherApi = otherApiGenerator.GenerateClientApi();

			var testing = Generator(c => c
					.ApiName.Set("Module1Api")
					.DefaultNamespace.Set("Routine.Test.ApiGen.Client.Module1")
					.InMemory.Set(false)
					.TypeIsRendered.Set(false, m => m.Module != "Module1")
					.ReferencedType.Set(s => s
						.By(t => t.Id.Replace("-", ".").Prepend("Routine.Test.ApiGen.Client.").ToTypeInfo(true))
						.When(t => t.Id.StartsWith("Module2")))
					)
				.AddReference(otherApi);

			var api = testing.GenerateClientApi();

			Assert.IsTrue(File.Exists(otherApi.Location));
			Assert.IsTrue(File.Exists(api.Location));
		}

		[Test][Ignore]
		public void Enum_support()
		{
			Assert.Fail("to be designed");
		}

		[Test][Ignore]
		public void Singleton_on_view_models()
		{
			Assert.Fail("to be designed");
		}
	}
}

