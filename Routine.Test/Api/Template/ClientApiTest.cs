using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Routine.Core;

namespace Routine.Test.Api.Template
{
	[TestFixture]
	public class ClientApiTest : ClientApiTestBase
	{
		protected override string DefaultNamespace { get { return "Routine.Test.Api.Template.ClientApiTest_Generated"; } }

		[Test]
		public void Rendered_types_are_concrete_classes()
		{
			ModelsAre(Model("TestClass").Name("TestClass").Member("Self", "TestClass"));

			var testClass = GetRenderedType("TestClassImpl");

			Assert.IsNotNull(testClass);
		}

		[Test]
		public void Interfaces_are_generated_for_all_rendered_types_to_enable_mocking()
		{
			ModelsAre(Model("TestClass").Name("TestClass").Member("Self", "TestClass"));

			var iTestClass = GetRenderedType("TestClass");

			Assert.IsNotNull(iTestClass);
			Assert.IsTrue(iTestClass.IsInterface);
		}

		[Test]
		public void Concrete_classes_are_rendered_as_internal()
		{
			ModelsAre(Model("TestClass").Name("TestClass").Member("Self", "TestClass"));

			var testClass = GetRenderedType("TestClassImpl");

			Assert.IsTrue(testClass.IsNotPublic);
		}

		[Test]
		public void Rendered_types_override_ToString_method_returning_value()
		{
			ModelsAre(Model("TestClass").Name("TestClass").Member("Self", "TestClass"));

			ObjectsAre(Object(Id("test", "TestClass")).Value("test_value"));

			var testObj1 = CreateInstance("test", "TestClass");

			Assert.AreEqual("test_value", testObj1.ToString());
		}

		[Test]
		public void Concrete_classes_implement_their_interfaces()
		{
			ModelsAre(Model("TestClass").Name("TestClass").Member("Self", "TestClass"));

			var assembly = Generator().Generate(DefaultTestTemplate);
			var testClass = GetRenderedType(assembly, "TestClassImpl");
			var iTestClass = GetRenderedType(assembly, "TestClass");

			Assert.IsTrue(iTestClass.IsAssignableFrom(testClass));
		}

		[Test]
		public void Members_are_rendered_as_read_only_interface_properties_implemented_explicitly_by_concrete_classes()
		{
			ModelsAre(
				Model("TestClass").Name("TestClass").Member("Name", "s-string"),
				Model("s-string").IsValue()
			);

			var testing = Generator(c => c
				.Use(p => p.ReferencedTypeByShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern()));

			var properties = GetRenderedType(testing.Generate(DefaultTestTemplate), "TestClass").GetProperties();

			Assert.IsTrue(properties.Any(p => p.Name == "Name"), "Name property was not found");

			var name = properties.Single(p => p.Name == "Name");

			Assert.AreEqual(typeof(string), name.PropertyType);
			Assert.IsTrue(name.CanRead);
			Assert.IsFalse(name.CanWrite);
		}

		[Test]
		public void Rendered_types_are_bound_via_interfaces()
		{
			ModelsAre(
				Model("TestClass").Name("TestClass").Member("Self", "TestClass")
			);

			var iTestClass = GetRenderedType("TestClass");
			var properties = iTestClass.GetProperties();

			var self = properties.Single(p => p.Name == "Self");

			Assert.AreEqual(iTestClass, self.PropertyType);
		}

		[Test]
		public void Operations_are_rendered_as_interface_methods_which_are_implemented_explicitly_by_concrete_classes()
		{
			ModelsAre(
				Model("TestClass").Name("TestClass")
				.Operation("VoidMethod", true)
			);

			var methods = GetRenderedType("TestClass").GetMethods();

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
				.Use(p => p.ReferencedTypeByShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern()));

			var methods = GetRenderedType(testing.Generate(DefaultTestTemplate), "TestClass").GetMethods();

			var stringMethod = methods.Single(m => m.Name == "StringMethod");

			Assert.IsTrue(stringMethod.ReturnType == typeof(string));
		}

		[Test]
		public void Operation_parameters_are_method_parameters()
		{
			ModelsAre(
				Model("TestClass")
				.Name("TestClass")
				.Operation("ParameterMethod", true, PModel("arg1", "s-string"), PModel("arg2", "TestClass", true)),
				Model("s-string").IsValue()
			);

			var testing = Generator(c => c
				.Use(p => p.ReferencedTypeByShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern()));

			var assembly = testing.Generate(DefaultTestTemplate);
			var iTestClass = GetRenderedType(assembly, "TestClass");
			var methods = iTestClass.GetMethods();

			var parameterMethod = methods.Single(m => m.Name == "ParameterMethod");

			Assert.AreEqual(2, parameterMethod.GetParameters().Length);
			Assert.AreEqual("arg1", parameterMethod.GetParameters()[0].Name);
			Assert.AreEqual(typeof(string), parameterMethod.GetParameters()[0].ParameterType);
			Assert.AreEqual("arg2", parameterMethod.GetParameters()[1].Name);
			Assert.AreEqual(typeof(List<>).MakeGenericType(iTestClass), parameterMethod.GetParameters()[1].ParameterType);
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
				.Use(p => p.ReferencedTypeByShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern()));

			var methods = GetRenderedType(testing.Generate(DefaultTestTemplate), "TestClass").GetMethods();

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

		public class CustomAttribute : Attribute { }

		[Test]
		public void When_configured__types__initializers__members__operations_and_parameters_renders_attributes()
		{
			ModelsAre(
				Model("TestClass")
				.Name("TestClass")
				.Member("Member", "TestClass")
				.Operation("Operation", true, PModel("parameter", "TestClass"))
			);

			var assembly = Generator(c => c
				.RenderedTypeAttributes.Add(type.of<CustomAttribute>())
				.RenderedMemberAttributes.Add(type.of<CustomAttribute>())
				.RenderedOperationAttributes.Add(type.of<CustomAttribute>())
				.RenderedParameterAttributes.Add(type.of<CustomAttribute>()))
				.AddReference<CustomAttribute>()
				.Generate(DefaultTestTemplate)
				;

			var testClass = GetRenderedType(assembly, "TestClass");

			var member = testClass.GetProperty("Member");
			var operation = testClass.GetMethod("Operation");
			var parameter = operation.GetParameters().Single(p => p.Name == "parameter");

			Assert.IsTrue(Attribute.IsDefined(testClass, typeof(CustomAttribute)));
			Assert.IsTrue(Attribute.IsDefined(member, typeof(CustomAttribute)));
			Assert.IsTrue(Attribute.IsDefined(operation, typeof(CustomAttribute)));
			Assert.IsTrue(Attribute.IsDefined(parameter, typeof(CustomAttribute)));

		}

		[Test]
		public void Interfaces_include_GetIdentifier_method_to_provide_id_value()
		{
			ModelsAre(
				Model("TestClass").Name("TestClass")
				.Operation("Dummy", true)
			);

			ObjectsAre(
				Object(Id("test_id", "TestClass"))
			);

			var testing = Generator();

			var assembly = testing.Generate(DefaultTestTemplate);

			var iTestClass = GetRenderedType(assembly, "TestClass");
			var getIdentifier = iTestClass.GetMethod("GetIdentifier");

			Assert.IsNotNull(getIdentifier);

			var testObj = CreateInstance(GetRenderedType(assembly, "TestClassImpl"), "test_id", "TestClass");
			var actual = getIdentifier.Invoke(testObj, new object[0]);

			Assert.AreEqual("test_id", actual.ToString());

			var actual2 = getIdentifier.Invoke(testObj, new object[0]);
			
			Assert.IsTrue(Equals(actual, actual2), "Two identifiers should be equal.");
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
				.Use(p => p.ReferencedTypeByShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern()));

			var assembly = testing.Generate(DefaultTestTemplate);

			var testObj = CreateInstance(GetRenderedType(assembly, "TestClassImpl"), "test_id", "TestClass");

			var actual = GetRenderedType(assembly, "TestClass").GetProperty("Name").GetValue(testObj, new object[0]) as string;
			Assert.AreEqual("test_name", actual);
		}

		[Test]
		public void Rendered_types_have_Invalidate_method_which_invalidates_member_values()
		{
			ModelsAre(
				Model("TestClass1").Name("TestClass1")
				.Member("Other", "TestClass1")
			);

			ObjectsAre(
				Object(Id("test2", "TestClass1"))
				.Value("test2_value")
			);

			ObjectsAre(
				Object(Id("test1", "TestClass1"))
				.Value("test1_value")
				.Member("Other", Id("test2", "TestClass1"))
			);

			var assembly = Generator().Generate(DefaultTestTemplate);

			var iTestClass1 = GetRenderedType(assembly, "TestClass1");

			var other = iTestClass1.GetProperty("Other");

			var testObj1 = CreateInstance(GetRenderedType(assembly, "TestClass1Impl"), "test1", "TestClass1");

			var otherValue = other.GetValue(testObj1, new object[0]);
			Assert.AreEqual("test2_value", otherValue.ToString());

			var invalidate = iTestClass1.GetMethod("Invalidate");

			Assert.IsNotNull(invalidate);

			invalidate.Invoke(testObj1, new object[0]);

			otherValue = other.GetValue(testObj1, new object[0]);
			Assert.AreEqual("test2_value", otherValue.ToString());

			objectServiceMock.Verify(o => o.Get(It.IsAny<ObjectReferenceData>()), Times.Exactly(2));
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
				.Use(p => p.ReferencedTypeByShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern()));

			var assembly = testing.Generate(DefaultTestTemplate);

			var testObj = CreateInstance(GetRenderedType(assembly, "TestClassImpl"), "test_id", "TestClass");

			var actual = GetRenderedType(assembly, "TestClass").GetMethod("Operation").Invoke(testObj, new object[] { "arg1_test", "arg2_test" }) as string;

			Assert.AreEqual("result_test", actual);
		}

		[Test]
		public void Rendered_type_instances_are_created_using_robject_constructors()
		{
			ModelsAre(
				Model("TestClass2").Name("TestClass2").Member("Self", "TestClass2"),
				Model("TestClass1").Name("TestClass1")
				.Member("Sub", "TestClass2")
				.Operation("Operation", "TestClass2"));

			ObjectsAre(
				Object(Id("test2", "TestClass2"))
				.Value("test2_value"),
				Object(Id("test1", "TestClass1"))
				.Value("test1_value")
				.Member("Sub", Id("test2", "TestClass2")));

			When(Id("test1", "TestClass1"))
				.Performs("Operation")
				.Returns(Result(Id("test2", "TestClass2")));

			var assembly = Generator().Generate(DefaultTestTemplate);
			var iTestClass1 = GetRenderedType(assembly, "TestClass1");

			var subProperty = iTestClass1.GetProperty("Sub");
			var operationMethod = iTestClass1.GetMethod("Operation");

			var testObj1 = CreateInstance(GetRenderedType(assembly, "TestClass1Impl"), "test1", "TestClass1");
			var testObj2 = CreateInstance(GetRenderedType(assembly, "TestClass2Impl"), "test2", "TestClass2");

			var subObj = subProperty.GetValue(testObj1, new object[0]);
			Assert.AreEqual(testObj2, subObj);

			var operationResult = operationMethod.Invoke(testObj1, new object[0]);
			Assert.AreEqual(testObj2, operationResult);
		}

		[Test]
		public void Parameters_of_rendered_types_are_casted_to_their_concrete_types()
		{
			ModelsAre(
				Model("TestClass2").Name("TestClass2").Member("Self", "TestClass2"),
				Model("TestClass1").Name("TestClass1")
				.Operation("Operation", true, PModel("arg1", "TestClass2")));

			ObjectsAre(
				Object(Id("test2", "TestClass2"))
				.Value("test2_value"),
				Object(Id("test1", "TestClass1"))
				.Value("test1_value")
			);

			When(Id("test1", "TestClass1"))
				.Performs("Operation", p =>
					p["arg1"].Values[0].ObjectModelId == "TestClass2" &&
					p["arg1"].Values[0].ReferenceId == "test2")
				.Returns(Void());

			var assembly = Generator().Generate(DefaultTestTemplate);
			var iTestClass1 = GetRenderedType(assembly, "TestClass1");

			var operationMethod = iTestClass1.GetMethod("Operation");

			var testObj1 = CreateInstance(GetRenderedType(assembly, "TestClass1Impl"), "test1", "TestClass1");
			var testObj2 = CreateInstance(GetRenderedType(assembly, "TestClass2Impl"), "test2", "TestClass2");

			var operationResult = operationMethod.Invoke(testObj1, new[] { testObj2 });
			Assert.IsNull(operationResult);
		}

		[Test]
		public void List_member__parameter_and_operation_result_support()
		{
			ModelsAre(
				Model("s-string").IsValue(),
				Model("TestClass2").Name("TestClass2").Member("Self", "TestClass2"),
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
				.Use(p => p.ReferencedTypeByShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern()));

			var assembly = testing.Generate(DefaultTestTemplate);
			var testClass1 = GetRenderedType(assembly, "TestClass1");

			var subsProperty = testClass1.GetProperty("Subs");
			var namesProperty = testClass1.GetProperty("Names");

			var subListOperation = testClass1.GetMethod("SubListOperation");
			var nameListOperation = testClass1.GetMethod("NameListOperation");

			var testObj1 = CreateInstance(GetRenderedType(assembly, "TestClass1Impl"), "test1", "TestClass1");
			var testObj2_1 = CreateInstance(GetRenderedType(assembly, "TestClass2Impl"), "test2.1", "TestClass2");
			var testObj2_2 = CreateInstance(GetRenderedType(assembly, "TestClass2Impl"), "test2.2", "TestClass2");
			var testObj2_3 = CreateInstance(GetRenderedType(assembly, "TestClass2Impl"), "test2.3", "TestClass2");
			var testObj2_4 = CreateInstance(GetRenderedType(assembly, "TestClass2Impl"), "test2.4", "TestClass2");

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
				.Member("Self", "TestClass1")
			);

			ObjectsAre(
				Object(Id("test1", "TestClass1"))
				.Value("test1_value")
				.Member("Name", Null("s-string"))
				.Member("Self", Null("TestClass1"))
			);

			var testing = Generator(c => c
				.Use(p => p.ReferencedTypeByShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern()));

			var assembly = testing.Generate(DefaultTestTemplate);
			var iTestClass1 = GetRenderedType(assembly, "TestClass1");

			var nameProperty = iTestClass1.GetProperty("Name");
			var selfProperty = iTestClass1.GetProperty("Self");

			var testObj1 = CreateInstance(GetRenderedType(assembly, "TestClass1Impl"), "test1", "TestClass1");

			var name = nameProperty.GetValue(testObj1, new object[0]);
			Assert.IsNull(name);

			var self = selfProperty.GetValue(testObj1, new object[0]);
			Assert.IsNull(self);
		}

		[Test]
		public void Null_parameter_and_operation_result_support()
		{
			ModelsAre(
				Model("s-string").IsValue(),
				Model("TestClass1").Name("TestClass1")
				.Operation("Operation", "s-string", PModel("arg", "s-string"))
				.Operation("Operation2", "TestClass1", PModel("arg", "TestClass1"))
			);

			ObjectsAre(
				Object(Id("test1", "TestClass1")).Value("test1_value"),
				Object(Id("fail", "TestClass1"))
			);

			When(Id("test1", "TestClass1"))
				.Performs("Operation")
				.Returns(Result(Id("fail", "s-string")));

			When(Id("test1", "TestClass1"))
				.Performs("Operation2")
				.Returns(Result(Id("fail", "TestClass1")));

			When(Id("test1", "TestClass1"))
				.Performs("Operation", p => 
					p["arg"].Values[0].IsNull
				).Returns(Result(Null("s-string")));

			When(Id("test1", "TestClass1"))
				.Performs("Operation2", p => 
					p["arg"].Values[0].IsNull
				).Returns(Result(Null("TestClass")));

			var testing = Generator(c => c
				.Use(p => p.ReferencedTypeByShortModelIdPattern("System", "s"))
				.Use(p => p.ParseableValueTypePattern()));

			var assembly = testing.Generate(DefaultTestTemplate);
			var iTestClass1 = GetRenderedType(assembly, "TestClass1");

			var operation = iTestClass1.GetMethod("Operation");
			var operation2 = iTestClass1.GetMethod("Operation2");

			var testObj1 = CreateInstance(GetRenderedType(assembly, "TestClass1Impl"), "test1", "TestClass1");

			var actual = operation.Invoke(testObj1, new object[]{null});
			Assert.IsNull(actual);

			actual = operation2.Invoke(testObj1, new object[]{null});
			Assert.IsNull(actual);
		}
	}
}