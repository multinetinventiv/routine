using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Routine.Api.Generator;
using Routine.Test.Common;
using Routine.Core.Service;
using Moq;
using Routine.Api;
using System.IO;

namespace Routine.Test.Api.Generator
{
	[TestFixture]
	public class ClientApiGeneratorTest : ApiTestBase
	{
		private ClientApiGenerator testing;

		public override void SetUp()
		{
			base.SetUp();

			testing = new ClientApiGenerator(objectServiceMock.Object);
			testing.InMemory = true;
			testing.DefaultNamespace = "Routine.Test.ApiGen.Client";
		}

		private Type BuildAndGetClientClass(string name)
		{
			return testing.Build().GetTypes().Single(t => t.Name == name);
		}

		private object BuildAndGetClientInstance(string id, string name)
		{
			return Activator.CreateInstance(BuildAndGetClientClass(name), Robj(id, name));
		}

		[Test]
		public void GeneratesAssemblyWithGivenDefaultNamespaceWhenInMemoryIsFalse()
		{
			testing.InMemory = false;
			testing.DefaultNamespace = "Default.Namespace";

			Assembly assembly = testing.Build();

			Assert.AreEqual("Default.Namespace", assembly.GetName().Name);
		}

		[Test]
		public void ClassIsDirectlyTheGivenModelName()
		{
			ModelsAre(Model("TestClassId").Name("TestClass"));

			var types = testing.Build().GetTypes();

			Assert.IsTrue(types.Any(t => t.Name == "TestClass"), "TestClass was not found");
		}

		[Test]
		public void MustHaveDefaultNamespace()
		{
			ModelsAre(Model("TestClassId").Name("TestClass"));
			testing.DefaultNamespace = "Default.Namespace";

			var testClass = BuildAndGetClientClass("TestClass");

			Assert.IsTrue(testClass.Namespace.StartsWith("Default.Namespace"), "Namespace should start with Default.Namespace, but it is -> " + testClass.Namespace);

			testing.DefaultNamespace = null;

			try
			{
				testing.Build();
				Assert.Fail("exception not thrown");
			}
			catch(InvalidOperationException) {}
		}

		[Test]
		public void AppendsModuleToDefaultNamespaceWhenModuleIsNotEmpty()
		{
			ModelsAre(
				Model("TestClassId1").Module("Module").Name("TestClass1"),
				Model("TestClassId2").Name("TestClass2"));

			var types = testing.Build().GetTypes();

			var testClass1 = types.Single(t => t.Name == "TestClass1");
			var testClass2 = types.Single(t => t.Name == "TestClass2");

			Assert.IsTrue(testClass1.Namespace.EndsWith("Module"), "Namespace should end with Module, but it is -> " + testClass1.Namespace);
			Assert.AreEqual(testing.DefaultNamespace, testClass2.Namespace);
		}

		[Test]
		public void DomainTypesAreRenderedUsingGeneratedClientTypes()
		{
			ModelsAre(
				Model("TestClass").Name("TestClass").Member("Sub", "TestClass2"),
				Model("TestClass2").Name("TestClass2"));

			var types = testing.Build().GetTypes();
			var testClass = types.Single(t => t.Name == "TestClass");
			var testClass2 = types.Single(t => t.Name == "TestClass2");

			var actual = testClass.GetProperties().Single(p => p.Name == "Sub").PropertyType;

			Assert.AreEqual(testClass2, actual);
		}
			
		[Test]
		public void ValueTypesAreNotRenderedButTheirAssembliesShouldBeReferenced()
		{
			ModelsAre(Model("Routine.Test.Common.Price").IsValue());

			testing.Using<Price>("Routine.Test.Common.Price");

			var actual = testing.Build().GetTypes().Length;

			Assert.AreEqual(0, actual);
		}

		[Test]
		public void TypesCanBeReferencedByConvention()
		{
			ModelsAre(
				Model("TestClass").Name("TestClass")
				.Member("Price", "c-price")
				.Member("Address", "c-fat-string")
			);

			testing.AddReference(typeof(FatString).Assembly);
			testing.UsingParseableValueTypes("Routine.Test.Common", "c");

			var testClass = BuildAndGetClientClass("TestClass");

			var actual = testClass.GetProperties().Single(p => p.Name == "Price").PropertyType;
			Assert.AreEqual(typeof(Price), actual);

			actual = testClass.GetProperties().Single(p => p.Name == "Address").PropertyType;
			Assert.AreEqual(typeof(FatString), actual);
		}

		[Test]
		public void WhenAnObjectModelIsNotFoundInApplicationModelAndTypeReferenceIsNotFoundAnExceptionIsThrown()
		{
			ModelsAre(Model("TestClass").Name("TestClass").Member("Name", "System.String"));

			try {
				testing.Build();
				Assert.Fail("exception not thrown");
			} catch (InvalidOperationException ex) {
				Assert.IsTrue(ex.Message.Contains("System.String"), "Exception message does not contain System.String");
			}
		}

		[Test]
		public void MembersAreRenderedAsReadOnlyProperties()
		{
			ModelsAre(Model("TestClass").Name("TestClass").Member("Name", "System.String"));
			
			testing.Using<string>("System.String");

			var properties = BuildAndGetClientClass("TestClass").GetProperties();

			Assert.IsTrue(properties.Any(p => p.Name == "Name"), "Name property was not found");

			var nameProperty = properties.Single(p => p.Name == "Name");

			Assert.AreEqual(typeof(string), nameProperty.PropertyType);
			Assert.IsTrue(nameProperty.CanRead);
			Assert.IsFalse(nameProperty.CanWrite);
		}

		[Test]
		public void Test_ListMember()
		{
			ModelsAre(Model("TestClass").Name("TestClass").Member("OrderIds", "System.String", false, true));

			testing.Using<string>("System.String");

			var properties = BuildAndGetClientClass("TestClass").GetProperties();

			var nameProperty = properties.Single(p => p.Name == "OrderIds");

			Assert.AreEqual(typeof(List<string>), nameProperty.PropertyType);
		}

		[Test]
		public void OperationsAreRenderedAsMethods()
		{
			ModelsAre(Model("TestClass").Name("TestClass").Operation("VoidMethod", false, true));

			var methods = BuildAndGetClientClass("TestClass").GetMethods();

			Assert.IsTrue(methods.Any(m => m.Name == "VoidMethod"), "VoidMethod method was not found");

			var voidMethod = methods.Single(m => m.Name == "VoidMethod");

			Assert.IsTrue(voidMethod.ReturnType == typeof(void));
			Assert.AreEqual(0, voidMethod.GetParameters().Length);
		}

		[Test]
		public void OperationResultsAreMethodReturnTypes()
		{
			ModelsAre(Model("TestClass").Name("TestClass").Operation("StringMethod", false, "System.String"));

			testing.Using<string>("System.String");

			var methods = BuildAndGetClientClass("TestClass").GetMethods();

			var stringMethod = methods.Single(m => m.Name == "StringMethod");

			Assert.IsTrue(stringMethod.ReturnType == typeof(string));
		}

		[Test]
		public void Test_ListResult()
		{
			ModelsAre(Model("TestClass").Name("TestClass").Operation("StringListMethod", false, "System.String", true));

			testing.Using<string>("System.String");

			var methods = BuildAndGetClientClass("TestClass").GetMethods();

			var stringMethod = methods.Single(m => m.Name == "StringListMethod");

			Assert.IsTrue(stringMethod.ReturnType == typeof(List<string>));
		}

		[Test]
		public void OperationsParametersAreMethodParameters()
		{
			ModelsAre(Model("TestClass").Name("TestClass")
				.Operation("ParameterMethod", false, true, PModel("arg1", "System.String"), PModel("arg2", "System.String", true)));

			testing.Using<string>("System.String");

			var methods = BuildAndGetClientClass("TestClass").GetMethods();

			var parameterMethod = methods.Single(m => m.Name == "ParameterMethod");

			Assert.AreEqual(2, parameterMethod.GetParameters().Length);
			Assert.AreEqual("arg1", parameterMethod.GetParameters()[0].Name);
			Assert.AreEqual(typeof(string), parameterMethod.GetParameters()[0].ParameterType);
			Assert.AreEqual("arg2", parameterMethod.GetParameters()[1].Name);
			Assert.AreEqual(typeof(List<string>), parameterMethod.GetParameters()[1].ParameterType);
		}

		[Test][Ignore]
		public void NakedObjectsHaveAConversionMethodForEachOfTheirViewModel()
		{
			Assert.Fail("to be designed");
		}

		[Test]
		public void CanHaveAModuleFilterToGenerateAssemblyForOnlySpecifiedModules()
		{
			ModelsAre(
				Model("Included1-TestClass1").Module("Included1").Name("TestClass1"),
				Model("Included2-TestClass2").Module("Included2").Name("TestClass2"),
				Model("Excluded1-TestClass3").Module("Excluded1").Name("TestClass3"));

			testing.Modules.Include("Included.*");
			var types = testing.Build().GetTypes();

			Assert.IsTrue(types.Any(t => t.Name == "TestClass1"), "TestClass1 was not found");
			Assert.IsTrue(types.Any(t => t.Name == "TestClass2"), "TestClass2 was not found");
			Assert.IsTrue(types.All(t => t.Name != "TestClass3"), "TestClass2 was found");
		}

		[Test]
		public void AutomaticallyExcludesMembersAndOperationsNeedingATypeInExcludedModules()
		{
			ModelsAre(
				Model("Included-TestClass1").Module("Included").Name("TestClass1")
				.Member("PropertyExcluded", "Excluded-TestClass2")
				.Member("PropertyIncluded", "System.String")
				.Operation("MethodExcludedBecauseOfReturnType", "Excluded-TestClass2")
				.Operation("MethodExcludedBecauseOfParameter", "System.String", PModel("excludeReason", "Excluded-TestClass2"))
				.Operation("MethodIncluded", "System.String"),
				Model("Excluded-TestClass2").Module("Excluded").Name("TestClass2"));

			testing.Using<string>("System.String");

			testing.Modules.Include("Included.*");
			var types = testing.Build().GetTypes();

			Assert.IsFalse(types.Any(t => t.Name == "TestClass2"), "TestClass2 was found");

			var testClass1 = types.Single(t => t.Name == "TestClass1");

			Assert.IsNull(testClass1.GetProperty("PropertyExcluded"), "PropertyExcluded was found");
			Assert.IsNull(testClass1.GetMethod("MethodExcludedBecauseOfReturnType"), "MethodExcludedBecauseOfReturnType was found");
			Assert.IsNull(testClass1.GetMethod("MethodExcludedBecauseOfParameter"), "MethodExcludedBecauseOfParameter was found");

			Assert.IsNotNull(testClass1.GetProperty("PropertyIncluded"), "PropertyIncluded was not found");
			Assert.IsNotNull(testClass1.GetMethod("MethodIncluded"), "MethodIncluded was not found");
		}

		[Test]
		public void NoClientClassIsGeneratedWhenExcludedModuleClassIsAlreadyReferenced()
		{
			ModelsAre(
				Model("Module1-TestClass1").Module("Module1").Name("TestClass1"),
				Model("Module2-TestClass2").Module("Module2").Name("TestClass2")
				.Member("DependentProperty", "Module1-TestClass1"));

			var otherApiGenerator = new ClientApiGenerator(objectServiceMock.Object);

			otherApiGenerator.DefaultNamespace = "Routine.Test.ApiGen.Client.Module1";
			otherApiGenerator.InMemory = false;
			otherApiGenerator.Modules.Include("Module1");
			var otherApi = otherApiGenerator.Build();

			var testClass1 = otherApi.GetTypes().Single(t => t.Name == "TestClass1");

			testing.DefaultNamespace = "Routine.Test.ApiGen.Client.Module2";
			testing.Modules.Include("Module2");
			testing.AddReference(otherApi);
			testing.Using(t => true, t => t.FullName.After("Routine.Test.ApiGen.Client.").Replace(".", "-"), true);

			var testClass2 = BuildAndGetClientClass("TestClass2");

			Assert.AreEqual(testClass1, testClass2.GetProperty("DependentProperty").PropertyType);
		}

		[Test]
		public void ModuleIsNotAppendedToDefaultNamespaceWhenDefaultNamespaceHasItAlready()
		{
			ModelsAre(
				Model("Module1-TestClass1").Module("Module1").Name("TestClass1"));

			testing.DefaultNamespace = "Routine.Test.ApiGen.Client.Module1";

			var testClass1 = BuildAndGetClientClass("TestClass1");

			Assert.AreEqual("Routine.Test.ApiGen.Client.Module1", testClass1.Namespace);
		}

		[Test]
		public void PropertyValuesAreFetchedViaClientApi()
		{
			ModelsAre(
				Model("System.String").IsValue(),
				Model("TestClass").Name("TestClass")
				.Member("Name", "System.String"));

			ObjectsAre(
				Object(Id("test_id", "TestClass"))
				.Value("test_value")
				.Member("Name", Id("test_name", "System.String")));

			testing.Using<string>("System.String");

			var testObj = BuildAndGetClientInstance("test_id", "TestClass");

			var actual = testObj.GetType().GetProperty("Name").GetValue(testObj, new object[0]) as string;
			Assert.AreEqual("test_name", actual);
		}

		[Test]
		public void MethodsAreCalledViaClientApi()
		{
			ModelsAre(
				Model("System.String").IsValue(),
				Model("TestClass").Name("TestClass")
				.Operation("Operation", "System.String", PModel("arg1", "System.String"), PModel("arg2", "System.String")));

			ObjectsAre(
				Object(Id("test_id", "TestClass"))
				.Value("test_value"));

			When(Id("test_id", "TestClass"))
				.Performs("Operation", p => 
					p["arg1"].References[0].Id == "arg1_test" && 
					p["arg2"].References[0].Id == "arg2_test")
				.Returns(Result(Id("result_test", "System.String")));

			testing.Using<string>("System.String");

			var testObj = BuildAndGetClientInstance("test_id", "TestClass");

			var actual = testObj.GetType().GetMethod("Operation").Invoke(testObj, new object[]{ "arg1_test", "arg2_test" }) as string;

			Assert.AreEqual("result_test", actual);
		}

		[Test]
		public void ValueTypesOtherThanStringAreConvertedUsingGivenConverterFunction()
		{
			var expectedGuid = Guid.NewGuid();
			const int expectedInt = 12;

			ModelsAre(
				Model("System.Int32").IsValue(),
				Model("System.Guid").IsValue(),
				Model("TestClass").Name("TestClass")
				.Member("Uid", "System.Guid")
				.Operation("Operation", "System.Int32", PModel("arg1", "System.Guid")));

			ObjectsAre(
				Object(Id("test_id", "TestClass"))
				.Value("test_value")
				.Member("Uid", Id(expectedGuid.ToString(), "System.Guid")));

			When(Id("test_id", "TestClass"))
				.Performs("Operation", p => 
					p["arg1"].References[0].Id == expectedGuid.ToString())
				.Returns(Result(Id(expectedInt.ToString(), "System.Int32")));

			testing.Using(
				t => t.CanParse(),
				t => t.FullName,
				"{value}.ToString()",
				"{type}.Parse({valueString})");

			var testObj = BuildAndGetClientInstance("test_id", "TestClass");

			var actualGuid = (Guid)testObj.GetType().GetProperty("Uid").GetValue(testObj, new object[0]);
			Assert.AreEqual(expectedGuid, actualGuid);
			
			var actualInt = (int)testObj.GetType().GetMethod("Operation").Invoke(testObj, new object[]{ expectedGuid });
			Assert.AreEqual(expectedInt, actualInt);
		}

		[Test]
		public void DomainTypeInstancesAreCreatedUsingClientClassConstructors()
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
					p["arg1"].References[0].ActualModelId == "TestClass2" &&
					p["arg1"].References[0].Id == "test2")
				.Returns(Result(Id("test2", "TestClass2")));

			var types = testing.Build().GetTypes();
			var testClass1 = types.Single(t => t.Name == "TestClass1");
			var testClass2 = types.Single(t => t.Name == "TestClass2");

			var subProperty = testClass1.GetProperty("Sub");
			var operationMethod = testClass1.GetMethod("Operation");

			var testObj1 = Activator.CreateInstance(testClass1, Robj("test1", "TestClass1"));
			var testObj2 = Activator.CreateInstance(testClass2, Robj("test2", "TestClass2"));

			var subObj = subProperty.GetValue(testObj1, new object[0]);
			Assert.AreEqual(testObj2, subObj);

			var operationResult = operationMethod.Invoke(testObj1, new []{ testObj2 });
			Assert.AreEqual(testObj2, operationResult);
		}

		[Test]
		public void ReferencedDomainTypeInstancesAreAlsoCreatedUsingClientClassConstructors()
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
					p["arg1"].References[0].ActualModelId == "Module2-TestClass2" &&
					p["arg1"].References[0].Id == "test2")
				.Returns(Result(Id("test2", "Module2-TestClass2")));

			var otherApiGenerator = new ClientApiGenerator(objectServiceMock.Object);

			otherApiGenerator.DefaultNamespace = "RoutineTest.ApiGen.Client.Module2";
			otherApiGenerator.InMemory = false;
			otherApiGenerator.Modules.Include("Module2");
			var otherApi = otherApiGenerator.Build();
			var testClass2 = otherApi.GetTypes().Single(t => t.Name == "TestClass2");

			testing.DefaultNamespace = "RoutineTest.ApiGen.Client.Module1";
			testing.Modules.Include("Module1");
			testing.AddReference(otherApi);
			testing.Using(
				t => true, 
				t => t.FullName.After("RoutineTest.ApiGen.Client.").Replace(".", "-"),
				true);

			var testingApi = testing.Build();
			var testClass1 = testingApi.GetTypes().Single(t => t.Name == "TestClass1");

			var subProperty = testClass1.GetProperty("Sub");
			var operationMethod = testClass1.GetMethod("Operation");

			var testObj1 = Activator.CreateInstance(testClass1, Robj("test1", "Module1-TestClass1"));
			var testObj2 = Activator.CreateInstance(testClass2, Robj("test2", "Module2-TestClass2"));

			var subObj = subProperty.GetValue(testObj1, new object[0]);
			Assert.AreEqual(testObj2, subObj);

			var operationResult = operationMethod.Invoke(testObj1, new []{ testObj2 });
			Assert.AreEqual(testObj2, operationResult);
		}

		[Test]
		public void Test_ListMemberParameterAndOperationResult()
		{
			ModelsAre(
				Model("s-string").IsValue(),
				Model("TestClass2").Name("TestClass2"),
				Model("TestClass1").Name("TestClass1")
				.Member("Subs", "TestClass2", false, true)
				.Member("Names", "s-string", false, true)
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
					p["arg1"].References[0].ActualModelId == "TestClass2" &&
					p["arg1"].References[0].Id == "test2.1" &&
					p["arg1"].References[1].ActualModelId == "TestClass2" &&
					p["arg1"].References[1].Id == "test2.2")
				.Returns(Result(Id("test2.3", "TestClass2"), Id("test2.4", "TestClass2")));

			When(Id("test1", "TestClass1"))
				.Performs("NameListOperation", p =>
					p["arg1"].References[0].ActualModelId == "s-string" &&
					p["arg1"].References[0].Id == "name1" &&
					p["arg1"].References[1].ActualModelId == "s-string" &&
					p["arg1"].References[1].Id == "name2")
				.Returns(Result(Id("name3", "s-string"), Id("name4", "s-string")));

			testing.UsingParseableValueTypes("System", "s");

			var types = testing.Build().GetTypes();
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

			var subListResult = subListOperation.Invoke(testObj1, new object[]{subObjs}) as IList;
			Assert.AreEqual(testObj2_3, subListResult[0]);
			Assert.AreEqual(testObj2_4, subListResult[1]);

			var names = namesProperty.GetValue(testObj1, new object[0]) as IList;
			Assert.AreEqual("name1", names[0]);
			Assert.AreEqual("name2", names[1]);

			var nameListResult = nameListOperation.Invoke(testObj1, new object[]{names}) as IList;
			Assert.AreEqual("name3", nameListResult[0]);
			Assert.AreEqual("name4", nameListResult[1]);
		}

		[Test]
		public void Test_NullMemberValue()
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

			testing.UsingParseableValueTypes("System", "s");

			var types = testing.Build().GetTypes();
			var testClass1 = types.Single(t => t.Name == "TestClass1");

			var nameProperty = testClass1.GetProperty("Name");

			var testObj1 = Activator.CreateInstance(testClass1, Robj("test1", "TestClass1"));

			var name = nameProperty.GetValue(testObj1, new object[0]);
			Assert.IsNull(name);
		}

		[Test]
		public void Test_NullParameterValue()
		{
			ModelsAre(
				Model("s-string").IsValue(),
				Model("TestClass1").Name("TestClass1")
				.Operation("Operation", "s-string", PModel("arg1", "s-string"))
			);

			ObjectsAre(Object(Id("test1", "TestClass1")).Value("test1_value"));

			When(Id("test1", "TestClass1"))
				.Performs("Operation", p => 
					p["arg1"].References[0].IsNull)
				.Returns(Result(Id("resultForNull", "s-string")));

			testing.UsingParseableValueTypes("System", "s");

			var types = testing.Build().GetTypes();
			var testClass1 = types.Single(t => t.Name == "TestClass1");

			var operation = testClass1.GetMethod("Operation");

			var testObj1 = Activator.CreateInstance(testClass1, Robj("test1", "TestClass1"));

			var actual = operation.Invoke(testObj1, new object[]{null});
			Assert.AreEqual("resultForNull", actual);
		}

		[Test]
		public void Test_NullOperationResult()
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

			testing.UsingParseableValueTypes("System", "s");

			var types = testing.Build().GetTypes();
			var testClass1 = types.Single(t => t.Name == "TestClass1");

			var operation = testClass1.GetMethod("Operation");

			var testObj1 = Activator.CreateInstance(testClass1, Robj("test1", "TestClass1"));

			var actual = operation.Invoke(testObj1, new object[0]);
			Assert.IsNull(actual);
		}

		[Test]
		public void Test_ToStringMethodReturnsValue()
		{
			ModelsAre(Model("TestClass1").Name("TestClass1"));

			ObjectsAre(Object(Id("test1", "TestClass1")).Value("test1_value"));

			var types = testing.Build().GetTypes();
			var testClass1 = types.Single(t => t.Name == "TestClass1");

			var testObj1 = Activator.CreateInstance(testClass1, Robj("test1", "TestClass1"));

			Assert.AreEqual("test1_value", testObj1.ToString());
		}

		[Test]
		public void Test_ToStringHasOverloadToIncludeIdForDebuggingPurposes()
		{
			ModelsAre(Model("TestClass1").Name("TestClass1"));

			ObjectsAre(Object(Id("test1", "TestClass1")).Value("test1_value"));

			var types = testing.Build().GetTypes();
			var testClass1 = types.Single(t => t.Name == "TestClass1");

			var testObj1 = Activator.CreateInstance(testClass1, Robj("test1", "TestClass1"));
			var toString = testClass1.GetMethod("ToString", new []{ typeof(bool) });

			var debugMode = toString.Invoke(testObj1, new object[]{ true }) as string;
			Assert.AreEqual("[Id: test1, Value: test1_value]", debugMode);

			var releaseMode = toString.Invoke(testObj1, new object[]{ false }) as string;
			Assert.AreEqual("test1_value", releaseMode);
		}

		[Test]
		public void ClientObjectsHaveInvalidateMethodThatInvalidatesMemberValues()
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

			testing.UsingParseableValueTypes("System", "s");

			var types = testing.Build().GetTypes();
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
		public void GeneratorCanCreateApiClassToAccessSingletonInstances()
		{
			ModelsAre(
				Model("TestClass1").Name("TestClass1"),
				Model("TestClass2").Name("TestClass2")
			);

			ObjectsAre(
				Object(Id("instance1", "TestClass1")).Value("instance1_value"),
				Object(Id("instance2", "TestClass1")).Value("instance2_value")
			);

			testing.ApiName = "TestApi";
			testing.AddSingleton(t => t.Id.EndsWith("Class1"), "instance1", "instance2");

			var testApi = testing.Build();

			var testClass1 = testApi.GetTypes().Single(t => t.Name == "TestClass1");
			var testClass2 = testApi.GetTypes().Single(t => t.Name == "TestClass2");
			var apiClass = testApi.GetTypes().Single(t => t.Name == "TestApi");

			Assert.AreEqual(testing.DefaultNamespace, apiClass.Namespace);
			Assert.IsNotNull(apiClass.GetConstructor(new []{ typeof(Rapplication) }));
			Assert.IsFalse(apiClass.GetMethods().Any(m => m.ReturnType == testClass2));

			var apiObj = Activator.CreateInstance(apiClass, testingRapplication);

			var instance1 = apiClass.GetMethod("GetTestClass1Instance1", new Type[0]).Invoke(apiObj, new object[0]);
			var instance2 = apiClass.GetMethod("GetTestClass1Instance2", new Type[0]).Invoke(apiObj, new object[0]);

			Assert.AreEqual("instance1_value", instance1.ToString());
			Assert.AreEqual("instance2_value", instance2.ToString());
		}

		[Test]
		public void WhenApiIsGeneratedRobjectPropertyAndConstructorRenderedAsInternal()
		{
			ModelsAre(
				Model("TestClass1").Name("TestClass1")
			);

			ObjectsAre(
				Object(Id("instance1", "TestClass1")).Value("instance1_value")
			);

			testing.ApiName = "TestApi";
			testing.AddSingleton(t => t.Id.EndsWith("Class1"), "instance");

			var testApi = testing.Build();

			var testClass1 = testApi.GetTypes().Single(t => t.Name == "TestClass1");

			Assert.AreEqual(0, testClass1.GetConstructors().Length);
			Assert.IsNull(testClass1.GetProperty("Robject"));
		}

		[Test]
		public void ClientAssembliesCanBeMarkedAsFriendToEnabledInterClientAssemblyDependency()
		{
			ModelsAre(
				Model("Module2-TestClass2").Module("Module2").Name("TestClass2"),
				Model("Module1-TestClass1").Module("Module1").Name("TestClass1")
				.Member("Sub", "Module2-TestClass2")
			);

			var otherApiGenerator = new ClientApiGenerator(objectServiceMock.Object);

			otherApiGenerator.DefaultNamespace = "Routine.Test.ApiGen.Client.Module2";
			otherApiGenerator.InMemory = false;
			otherApiGenerator.ApiName = "Module2Api";
			otherApiGenerator.AddFriendAssembly("Routine.Test.ApiGen.Client.Module1");
			otherApiGenerator.Modules.Include("Module2");
			var otherApi = otherApiGenerator.Build();

			testing.DefaultNamespace = "Routine.Test.ApiGen.Client.Module1";
			testing.Modules.Include("Module1");
			testing.InMemory = false;
			testing.ApiName = "Module1Api";
			testing.AddReference(otherApi);
			testing.Using(
				t => true,
				t => t.FullName.After("Routine.Test.ApiGen.Client.").Replace(".", "-"),
				true);

			var api = testing.Build();

			Assert.IsTrue(File.Exists(otherApi.Location));
			Assert.IsTrue(File.Exists(api.Location));
		}

		[Test][Ignore]
		public void Enum()
		{
			Assert.Fail("to be designed");
		}

		[Test][Ignore]
		public void SingletonOnViewModels()
		{
			Assert.Fail("to be designed");
		}
	}
}

