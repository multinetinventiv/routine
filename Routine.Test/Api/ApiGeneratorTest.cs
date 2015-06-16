using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Routine.Api;
using Routine.Api.Configuration;
using Routine.Core.Configuration;
using Routine.Test.Common;

namespace Routine.Test.Api
{
	[TestFixture]
	public class ApiGeneratorTest : ApiTestBase
	{
		protected override string DefaultNamespace { get { return "Routine.Test.Api.ApiGeneratorTest_Generated"; } }
		protected override IApiTemplate DefaultTestTemplate { get { return new TestTemplate(); } }

		protected override ConventionalApiConfiguration BaseConfiguration()
		{
			return BuildRoutine.ApiConfig().TestTemplate().Modes.Add(TestTemplate.Mode.Default);
		}

		[Test]
		public void Default_namespace_is_required()
		{
			try
			{
				var testing = EmptyGenerator(c => c.InMemory.Set(true));
				testing.Generate(new TestTemplate());
				Assert.Fail("exception not thrown");
			}
			catch (ConfigurationException ex)
			{
				Assert.IsTrue(ex.Message.Contains("DefaultNamespace"), string.Format("DefaultNamespace exception was expected, thrown exception message is: {0}", ex.Message));
			}
		}

		[Test]
		public void Generates_assembly_with_given_default_namespace_when_in_memory_is_set_to_false()
		{
			var testing = Generator(c => c.InMemory.Set(false).DefaultNamespace.Set("Default.Namespace"));

			var assembly = testing.Generate(new TestTemplate());

			Assert.AreEqual("Default.Namespace", assembly.GetName().Name);
		}

		[Test]
		public void Assembly_name_can_be_configured_explicitly()
		{
			var testing = Generator(c => c.InMemory.Set(false).DefaultNamespace.Set("Default.Namespace").OutputFileName.Set("output"));

			var assembly = testing.Generate(new TestTemplate());

			Assert.AreEqual("output", assembly.GetName().Name);
		}

		[Test]
		public void By_default_assembly_version_is_1_0_0_0()
		{
			var testing = Generator();

			var assembly = testing.Generate(new TestTemplate());

			Assert.AreEqual(new Version(1, 0, 0, 0), assembly.GetName().Version);
		}

		[Test]
		public void Assembly_version_can_be_configured()
		{
			var testing = Generator(c => c.AssemblyVersion.Set(new Version(2, 3, 4, 5)));

			var assembly = testing.Generate(new TestTemplate());

			Assert.AreEqual(new Version(2, 3, 4, 5), assembly.GetName().Version);
		}

		[Test]
		public void Assembly_guid_can_be_configured()
		{
			var testing = Generator(c => c.AssemblyGuid.Set(Guid.Parse("7461227D-5C22-4D10-9ACF-F699C95B3F10")));

			var assembly = testing.Generate(new TestTemplate());
			
			var attribute = (GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute),true)[0];
			Assert.AreEqual(Guid.Parse("7461227D-5C22-4D10-9ACF-F699C95B3F10"), Guid.Parse(attribute.Value));
		}

		[Test]
		public void By_default__rendered_types_are_under_given_default_namespace()
		{
			ModelsAre(Model("TestClassId").Name("TestClass"));

			var testing = Generator(c => c.DefaultNamespace.Set("Default.Namespace"));

			var testClass = GetRenderedType(testing.Generate(DefaultTestTemplate), "TestClass");

			Assert.IsTrue(testClass.Namespace.StartsWith("Default.Namespace"), "Namespace should start with Default.Namespace, but it is -> " + testClass.Namespace);
		}

		[Test]
		public void By_default__when_module_of_a_rendered_type_is_not_empty__its_namespace_contains_the_module_it_belongs()
		{
			ModelsAre(
				Model("TestClassId1").Module("Module").Name("TestClass1"),
				Model("TestClassId2").Name("TestClass2"));

			var types = Generator().Generate(DefaultTestTemplate).GetTypes();

			var testClass1 = types.Single(t => t.Name == "TestClass1");
			var testClass2 = types.Single(t => t.Name == "TestClass2");

			Assert.IsTrue(testClass1.Namespace.EndsWith("Module"), "Namespace should end with Module, but it is -> " + testClass1.Namespace);
			Assert.AreEqual(DefaultNamespace, testClass2.Namespace);
		}

		[Test]
		public void By_default__when_default_namespace_has_module__then_it_is_not_included_in_namespace_()
		{
			ModelsAre(
				Model("Module1-TestClass1").Module("Module1").Name("TestClass1"));

			var testing = Generator(c => c.DefaultNamespace.Set("Routine.Test.ApiGen.Client.Module1"));

			var testClass1 = GetRenderedType(testing.Generate(DefaultTestTemplate), "TestClass1");

			Assert.AreEqual("Routine.Test.ApiGen.Client.Module1", testClass1.Namespace);
		}

		[Test]
		public void By_default__type_name_is_directly_the_given_model_name()
		{
			ModelsAre(Model("TestClassId").Name("TestClass"));

			var types = Generator().Generate(new TestTemplate()).GetTypes();

			Assert.IsTrue(types.Any(t => t.Name == "TestClass"), "TestClass was not found");
		}

		[Test]
		public void Types_are_rendered_only_when_they_are_configured_so()
		{
			ModelsAre(
				Model("Included1-TestClass1").Module("Included1").Name("TestClass1"),
				Model("Included2-TestClass2").Module("Included2").Name("TestClass2"),
				Model("Excluded1-TestClass3").Module("Excluded1").Name("TestClass3"));

			var testing = Generator(c => c
				.TypeIsRendered.Set(false, m => !m.Module.Matches("Included.*"))
			);

			var types = testing.Generate(new TestTemplate()).GetTypes();

			Assert.IsTrue(types.Any(t => t.Name == "TestClass1"), "TestClass1 was not found");
			Assert.IsTrue(types.Any(t => t.Name == "TestClass2"), "TestClass2 was not found");
			Assert.IsTrue(types.All(t => t.Name != "TestClass3"), "TestClass2 was found");
		}

		[Test]
		public void By_default__rendered_type_instances_are_converted_to_robject_by_accessing_Robject_property_and_converted_to_object_by_passing_corresponding_robject_to_constructor()
		{
			ModelsAre(
				Model("TestClass1").Module("Module").Name("TestClass1")
				.Operation("TestClass2", "TestClass2", PModel("arg1", "TestClass2")),
				Model("TestClass2").Module("Module").Name("TestClass2"));

			ObjectsAre(
				Object(Id("1", "TestClass1")),
				Object(Id("2", "TestClass2"))
			);

			When(Id("1", "TestClass1"))
				.Performs("TestClass2", p =>
					p["arg1"].Values[0].ReferenceId == "2" &&
					p["arg1"].Values[0].ObjectModelId == "TestClass2"
				).Returns(Result(Id("2", "TestClass2")));

			var testing = Generator();
			var assembly = testing.Generate(new TestTemplate());

			var testClass1 = assembly.GetTypes().Single(t => t.Name == "TestClass1");
			var testClass2 = assembly.GetTypes().Single(t => t.Name == "TestClass2");

			var expected = Activator.CreateInstance(testClass2, Robj("2", "TestClass2"));

			var target = Activator.CreateInstance(testClass1, Robj("1", "TestClass1"));
			var actual = target.GetType().GetMethod("TestClass2").Invoke(target, new[] { expected });

			Assert.AreEqual(expected.ToString(), actual.ToString());
		}

		[Test]
		public void Rendered_type_template_can_be_altered_to_customize_the_initialization_of_rendered_types()
		{
			ModelsAre(
				Model("TestClass1").Module("Module").Name("TestClass1")
				.Operation("TestClass2", "TestClass2", PModel("arg1", "TestClass2")),
				Model("TestClass2").Module("Module").Name("TestClass2"));

			ObjectsAre(
				Object(Id("1", "TestClass1")),
				Object(Id("2", "TestClass2"))
			);

			When(Id("1", "TestClass1"))
				.Performs("TestClass2", p =>
					p["arg1"].Values[0].IsNull
				).Returns(Result(Id("2", "TestClass2")));

			var testing = Generator(c => c
				.RenderedTypeTemplate.Set(new SimpleTypeConversionTemplate("((TestClass2)null)", "new Routine.Client.Robject()"), mm => mm.Model.Id == "TestClass2")
			);

			var assembly = testing.Generate(new TestTemplate());

			var testClass1 = assembly.GetTypes().Single(t => t.Name == "TestClass1");
			var testClass2 = assembly.GetTypes().Single(t => t.Name == "TestClass2");

			var arg1 = CreateInstance(testClass2, "2", "TestClass2");

			var target = CreateInstance(testClass1, "1", "TestClass1");
			var actual = target.GetType().GetMethod("TestClass2").Invoke(target, new[] { arg1 });

			Assert.IsNull(actual);
		}

		[Test]
		public void View_types_can_be_used_to_render_conversion_methods()
		{
			ModelsAre(
				Model("ViewClass").Module("Module").Name("ViewClass").IsView("ActualClass"),
				Model("ActualClass").Module("Module").Name("ActualClass").ViewModelIds("ViewClass")
			);

			var testing = Generator();
			var assembly = testing.Generate(new TestTemplate());

			var viewClass = assembly.GetTypes().Single(t => t.Name == "ViewClass");
			var actualClass = assembly.GetTypes().Single(t => t.Name == "ActualClass");

			var actual = actualClass.GetMethod("AsViewClass");

			Assert.IsNotNull(actual);
			Assert.AreEqual(viewClass, actual.ReturnType);
		}

		[Test]
		public void Actual_types_can_be_used_to_find_implementations()
		{
			ModelsAre(
				Model("ViewClass").Module("Module").Name("ViewClass").IsView("ActualClass"),
				Model("ActualClass").Module("Module").Name("ActualClass").ViewModelIds("ViewClass")
			);

			var testing = Generator();
			var assembly = testing.Generate(new TestTemplate());

			var viewClass = assembly.GetTypes().Single(t => t.Name == "ViewClass");

			var actual = viewClass.GetMethod("IsActualClass");

			Assert.IsNotNull(actual);
			Assert.AreEqual(typeof(bool), actual.ReturnType);
		}
		
		[Test]
		public void When_a_keyword_is_used_for_parameter_name__escape_character_is_added_automatically()
		{
			ModelsAre(
				Model("TestClass").Module("Module").Name("TestClass")
				.Operation("Test", PModel("event", "TestClass"), PModel("object", "TestClass"), PModel("class", "TestClass"))
			);

			var testing = Generator();
			var assembly = testing.Generate(new TestTemplate());

			var viewClass = assembly.GetTypes().Single(t => t.Name == "TestClass");

			var actual = viewClass.GetMethod("Test");

			Assert.IsNotNull(actual);
			Assert.IsTrue(actual.GetParameters().Any(p => p.Name == "event"));
			Assert.IsTrue(actual.GetParameters().Any(p => p.Name == "object"));
			Assert.IsTrue(actual.GetParameters().Any(p => p.Name == "class"));
		}

		[Test]
		public void Initializer_is_rendered_only_when_it_is_configured_so()
		{
			ModelsAre(
				Model("Included-TestClass1").Module("Included").Name("TestClass1")
				.Initializer(PModel("input", "Included-InputClass")),
				Model("Excluded-TestClass2").Module("Excluded").Name("TestClass2")
				.Initializer(PModel("input", "Included-InputClass")),
				Model("Included-InputClass").Module("Included").Name("InputClass"));

			var testing = Generator(c => c
				.InitializerIsRendered.Set(false, ri => ri.Type.Module == "Excluded")
			);

			var types = testing.Generate(new TestTemplate()).GetTypes();
			var testClass1 = types.Single(t => t.Name == "TestClass1");
			var testClass2 = types.Single(t => t.Name == "TestClass2");
			var inputClass = types.Single(t => t.Name == "InputClass");

			Assert.IsNotNull(testClass1.GetConstructor(new[] { inputClass }));
			Assert.IsNull(testClass2.GetConstructor(new[] { inputClass }));
		}

		[Test]
		public void Members_are_rendered_only_when_they_are_configured_so()
		{
			ModelsAre(
				Model("TestClass1").Module("Module").Name("TestClass1")
				.Member("Included", "OutputClass")
				.Member("Excluded", "OutputClass"),
				Model("OutputClass").Module("Module").Name("OutputClass"));

			var testing = Generator(c => c
				.MemberIsRendered.Set(false, m => m.Id == "Excluded")
			);

			var types = testing.Generate(new TestTemplate()).GetTypes();
			var testClass1 = types.Single(t => t.Name == "TestClass1");

			Assert.IsNotNull(testClass1.GetProperty("Included"));
			Assert.IsNull(testClass1.GetProperty("Excluded"));
		}

		[Test]
		public void Operations_are_rendered_only_when_they_are_configured_so()
		{
			ModelsAre(
				Model("TestClass1").Module("Module").Name("TestClass1")
				.Operation("Included", "OutputClass")
				.Operation("Excluded", "OutputClass"),
				Model("OutputClass").Module("Module").Name("OutputClass"));

			var testing = Generator(c => c
				.OperationIsRendered.Set(false, m => m.Id == "Excluded")
			);

			var types = testing.Generate(new TestTemplate()).GetTypes();
			var testClass1 = types.Single(t => t.Name == "TestClass1");

			Assert.IsNotNull(testClass1.GetMethod("Included"));
			Assert.IsNull(testClass1.GetMethod("Excluded"));
		}

		[Test]
		public void Generated_assemblies_can_have_friend_assemblies()
		{
			ModelsAre(
				Model("TestClass1").Module("Module1").Name("TestClass1")
			);

			var generator = Generator(c => c
					.DefaultNamespace.Set("Routine.Test.ApiGen.Client.Module1")
					.InMemory.Set(false)
					.FriendlyAssemblyNames.Add("Routine.Test.ApiGen.Client.Module2", "Routine.Test.ApiGen.Client.Module3")
					.TypeIsRendered.Set(false, m => m.Module != "Module1")
				);

			var api = generator.Generate(new TestTemplate());
			var actual = api.GetCustomAttributes(typeof(InternalsVisibleToAttribute), false);

			Assert.AreEqual(2, actual.Length);

			Assert.AreEqual("Routine.Test.ApiGen.Client.Module2", ((InternalsVisibleToAttribute)actual[0]).AssemblyName);
			Assert.AreEqual("Routine.Test.ApiGen.Client.Module3", ((InternalsVisibleToAttribute)actual[1]).AssemblyName);
		}

		[Test]
		public void Types_can_directly_be_referenced_instead_of_being_rendered()
		{
			ModelsAre(
				Model("TestClass1").Module("Module").Name("TestClass1")
				.Operation("Reference", "ReferencedClass"),
				Model("ReferencedClass").Name("ReferencedClass"));

			var testing = Generator(c => c
				.TypeIsRendered.Set(false, t => t.Id == "ReferencedClass")
				.ReferencedType.Set(type.of<string>(), t => t.Id == "ReferencedClass")
			);

			var types = testing.Generate(new TestTemplate()).GetTypes();
			var testClass1 = types.Single(t => t.Name == "TestClass1");

			Assert.AreEqual(typeof(string), testClass1.GetMethod("Reference").ReturnType);
		}

		[Test]
		public void When_referenced__a_type_requires_a_referenced_type_template_to_be_configured()
		{
			ModelsAre(
				Model("TestClass1").Module("Module").Name("TestClass1")
				.Operation("Reference", "ReferencedClass"),
				Model("ReferencedClass").Name("ReferencedClass"));

			var testing = Generator(c => c
				.TypeIsRendered.Set(false, t => t.Id == "ReferencedClass")
				.ReferencedType.Set(type.of<int>(), t => t.Id == "ReferencedClass")
			);

			Assert.Throws<ConfigurationException>(() => testing.Generate(new TestTemplate()));

			testing = Generator(c => c
				.TypeIsRendered.Set(false, t => t.Id == "ReferencedClass")
				.ReferencedType.Set(type.of<int>(), t => t.Id == "ReferencedClass")
				.ReferencedTypeTemplate.Set(new SimpleTypeConversionTemplate("int.Parse({robject}.Id)", "{rtype}.Get({object}.ToString())"))
			);

			var types = testing.Generate(new TestTemplate()).GetTypes();
			var testClass1 = types.Single(t => t.Name == "TestClass1");

			Assert.AreEqual(typeof(int), testClass1.GetMethod("Reference").ReturnType);
		}

		[Test]
		public void When_referenced__type_conversion_is_made_through_referenced_type_templates()
		{
			var inputGuid = Guid.NewGuid();
			var expectedGuid = Guid.NewGuid();

			ModelsAre(
				Model("Guid").IsValue(),
				Model("TestClass").Name("TestClass")
				.Operation("Uid", "Guid", PModel("uid", "Guid")));

			ObjectsAre(
				Object(Id("test_id", "TestClass")));

			When(Id("test_id", "TestClass"))
				.Performs("Uid", p =>
					p["uid"].Values[0].ReferenceId == inputGuid.ToString() &&
					p["uid"].Values[0].ObjectModelId == "Guid"
				).Returns(Result(Id(expectedGuid.ToString(), "Guid")));

			var assembly = Generator(c => c
				.TypeIsRendered.Set(false, t => t.Id == "Guid")
				.ReferencedType.Set(type.of<Guid>(), t => t.Id == "Guid")
				.ReferencedTypeTemplate.Set(new SimpleTypeConversionTemplate("System.Guid.Parse({robject}.Id)", "{rtype}.Get({object}.ToString())"))
			).Generate(DefaultTestTemplate);

			var target = CreateInstance(GetRenderedType(assembly, "TestClass"), "test_id", "TestClass");

			var actualGuid = (Guid)target.GetType().GetMethod("Uid").Invoke(target, new object[] { inputGuid });

			Assert.AreEqual(expectedGuid, actualGuid);
		}

		[Test]
		public void When_referenced__by_default__string_values_are_converted_to_robject_using_id()
		{
			var inputGuid = Guid.NewGuid();
			var expectedGuid = Guid.NewGuid();

			ModelsAre(
				Model("Guid").IsValue(),
				Model("TestClass").Name("TestClass")
				.Operation("Uid", "Guid", PModel("uid", "Guid")));

			ObjectsAre(
				Object(Id("test_id", "TestClass")));

			When(Id("test_id", "TestClass"))
				.Performs("Uid", p =>
					p["uid"].Values[0].ReferenceId == inputGuid.ToString() &&
					p["uid"].Values[0].ObjectModelId == "Guid"
				).Returns(Result(Id(expectedGuid.ToString(), "Guid")));

			var assembly = Generator(c => c
				.TypeIsRendered.Set(false, t => t.Id == "Guid")
				.ReferencedType.Set(type.of<string>(), t => t.Id == "Guid")
			).Generate(DefaultTestTemplate);

			var testObj = CreateInstance(GetRenderedType(assembly, "TestClass"), "test_id", "TestClass");

			var actualGuid = (string)testObj.GetType().GetMethod("Uid").Invoke(testObj, new object[] { inputGuid.ToString() });
			Assert.AreEqual(expectedGuid.ToString(), actualGuid);
		}

		public class ReferencedClass { }

		[Test]
		public void Referenced_types_other_than_System_types__should_have_their_assembly_added_to_the_references()
		{
			ModelsAre(
				Model("TestClass1").Module("Module").Name("TestClass1")
				.Operation("Reference", "ReferencedClass"),
				Model("ReferencedClass").Module("Module").Name("ReferencedClass"));

			var testing = Generator(c => c
				.TypeIsRendered.Set(false, t => t.Id == "ReferencedClass")
				.ReferencedType.Set(type.of<ReferencedClass>(), t => t.Id == "ReferencedClass")
				.ReferencedTypeTemplate.Set(new SimpleTypeConversionTemplate("new " + type.of<ReferencedClass>().ToCSharpString() + "()", "new Routine.Client.Robject()"), type.of<ReferencedClass>()) //dummy template to get over configuration & compile exception
			);

			Assert.Throws<ApiGenerationException>(() => testing.Generate(new TestTemplate()));

			testing.AddReference<ReferencedClass>();

			var types = testing.Generate(new TestTemplate()).GetTypes();
			var testClass1 = types.Single(t => t.Name == "TestClass1");

			Assert.AreEqual(typeof(ReferencedClass), testClass1.GetMethod("Reference").ReturnType);
		}

		[Test]
		public void Referenced_type_by_short_model_id_pattern__Types_having_their_id_starting_with_given_prefix__are_referenced_to_the_type_resolved_by_short_model_id_pattern()
		{
			ModelsAre(
				Model("TestClass").Name("TestClass")
				.Member("Price", "s-int-32")
				.Member("Name", "s-string")
				.Member("Address", "c-fat-string"),
				Model("s-string").IsValue(),
				Model("s-int-32").IsValue(),
				Model("c-fat-string").IsValue()
			);

			var testing =
				Generator(c => c
					.Use(p => p.ReferencedTypeByShortModelIdPattern("System", "s"))
					.Use(p => p.ReferencedTypeByShortModelIdPattern("Routine.Test.Common", "c"))
					.ReferencedTypeTemplate.Set(rtt => rtt
						.By(t => new SimpleTypeConversionTemplate("new " + t.ToCSharpString() + "()", "new Routine.Client.Robject()"))
						.When(t => t is TypeInfo)
					)
				).AddReference<FatString>();

			var assembly = testing.Generate(new TestTemplate());

			Assert.AreEqual(1, assembly.GetTypes().Count());

			var testClass = assembly.GetTypes().Single(t => t.Name == "TestClass");

			var actual = testClass.GetProperties().Single(p => p.Name == "Price").PropertyType;
			Assert.AreEqual(typeof(int), actual);

			actual = testClass.GetProperties().Single(p => p.Name == "Name").PropertyType;
			Assert.AreEqual(typeof(string), actual);

			actual = testClass.GetProperties().Single(p => p.Name == "Address").PropertyType;
			Assert.AreEqual(typeof(FatString), actual);
		}

		[Test]
		public void Parseable_value_type_pattern__Applies_to_value_types_with_static_parse_method_and_converts_robject_to_target_type_by_passing_its_id_to_the_parse_method()
		{
			const int expectedInt = 10;
			var expectedGuid = Guid.NewGuid();

			ModelsAre(
				Model("TestClass").Name("TestClass")
				.Operation("ParseableValueTypes", "s-int-32", PModel("uid", "s-guid")),
				Model("s-guid").IsValue(),
				Model("s-int-32").IsValue()
			);

			ObjectsAre(
				Object(Id("test_id", "TestClass"))
			);

			When(Id("test_id", "TestClass"))
				.Performs("ParseableValueTypes", p =>
					p["uid"].Values[0].ReferenceId == expectedGuid.ToString() &&
					p["uid"].Values[0].ObjectModelId == "s-guid"
				).Returns(Result(Id(expectedInt.ToString(CultureInfo.InvariantCulture), "s-int-32")));

			var assembly =
				Generator(c => c
					.Use(p => p.ReferencedTypeByShortModelIdPattern("System", "s"))
					.Use(p => p.ParseableValueTypePattern())
				).Generate(DefaultTestTemplate);

			var testClassInstance = CreateInstance(GetRenderedType(assembly, "TestClass"), "test_id", "TestClass");

			var actualInt = (int)testClassInstance.GetType().GetMethod("ParseableValueTypes").Invoke(testClassInstance, new object[] { expectedGuid });
			Assert.AreEqual(expectedInt, actualInt);
		}

		[Test]
		public void Parseable_value_type_pattern__Nullable_support()
		{
			ModelsAre(
				Model("TestClass").Name("TestClass")
				.Operation("NullableTypes", "s-int-32?", PModel("uid", "s-guid?")),
				Model("s-guid?").IsValue(),
				Model("s-int-32?").IsValue()
			);

			ObjectsAre(
				Object(Id("test_id", "TestClass"))
			);
			
			//this is set to prevent default behaviour to return a non-null value
			When(Id("test_id", "TestClass"))
				.Performs("NullableTypes").Returns(Result(Id("1", "s-int-32?")));

			When(Id("test_id", "TestClass"))
				.Performs("NullableTypes", p =>
					p["uid"].Values[0].IsNull
				).Returns(Result(Null("s-int-32?")));

			var assembly =
				Generator(c => c
					.Use(p => p.ReferencedTypeByShortModelIdPattern("System", "s"))
					.Use(p => p.ParseableValueTypePattern())
				).Generate(DefaultTestTemplate);

			var testClassInstance = CreateInstance(GetRenderedType(assembly, "TestClass"), "test_id", "TestClass");

			var actual = (int?)testClassInstance.GetType().GetMethod("NullableTypes").Invoke(testClassInstance, new object[] { null });
			Assert.IsNull(actual);

			//tests if default behaviour change is working
			actual = (int?)testClassInstance.GetType().GetMethod("NullableTypes").Invoke(testClassInstance, new object[] { Guid.NewGuid() });
			Assert.IsNotNull(actual);
		}

		[Test]
		public void Skips_view_types__initializers__members_and_operations_needing_a_type_that_is_neither_rendered_nor_referenced()
		{
			ModelsAre(
				Model("Included-TestClass1").Module("Included").Name("TestClass1")
				.ViewModelIds("Excluded-ViewClass")
				.Initializer(PModel("parameterExcluded", "Excluded-TestClass2"))
				.Member("PropertyExcluded", "Excluded-TestClass2")
				.Member("PropertyIncluded", "s-string")
				.Operation("MethodExcludedBecauseOfReturnType", "Excluded-TestClass2")
				.Operation("MethodExcludedBecauseOfParameter", "s-string", PModel("excludeReason", "Excluded-TestClass2"))
				.Operation("MethodIncluded", "s-string"),
				Model("Excluded-TestClass2").Module("Excluded").Name("TestClass2"),
				Model("Excluded-ViewClass").Module("Excluded").Name("ViewClass").IsView("Included-TestClass1"),
				Model("s-string").IsValue()
			);

			var testing = Generator(c => c
				.Use(p => p.ReferencedTypeByShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern())
				.TypeIsRendered.Set(false, m => !m.Module.Matches("Included.*")));

			var types = testing.Generate(new TestTemplate()).GetTypes();

			Assert.IsFalse(types.Any(t => t.Name == "TestClass2"), "TestClass2 was found");

			var testClass1 = types.Single(t => t.Name == "TestClass1");

			Assert.IsNull(testClass1.GetMethod("AsViewClass"));

			Assert.IsFalse(testClass1.GetConstructors().Any(c => c.GetParameters().Any(p => p.Name == "parameterExcluded")), "Initializer should be excluded but was found");

			Assert.IsNull(testClass1.GetProperty("PropertyExcluded"), "PropertyExcluded was found");
			Assert.IsNull(testClass1.GetMethod("MethodExcludedBecauseOfReturnType"), "MethodExcludedBecauseOfReturnType was found");
			Assert.IsNull(testClass1.GetMethod("MethodExcludedBecauseOfParameter"), "MethodExcludedBecauseOfParameter was found");

			Assert.IsNotNull(testClass1.GetProperty("PropertyIncluded"), "PropertyIncluded was not found");
			Assert.IsNotNull(testClass1.GetMethod("MethodIncluded"), "MethodIncluded was not found");
		}

		[Test]
		public void List_member_support()
		{
			ModelsAre(
				Model("TestClass").Name("TestClass").Member("OrderIds", "s-string", true),
				Model("s-string").IsValue()
			);

			var testing = Generator(c => c
				.Use(p => p.ReferencedTypeByShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern()));

			var properties = GetRenderedType(testing.Generate(DefaultTestTemplate), "TestClass").GetProperties();

			var nameProperty = properties.Single(p => p.Name == "OrderIds");

			Assert.AreEqual(typeof(List<string>), nameProperty.PropertyType);
		}

		[Test]
		public void List_operation_result_support()
		{
			ModelsAre(
				Model("TestClass").Name("TestClass").Operation("StringListMethod", "s-string", true),
				Model("s-string").IsValue()
			);
			var testing = Generator(c => c
				.Use(p => p.ReferencedTypeByShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern()));

			var methods = GetRenderedType(testing.Generate(DefaultTestTemplate), "TestClass").GetMethods();

			var stringMethod = methods.Single(m => m.Name == "StringListMethod");

			Assert.AreEqual(typeof(List<string>), stringMethod.ReturnType);
		}

		[Test]
		public void List_parameter_support()
		{
			ModelsAre(
				Model("TestClass").Name("TestClass").Operation("StringListParameterMethod", "s-string", PModel("strs", "s-string", true)),
				Model("s-string").IsValue()
			);
			var testing = Generator(c => c
				.Use(p => p.ReferencedTypeByShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern()));

			var methods = GetRenderedType(testing.Generate(DefaultTestTemplate), "TestClass").GetMethods();

			var stringMethod = methods.Single(m => m.Name == "StringListParameterMethod");

			Assert.AreEqual(typeof(List<string>), stringMethod.GetParameters()[0].ParameterType);
		}

		[Test]
		public void Null_input_and_output_support()
		{
			ModelsAre(
				Model("TestClass1").Module("Module").Name("TestClass1")
				.Operation("Operation", "TestClass2", PModel("arg", "TestClass2")),
				Model("TestClass2").Module("Module").Name("TestClass2"));

			ObjectsAre(
				Object(Id("test1", "TestClass1")),
				Object(Id("fail", "TestClass2"))
			);

			When(Id("test1", "TestClass1")).Performs("Operation"
				).Returns(Result(Id("fail", "TestClass2")))
			;

			When(Id("test1", "TestClass1")).Performs("Operation", 
				p => p["arg"].Values[0].IsNull
				).Returns(Result(Null("TestClass2")));

			var testing = Generator();

			var assembly = testing.Generate(new TestTemplate());
			var testClass1 = GetRenderedType(assembly, "TestClass1");

			var operation = testClass1.GetMethod("Operation");

			var test1 = CreateInstance(testClass1, "test1", "TestClass1");

			var actual = operation.Invoke(test1, new object[] {null});
			Assert.IsNull(actual);
		}

		[Test]
		public void When_type_is_list__conversion_template_uses_list_methods_of_rapplication_and_rvariable()
		{
			ModelsAre(
				Model("TestClass").Name("TestClass").Operation("StringListParameterMethod", "s-string", true, PModel("strs", "s-string", true)),
				Model("s-string").IsValue()
			);

			ObjectsAre(
				Object(Id("test_id", "TestClass"))
			);

			When(Id("test_id", "TestClass"))
				.Performs("StringListParameterMethod", p =>
					p["strs"].IsList &&
					p["strs"].Values[0].ReferenceId == "str1" &&
					p["strs"].Values[1].ReferenceId == "str2"
				).Returns(Result(Id("str_out1", "s-string"), Id("str_out2", "s-string")));

			var assembly = Generator(c => c
				.Use(p => p.ReferencedTypeByShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern()))
				.Generate(DefaultTestTemplate);

			var instance = CreateInstance(GetRenderedType(assembly, "TestClass"), "test_id", "TestClass");

			var actual = (List<string>)instance.GetType().GetMethod("StringListParameterMethod").Invoke(instance, new object[] { new List<string> { "str1", "str2" } });

			Assert.AreEqual(2, actual.Count);
			Assert.AreEqual("str_out1", actual[0]);
			Assert.AreEqual("str_out2", actual[1]);
		}

		[Test]
		public void Void_operation_result_support()
		{
			ModelsAre(
				Model("TestClass").Name("TestClass").Operation("VoidMethod", true)
			);
			var testing = Generator();

			var methods = GetRenderedType(testing.Generate(DefaultTestTemplate), "TestClass").GetMethods();

			var stringMethod = methods.Single(m => m.Name == "VoidMethod");

			Assert.AreEqual(typeof(void), stringMethod.ReturnType);
		}

		[Test]
		public void Namespaces_can_be_configured()
		{
			ModelsAre(
				Model("TestClass").Module("Module").Name("TestClass")
			);

			var testing = Generator(c => c.RenderedTypeNamespace.Set("Test"));

			var assembly = testing.Generate(DefaultTestTemplate);

			var testClass = GetRenderedType(assembly, "TestClass");

			Assert.AreEqual("Test", testClass.Namespace);
		}

		[Test]
		public void Type_names_can_be_configured()
		{
			ModelsAre(
				Model("TestClass").Module("Module").Name("TestClass")
			);

			var testing = Generator(c => c.RenderedTypeName.Set("Test"));

			var assembly = testing.Generate(DefaultTestTemplate);

			var testClass = GetRenderedType(assembly, "Test");

			Assert.IsNotNull(testClass);
			Assert.AreEqual("Test", testClass.Name);
		}

		[Test]
		public void Member__operation_and_parameter_names_can_be_configured()
		{
			ModelsAre(
				Model("TestClass").Module("Module").Name("TestClass")
				.Member("Member", "TestClass")
				.Operation("Operation", "TestClass", PModel("parameter", "TestClass"))
			);

			var testing = Generator(cfg => cfg
				.RenderedTypeName.Set(c => c.By(mm => "Changed" + mm.Model.Type.Name))
				.RenderedMemberName.Set(c => c.By(mm => "Changed" + mm.Model.Id))
				.RenderedOperationName.Set(c => c.By(mm => "Changed" + mm.Model.Id))
				.RenderedParameterName.Set(c => c.By(mm => "changed" + mm.Model.Id.ToUpperInitial()))
			);

			var assembly = testing.Generate(DefaultTestTemplate);

			var testClass = GetRenderedType(assembly, "ChangedTestClass");

			Assert.IsNotNull(testClass.GetProperty("ChangedMember"));
			Assert.AreEqual(testClass, testClass.GetProperty("ChangedMember").PropertyType);

			Assert.IsNotNull(testClass.GetMethod("ChangedOperation"));
			Assert.AreEqual(testClass, testClass.GetMethod("ChangedOperation").ReturnType);
			Assert.AreEqual("changedParameter", testClass.GetMethod("ChangedOperation").GetParameters()[0].Name);
			Assert.AreEqual(testClass, testClass.GetMethod("ChangedOperation").GetParameters()[0].ParameterType);
		}

		[Test]
		public void Rendered_types_have_modes_so_that_templates_can_render_more_than_one_type_for_each_rendered_type__eg__interface_and_implementation()
		{
			ModelsAre(
				Model("TestClass").Module("Module").Name("TestClass")
			);

			var testing = Generator(cfg => cfg
				.Modes.Add(TestTemplate.Mode.Mode1)
				.Modes.Add(TestTemplate.Mode.Mode2)
				.RenderedTypeName.Set(c => c.By(mm => "Mode1" + mm.Model.Type.Name).When(mm => mm.Mode == TestTemplate.Mode.Mode1))
				.RenderedTypeName.Set(c => c.By(mm => "Mode2" + mm.Model.Type.Name).When(mm => mm.Mode == TestTemplate.Mode.Mode2))
			);

			var assembly = testing.Generate(DefaultTestTemplate);

			var mode1 = GetRenderedType(assembly, "Mode1TestClass");
			var mode2 = GetRenderedType(assembly, "Mode2TestClass");

			Assert.IsNotNull(mode1);
			Assert.IsNotNull(mode2);
		}

		[Test]
		public void Namespace__member__operation_and_parameter_names_can_be_configured_based_on_modes()
		{
			ModelsAre(
				Model("TestClass").Module("Module").Name("TestClass")
				.Member("Member", "TestClass")
				.Operation("Operation", "TestClass", PModel("parameter", "TestClass"))
			);

			var testing = Generator(cfg => cfg
				.Modes.Add(TestTemplate.Mode.Mode1)
				.Modes.Add(TestTemplate.Mode.Mode2)

				.RenderedTypeNamespace.Set(c => c.By(mm => "Mode1Namespace").When(mm => mm.Mode == TestTemplate.Mode.Mode1))
				.RenderedTypeName.Set(c => c.By(mm => "Mode1" + mm.Model.Type.Name).When(mm => mm.Mode == TestTemplate.Mode.Mode1))
				.RenderedMemberName.Set(c => c.By(mm => "Mode1" + mm.Model.Id).When(mm => mm.Mode == TestTemplate.Mode.Mode1))
				.RenderedOperationName.Set(c => c.By(mm => "Mode1" + mm.Model.Id).When(mm => mm.Mode == TestTemplate.Mode.Mode1))
				.RenderedParameterName.Set(c => c.By(mm => "mode1" + mm.Model.Id.ToUpperInitial()).When(mm => mm.Mode == TestTemplate.Mode.Mode1))

				.RenderedTypeNamespace.Set(c => c.By(mm => "Mode2Namespace").When(mm => mm.Mode == TestTemplate.Mode.Mode2))
				.RenderedTypeName.Set(c => c.By(mm => "Mode2" + mm.Model.Type.Name).When(mm => mm.Mode == TestTemplate.Mode.Mode2))
				.RenderedMemberName.Set(c => c.By(mm => "Mode2" + mm.Model.Id).When(mm => mm.Mode == TestTemplate.Mode.Mode2))
				.RenderedOperationName.Set(c => c.By(mm => "Mode2" + mm.Model.Id).When(mm => mm.Mode == TestTemplate.Mode.Mode2))
				.RenderedParameterName.Set(c => c.By(mm => "mode2" + mm.Model.Id.ToUpperInitial()).When(mm => mm.Mode == TestTemplate.Mode.Mode2))
			);

			var assembly = testing.Generate(DefaultTestTemplate);

			var mode1 = GetRenderedType(assembly, "Mode1TestClass");

			Assert.AreEqual("Mode1Namespace", mode1.Namespace);
			Assert.IsNotNull(mode1.GetProperty("Mode1Member"));
			Assert.AreEqual(mode1, mode1.GetProperty("Mode1Member").PropertyType);

			Assert.IsNotNull(mode1.GetMethod("Mode1Operation"));
			Assert.AreEqual(mode1, mode1.GetMethod("Mode1Operation").ReturnType);
			Assert.AreEqual("mode1Parameter", mode1.GetMethod("Mode1Operation").GetParameters()[0].Name);
			Assert.AreEqual(mode1, mode1.GetMethod("Mode1Operation").GetParameters()[0].ParameterType);

			var mode2 = GetRenderedType(assembly, "Mode2TestClass");

			Assert.AreEqual("Mode2Namespace", mode2.Namespace);
			Assert.IsNotNull(mode2.GetProperty("Mode2Member"));
			Assert.AreEqual(mode2, mode2.GetProperty("Mode2Member").PropertyType);

			Assert.IsNotNull(mode2.GetMethod("Mode2Operation"));
			Assert.AreEqual(mode2, mode2.GetMethod("Mode2Operation").ReturnType);
			Assert.AreEqual("mode2Parameter", mode2.GetMethod("Mode2Operation").GetParameters()[0].Name);
			Assert.AreEqual(mode2, mode2.GetMethod("Mode2Operation").GetParameters()[0].ParameterType);
		}

		public class CustomAttribute : Attribute { }

		[Test]
		public void Types__initializers__members__operations_and_parameters_can_be_decorated_with_configured_attributes()
		{
			ModelsAre(
				Model("TestClass").Module("Module").Name("TestClass")
				.Initializer()
				.Member("Member", "TestClass")
				.Operation("Operation", "TestClass", PModel("parameter", "TestClass"))
			);

			var testing = Generator(cfg => cfg
				.RenderedTypeAttributes.Add(type.of<CustomAttribute>())
				.RenderedInitializerAttributes.Add(type.of<CustomAttribute>())
				.RenderedMemberAttributes.Add(type.of<CustomAttribute>())
				.RenderedOperationAttributes.Add(type.of<CustomAttribute>())
				.RenderedParameterAttributes.Add(type.of<CustomAttribute>())
			).AddReference<CustomAttribute>();

			var assembly = testing.Generate(DefaultTestTemplate);

			var testClass = GetRenderedType(assembly, "TestClass");
			var initializer = testClass.GetConstructor(new Type[0]);
			var member = testClass.GetProperty("Member");
			var operation = testClass.GetMethod("Operation");
			var parameter = operation.GetParameters().Single(p => p.Name == "parameter");

			Assert.IsTrue(Attribute.IsDefined(testClass, typeof(CustomAttribute)));
			Assert.IsTrue(Attribute.IsDefined(initializer, typeof(CustomAttribute)));
			Assert.IsTrue(Attribute.IsDefined(member, typeof(CustomAttribute)));
			Assert.IsTrue(Attribute.IsDefined(operation, typeof(CustomAttribute)));
			Assert.IsTrue(Attribute.IsDefined(parameter, typeof(CustomAttribute)));
		}

		[Test]
		public void ObsoletePattern__Types__initializers__members_and_operations_are_added_ObsoleteAttribute_when_has_given_mark()
		{
			ModelsAre(
				Model("TestClass").Module("Module").Name("TestClass").Mark("obsolete")
				.Initializer().MarkInitializer("obsolete")
				.Member("Member", "TestClass").MarkMember("Member", "obsolete")
				.Operation("Operation", "TestClass", PModel("parameter", "TestClass")).MarkOperation("Operation", "obsolete")
			);

			var testing = Generator(cfg => cfg
				.Use(p => p.ObsoletePattern("obsolete"))
			);

			var assembly = testing.Generate(DefaultTestTemplate);

			var testClass = GetRenderedType(assembly, "TestClass");
			var initializer = testClass.GetConstructor(new Type[0]);
			var member = testClass.GetProperty("Member");
			var operation = testClass.GetMethod("Operation");

			Assert.IsTrue(Attribute.IsDefined(testClass, typeof(ObsoleteAttribute)));
			Assert.IsTrue(Attribute.IsDefined(initializer, typeof(ObsoleteAttribute)));
			Assert.IsTrue(Attribute.IsDefined(member, typeof(ObsoleteAttribute)));
			Assert.IsTrue(Attribute.IsDefined(operation, typeof(ObsoleteAttribute)));
		}

		[Test][Ignore]
		public void Application_model_is_integrated_so_that_further_usage_of_the_api_does_not_require_application_model_to_be_loaded()
		{
			Assert.Fail();
		}
	}
}

