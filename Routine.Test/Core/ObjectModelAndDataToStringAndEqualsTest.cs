using System;
using System.Collections.Generic;
using NUnit.Framework;
using Routine.Core;

namespace Routine.Test.Core
{
	[TestFixture]
	public class ObjectModelAndDataToStringAndEqualsTest : CoreTestBase
	{
		#region Helpers

		private class TestDataPrototype<T>
		{
			private readonly Func<T> defaultFunction;

			public TestDataPrototype(Func<T> defaultFunction)
			{
				this.defaultFunction = defaultFunction;
			}

			public T Create() { return Create(m => { }); }
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
			Assert.IsTrue(unitUnderTest.ToString().Contains(propertyName + "="), WrongToString(unitUnderTest.ToString()));
		}

		private void AssertToStringHasPropertyAndItsValue<T>(T unitUnderTest, string propertyName, object propertyValue)
		{
			Assert.IsTrue(unitUnderTest.ToString().Contains(propertyName + "=" + propertyValue), WrongToString(unitUnderTest.ToString()));
		}

		private string WrongToString(string toString)
		{
			return "Wrong ToString: " + toString;
		}

		#endregion

		[Test]
		public void MemberModel()
		{
			var prototype = new TestDataPrototype<MemberModel>(() => new MemberModel
			{
				Id = "id",
				Marks = new List<string> { "mark1", "mark2" },
				IsList = false,
				ViewModelId = "view_model_id"
			});

			var testing = prototype.Create();

			//ToString tests
			AssertToStringHasTypeName(testing);

			AssertToStringHasPropertyAndItsValue(testing, "Id", testing.Id);
			AssertToStringHasPropertyAndItsValue(testing, "Marks", testing.Marks.ToItemString());
			AssertToStringHasPropertyAndItsValue(testing, "IsList", testing.IsList);
			AssertToStringHasPropertyAndItsValue(testing, "ViewModelId", testing.ViewModelId);

			//Equals & HashCode tests
			AssertEqualsAndHasSameHashCode(testing, prototype.Create());

			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Id = "different"));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Marks.Add("different")));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.IsList = true));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.ViewModelId = "different"));
		}

		[Test]
		public void ParameterModel()
		{
			var prototype = new TestDataPrototype<ParameterModel>(() => new ParameterModel
			{
				Id = "id",
				Marks = new List<string> { "mark1", "mark2" },
				Groups = new List<int> { 1, 2 },
				IsList = true,
				ViewModelId = "view_model_id"
			});

			var testing = prototype.Create();

			//ToString tests
			AssertToStringHasTypeName(testing);

			AssertToStringHasPropertyAndItsValue(testing, "Id", testing.Id);
			AssertToStringHasPropertyAndItsValue(testing, "Marks", testing.Marks.ToItemString());
			AssertToStringHasPropertyAndItsValue(testing, "Groups", testing.Groups.ToItemString());
			AssertToStringHasPropertyAndItsValue(testing, "IsList", testing.IsList);
			AssertToStringHasPropertyAndItsValue(testing, "ViewModelId", testing.ViewModelId);

			//Equals & HashCode tests
			AssertEqualsAndHasSameHashCode(testing, prototype.Create());

			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Id = "different"));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Marks.Add("different")));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Groups.Add(3)));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.IsList = false));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.ViewModelId = "different"));
		}

		[Test]
		public void ResultModel()
		{
			var prototype = new TestDataPrototype<ResultModel>(() => new ResultModel
			{
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
			var prototype = new TestDataPrototype<OperationModel>(() => new OperationModel
			{
				Id = "operation",
				Marks = new List<string> { "mark1", "mark2" },
				GroupCount = 2,
				Parameters = new List<ParameterModel> { new ParameterModel { Id = "parameter" } },
				Result = new ResultModel { ViewModelId = "view_model_id" }
			});

			var testing = prototype.Create();

			//ToString tests
			AssertToStringHasTypeName(testing);

			AssertToStringHasPropertyAndItsValue(testing, "Id", testing.Id);
			AssertToStringHasPropertyAndItsValue(testing, "Marks", testing.Marks.ToItemString());
			AssertToStringHasPropertyAndItsValue(testing, "GroupCount", testing.GroupCount);

			AssertToStringHasProperty(testing, "Parameters");
			AssertToStringHasPropertyAndItsValue(testing, "Id", testing.Parameters[0].Id);

			AssertToStringHasProperty(testing, "Result");
			AssertToStringHasPropertyAndItsValue(testing, "ViewModelId", testing.Result.ViewModelId);

			//Equals & HashCode tests
			AssertEqualsAndHasSameHashCode(testing, prototype.Create());

			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Id = "different"));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Marks.Add("different")));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.GroupCount = 3));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Parameters[0].Id = "different"));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Result.ViewModelId = "different"));
		}

		[Test]
		public void InitializerModel()
		{
			var prototype = new TestDataPrototype<InitializerModel>(() => new InitializerModel
			{
				Marks = new List<string> { "mark1", "mark2" },
				GroupCount = 2,
				Parameters = new List<ParameterModel> { new ParameterModel { Id = "parameter" } }
			});

			var testing = prototype.Create();

			//ToString tests
			AssertToStringHasTypeName(testing);

			AssertToStringHasPropertyAndItsValue(testing, "Marks", testing.Marks.ToItemString());
			AssertToStringHasPropertyAndItsValue(testing, "GroupCount", testing.GroupCount);

			AssertToStringHasProperty(testing, "Parameters");
			AssertToStringHasPropertyAndItsValue(testing, "Id", testing.Parameters[0].Id);

			//Equals & HashCode tests
			AssertEqualsAndHasSameHashCode(testing, prototype.Create());

			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Marks.Add("different")));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.GroupCount = 3));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Parameters[0].Id = "different"));
		}

		[Test]
		public void ObjectModel()
		{
			var prototype = new TestDataPrototype<ObjectModel>(() => new ObjectModel
			{
				Id = "object",
				Marks = new List<string> { "mark1", "mark2" },
				Name = "object",
				Module = "system",
				IsValueModel = true,
				IsViewModel = false,
				ViewModelIds = new List<string> { "interface" },
				Initializer = new InitializerModel { GroupCount = 2 },
				Members = new List<MemberModel> { new MemberModel { Id = "member" } },
				Operations = new List<OperationModel> { new OperationModel { Id = "operation" } },
				StaticInstances = new List<ObjectData> { new ObjectData { Value = "value1" }, new ObjectData { Value = "value2" } }
			});

			var testing = prototype.Create();

			//ToString tests
			AssertToStringHasTypeName(testing);

			AssertToStringHasPropertyAndItsValue(testing, "Id", testing.Id);
			AssertToStringHasPropertyAndItsValue(testing, "Marks", testing.Marks.ToItemString());
			AssertToStringHasPropertyAndItsValue(testing, "Name", testing.Name);
			AssertToStringHasPropertyAndItsValue(testing, "Module", testing.Module);
			AssertToStringHasPropertyAndItsValue(testing, "IsValueModel", testing.IsValueModel);
			AssertToStringHasPropertyAndItsValue(testing, "IsViewModel", testing.IsViewModel);
			AssertToStringHasPropertyAndItsValue(testing, "ViewModelIds", testing.ViewModelIds[0].SurroundWith("[", "]"));

			AssertToStringHasProperty(testing, "Initializer");
			AssertToStringHasPropertyAndItsValue(testing, "GroupCount", testing.Initializer.GroupCount);

			AssertToStringHasProperty(testing, "Members");
			AssertToStringHasPropertyAndItsValue(testing, "Id", testing.Members[0].Id);

			AssertToStringHasProperty(testing, "Operations");
			AssertToStringHasPropertyAndItsValue(testing, "Id", testing.Operations[0].Id);

			AssertToStringHasProperty(testing, "StaticInstances");
			AssertToStringHasPropertyAndItsValue(testing, "Value", testing.StaticInstances[0].Value);

			//Equals & HashCode tests
			AssertEqualsAndHasSameHashCode(testing, prototype.Create());

			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Id = "different"));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Marks.Add("different")));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Name = "different"));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Module = "different"));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.IsValueModel = false));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.IsViewModel = true));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.ViewModelIds[0] = "different"));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Initializer.GroupCount = 3));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Members[0].Id = "different"));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.Operations[0].Id = "different"));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(m => m.StaticInstances[0].Value = "different"));
		}

		[Test]
		public void ApplicationModel()
		{
			var prototype = new TestDataPrototype<ApplicationModel>(() => new ApplicationModel
			{
				Models = new List<ObjectModel> { new ObjectModel { Id = "model" } }
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
			var prototype = new TestDataPrototype<ObjectReferenceData>(() => new ObjectReferenceData
			{
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
		public void ObjectReferenceData__when_null__ToString_Equals_and_GetHashCode_methods_only_uses_IsNull_property()
		{
			var prototype = new TestDataPrototype<ObjectReferenceData>(() => new ObjectReferenceData
			{
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
		public void ParameterValueData()
		{
			var prototype = new TestDataPrototype<ParameterValueData>(() => new ParameterValueData
			{
				IsList = true,
				Values = new List<ParameterData> { new ParameterData { ReferenceId = "id" } },
			});

			var testing = prototype.Create();

			//ToString tests
			AssertToStringHasTypeName(testing);

			AssertToStringHasPropertyAndItsValue(testing, "IsList", testing.IsList);
			AssertToStringHasProperty(testing, "Values");
			AssertToStringHasPropertyAndItsValue(testing, "ReferenceId", testing.Values[0].ReferenceId);

			//Equals & HashCode tests
			AssertEqualsAndHasSameHashCode(testing, prototype.Create());

			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.IsList = false));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.Values[0].ReferenceId = "different"));
		}

		[Test]
		public void ParameterData()
		{
			var prototype = new TestDataPrototype<ParameterData>(() => new ParameterData
			{
				ReferenceId = "id",
				ObjectModelId = "modelid",
				IsNull = false,
				InitializationParameters = new Dictionary<string, ParameterValueData>
				{
					{
						"param1", 
						new ParameterValueData
						{
							IsList = false,
							Values = new List<ParameterData>
							{
								new ParameterData
								{
									ReferenceId = "childid",
									IsNull = false,
								}
							}
						}
					}
				},
			});

			var testing = prototype.Create();

			//ToString tests
			AssertToStringHasTypeName(testing);

			AssertToStringHasPropertyAndItsValue(testing, "ReferenceId", testing.ReferenceId);
			AssertToStringHasPropertyAndItsValue(testing, "ObjectModelId", testing.ObjectModelId);
			AssertToStringHasPropertyAndItsValue(testing, "IsNull", testing.IsNull);

			AssertToStringHasProperty(testing, "InitializationParameters");
			AssertToStringHasPropertyAndItsValue(testing, "Id", testing.InitializationParameters["param1"].Values[0].ReferenceId);

			//Equals & HashCode tests
			AssertEqualsAndHasSameHashCode(testing, prototype.Create());

			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.ReferenceId = "different"));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.ObjectModelId = "different"));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.IsNull = true));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.InitializationParameters["param1"].Values[0].ReferenceId = "different"));
		}

		[Test]
		public void ParameterData__when_null__ToString_Equals_and_GetHashCode_methods_only_uses_IsNull_property()
		{
			var prototype = new TestDataPrototype<ParameterData>(() => new ParameterData
			{
				IsNull = true,
			});

			var testing = prototype.Create();

			//Equals & HashCode tests
			AssertEqualsAndHasSameHashCode(testing, prototype.Create());
			AssertEqualsAndHasSameHashCode(testing, prototype.Create(d => d.ReferenceId = "different"));
			AssertEqualsAndHasSameHashCode(testing, prototype.Create(d => d.ObjectModelId = "different"));
		}

		[Test]
		public void ValueData()
		{
			var prototype = new TestDataPrototype<ValueData>(() => new ValueData
			{
				IsList = true,
				Values = new List<ObjectData> { new ObjectData { Value = "value" } },
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
		public void ObjectData()
		{
			var prototype = new TestDataPrototype<ObjectData>(() => new ObjectData
			{
				Value = "value",
				Reference = new ObjectReferenceData { Id = "id" },
				Members = new Dictionary<string, ValueData>
				{
					{
						"mmodel", 
						new ValueData
						{
							Values = new List<ObjectData>
							{
								new ObjectData
								{
									Reference = new ObjectReferenceData
									{
										Id="childid"
									},
									Value = "childvalue"
								}
							}
						}
					}
				}
			});

			var testing = prototype.Create();

			//ToString tests
			AssertToStringHasTypeName(testing);

			AssertToStringHasPropertyAndItsValue(testing, "Value", testing.Value);

			AssertToStringHasProperty(testing, "Reference");
			AssertToStringHasPropertyAndItsValue(testing, "Id", testing.Reference.Id);

			AssertToStringHasProperty(testing, "Members");
			AssertToStringHasPropertyAndItsValue(testing, "Id", testing.Members["mmodel"].Values[0].Reference.Id);
			AssertToStringHasPropertyAndItsValue(testing, "Value", testing.Members["mmodel"].Values[0].Value);


			//Equals & HashCode tests
			AssertEqualsAndHasSameHashCode(testing, prototype.Create());

			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.Value = "different"));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.Reference.Id = "different"));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.Members["mmodel"].Values[0].Value = "different"));
			AssertDoesNotEqualAndHasDifferentHashCode(testing, prototype.Create(d => d.Members["mmodel"].Values[0].Reference.Id = "different"));
		}
	}
}