using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Routine.Core.Configuration;
using Routine.Test.Core.Service.Ignored;

namespace Routine.Test.Core.Service.Ignored
{
	public class IgnoredModel {}
}

namespace Routine.Test.Core.Service.Domain.ObjectServiceTest_GetObjectModel
{
	public class BusinessModel
	{
		public int Id {get;set;}
		public List<string> List {get;set;}

		public void VoidOp(){}
		public string StringOp(List<string> list){return null;}
		public List<string> ListOp(bool boolParam){return null;}

		public IgnoredModel IgnoredMember{get;set;}
		public IgnoredModel IgnoredForReturnType(){return null;}
		public void IgnoredForParameter(IgnoredModel ignoreReason){}
	}

	public interface IBusinessModel{}

	public class BusinessValueModel
	{
		public static BusinessValueModel Parse(string value) {return null;}

		public string AutoExcludedMember{ get; set;}
		public void AutoExcludedOperation(){}
	}
}

namespace Routine.Test.Core.Service
{

	[TestFixture]
	public class ObjectServiceTest_GetObjectModel : ObjectServiceTestBase
	{
		private const string TESTED_OM_ID = "Routine-Test-Core-Service-Domain-ObjectServiceTest_GetObjectModel-BusinessModel";
		private const string TESTED_VAM_ID = ":Routine.Test.Core.Service.Domain.ObjectServiceTest_GetObjectModel.BusinessValueModel";
		private const string TESTED_VIM_ID = "Routine-Test-Core-Service-Domain-ObjectServiceTest_GetObjectModel-IBusinessModel";

		protected override string DefaultModelId{get{return TESTED_OM_ID;}}
		public override string[] DomainTypeRootNamespaces{get{return new[]{"Routine.Test.Core.Service.Domain.ObjectServiceTest_GetObjectModel"};}}

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			codingStyle
				.SerializeModelId.Done(s => s.SerializeBy(t => t.FullName.Replace(".", "-"))
									.SerializeWhen(t => !t.Namespace.Contains("Ignored") && t.IsDomainType)
									.DeserializeBy(id => id.Replace("-", ".").ToType())
									.DeserializeWhen(id => id.Contains("-")))
				.ExtractModelModule.Done(e => e.Always("Test"))
				.SelectModelMarks.Done(s => s.Always("value").When(t => t.CanParse()))
				.SelectModelMarks.Done(s => s.Always("all"))
				.SelectMemberMarks.Done(s => s.Always("heavy").When(m => m.ReturnsCollection()))
				.SelectOperationMarks.Done(s => s.Always("heavy").When(o => o.Parameters.Any(p => p.ParameterType.CanBeCollection())))
				.SelectParameterMarks.Done(s => s.Always("list").When(p => p.ParameterType.CanBeCollection()))

				.ExtractMemberIsHeavy.Done(e => e.Always(true).When(m => m.ReturnsCollection()))
				.ExtractOperationIsHeavy.Done(e => e.Always(true).When(o => o.Parameters.Any(p => p.ParameterType.CanBeCollection())))
				;
		}

		[Test]
		public void ModelIdIsCreatedUsingGivenSerializer()
		{
			var actual = testing.GetObjectModel(TESTED_OM_ID);

			Assert.AreEqual("Routine-Test-Core-Service-Domain-ObjectServiceTest_GetObjectModel-BusinessModel", actual.Id);
		}

		[Test]
		public void ObjectModelIsMarkedWithGivenSelectors()
		{
			var om = testing.GetObjectModel(TESTED_OM_ID);
			var vam = testing.GetObjectModel(TESTED_VAM_ID);

			Assert.IsFalse(om.Marks.Any(m => m == "value"));
			Assert.IsTrue(vam.Marks.Any(m => m == "value"));

			Assert.IsTrue(om.Marks.Any(m => m == "all"));
			Assert.IsTrue(vam.Marks.Any(m => m == "all"));
		}

		[Test]
		public void ObjectModelIsValueModelIfCorrespondingExtractorSaysSo()
		{
			var actual = testing.GetObjectModel(":System.String");

			Assert.IsTrue(actual.IsValueModel);
		}

		[Test]
		public void ValueModelDoesNotHaveMemberOrOperation()
		{
			var actual = testing.GetObjectModel(TESTED_VAM_ID);

			Assert.AreEqual(0, actual.Members.Count);
			Assert.AreEqual(0, actual.Operations.Count);
		}

		[Test]
		public void ObjectModelIsViewModelIfCorrespondingExtractorSaysSo()
		{
			var actual = testing.GetObjectModel(TESTED_VIM_ID);

			Assert.IsTrue(actual.IsViewModel);
		}

		[Test]
		public void NameIsTheSameWithCorrespondingClassName()
		{
			var actual = testing.GetObjectModel(TESTED_OM_ID);

			Assert.AreEqual("BusinessModel", actual.Name);
		}

		[Test]
		public void ModuleIsExtractedUsingCorrespondingExtractor()
		{
			var actual = testing.GetObjectModel(TESTED_OM_ID);

			Assert.AreEqual("Test", actual.Module);
		}

		[Test]
		public void MemberListIsCreatedUsingGivenSelector()
		{
			var actual = testing.GetObjectModel(TESTED_OM_ID);

			Assert.AreEqual(2, actual.Members.Count);
			Assert.AreEqual("Id", actual.Members[0].Id);
			Assert.AreEqual(":System.Int32", actual.Members[0].ViewModelId);
			Assert.AreEqual("List", actual.Members[1].Id);
			Assert.AreEqual(":System.String", actual.Members[1].ViewModelId);
		}

		[Test]
		public void AMemberIsMarkedWithGivenSelectors()
		{
			var om = testing.GetObjectModel(TESTED_OM_ID);
			
			Assert.IsFalse(om.Members[0].Marks.Any(m => m == "heavy"));
			Assert.IsTrue(om.Members[1].Marks.Any(m => m == "heavy"));
		}

		[Test]
		public void MembersAutomaticallyMarkedAsListWhenTheirTypeConformsToAGenericCollection()
		{
			var actual = testing.GetObjectModel(TESTED_OM_ID);

			Assert.IsFalse(actual.Members[0].IsList);
			Assert.IsTrue(actual.Members[1].IsList);
 		}

		[Test]
		public void AMemberIsMarkedAsHeavyIfCorrespondingExtractorSaysSo()
		{
			var actual = testing.GetObjectModel(TESTED_OM_ID);

			Assert.IsFalse(actual.Members[0].IsHeavy);
			Assert.IsTrue(actual.Members[1].IsHeavy);
		}

		[Test]
		public void OperationListIsCreatedUsingGivenSelector()
		{
			var actual = testing.GetObjectModel(TESTED_OM_ID);

			Assert.AreEqual(3, actual.Operations.Count);
			Assert.AreEqual("VoidOp", actual.Operations[0].Id);
			Assert.AreEqual(GenericCodingStyle.VOID_MODEL_ID, actual.Operations[0].Result.ViewModelId);
			Assert.AreEqual("StringOp", actual.Operations[1].Id);
			Assert.AreEqual(":System.String", actual.Operations[1].Result.ViewModelId);
			Assert.AreEqual("ListOp", actual.Operations[2].Id);
			Assert.AreEqual(":System.String", actual.Operations[2].Result.ViewModelId);
		}

		[Test]
		public void AnOperationCanReturnVoid()
		{
			var actual = testing.GetObjectModel(TESTED_OM_ID);

			Assert.IsTrue(actual.Operations[0].Result.IsVoid);
			Assert.IsFalse(actual.Operations[1].Result.IsVoid);
			Assert.IsFalse(actual.Operations[2].Result.IsVoid);
		}

		[Test]
		public void AnOperationCanReturnList()
		{
			var actual = testing.GetObjectModel(TESTED_OM_ID);

			Assert.IsFalse(actual.Operations[0].Result.IsList);
			Assert.IsFalse(actual.Operations[1].Result.IsList);
			Assert.IsTrue(actual.Operations[2].Result.IsList);
		}

		[Test]
		public void AnOperationIsMarkedWithGivenSelectors()
		{
			var om = testing.GetObjectModel(TESTED_OM_ID);

			Assert.IsFalse(om.Operations[0].Marks.Any(m => m == "heavy"));
			Assert.IsTrue(om.Operations[1].Marks.Any(m => m == "heavy"));
			Assert.IsFalse(om.Operations[2].Marks.Any(m => m == "heavy"));
		}

		[Test]
		public void AnOperationIsMarkedAsHeavyIfCorrespondingExtractorSaysSo()
		{			
			var actual = testing.GetObjectModel(TESTED_OM_ID);

			Assert.IsFalse(actual.Operations[0].IsHeavy);
			Assert.IsTrue(actual.Operations[1].IsHeavy);
			Assert.IsFalse(actual.Operations[2].IsHeavy);
		}

		[Test]
		public void AnOperationCanHaveParameters()
		{
			var actual = testing.GetObjectModel(TESTED_OM_ID);

			Assert.AreEqual(0, actual.Operations[0].Parameters.Count);
			Assert.AreEqual(1, actual.Operations[1].Parameters.Count);
			Assert.AreEqual("list", actual.Operations[1].Parameters[0].Id);
			Assert.AreEqual(":System.String", actual.Operations[1].Parameters[0].ViewModelId);
			Assert.AreEqual(1, actual.Operations[2].Parameters.Count);
			Assert.AreEqual("boolParam", actual.Operations[2].Parameters[0].Id);
			Assert.AreEqual(":System.Boolean", actual.Operations[2].Parameters[0].ViewModelId);
		}

		[Test]
		public void AParameterCanBeOfListType()
		{
			var actual = testing.GetObjectModel(TESTED_OM_ID);

			Assert.IsTrue(actual.Operations[1].Parameters[0].IsList);
			Assert.IsFalse(actual.Operations[2].Parameters[0].IsList);		
		}

		[Test]
		public void AParameterIsMarkedWithGivenSelectors()
		{
			var om = testing.GetObjectModel(TESTED_OM_ID);

			Assert.IsTrue(om.Operations[1].Parameters[0].Marks.Any(m => m == "list"));
			Assert.IsFalse(om.Operations[2].Parameters[0].Marks.Any(m => m == "list"));
		}

		[Test]
		public void MembersWithUnrecognizedViewModelIdsAreIgnoredAutomatically()
		{
			var actual = testing.GetObjectModel(TESTED_OM_ID);

			Assert.IsFalse(actual.Members.Any(m => m.ViewModelId.Contains("Ignored")));
		}

		[Test]
		public void OperationsWithUnrecognizedResultViewModelIdsAreIgnoredAutomatically()
		{
			var actual = testing.GetObjectModel(TESTED_OM_ID);

			Assert.IsFalse(actual.Operations.Any(m => m.Result.ViewModelId.Contains("Ignored")));
		}

		[Test]
		public void OperationsWithAnyUnrecognizedParameterViewModelIdsAreIgnoredAutomatically()
		{
			var actual = testing.GetObjectModel(TESTED_OM_ID);

			Assert.IsFalse(actual.Operations.Any(m => m.Parameters.Any(p => p.ViewModelId.Contains("Ignored"))));
		}
	}
}

