using System.Collections.Generic;
using System.Linq;

namespace Routine.Core.Api
{
	public class Robject
	{
		#region Data Adapter Methods

		private static ObjectReferenceData ORD(string id, string amid, string vmid) { return new ObjectReferenceData { Id = id, ActualModelId = amid, ViewModelId = vmid }; }

		private static ObjectData OD(ObjectReferenceData ord) { return OD(ord, null); }
		private static ObjectData OD(ObjectReferenceData ord, string value) { return new ObjectData { Reference = ord, Value = value }; }

		#endregion

		private readonly IApiContext context;

        public Robject(IApiContext context)
		{
            this.context = context;
		}
		
		private ObjectReferenceData objectReferenceData;
		private string value;
		private ObjectModel model;
		private List<Rvariable> initializationParameters;
		private Dictionary<string, Rmember> members;
		private Dictionary<string, Roperation> operations;

		public Robject Null(){return With(new ObjectReferenceData{IsNull = true});}

		public Robject With(string modelId, IEnumerable<Rvariable> initializationParameters) { return With(OD(ORD(null, modelId, modelId)), initializationParameters.ToList());}
		public Robject With(string id, string modelId) { return With(id, modelId, modelId); }
		public Robject With(string id, string actualModelId, string viewModelId) { return With(ORD(id, actualModelId, viewModelId)); }
		internal Robject With(ObjectReferenceData objectReferenceData) { return With(OD(objectReferenceData)); }
		internal Robject With(ObjectReferenceData objectReferenceData, string value) { return With(OD(objectReferenceData, value)); }
		internal Robject With(ObjectData objectData) { return With(objectData, new List<Rvariable>()); }
		private Robject With(ObjectData objectData, List<Rvariable> initializationParameters)
		{
			this.objectReferenceData = objectData.Reference;
			this.initializationParameters = initializationParameters;
			this.members = new Dictionary<string, Rmember>();
			this.operations = new Dictionary<string, Roperation>();

			if(!IsNull)
			{
				this.model = Application.ObjectModel[objectReferenceData.ViewModelId];

				FillObject(objectData);
			}
			
			return this;
		}

		private void FillObject(ObjectData data)
		{
			value = data.Value;

			if (data.Members.Count <= 0) { return; }

			LoadMembersAndOperationsIfNecessary();
			foreach (var memberModelId in data.Members.Keys)
			{
				members[memberModelId].SetData(data.Members[memberModelId]);
			}
		}

		private void LoadMembersAndOperationsIfNecessary()
		{
			if (ModelIsLoaded) { return; }
			if (IsNull) { return; }
			if (!IsDomain) { return; }

			foreach (var member in model.Members)
			{
				members.Add(member.Id, context.CreateRmember().With(this, member));
			}

			foreach (var operation in model.Operations)
			{
				operations.Add(operation.Id, context.CreateRoperation().With(this, operation));
			}
		}

		private void FetchValueIfNecessary()
		{
			if(value != null) {return;}

			if(IsNull) 
			{
				value = "";
				return;
			}

			if(model.IsValueModel)
			{
				value = objectReferenceData.Id;
				return;
			}

			if (IsInitializedOnClient)
			{
				throw new RobjectIsInitializedOnClientException();
			}

			value = context.ObjectService.GetValue(objectReferenceData);
		}

		internal void LoadObject() 
		{
			if (IsNull) { return; }
			if (!IsDomain) { return; }

			FillObject(context.ObjectService.Get(objectReferenceData));
		}

		private bool ModelIsLoaded { get { return members.Any() || operations.Any(); } }
		internal ObjectReferenceData ObjectReferenceData { get { return objectReferenceData; } }
		
		internal ParameterData GetParameterData()
		{ 
			var result = new ParameterData  {
				IsNull = IsNull,
				ObjectModelId = objectReferenceData.ActualModelId,
				ReferenceId = objectReferenceData.Id
			};

			foreach (var initializationParameter in initializationParameters)
			{
				result.InitializationParameters.Add(initializationParameter.Name, initializationParameter.GetParameterValueData());
			}

			return result;
		}

		public Rapplication Application { get { return context.Rapplication; } }
		public string ActualModelId { get { return objectReferenceData.ActualModelId; } }
		public string ViewModelId { get { return objectReferenceData.ViewModelId; } }
		public string Id { get { return objectReferenceData.Id; } }
		public bool IsNull { get { return objectReferenceData.IsNull; } }
		public bool IsNaked { get { return ActualModelId == ViewModelId; } }
		public bool IsInitializedOnClient { get { return string.IsNullOrEmpty(objectReferenceData.Id); } }
		public bool IsDomain { get { return !model.IsValueModel; } }
		public string Module { get { return model.Module; } }

		public string Value { get { FetchValueIfNecessary(); return value; } }
		public List<Rmember> Members { get { LoadMembersAndOperationsIfNecessary(); return members.Values.ToList(); } }
		public Rmember this[string memberModelId] { get { LoadMembersAndOperationsIfNecessary(); return members[memberModelId]; } }
		public List<Roperation> Operations { get { LoadMembersAndOperationsIfNecessary(); return operations.Values.ToList(); } }

		public bool MarkedAs(string mark)
		{
			if (IsNull) { return false; }

			return model.Marks.Any(m => m == mark);
		}

		public Rvariable Perform(string operationModelId, params Rvariable[] parameters) { return Perform(operationModelId, parameters.ToList()); }
		public Rvariable Perform(string operationModelId, List<Rvariable> parameters)
		{
			if(IsNull){return context.CreateRvariable().Null();}

			Roperation operation;
			if(ModelIsLoaded)
			{
				operation = operations[operationModelId];
			}
			else
			{
				operation = context.CreateRoperation()
					.With(this, model.Operations.Single(o => o.Id == operationModelId));
			}

			return operation.Perform(parameters);
		}

		public void Invalidate()
		{
			value = null;
			foreach(var member in members.Values)
			{
				member.Invalidate();
			}
		}

		public override bool Equals(object obj)
		{
			if(obj == null)
				return false;
			if(ReferenceEquals(this, obj))
				return true;
			if(obj.GetType() != typeof(Robject))
				return false;

			var robj = (Robject)obj;
			if (IsInitializedOnClient || robj.IsInitializedOnClient)
			{
				return false;
			}

			return objectReferenceData.Equals(robj.objectReferenceData);
		}

		public override int GetHashCode()
		{
			return objectReferenceData.GetHashCode();
		}
	}
}
