using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Routine.Api.Template;

namespace Routine.Test.Api.Template
{
	[TestFixture]
	public class ClientApiReferencedClientApiTest : ClientApiFeatureTestBase
	{
		protected override string DefaultNamespace { get { return "Routine.Test.Api.Template.ClientApiReferencedClientApiFeatureTest_Generated"; } }
		
		[Test]
		public void When_a_rendered_type_from_another_assembly_is_referenced__then_interfaces_used_in_interfaces__internal_classes_are_used_via_friendly_assembly_feature()
		{
			ModelsAre(
				Model("Module2-TestClass2").Module("Module2").Name("TestClass2").Member("Self", "Module2-TestClass2"),
				Model("Module1-TestClass1").Module("Module1").Name("TestClass1")
				.Member("Sub", "Module2-TestClass2")
				.Operation("Operation", "Module2-TestClass2", PModel("arg1", "Module2-TestClass2"))
			);

			ObjectsAre(
				Object(Id("test2", "Module2-TestClass2")).Value("test2_value")
			);

			ObjectsAre(
				Object(Id("test1", "Module1-TestClass1"))
				.Value("test1_value")
				.Member("Sub", Id("test2", "Module2-TestClass2"))
			);

			When(Id("test1", "Module1-TestClass1"))
				.Performs("Operation", p =>
					p["arg1"].Values[0].ObjectModelId == "Module2-TestClass2" &&
					p["arg1"].Values[0].ReferenceId == "test2"
				).Returns(Result(Id("test2", "Module2-TestClass2"))
			);

			var assemblyName = Guid.NewGuid().ToString("N");
			var otherAssemblyName = Guid.NewGuid().ToString("N");

			var otherApiGenerator = Generator(c => c
				.FriendlyAssemblyNames.Add(assemblyName)
				.DefaultNamespace.Set(DefaultNamespace + ".Module2")
				.InMemory.Set(false)
				.OutputFileName.Set(otherAssemblyName)
				.TypeIsRendered.Set(true, m => m.Module == "Module2")
				.TypeIsRendered.Set(false)
			);

			var otherAssembly = otherApiGenerator.Generate(DefaultTestTemplate);

			var testing = Generator(c => c
				.DefaultNamespace.Set(DefaultNamespace + ".Module1")
				.TypeIsRendered.Set(true, m => m.Module == "Module1")
				.TypeIsRendered.Set(false)
				.InMemory.Set(false)
				.OutputFileName.Set(assemblyName)
				.Use(p => p.ReferenceOtherClientApiPattern(otherAssembly, otherApiGenerator.Context))
			).AddReference(otherAssembly);

			var assembly = testing.Generate(DefaultTestTemplate);

			var iTestClass1 = GetRenderedType(assembly, "ITestClass1");

			var sub = iTestClass1.GetProperty("Sub");
			var operation = iTestClass1.GetMethod("Operation");

			var testObj1 = CreateInstance(GetRenderedType(assembly, "TestClass1"), "test1", "Module1-TestClass1");
			var testObj2 = CreateInstance(GetRenderedType(otherAssembly, "TestClass2"), "test2", "Module2-TestClass2");

			var subObj = sub.GetValue(testObj1, new object[0]);
			Assert.AreEqual(testObj2, subObj);

			var operationResult = operation.Invoke(testObj1, new[] { testObj2 });
			Assert.AreEqual(testObj2, operationResult);
		}

		protected override void Referenced_client_api_support_case()
		{
			Assert.Pass("This is the referenced client api test fixture");
		}

		protected override void List_input_and_output_case()
		{
			ModelsAre(
				Model("Module2-TestClass2").Module("Module2").Name("TestClass2").Member("Self", "Module2-TestClass2"),
				Model("Module1-TestClass1").Module("Module1").Name("TestClass1")
				.Member("SubList", "Module2-TestClass2", true)
				.Operation("ListOperation", "Module2-TestClass2", true, PModel("arg1", "Module2-TestClass2", true))
			);

			ObjectsAre(
				Object(Id("test2_1", "Module2-TestClass2")).Value("test2_1_value"),
				Object(Id("test2_2", "Module2-TestClass2")).Value("test2_2_value"),
				Object(Id("test2_3", "Module2-TestClass2")).Value("test2_3_value"),
				Object(Id("test2_4", "Module2-TestClass2")).Value("test2_4_value"),
				Object(Id("test2_5", "Module2-TestClass2")).Value("test2_5_value"),
				Object(Id("test2_6", "Module2-TestClass2")).Value("test2_6_value")
			);
			ObjectsAre(
				Object(Id("test1", "Module1-TestClass1"))
				.Value("test1_value")
				.Member("SubList", Id("test2_1", "Module2-TestClass2"), Id("test2_2", "Module2-TestClass2"))
			);

			When(Id("test1", "Module1-TestClass1"))
				.Performs("ListOperation", p =>
					p["arg1"].Values[0].ObjectModelId == "Module2-TestClass2" &&
					p["arg1"].Values[0].ReferenceId == "test2_3" &&
					p["arg1"].Values[1].ObjectModelId == "Module2-TestClass2" &&
					p["arg1"].Values[1].ReferenceId == "test2_4"
				).Returns(Result(Id("test2_5", "Module2-TestClass2"), Id("test2_6", "Module2-TestClass2"))
			);

			var assemblyName = Guid.NewGuid().ToString("N");
			var otherAssemblyName = Guid.NewGuid().ToString("N");

			var otherApiGenerator = Generator(c => c
				.FriendlyAssemblyNames.Add(assemblyName)
				.DefaultNamespace.Set(DefaultNamespace + ".Module2")
				.InMemory.Set(false)
				.OutputFileName.Set(otherAssemblyName)
				.TypeIsRendered.Set(true, m => m.Module == "Module2")
				.TypeIsRendered.Set(false)
			);

			var otherAssembly = otherApiGenerator.Generate(DefaultTestTemplate);

			var testing = Generator(c => c
				.DefaultNamespace.Set(DefaultNamespace + ".Module1")
				.TypeIsRendered.Set(true, m => m.Module == "Module1")
				.TypeIsRendered.Set(false)
				.InMemory.Set(false)
				.OutputFileName.Set(assemblyName)
				.Use(p => p.ReferenceOtherClientApiPattern(otherAssembly, otherApiGenerator.Context))
			).AddReference(otherAssembly);

			var assembly = testing.Generate(DefaultTestTemplate);

			var iTestClass1 = GetRenderedType(assembly, "ITestClass1");
			var iTestClass2 = GetRenderedType(otherAssembly, "ITestClass2");

			var subList = iTestClass1.GetProperty("SubList");
			var listOperation = iTestClass1.GetMethod("ListOperation");

			var testObj1 = CreateInstance(GetRenderedType(assembly, "TestClass1"), "test1", "Module1-TestClass1");
			var testObj2_1 = CreateInstance(GetRenderedType(otherAssembly, "TestClass2"), "test2_1", "Module2-TestClass2");
			var testObj2_2 = CreateInstance(GetRenderedType(otherAssembly, "TestClass2"), "test2_2", "Module2-TestClass2");
			var testObj2_3 = CreateInstance(GetRenderedType(otherAssembly, "TestClass2"), "test2_3", "Module2-TestClass2");
			var testObj2_4 = CreateInstance(GetRenderedType(otherAssembly, "TestClass2"), "test2_4", "Module2-TestClass2");
			var testObj2_5 = CreateInstance(GetRenderedType(otherAssembly, "TestClass2"), "test2_5", "Module2-TestClass2");
			var testObj2_6 = CreateInstance(GetRenderedType(otherAssembly, "TestClass2"), "test2_6", "Module2-TestClass2");
			
			var subListObj = (IList)subList.GetValue(testObj1, new object[0]);
			Assert.AreEqual(testObj2_1, subListObj[0]);
			Assert.AreEqual(testObj2_2, subListObj[1]);

			var listOperationArg1 = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(iTestClass2));
			listOperationArg1.Add(testObj2_3);
			listOperationArg1.Add(testObj2_4);

			var listOperationResult = (IList)listOperation.Invoke(testObj1, new object[] { listOperationArg1 });
			Assert.AreEqual(testObj2_5, listOperationResult[0]);
			Assert.AreEqual(testObj2_6, listOperationResult[1]);
		}
	}
}