using System;
using System.Collections.Generic;
using NUnit.Framework;
using Routine.Core.Service;

namespace Routine.Test.Core.Service
{
	[TestFixture]
	public class ObjectServiceTest_ToStringAndEqualsOfModelAndDataClasses : CoreTestBase
	{
		public override string[] DomainTypeRootNamespaces{get{return new[]{"Routine.Test.Core.Service"};}}

		#region Helpers
		private class TestDataPrototype<T>
		{
			private readonly Func<T> defaultFunction;

			public TestDataPrototype(Func<T> defaultFunction)
			{
				this.defaultFunction = defaultFunction;
			}

			public T Create(){return Create(m => {});}
			public T Create(Action<T> manipulation)
			{				
				var result = defaultFunction();

				manipulation(result);

				return result;
			}
		}

		private void AssertEqualsAndHasSameHashCode<T>(T left, T right)
		{
			Assert.IsTrue(left.Equals(right), left + " and " + right + " should be equal");
			Assert.AreEqual(left.GetHashCode(), right.GetHashCode());
		}

		private void AssertDoesNotEqualAndHasDifferentHashCode<T>(T left, T right)
		{
			Assert.IsFalse(left.Equals(right), left + " and " + right + " should not be equal");
			Assert.AreNotEqual(left.GetHashCode(), right.GetHashCode());
		}

		private void AssertToStringHasTypeName<T>(T unitUnderTest)
		{
			Assert.IsTrue(unitUnderTest.ToString().Contains(typeof(T).Name), WrongToString(unitUnderTest.ToString()));
		}

		private void AssertToStringHasProperty<T>(T unitUnderTest, string propertyName)
		{
			Assert.IsTrue(unitUnderTest.ToString().Contains(propertyName+"="), WrongToString(unitUnderTest.ToString()));
		}

		private void AssertToStringHasPropertyAndItsValue<T>(T unitUnderTest, string propertyName, object propertyValue)
		{
			Assert.IsTrue(unitUnderTest.ToString().Contains(propertyName+"="+propertyValue), WrongToString(unitUnderTest.ToString()));
		}

		private string WrongToString(string toString)
		{
			return "Wrong ToString: " + toString;
		}
		#endregion

		[Test]
		public void MemberModel()
		{
			var prototype = new TestDataPrototype<MemberModel>(() => new MemberModel {
				Id = "id", 
				IsHeavy = true, 
				IsList = false, 
				ViewModelId = "view_model_id"
			});

			var testing = prototype.Create();

			//ToString tests
			AssertToStringHasTypeName(testing);

			AssertToStringHasPropertyAndItsValue(testing, "Id", testing.Id);
			AssertToStringHasPropertyAndItsValue(testing, "IsHeavy", testing.IsHeavy);
			AssertToStringHasPropertyAndItsValue(testing, "IsList", testing.IsList);
			AssertToStringHasPropertyAndItsValue(testing, "ViewModelId", testing.ViewModelId);

			//Equals & HashCode tests
			AssertEqualsAndHasSameHashCode(testing, prototype.Create());

			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Id = "different"));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.IsHeavy = false));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.IsList = true));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.ViewModelId = "different"));
		}

		[Test]
		public void ParameterModel()
		{
			var prototype = new TestDataPrototype<ParameterModel>(() => new ParameterModel {
				Id = "id", 
				IsList = true, 
				ViewModelId = "view_model_id"
			});

			var testing = prototype.Create();

			//ToString tests
			AssertToStringHasTypeName(testing);

			AssertToStringHasPropertyAndItsValue(testing, "Id", testing.Id);
			AssertToStringHasPropertyAndItsValue(testing, "IsList", testing.IsList);
			AssertToStringHasPropertyAndItsValue(testing, "ViewModelId", testing.ViewModelId);

			//Equals & HashCode tests
			AssertEqualsAndHasSameHashCode(testing, prototype.Create());

			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Id = "different"));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.IsList = false));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.ViewModelId = "different"));
		}

		[Test]
		public void ResultModel()
		{
			var prototype = new TestDataPrototype<ResultModel>(() => new ResultModel {
				IsList = true, 
				IsVoid = false, 
				ViewModelId = "view_model_id"
			});

			var testing = prototype.Create();

			//ToString tests
			AssertToStringHasTypeName(testing);

			AssertToStringHasPropertyAndItsValue(testing, "IsList", testing.IsList);
			AssertToStringHasPropertyAndItsValue(testing, "IsVoid", testing.IsVoid);
			AssertToStringHasPropertyAndItsValue(testing, "ViewModelId", testing.ViewModelId);

			//Equals & HashCode tests
			AssertEqualsAndHasSameHashCode(testing, prototype.Create());

			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.IsList = false));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.IsVoid = true));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.ViewModelId = "different"));
		}

		[Test]
		public void OperationModel()
		{
			var prototype = new TestDataPrototype<OperationModel>(() => new OperationModel {
				Id = "operation", 
				IsHeavy = true, 
				Parameters = new List<ParameterModel>{new ParameterModel{Id = "parameter"}},
				Result = new ResultModel{ViewModelId = "view_model_id"}
			});

			var testing = prototype.Create();

			//ToString tests
			AssertToStringHasTypeName(testing);

			AssertToStringHasPropertyAndItsValue(testing, "Id", testing.Id);
			AssertToStringHasPropertyAndItsValue(testing, "IsHeavy", testing.IsHeavy);

			AssertToStringHasProperty(testing, "Parameters");
			AssertToStringHasPropertyAndItsValue(testing, "Id", testing.Parameters[0].Id);

			AssertToStringHasProperty(testing, "Result");
			AssertToStringHasPropertyAndItsValue(testing, "ViewModelId", testing.Result.ViewModelId);

			//Equals & HashCode tests
			AssertEqualsAndHasSameHashCode(testing, prototype.Create());

			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Id = "different"));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.IsHeavy = false));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Parameters[0].Id = "different"));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Result.ViewModelId = "different"));
		}

		[Test]
		public void ObjectModel()
		{
			var prototype = new TestDataPrototype<ObjectModel>(() => new ObjectModel {
				Id = "object",
				Name = "object",
				Module = "system",
				IsValueModel = true,
				IsViewModel = false,
				Members = new List<MemberModel>{new MemberModel{Id = "member"}},
				Operations = new List<OperationModel>{new OperationModel{Id = "operation"}}
			});

			var testing = prototype.Create();

			//ToString tests
			AssertToStringHasTypeName(testing);

			AssertToStringHasPropertyAndItsValue(testing, "Id", testing.Id);
			AssertToStringHasPropertyAndItsValue(testing, "Name", testing.Name);
			AssertToStringHasPropertyAndItsValue(testing, "Module", testing.Module);
			AssertToStringHasPropertyAndItsValue(testing, "IsValueModel", testing.IsValueModel);
			AssertToStringHasPropertyAndItsValue(testing, "IsViewModel", testing.IsViewModel);

			AssertToStringHasProperty(testing, "Members");
			AssertToStringHasPropertyAndItsValue(testing, "Id", testing.Members[0].Id);

			AssertToStringHasProperty(testing, "Operations");
			AssertToStringHasPropertyAndItsValue(testing, "Id", testing.Operations[0].Id);

			//Equals & HashCode tests
			AssertEqualsAndHasSameHashCode(testing, prototype.Create());

			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Id = "different"));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Name = "different"));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Module = "different"));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.IsValueModel = false));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.IsViewModel = true));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Members[0].Id = "different"));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Operations[0].Id = "different"));
		}

		[Test]
		public void ApplicationModel()
		{
			var prototype = new TestDataPrototype<Routine.Core.Service.ApplicationModel>(() => new Routine.Core.Service.ApplicationModel {
				Models = new List<ObjectModel>{new ObjectModel{Id = "model"}}
			});

			var testing = prototype.Create();

			//ToString tests
			AssertToStringHasTypeName(testing);

			AssertToStringHasProperty(testing, "Models");
			AssertToStringHasPropertyAndItsValue(testing, "Id", testing.Models[0].Id);

			//Equals & HashCode tests
			AssertEqualsAndHasSameHashCode(testing, prototype.Create());

			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Models[0].Id = "different"));
		}

		[Test]
		public void ObjectReferenceData()
		{
			var prototype = new TestDataPrototype<ObjectReferenceData>(() => new ObjectReferenceData {
				Id = "object", 
				ActualModelId = "actual",
				ViewModelId = "view",
				IsNull = false,
			});

			var testing = prototype.Create();

			//ToString tests
			AssertToStringHasTypeName(testing);

			AssertToStringHasPropertyAndItsValue(testing, "Id", testing.Id);
			AssertToStringHasPropertyAndItsValue(testing, "ActualModelId", testing.ActualModelId);
			AssertToStringHasPropertyAndItsValue(testing, "ViewModelId", testing.ViewModelId);
			AssertToStringHasPropertyAndItsValue(testing, "IsNull", testing.IsNull);

			//Equals & HashCode tests
			AssertEqualsAndHasSameHashCode(testing, prototype.Create());

			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.Id = "different"));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.ActualModelId = "different"));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.ViewModelId = "different"));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.IsNull = true));
		}

		[Test]
		public void ObjectReferenceData_NullCase()
		{
			var prototype = new TestDataPrototype<ObjectReferenceData>(() => new ObjectReferenceData {
				IsNull = true,
			});

			var testing = prototype.Create();

			//Equals & HashCode tests
			AssertEqualsAndHasSameHashCode(testing, prototype.Create());
			AssertEqualsAndHasSameHashCode(testing, prototype.Create(d => d.Id = "different"));
			AssertEqualsAndHasSameHashCode(testing, prototype.Create(d => d.ActualModelId = "different"));
			AssertEqualsAndHasSameHashCode(testing, prototype.Create(d => d.ViewModelId = "different"));
		}

		[Test]
		public void ReferenceData()
		{
			var prototype = new TestDataPrototype<ReferenceData>(() => new ReferenceData {
				IsList = true,
				References = new List<ObjectReferenceData>{new ObjectReferenceData{Id = "id"}}
			});

			var testing = prototype.Create();

			//ToString tests
			AssertToStringHasTypeName(testing);

			AssertToStringHasPropertyAndItsValue(testing, "IsList", testing.IsList);

			AssertToStringHasProperty(testing, "References");
			AssertToStringHasPropertyAndItsValue(testing, "Id", testing.References[0].Id);

			//Equals & HashCode tests
			AssertEqualsAndHasSameHashCode(testing, prototype.Create());

			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.IsList = false));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.References[0].Id = "different"));
		}

		[Test]
		public void SingleValueData()
		{
			var prototype = new TestDataPrototype<SingleValueData>(() => new SingleValueData {
				Reference = new ObjectReferenceData{Id = "id" },
				Value = "value",
			});

			var testing = prototype.Create();

			//ToString tests
			AssertToStringHasTypeName(testing);

			AssertToStringHasPropertyAndItsValue(testing, "Value", testing.Value);
			AssertToStringHasProperty(testing, "Reference");
			AssertToStringHasPropertyAndItsValue(testing, "Id", testing.Reference.Id);

			//Equals & HashCode tests
			AssertEqualsAndHasSameHashCode(testing, prototype.Create());

			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.Value = "different"));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.Reference.Id = "different"));
		}

		[Test]
		public void ValueData()
		{
			var prototype = new TestDataPrototype<ValueData>(() => new ValueData {
				IsList = true,
				Values = new List<SingleValueData>{new SingleValueData{Value = "value" }},
			});

			var testing = prototype.Create();

			//ToString tests
			AssertToStringHasTypeName(testing);

			AssertToStringHasPropertyAndItsValue(testing, "IsList", testing.IsList);
			AssertToStringHasProperty(testing, "Values");
			AssertToStringHasPropertyAndItsValue(testing, "Value", testing.Values[0].Value);

			//Equals & HashCode tests
			AssertEqualsAndHasSameHashCode(testing, prototype.Create());

			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.IsList = false));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.Values[0].Value = "different"));
		}

		[Test]
		public void ParameterValueData()
		{
			var prototype = new TestDataPrototype<ParameterValueData>(() => new ParameterValueData {
				ParameterModelId = "model",
				Value = new ReferenceData{IsList = true },
			});

			var testing = prototype.Create();

			//ToString tests
			AssertToStringHasTypeName(testing);

			AssertToStringHasPropertyAndItsValue(testing, "ParameterModelId", testing.ParameterModelId);
			AssertToStringHasProperty(testing, "Value");
			AssertToStringHasPropertyAndItsValue(testing, "IsList", testing.Value.IsList);

			//Equals & HashCode tests
			AssertEqualsAndHasSameHashCode(testing, prototype.Create());

			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.ParameterModelId = "different"));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.Value.IsList = false));
		}

		[Test]
		public void ParameterData()
		{
			var prototype = new TestDataPrototype<ParameterData>(() => new ParameterData {
				ModelId = "model",
			});

			var testing = prototype.Create();

			//ToString tests
			AssertToStringHasTypeName(testing);

			AssertToStringHasPropertyAndItsValue(testing, "ModelId", testing.ModelId);

			//Equals & HashCode tests
			AssertEqualsAndHasSameHashCode(testing, prototype.Create());

			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.ModelId = "different"));
		}

		[Test]
		public void ResultData()
		{
			var prototype = new TestDataPrototype<ResultData>(() => new ResultData {
				Value = new ValueData{IsList = true },
			});

			var testing = prototype.Create();

			//ToString tests
			AssertToStringHasTypeName(testing);

			AssertToStringHasProperty(testing, "Value");
			AssertToStringHasPropertyAndItsValue(testing, "IsList", testing.Value.IsList);

			//Equals & HashCode tests
			AssertEqualsAndHasSameHashCode(testing, prototype.Create());

			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.Value.IsList = false));
		}

		[Test]
		public void OperationData()
		{
			var prototype = new TestDataPrototype<OperationData>(() => new OperationData {
				ModelId = "model",
				Parameters = new List<ParameterData>{new ParameterData{ModelId = "pmodel" }}
			});

			var testing = prototype.Create();

			//ToString tests
			AssertToStringHasTypeName(testing);

			AssertToStringHasPropertyAndItsValue(testing, "ModelId", testing.ModelId);

			AssertToStringHasProperty(testing, "Parameters");
			AssertToStringHasPropertyAndItsValue(testing, "ModelId", testing.Parameters[0].ModelId);

			//Equals & HashCode tests
			AssertEqualsAndHasSameHashCode(testing, prototype.Create());

			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.ModelId = "different"));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.Parameters[0].ModelId = "different"));
		}

		[Test]
		public void MemberData()
		{
			var prototype = new TestDataPrototype<MemberData>(() => new MemberData {
				ModelId = "model",
				Value = new ValueData{IsList = true },
			});

			var testing = prototype.Create();

			//ToString tests
			AssertToStringHasTypeName(testing);

			AssertToStringHasPropertyAndItsValue(testing, "ModelId", testing.ModelId);

			AssertToStringHasProperty(testing, "Value");
			AssertToStringHasPropertyAndItsValue(testing, "IsList", testing.Value.IsList);

			//Equals & HashCode tests
			AssertEqualsAndHasSameHashCode(testing, prototype.Create());

			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.ModelId = "different"));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.Value.IsList = false));
		}

		[Test]
		public void ObjectData()
		{
			var prototype = new TestDataPrototype<ObjectData>(() => new ObjectData {
				Value = "value",
				Reference = new ObjectReferenceData{Id = "id" },
				Members = new List<MemberData>{new MemberData{ModelId = "mmodel" }},
				Operations = new List<OperationData>{new OperationData{ModelId = "omodel" }}
			});

			var testing = prototype.Create();

			//ToString tests
			AssertToStringHasTypeName(testing);

			AssertToStringHasPropertyAndItsValue(testing, "Value", testing.Value);

			AssertToStringHasProperty(testing, "Reference");
			AssertToStringHasPropertyAndItsValue(testing, "Id", testing.Reference.Id);

			AssertToStringHasProperty(testing, "Members");
			AssertToStringHasPropertyAndItsValue(testing, "Id", testing.Members[0].ModelId);

			AssertToStringHasProperty(testing, "Operations");
			AssertToStringHasPropertyAndItsValue(testing, "Id", testing.Operations[0].ModelId);

			//Equals & HashCode tests
			AssertEqualsAndHasSameHashCode(testing, prototype.Create());

			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.Value = "different"));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.Reference.Id = "different"));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.Members[0].ModelId = "different"));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.Operations[0].ModelId = "different"));
		}
	}
}