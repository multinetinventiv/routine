using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Routine.Api;
using Routine.Core;

namespace Routine.Test.Api
{
	[TestFixture]
	public class ApiTest : ApiTestBase
	{
		[Test]
		public void ClientGetsAllTypesInApplicationViaRapplication()
		{
			ModelsAre(Model("model1"), Model("model2"));

			var actual = testingRapplication.ObjectModels;

			Assert.AreEqual("model1", actual[0].Id);
			Assert.AreEqual("model2", actual[1].Id);
		}

		[Test]
		public void ClientCanAccessABusinessObjectByCreatingARobjectUsingModelAndIdInformation()
		{
			ModelsAre(Model("actualModel"), Model("viewModel"));
			ObjectsAre(Object(Id("id", "actualModel", "viewModel")).Value("value"));

			var testingRobject = Robj("id", "actualModel", "viewModel");

			Assert.AreEqual("value", testingRobject.Value);
			objectServiceMock.Verify(o => o.GetObjectModel(It.IsAny<string>()), Times.Never());
		}

		[Test]
		public void WhenCreatingRobjectGetObjectModelIsNotCalledCachedApplicationModelIsUsed()
		{
			ModelsAre(Model("actualModel"));
			ObjectsAre(Object(Id("id", "actualModel")));

			Robj("id", "actualModel");
			Robj("id", "actualModel");
			Robj("id", "actualModel");

			objectServiceMock.Verify(o => o.GetObjectModel(It.IsAny<string>()), Times.Never());
			objectServiceMock.Verify(o => o.GetApplicationModel(), Times.Once());
		}

		[Test]
		public void ClientGetsObjectIdAndValueAndModelInformationViaRobject()
		{
			ModelsAre(Model("actualModel").Module("actualModule"), Model("viewModel").Module("viewModule"));
			ObjectsAre(Object(Id("id", "actualModel", "viewModel")).Value("value"));
			
			var testingRobject = Robj("id", "actualModel", "viewModel");
			
			Assert.AreEqual("id", testingRobject.Id);
			Assert.AreEqual("actualModel", testingRobject.ActualModelId);
			Assert.AreEqual("value", testingRobject.Value);
			Assert.AreEqual("viewModel", testingRobject.ViewModelId);
			Assert.AreEqual("viewModule", testingRobject.Module);
		}
		
		[Test]
		public void RobjectsFetchMemberDataOnlyWhenAsked()
		{
			ModelsAre(
				Model("model")
				.Member("member1")
				.Member("member2")
				.Operation("operation1", PModel("param1"))
				.Operation("operation2", PModel("param2")));

			ObjectsAre(
				Object(Id("id1")),
				Object(Id("id2")));
			ObjectsAre(
				Object(Id("id", "model"))
				.Member("member1", Id("id1"))
				.Member("member2", Id("id2")));

			var testingRobject = Robj("id", "model");
			var members = testingRobject.Members;
			members = testingRobject.Members;

			Assert.AreEqual("member1", members[0].Id);
			Assert.AreEqual("id1", members[0].GetValue().Object.Id);

			Assert.AreEqual("member2", members[1].Id);
			Assert.AreEqual("id2", members[1].GetValue().Object.Id);

			objectServiceMock.Verify(o => o.Get(It.IsAny<ObjectReferenceData>()), Times.Once());
		}

		[Test]
		public void RobjectsFetchDisplayValueAlongWithMemberAndOperationData()
		{
			ModelsAre(Model("model").Member("member1"));
			ObjectsAre(
				Object(Id("id1")).Value("value1"),
				Object(Id("id", "model"))
				.Value("value")
				.Member("member1", Id("id1")));

			var testingRobject = Robj("id", "model");
			testingRobject.Members[0].GetValue();

			Assert.AreEqual("value", testingRobject.Value);
			objectServiceMock.Verify(o => o.GetValue(It.IsAny<ObjectReferenceData>()), Times.Never());
		}

		[Test]
		public void RmembersFetchDataOnlyWhenAskedIfTheyAreHeavy()
		{
			ModelsAre(
				Model("model")
				.Member("lightMember")
				.Member("heavyMember", true));

			ObjectsAre(
				Object(Id("id1")), 
				Object(Id("id2")), 
				Object(Id("id", "model"))
				.Member("lightMember", Id("id1"))
				.Member("heavyMember", Id("id2")));

			var testingRobject = Robj("id", "model");

			var members = testingRobject.Members;

			Assert.AreEqual("lightMember", members[0].Id);
			Assert.AreEqual("heavyMember", members[1].Id);

			var member1 = members[0].GetValue();
			Assert.AreEqual("id1", member1.Object.Id);

			var member2 = members[1].GetValue();
			Assert.AreEqual("id2", member2.Object.Id);

			objectServiceMock.Verify(o => o.GetMember(It.IsAny<ObjectReferenceData>(), "lightMember"), Times.Never());
			objectServiceMock.Verify(o => o.GetMember(It.IsAny<ObjectReferenceData>(), "heavyMember"), Times.Once());
		}

		[Test]
		public void RvariableBehaveOptimisticOnSingleListVoidAndNullMode()
		{
			ModelsAre(
				Model("model")
				.Operation("operation", false, true));

			ObjectsAre(
				Object(Id("id1")), 
				Object(Id("id2")),
				Object(Id("id3", "model")));

			var obj1 = Robj("id1");
			var obj2 = Robj("id2");
			var obj3 = Robj("id3", "model");

			//accessing single as list returns a list with that single object
			var testingRvariable = Rvar("testingSingle", obj1);
			Assert.AreEqual(1, testingRvariable.List.Count);
			Assert.AreEqual("id1", testingRvariable.List[0].Id);

			//accessing list as single returns the first item
			testingRvariable = Rvarlist("testingList", obj1, obj2);
			Assert.AreEqual("id1", testingRvariable.Object.Id);

			//accessing list as single returns null if there is no items in the list
			testingRvariable = Rvarlist("testingEmptyList");
			Assert.IsTrue(testingRvariable.Object.IsNull);

			//accessing void as single returns null object
			When(Id("id3", "model")).Performs("operation1").Returns(Void());
			testingRvariable = obj3.Perform("operation");
			Assert.IsTrue(testingRvariable.IsNull);
			Assert.IsTrue(testingRvariable.Object.IsNull);

			//accessing void as list returns empty list
			Assert.AreEqual(0, testingRvariable.List.Count);
		}

		[Test]
		public void RvariableCanBeNull()
		{
			ModelsAre(Model("model").Member("member"));

			ObjectsAre(Object(Id("id", "model")).Member("member", Null()));

			var robj = Robj("id", "model");
			var testingRvariable = robj.Members[0].GetValue();

			Assert.IsTrue(testingRvariable.IsNull);
			Assert.IsTrue(testingRvariable.ToList().IsNull);
		}

		[Test]
		public void RobjectCanBeNull()
		{
			ModelsAre(Model("model").Member("member"));

			ObjectsAre(Object(Id("id", "model")).Member("member", Null()));

			var robj = Robj("id", "model");
			var testingRvariable = robj.Members[0].GetValue();

			Assert.IsTrue(testingRvariable.Object.IsNull);
		}

		[Test]
		public void RobjectBehaveOptimisticWhenItIsNull()
		{
			ModelsAre(Model("model").Member("member"));

			ObjectsAre(Object(Id("id", "model")).Member("member", Null()));

			var robj = Robj("id", "model");
			var testingRobject = robj.Members[0].GetValue().Object;

			Assert.AreEqual(0, testingRobject.Members.Count);
			Assert.AreEqual(0, testingRobject.Operations.Count);
			Assert.IsTrue(testingRobject.Perform("some non existing operation").IsNull);
			Assert.IsFalse(testingRobject.MarkedAs("dummy"));
		}

		[Test]
		public void ClientPerformsOperationViaRobject()
		{
			ModelsAre(
				Model("view_model")
				.Operation("operation", PModel("param1"), PModel("param2")),
				Model("actual_model"));

			ObjectsAre(
				Object(Id("id", "actual_model", "view_model")),
				Object(Id("id_param1")),
				Object(Id("id_param2")),
				Object(Id("id_result")));

			When(Id("id", "actual_model", "view_model"))
				.Performs("operation", p => 				
					p["param1"].References[0].Id == "id_param1" &&
					p["param2"].References[0].Id == "id_param2")
				.Returns(Result(Id("id_result")));

			var testingRobject = Robj("id", "actual_model", "view_model");

			var result = testingRobject.Perform("operation", 
				Rvar("param1", Robj("id_param1")), 
				Rvar("param2", Robj("id_param2")));

			Assert.AreEqual("id_result", result.Object.Id);
		}

		[Test]
		public void RparameterUsesItsOwnViewModelIdNoMatterWhatGivenRobjectsViewModelIdIs()
		{	
			ModelsAre(
				Model("model")
				.Operation("operation", PModel("param", "param_model")),
				Model("param_given_model"),
				Model("param_model"));

			ObjectsAre(
				Object(Id("id", "model")),
				Object(Id("id_param", "param_given_model")),
				Object(Id("id_result")));

			When(Id("id", "model"))
				.Performs("operation", p => 				
					p["param"].References[0].ActualModelId == "param_given_model" &&
					p["param"].References[0].ViewModelId == "param_model")
				.Returns(Result(Id("id_result")));

			var testingRobject = Robj("id", "model");

			var result = testingRobject.Perform("operation", 
				Rvar("param", Robj("id_param", "param_given_model")));

			Assert.AreEqual("id_result", result.Object.Id);
		}

		[Test]
		public void RoperationCanReturnVoidResult()
		{
			ModelsAre(
				Model("model")
				.Operation("operation1", false, true));

			ObjectsAre(Object(Id("id", "model")));

			When(Id("id", "model")).Performs("operation1").Returns(Void());

			var result = Robj("id", "model").Perform("operation1");

			Assert.IsTrue(result.IsVoid);
		}

		[Test]
		public void RobjectIsNakedWhenItDoesNotHaveAViewModelId()
		{
			ModelsAre(
				Model("actual_model"), Model("view_model"));

			ObjectsAre(
				Object(Id("id")), 
				Object(Id("id", "actual_model", "view_model")));

			Assert.IsTrue(Robj("id").IsNaked);
			Assert.IsFalse(Robj("id", "actual_model", "view_model").IsNaked);
		}

		[Test]
		public void RobjectIsDomainWhenItsModelIsNotValueType()
		{
			ModelsAre(
				Model("value_model").IsValue(),
				Model("domain_model1"),
				Model("domain_model2"));

			ObjectsAre(
				Object(Id("id", "value_model")),
				Object(Id("id1", "domain_model1")),
				Object(Id("id2", "domain_model2")));

			Assert.IsFalse(Robj("id", "value_model").IsDomain);
			Assert.IsTrue(Robj("id1", "domain_model1").IsDomain);
			Assert.IsTrue(Robj("id2", "domain_model2").IsDomain);
		}

		[Test]
		public void RparameterCanCreateValueVariableUsingGivenRobjects()
		{
			ModelsAre(
				Model("model")
				.Operation("operation", PModel("param", "param_model", true)),
				Model("param_model"));

			ObjectsAre(
				Object(Id("id_root", "model")),
				Object(Id("id_param_value")));

			var root = Robj("id_root", "model");
			var rparam = root.Operations[0].Parameters[0];

			var paramvar = rparam.CreateVariable(Robj("id_param_value"));

			Assert.AreEqual("param", paramvar.Name);
			Assert.IsTrue(paramvar.IsList);
		}

		[Test]
		public void RobjectDoesNotFetchValueIfModelIsValue()
		{
			ModelsAre(Model("model").IsValue());

			ObjectsAre(Object(Id("value", "model")));

			var robj = Robj("value", "model");

			var actual = robj.Value;

			Assert.AreEqual("value", actual);
			objectServiceMock.Verify(o => o.GetValue(It.IsAny<ObjectReferenceData>()), Times.Never());
		}

		[Test]
		public void WhenNoModelIsFoundInCachedApplicationModelGivenModelIdIsAutomaticallyTreatedAsValueModel()
		{
			objectServiceMock.Setup(o => o.GetApplicationModel()).Returns(new ApplicationModel());

			ObjectsAre(Object(Id("value", "model")));

			var robj = Robj("value", "model");

			var actual = robj.Value;

			Assert.AreEqual("value", actual);
		}

		[Test]
		public void ClientFetchesAvailableInstancesViaRapplication()
		{
			ModelsAre(Model("model").AvailableIds("id1", "id2"));

			ObjectsAre(
				Object(Id("id1", "model")).Value("value 1"), 
				Object(Id("id2", "model")).Value("value 2"));

			var actual = testingRapplication.GetAvailableObjects("model");

			Assert.AreEqual(2, actual.Count);
			Assert.AreEqual("id1", actual[0].Id);
			Assert.AreEqual("value 1", actual[0].Value);
			Assert.AreEqual("id2", actual[1].Id);
			Assert.AreEqual("value 2", actual[1].Value);
		}

		[Test]
		public void RobjectsImplementsEqualityMembers()
		{
			ObjectsAre(Object(Id("value", "model")));

			var left = Robj("value", "model");
			var right = Robj("value", "model");

			Assert.AreEqual(left, right);
			Assert.AreNotSame(left, right);

			Assert.AreEqual(left.GetHashCode(), right.GetHashCode());
		}

		[Test]
		public void WhenInvalidatedRobjectClearsFetchedData()
		{
			ModelsAre(
				Model("model")
				.Member("member1")
				.Member("member2"));

			ObjectsAre(
				Object(Id("id1")),
				Object(Id("id2")));
			ObjectsAre(
				Object(Id("id", "model")).Value("value")
				.Member("member1", Id("id1"))
				.Member("member2", Id("id2")));

			var testingRobject = Robj("id", "model");
			var value1 = testingRobject.Value;
			testingRobject["member1"].GetValue();
			testingRobject["member2"].GetValue();

			Assert.AreEqual("value", value1);

			testingRobject.Invalidate();

			var value2 = testingRobject.Value;
			testingRobject["member1"].GetValue();
			testingRobject["member2"].GetValue();

			Assert.AreEqual("value", value2);
			objectServiceMock.Verify(o => o.GetValue(It.IsAny<ObjectReferenceData>()), Times.Exactly(2));
			objectServiceMock.Verify(o => o.Get(It.IsAny<ObjectReferenceData>()), Times.Exactly(2));
		}

		[Test]
		public void RobjectCanCheckIfItsModelIsMarkedAsGivenMark()
		{
			ModelsAre(
				Model("model").Mark("mark"));

			ObjectsAre(
				Object(Id("id1", "model")));

			var testingRobject = Robj("id", "model");

			Assert.IsTrue(testingRobject.MarkedAs("mark"));
			Assert.IsFalse(testingRobject.MarkedAs("nonexistingmark"));
		}

		[Test]
		public void RmemberCanCheckIfItsModelIsMarkedAsGivenMark()
		{
			ModelsAre(
				Model("model")
				.Member("member")
				.MarkMember("member", "mark"));

			ObjectsAre(
				Object(Id("id")));

			ObjectsAre(
				Object(Id("id1", "model"))
				.Member("member", Id("id")));

			var testingRmember = Robj("id", "model")["member"];

			Assert.IsTrue(testingRmember.MarkedAs("mark"));
			Assert.IsFalse(testingRmember.MarkedAs("nonexistingmark"));
		}

		[Test]
		public void RoperationCanCheckIfItsModelIsMarkedAsGivenMark()
		{
			ModelsAre(
				Model("model")
				.Operation("operation")
				.MarkOperation("operation", "mark"));

			ObjectsAre(
				Object(Id("id1", "model")));

			var testingRoperation = Robj("id", "model").Operations.Single(o => o.Id == "operation");

			Assert.IsTrue(testingRoperation.MarkedAs("mark"));
			Assert.IsFalse(testingRoperation.MarkedAs("nonexistingmark"));
		}

		[Test]
		public void Facade_Rvariable_As()
		{
			ModelsAre(Model(":System.Int32").IsValue());

			int result = Rvar("value", Robj("10", ":System.Int32")).As(robj => int.Parse(robj.Value));

			Assert.AreEqual(10, result);
		}

		[Test]
		public void Facade_Rvariable_As_ReturnsDefaultWhenValueIsNull()
		{
			int intResult = Rvar("value", RobjNull()).As(robj => int.Parse(robj.Value));

			Assert.AreEqual(0, intResult);

			string stringResult = Rvar("value", RobjNull()).As(robj => robj.Value);

			Assert.IsNull(stringResult);
		}

		[Test]
		public void Facade_Rvariable_AsList()
		{
			ModelsAre(Model(":System.Int32").IsValue());

			List<int> result = Rvarlist("value", Robj("10", ":System.Int32"), Robj("11", ":System.Int32")).AsList(robj => int.Parse(robj.Value));

			Assert.AreEqual(10, result[0]);
			Assert.AreEqual(11, result[1]);
		}

		[Test]
		public void Facade_Rvariable_AsList_PutsDefaultValueWhenAnItemIsNull()
		{
			List<int> intResult = Rvarlist("value", RobjNull()).AsList(robj => int.Parse(robj.Value));

			Assert.AreEqual(0, intResult[0]);

			List<string> stringResult = Rvarlist("value", RobjNull()).AsList(robj => robj.Value);

			Assert.IsNull(stringResult[0]);
		}

		[Test]
		public void Facade_Rapplication_NewVar()
		{
			ModelsAre(Model(":System.Int32").IsValue());

			var actual = testingRapplication.NewVar("name", Robj("10", ":System.Int32"));

			Assert.AreEqual("10", actual.Object.Id);
			Assert.AreEqual("name", actual.Name);

			actual = testingRapplication.NewVar("name", 10, ":System.Int32");

			Assert.AreEqual("10", actual.Object.Id);
			Assert.AreEqual("name", actual.Name);

			actual = testingRapplication.NewVar("name", 10, o => o.ToString(), ":System.Int32");

			Assert.AreEqual("10", actual.Object.Id);
			Assert.AreEqual("name", actual.Name);
		}

		[Test]
		public void Facade_Rapplication_NewVar_CreatesNullVariableWhenObjectIsNull()
		{
			ModelsAre(
				Model(":System.Int32").IsValue(), 
				Model(":System.String").IsValue());

			var actual = testingRapplication.NewVar("name", RobjNull());
			Assert.IsTrue(actual.IsNull);

			string o = null;
			actual = testingRapplication.NewVar("name", o, ":System.String");
			Assert.IsTrue(actual.IsNull);

			actual = testingRapplication.NewVar("name", 0, ":System.Int32");
			Assert.IsTrue(actual.IsNull);
		}

		[Test]
		public void Facade_Rapplication_NewVarList()
		{
			ModelsAre(Model(":System.Int32").IsValue());

			var actual = testingRapplication.NewVarList("name", new List<Robject>{Robj("10", ":System.Int32"), Robj("11", ":System.Int32")});

			Assert.AreEqual("name", actual.Name);
			Assert.AreEqual("10", actual.List[0].Id);
			Assert.AreEqual("11", actual.List[1].Id);

			actual = testingRapplication.NewVarList("name", new List<int>{10, 11}, ":System.Int32");

			Assert.AreEqual("name", actual.Name);
			Assert.AreEqual("10", actual.List[0].Id);
			Assert.AreEqual("11", actual.List[1].Id);

			actual = testingRapplication.NewVarList("name", new List<int>{10, 11}, o => o.ToString(), ":System.Int32");

			Assert.AreEqual("name", actual.Name);
			Assert.AreEqual("10", actual.List[0].Id);
			Assert.AreEqual("11", actual.List[1].Id);
		}

		[Test]
		public void Facade_Rapplication_NewVarList_CreatesNullVariableWhenListIsNull()
		{
			ModelsAre(
				Model(":System.Int32").IsValue(), 
				Model(":System.String").IsValue());

			var actual = testingRapplication.NewVarList("name", new List<Robject>{RobjNull()});
			Assert.IsTrue(actual.List[0].IsNull);

			string nullString = null;
			actual = testingRapplication.NewVarList("name", new List<string>{nullString}, ":System.String");
			Assert.IsTrue(actual.List[0].IsNull);

			actual = testingRapplication.NewVarList("name", new List<int>{0}, ":System.Int32");
			Assert.IsTrue(actual.List[0].IsNull);
		}

		[Test] [Ignore]
		public void RobjectPerformsClientValidationGivenParametersAgainstOperationModel()
		{
			//TODO perform operation should check given parameters against operation model
			Assert.Fail("not implemented");
		}
	}
}

