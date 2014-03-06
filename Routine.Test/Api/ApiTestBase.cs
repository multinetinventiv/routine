using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Moq;
using Moq.Language.Flow;
using NUnit.Framework;
using Routine.Api;
using Routine.Core.Service;

namespace Routine.Test.Api
{
	public abstract class ApiTestBase
	{
		protected Mock<IObjectService> objectServiceMock;
		private Mock<IFactory> factoryMock;
		private Dictionary<string, ObjectModel> objectModelDictionary;
		private Dictionary<ObjectReferenceData, ObjectData> objectDictionary;

		protected Rapplication testingRapplication;

		protected virtual string DefaultObjectModelId{get{return "DefaultModel";}}
		protected IFactory Factory {get{return factoryMock.Object;}}
		private IObjectService ObjectService{get{return objectServiceMock.Object;}}

		[SetUp]
		public virtual void SetUp()
		{
			objectServiceMock = new Mock<IObjectService>();
			factoryMock = new Mock<IFactory>();
			objectModelDictionary = new Dictionary<string, ObjectModel>();
			objectDictionary = new Dictionary<ObjectReferenceData, ObjectData>();

			testingRapplication = new Rapplication(objectServiceMock.Object, factoryMock.Object);

			factoryMock.Setup(o => o.Create<Rapplication>()).Returns(testingRapplication);
			factoryMock.Setup(o => o.Create<Robject>()).Returns(() => new Robject(ObjectService, Factory));
			factoryMock.Setup(o => o.Create<Rmember>()).Returns(() => new Rmember(ObjectService, Factory));
			factoryMock.Setup(o => o.Create<Roperation>()).Returns(() => new Roperation(ObjectService, Factory));
			factoryMock.Setup(o => o.Create<Rparameter>()).Returns(() => new Rparameter(Factory));
			factoryMock.Setup(o => o.Create<Rvariable>()).Returns(() => new Rvariable(Factory));

			objectServiceMock.Setup(o => o.GetApplicationModel())
				.Returns(() => new ApplicationModel{Models = objectModelDictionary.Select(o => o.Value).ToList()});
			objectServiceMock.Setup(o => o.GetObjectModel(It.IsAny<string>()))
				.Returns((string omid) => objectModelDictionary[omid]);
			objectServiceMock.Setup(o => o.Get(It.IsAny<ObjectReferenceData>()))
				.Returns((ObjectReferenceData ord) => FilterHeavy(objectDictionary[ord]));
			objectServiceMock.Setup(o => o.GetValue(It.IsAny<ObjectReferenceData>()))
				.Returns((ObjectReferenceData ord) => objectDictionary[ord].Value);
			objectServiceMock.Setup(o => o.GetMember(It.IsAny<ObjectReferenceData>(), It.IsAny<string>()))
				.Returns((ObjectReferenceData ord, string mid) => objectDictionary[ord].Members.Single(m => m.ModelId == mid));
			objectServiceMock.Setup(o => o.GetOperation(It.IsAny<ObjectReferenceData>(), It.IsAny<string>()))
				.Returns((ObjectReferenceData ord, string oid) => objectDictionary[ord].Operations.Single(o => o.ModelId == oid));
			objectServiceMock.Setup(o => o.PerformOperation(It.IsAny<ObjectReferenceData>(), It.IsAny<string>(), It.IsAny<List<ParameterValueData>>()))
				.Returns(Void());
		}

		[TearDown]
		public virtual void TearDown() {}

		private ObjectData FilterHeavy(ObjectData full)
		{
			var result = new ObjectData { Reference = full.Reference };
			result.Value = full.Value;
			result.Members.AddRange(
				full.Members
				.Where(m => objectModelDictionary[full.Reference.ViewModelId]
					.Members.SingleOrDefault(mm => !mm.IsHeavy && mm.Id == m.ModelId) != null));
			result.Operations.AddRange(
				full.Operations
				.Where(o => objectModelDictionary[full.Reference.ViewModelId]
					.Operations.SingleOrDefault(om => !om.IsHeavy && om.Id == o.ModelId) != null));

			return result;
		}

		#region Model Builders
		protected void ModelsAre(params ObjectModelBuilder[] objectModels)
		{
			foreach(var objectModel in objectModels.Select(o => o.Build(objectServiceMock)))
			{
				try
				{
					objectModelDictionary.Add(objectModel.Id, objectModel);
				}
				catch(ArgumentException ex)
				{
					throw new Exception(objectModel.Id + " was already registered", ex);
				}
			}
		}

		protected ObjectModelBuilder Model() {return Model(DefaultObjectModelId);}
		protected ObjectModelBuilder Model(string id) { return new ObjectModelBuilder(DefaultObjectModelId).Id(id); }
		protected class ObjectModelBuilder
		{
			private readonly string defaultObjectModelId;

			private readonly ObjectModel result;
			private readonly List<string> availableIds;

			public ObjectModelBuilder(string defaultObjectModelId) 
			{
				this.defaultObjectModelId = defaultObjectModelId;
				result = new ObjectModel();
				availableIds = new List<string>();
			}

			public ObjectModelBuilder Id(string id) {result.Id = id; return this;}
			public ObjectModelBuilder IsValue() {result.IsValueModel = true; return this;}
			public ObjectModelBuilder IsView() {result.IsViewModel = true; return this;}
			public ObjectModelBuilder Name(string name){result.Name = name; return this;}
			public ObjectModelBuilder Module(string module){result.Module = module; return this;}
			public ObjectModelBuilder Member(string memberId) { return Member(memberId, defaultObjectModelId); }
			public ObjectModelBuilder Member(string memberId, bool isHeavy) {return Member(memberId, defaultObjectModelId, isHeavy);}
			public ObjectModelBuilder Member(string memberId, string viewModelId) {return Member(memberId, viewModelId, false);}
			public ObjectModelBuilder Member(string memberId, string viewModelId, bool isHeavy) {return Member(memberId, viewModelId, isHeavy, false);}
			public ObjectModelBuilder Member(string memberId, string viewModelId, bool isHeavy, bool isList)
			{
				result.Members.Add(new MemberModel{
					Id = memberId,
					ViewModelId = viewModelId,
					IsHeavy = isHeavy,
					IsList = isList,
				}); 
				
				return this; 
			}

			public ObjectModelBuilder Operation(string operationId, params ParameterModel[] parameters) {return Operation(operationId, false, parameters);}
			public ObjectModelBuilder Operation(string operationId, bool isHeavy, params ParameterModel[] parameters) {return Operation(operationId, isHeavy, false, parameters);}
			public ObjectModelBuilder Operation(string operationId, bool isHeavy, bool isVoid, params ParameterModel[] parameters){return Operation(operationId, isHeavy, isVoid?null:defaultObjectModelId, parameters);}
			public ObjectModelBuilder Operation(string operationId, string resultViewModelId, params ParameterModel[] parameters){return Operation(operationId, false, resultViewModelId, false, parameters);}
			public ObjectModelBuilder Operation(string operationId, string resultViewModelId, bool isList, params ParameterModel[] parameters){return Operation(operationId, false, resultViewModelId, isList, parameters);}
			public ObjectModelBuilder Operation(string operationId, bool isHeavy, string resultViewModelId, params ParameterModel[] parameters){return Operation(operationId, isHeavy, resultViewModelId, false, parameters);}
			public ObjectModelBuilder Operation(string operationId, bool isHeavy, string resultViewModelId, bool isList, params ParameterModel[] parameters)
			{
				var operationModel = new OperationModel {
					Id = operationId,
					Parameters = parameters.ToList(),
					IsHeavy = isHeavy,
				};
				operationModel.Result.IsVoid = resultViewModelId == null;
				operationModel.Result.ViewModelId = resultViewModelId;
				operationModel.Result.IsList = isList;

				result.Operations.Add(operationModel);

				return this;
			}

			public ObjectModelBuilder AvailableIds(params string[] ids)
			{
				availableIds.AddRange(ids);

				return this;
			}


			public ObjectModel Build(Mock<IObjectService> objectServiceMock)
			{
				objectServiceMock
					.Setup(o => o.GetAvailableObjects(result.Id))
					.Returns((string omid) => availableIds.Select(id => {
						var ord = new ObjectReferenceData {
							ActualModelId = omid,
							ViewModelId = omid,
							Id = id,
						};
						return new SingleValueData {
							Reference = ord,
							Value = objectServiceMock.Object.GetValue(ord)
						};
					}).ToList());


				return result; 
			}
		}

		protected ParameterModel PModel(string id) {return PModel(id, false);}
		protected ParameterModel PModel(string id, string viewModelId) {return PModel(id, viewModelId, false);}
		protected ParameterModel PModel(string id, bool isList) {return PModel(id, DefaultObjectModelId, isList);}
		protected ParameterModel PModel(string id, string viewModelId, bool isList)
		{
			return new ParameterModel {
				Id = id,
				IsList = isList,
				ViewModelId = viewModelId
			};
		}

		#endregion

		#region Data Builders

		protected void EnsureModels(params string[] modelIds)
		{
			foreach(var modelId in modelIds)
			{
				if(!objectModelDictionary.ContainsKey(modelId))
				{
					ModelsAre(Model(modelId).Name(modelId));
				}
			}
		}

		protected ObjectReferenceData Null() {return Null(DefaultObjectModelId);}
		protected ObjectReferenceData Null(string modelId) {return Id("null", modelId, modelId, true);}
		protected ObjectReferenceData Id(string id){return Id(id, DefaultObjectModelId);}
		protected ObjectReferenceData Id(string id, string modelId) {return Id(id, modelId, modelId);}
		protected ObjectReferenceData Id(string id, string actualModelId, string viewModelId) {return Id(id, actualModelId, viewModelId, false);}
		protected ObjectReferenceData Id(string id, string actualModelId, string viewModelId, bool isNull)
		{
			EnsureModels(actualModelId, viewModelId);

			return new ObjectReferenceData {
				Id = id,
				ActualModelId = actualModelId,
				ViewModelId = viewModelId,
				IsNull = isNull,
			};
		}

		protected void ObjectsAre(params ObjectBuilder[] objects)
		{
			foreach(var @object in objects.Select(o => o.Build()))
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
				if(!objectModels.ContainsKey(reference.ViewModelId))
				{
					return this;
				}

				var model = objectModels[reference.ViewModelId];

				foreach(var memberModel in model.Members)
				{
					result.Members.Add(new MemberData {
						ModelId = memberModel.Id, 
						Value = new ValueData {
							IsList = memberModel.IsList,
						}
					});
				}

				foreach(var operationModel in model.Operations)
				{
					result.Operations.Add(new OperationData {
						ModelId = operationModel.Id,
						Parameters = operationModel.Parameters.Select(p => new ParameterData {
							ModelId = p.Id
						}).ToList()
					});
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
				result.Members
					.Single(m => m.ModelId == memberModelId)
					.Value.Values.AddRange(memberData.Select(m => new SingleValueData{Reference = m}));

				return this;
			}

			public ObjectBuilder Operation(string operationModelId, params ParameterData[] parameters) {return Operation(operationModelId, true, parameters);}
			public ObjectBuilder Operation(string operationModelId, bool isAvailable, params ParameterData[] parameters) { 
				var operation = result.Operations.Single(o => o.ModelId == operationModelId);
				operation.IsAvailable = isAvailable;

				return this;
			}

			public ObjectData Build() 
			{
				foreach(var mem in result.Members)
				{
					foreach(var val in mem.Value.Values)
					{
						if(val.Reference.IsNull) {continue;}
						if(objectModels.ContainsKey(val.Reference.ActualModelId) && objectModels[val.Reference.ActualModelId].IsValueModel)
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

		protected ParameterData PData(string parameterModelId)
		{
			return new ParameterData {
				ModelId = parameterModelId,

			};
		}	

		private string ValueOf(ObjectReferenceData ord)
		{
			if(ord.IsNull) {return "";}
			if(objectModelDictionary.ContainsKey(ord.ActualModelId) && objectModelDictionary[ord.ActualModelId].IsValueModel)
			{
				return ord.Id;
			}

			return objectDictionary[ord].Value;
		}

		protected ResultData Result(params ObjectReferenceData[] ords)
		{
			var result = new ResultData();

			result.Value.Values.AddRange(
				ords.Select(ord => new SingleValueData{
					Reference = ord, 
					Value = ValueOf(ord)}).ToList());

			return result;
		}

		protected ResultData Void()
		{
			return new ResultData();
		}

		#endregion

		#region Stubbers

		protected ObjectStubber When(ObjectReferenceData objectReferenceData)
		{
			return new ObjectStubber(this, objectReferenceData);
		}

		protected class ObjectStubber
		{
			private readonly ApiTestBase test;
			private readonly ObjectReferenceData objectReferenceData;

			public ObjectStubber(ApiTestBase test, ObjectReferenceData objectReferenceData)
			{
				this.test = test;
				this.objectReferenceData = objectReferenceData;
			}

			public ISetup<IObjectService, ResultData> Performs(string operationModelId) { return Performs(operationModelId, (p => true)); }
			public ISetup<IObjectService, ResultData> Performs(string operationModelId, Expression<Func<List<ParameterValueData>, bool>> parameterMatcher)
			{
				return test.objectServiceMock
						.Setup(o => o.PerformOperation(
							objectReferenceData, 
							operationModelId, 
							It.Is<List<ParameterValueData>>(parameterMatcher)));
			}
		}

		#endregion

		protected Robject RobjNull(){return Factory.Create<Robject>().Null();}
		protected Robject Robj(string id) { return Robj(id, DefaultObjectModelId);}
		protected Robject Robj(string id, string modelId) {return Robj(id, modelId, modelId);}
		protected Robject Robj(string id, string actualModelId, string viewModelId)
		{
			return testingRapplication.Get(id, actualModelId, viewModelId);
		}

		protected Rvariable Rvar(string name, Robject value)
		{
			return Factory.Create<Rvariable>().WithSingle(name, value);
		}

		protected Rvariable Rvarlist(string name, params Robject[] value)
		{
			return Factory.Create<Rvariable>().WithList(name, value);
		}
	}
}

