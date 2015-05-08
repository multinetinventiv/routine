using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Moq;
using Moq.Language.Flow;
using NUnit.Framework;
using Routine.Client;
using Routine.Client.Context;
using Routine.Core;

namespace Routine.Test.Client
{
	public abstract class ClientTestBase
	{
		protected Mock<IObjectService> objectServiceMock;
		private Dictionary<string, ObjectModel> objectModelDictionary;
		private Dictionary<ObjectReferenceData, ObjectData> objectDictionary;

		protected IClientContext ctx;
		protected Rapplication testingRapplication;

		protected virtual string DefaultObjectModelId { get { return "DefaultModel"; } }

		[SetUp]
		public virtual void SetUp()
		{
			objectServiceMock = new Mock<IObjectService>();
			objectModelDictionary = new Dictionary<string, ObjectModel>();
			objectDictionary = new Dictionary<ObjectReferenceData, ObjectData>();

			var clientContext = new DefaultClientContext(objectServiceMock.Object, new Rapplication(objectServiceMock.Object));

			ctx = clientContext;
			testingRapplication = clientContext.Application;

			objectServiceMock.Setup(o => o.GetApplicationModel())
				.Returns(() => new ApplicationModel { Models = objectModelDictionary.Select(o => o.Value).ToList() });
			objectServiceMock.Setup(o => o.GetObjectModel(It.IsAny<string>()))
				.Returns((string omid) => objectModelDictionary[omid]);
			objectServiceMock.Setup(o => o.Get(It.IsAny<ObjectReferenceData>()))
				.Returns((ObjectReferenceData ord) => objectDictionary[ord]);
			objectServiceMock.Setup(o => o.GetValue(It.IsAny<ObjectReferenceData>()))
				.Returns((ObjectReferenceData ord) => objectDictionary[ord].Value);
			objectServiceMock.Setup(o => o.PerformOperation(It.IsAny<ObjectReferenceData>(), It.IsAny<string>(), It.IsAny<Dictionary<string, ParameterValueData>>()))
				.Returns(Void());

			ModelsAre(Model());
		}

		#region Model Builders
		protected void ModelsAre(params ObjectModelBuilder[] objectModels) { ModelsAre(false, objectModels); }
		protected void ModelsAre(bool clearPreviousModels, params ObjectModelBuilder[] objectModels)
		{
			if (clearPreviousModels)
			{
				objectModelDictionary.Clear();
				var defaultModel = Model().Build();
				objectModelDictionary.Add(defaultModel.Id, defaultModel);
			}

			foreach (var objectModel in objectModels.Select(o => o.Build()))
			{
				try
				{
					objectModelDictionary.Add(objectModel.Id, objectModel);
				}
				catch (ArgumentException ex)
				{
					throw new Exception(objectModel.Id + " was already registered", ex);
				}
			}
		}

		protected ObjectModelBuilder Model() { return Model(DefaultObjectModelId).IsValue(); }
		protected ObjectModelBuilder Model(string id) { return new ObjectModelBuilder(DefaultObjectModelId).Id(id); }
		protected class ObjectModelBuilder
		{
			private readonly string defaultObjectModelId;

			private readonly ObjectModel result;

			public ObjectModelBuilder(string defaultObjectModelId)
			{
				this.defaultObjectModelId = defaultObjectModelId;
				result = new ObjectModel();
			}

			public ObjectModelBuilder Id(string id) { result.Id = id; return this; }

			public ObjectModelBuilder ViewModelIds(params string[] viewModelIds)
			{
				result.ViewModelIds.AddRange(viewModelIds);
				return this;
			}

			public ObjectModelBuilder Mark(params string[] marks) { result.Marks.AddRange(marks); return this; }
			public ObjectModelBuilder MarkMember(string memberId, params string[] marks) { result.Members.Single(m => m.Id == memberId).Marks.AddRange(marks); return this; }
			public ObjectModelBuilder MarkOperation(string operationId, params string[] marks) { result.Operations.Single(o => o.Id == operationId).Marks.AddRange(marks); return this; }
			public ObjectModelBuilder MarkParameter(string operationId, string parameterId, params string[] marks) { result.Operations.Single(o => o.Id == operationId).Parameters.Single(p => p.Id == parameterId).Marks.AddRange(marks); return this; }

			public ObjectModelBuilder IsValue() { result.IsValueModel = true; return this; }

			public ObjectModelBuilder IsView(string firstActualModelId, params string[] restOfTheActualModelIds)
			{
				if (string.IsNullOrEmpty(firstActualModelId)) { throw new ArgumentException("firstActualModelId cannot be null or empty. A view model should have at least one actual model id", "firstActualModelId"); }

				result.IsViewModel = true;
				result.ActualModelIds.Add(firstActualModelId);
				result.ActualModelIds.AddRange(restOfTheActualModelIds);

				return this;
			}

			public ObjectModelBuilder Name(string name) { result.Name = name; return this; }
			public ObjectModelBuilder Module(string module) { result.Module = module; return this; }

			public ObjectModelBuilder Initializer(params ParameterModel[] parameters) { return Initializer(parameters.Any() ? parameters.Max(p => p.Groups.Max()) + 1 : 1, parameters); }
			public ObjectModelBuilder Initializer(int groupCount, params ParameterModel[] parameters)
			{
				result.Initializer = new InitializerModel
				{
					Parameters = parameters.ToList(),
					GroupCount = groupCount
				};

				return this;
			}

			public ObjectModelBuilder Member(string memberId) { return Member(memberId, defaultObjectModelId); }
			public ObjectModelBuilder Member(string memberId, string viewModelId) { return Member(memberId, viewModelId, false); }
			public ObjectModelBuilder Member(string memberId, string viewModelId, bool isList)
			{
				result.Members.Add(new MemberModel
				{
					Id = memberId,
					ViewModelId = viewModelId,
					IsList = isList,
				});

				return this;
			}

			public ObjectModelBuilder Operation(string operationId, params ParameterModel[] parameters) { return Operation(operationId, false, parameters); }
			public ObjectModelBuilder Operation(string operationId, bool isVoid, params ParameterModel[] parameters) { return Operation(operationId, isVoid ? null : defaultObjectModelId, parameters); }
			public ObjectModelBuilder Operation(string operationId, string resultViewModelId, params ParameterModel[] parameters) { return Operation(operationId, resultViewModelId, false, parameters); }
			public ObjectModelBuilder Operation(string operationId, string resultViewModelId, bool isList, params ParameterModel[] parameters)
			{
				var operationModel = new OperationModel
				{
					Id = operationId,
					Parameters = parameters.ToList(),
					GroupCount = parameters.Any() ? parameters.Max(p => p.Groups.Max()) + 1 : 1
				};
				operationModel.Result.IsVoid = resultViewModelId == null;
				operationModel.Result.ViewModelId = resultViewModelId;
				operationModel.Result.IsList = isList;

				return Operation(operationModel);
			}

			public ObjectModelBuilder Operation(OperationModel operationModel)
			{
				result.Operations.Add(operationModel);

				return this;
			}

			public ObjectModelBuilder StaticInstanceIds(params string[] ids)
			{
				foreach (var id in ids)
				{
					StaticInstanceId(id, result.Id);
				}

				return this;
			}

			public ObjectModelBuilder StaticInstanceId(string id, string actualModelId)
			{
				result
					.StaticInstances
					.Add(
						new ObjectData
						{
							Reference = new ObjectReferenceData
							{
								Id = id,
								ActualModelId = actualModelId,
								ViewModelId = result.Id
							},
						}
					);

				return this;
			}


			public ObjectModel Build()
			{
				return result;
			}
		}

		protected ParameterModel PModel(string id, params int[] groups) { return PModel(id, false, groups); }
		protected ParameterModel PModel(string id, string viewModelId, params int[] groups) { return PModel(id, viewModelId, false, groups); }
		protected ParameterModel PModel(string id, bool isList, params int[] groups) { return PModel(id, DefaultObjectModelId, isList, groups); }
		protected ParameterModel PModel(string id, string viewModelId, bool isList, params int[] groups)
		{
			return new ParameterModel
			{
				Id = id,
				IsList = isList,
				ViewModelId = viewModelId,
				Groups = groups.Any() ? groups.ToList() : new List<int> { 0 }
			};
		}

		#endregion

		#region Data Builders

		protected void EnsureModels(params string[] modelIds)
		{
			foreach (var modelId in modelIds)
			{
				if (modelId != null && !objectModelDictionary.ContainsKey(modelId))
				{
					ModelsAre(Model(modelId).Name(modelId));
				}
			}
		}

		protected ObjectReferenceData Null() { return Null(null); }
		protected ObjectReferenceData Null(string modelId) { return Id("null", modelId, modelId, true); }
		protected ObjectReferenceData Id(string id) { return Id(id, DefaultObjectModelId); }
		protected ObjectReferenceData Id(string id, string modelId) { return Id(id, modelId, modelId); }
		protected ObjectReferenceData Id(string id, string actualModelId, string viewModelId) { return Id(id, actualModelId, viewModelId, false); }
		protected ObjectReferenceData Id(string id, string actualModelId, string viewModelId, bool isNull)
		{
			EnsureModels(actualModelId, viewModelId);

			return new ObjectReferenceData
			{
				Id = id,
				ActualModelId = actualModelId,
				ViewModelId = viewModelId,
				IsNull = isNull,
			};
		}

		protected void ObjectsAre(params ObjectBuilder[] objects)
		{
			foreach (var @object in objects.Select(o => o.Build()))
			{
				objectDictionary.Add(@object.Reference, @object);
			}
		}

		protected ObjectBuilder Object(ObjectReferenceData reference) { return new ObjectBuilder(objectModelDictionary, objectDictionary).Reference(reference); }
		protected class ObjectBuilder
		{
			private readonly Dictionary<string, ObjectModel> objectModels;
			private readonly Dictionary<ObjectReferenceData, ObjectData> objects;

			private readonly ObjectData result;

			public ObjectBuilder(Dictionary<string, ObjectModel> objectModels, Dictionary<ObjectReferenceData, ObjectData> objects)
			{
				this.objectModels = objectModels;
				this.objects = objects;
				result = new ObjectData();
			}

			public ObjectBuilder Reference(ObjectReferenceData reference)
			{
				result.Reference = reference;
				if (!objectModels.ContainsKey(reference.ViewModelId))
				{
					return this;
				}

				var model = objectModels[reference.ViewModelId];

				foreach (var memberModel in model.Members)
				{
					result.Members.Add(memberModel.Id, new ValueData { IsList = memberModel.IsList });
				}

				return this;
			}

			public ObjectBuilder Value(string value)
			{
				result.Value = value;

				return this;
			}

			public ObjectBuilder Member(string memberModelId, params ObjectReferenceData[] memberData)
			{
				result.Members[memberModelId]
					.Values.AddRange(memberData.Select(m => new ObjectData { Reference = m }));

				return this;
			}

			public ObjectData Build()
			{
				foreach (var mem in result.Members)
				{
					foreach (var val in mem.Value.Values)
					{
						if (val.Reference.IsNull) { continue; }
						if (objectModels.ContainsKey(val.Reference.ActualModelId) && objectModels[val.Reference.ActualModelId].IsValueModel)
						{
							val.Value = val.Reference.Id;
							continue;
						}

						val.Value = objects[val.Reference].Value;
					}
				}
				return result;
			}
		}

		private string ValueOf(ObjectReferenceData ord)
		{
			if (ord.IsNull) { return ""; }
			if (objectModelDictionary.ContainsKey(ord.ActualModelId) && objectModelDictionary[ord.ActualModelId].IsValueModel)
			{
				return ord.Id;
			}

			return objectDictionary[ord].Value;
		}

		protected ValueData Result(params ObjectReferenceData[] ords)
		{
			var result = new ValueData();

			result.Values.AddRange(
				ords.Select(ord => new ObjectData
				{
					Reference = ord,
					Value = ValueOf(ord)
				}).ToList());

			return result;
		}

		protected ValueData Void()
		{
			return new ValueData();
		}

		#endregion

		#region Stubbers

		protected ObjectStubber When(ObjectReferenceData objectReferenceData)
		{
			return new ObjectStubber(this, objectReferenceData);
		}

		protected class ObjectStubber
		{
			private readonly ClientTestBase test;
			private readonly ObjectReferenceData objectReferenceData;

			public ObjectStubber(ClientTestBase test, ObjectReferenceData objectReferenceData)
			{
				this.test = test;
				this.objectReferenceData = objectReferenceData;
			}

			public ISetup<IObjectService, ValueData> Performs(string operationModelId) { return Performs(operationModelId, p => true); }
			public ISetup<IObjectService, ValueData> Performs(string operationModelId, Expression<Func<Dictionary<string, ParameterValueData>, bool>> parameterMatcher)
			{
				return test.objectServiceMock
						.Setup(o => o.PerformOperation(
							objectReferenceData,
							operationModelId,
							It.Is(parameterMatcher)));
			}
		}

		#endregion

		protected Rtype Rtyp(string id)
		{
			return testingRapplication[id];
		}

		protected Robject RobjNull() { return testingRapplication.NullObject(); }
		protected Robject Robj(string id) { return Robj(id, DefaultObjectModelId); }
		protected Robject Robj(string id, string modelId) { return Robj(id, modelId, modelId); }
		protected Robject Robj(string id, string actualModelId, string viewModelId)
		{
			return testingRapplication.Get(id, actualModelId, viewModelId);
		}

		protected Robject Robj(string modelId, params Rvariable[] initializationParameters)
		{
			return testingRapplication.Init(modelId, initializationParameters);
		}

		protected Rvariable Rvar(string name, Robject value)
		{
			return new Rvariable(name, value);
		}

		protected Rvariable Rvarlist(string name, IEnumerable<Robject> values)
		{
			return new Rvariable(name, values);
		}
	}
}

