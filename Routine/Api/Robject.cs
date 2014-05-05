using System.Collections.Generic;
using System.Linq;
using Routine.Core;

namespace Routine.Api
{
	public class Robject
	{
		private readonly IApiContext context;

        public Robject(IApiContext context)
		{
            this.context = context;
		}

		private ObjectReferenceData objectReferenceData;
		private string value;
		private ObjectModel model;
		private Dictionary<string, Rmember> members;
		private Dictionary<string, Roperation> operations;

		public Robject Null(){return With(new ObjectReferenceData{IsNull = true});}
		public Robject With(string id, string modelId) {return With(id, modelId, modelId);}
		public Robject With(string id, string actualModelId, string viewModelId) {return With(new ObjectReferenceData{Id = id, ActualModelId = actualModelId, ViewModelId = viewModelId});}
		internal Robject With(ObjectReferenceData objectReferenceData){return With(objectReferenceData, null);}
		internal Robject With(ObjectReferenceData objectReferenceData, string value) { return With(new ObjectData { Reference = objectReferenceData, Value = value }); }
		internal Robject With(ObjectData objectData)
		{
			this.objectReferenceData = objectData.Reference;
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

			value = context.ObjectService.GetValue(objectReferenceData);
		}

		internal void LoadObject() 
		{
			if (IsNull) { return; }
			if (!IsDomain) { return; }

			FillObject(context.ObjectService.Get(objectReferenceData));
		}

		private bool ModelIsLoaded { get { return members.Any() || operations.Any(); } }
		internal ObjectReferenceData ObjectReferenceData {get{return objectReferenceData;}}

		public Rapplication Application{get{return context.Rapplication;}}
		public string ActualModelId{get{return objectReferenceData.ActualModelId;}}
		public string ViewModelId{get{return objectReferenceData.ViewModelId;}}
		public string Id {get{return objectReferenceData.Id;}}
		public bool IsNull {get{return objectReferenceData.IsNull;}}
		public bool IsNaked {get{return ActualModelId == ViewModelId;}}
		public bool IsDomain {get{return !model.IsValueModel;}}
		public string Module{get{return model.Module;}}

		public string Value {get{FetchValueIfNecessary(); return value;}}
		public List<Rmember> Members{get{LoadMembersAndOperationsIfNecessary(); return members.Values.ToList();}}
		public Rmember this[string memberModelId] { get { LoadMembersAndOperationsIfNecessary(); return members[memberModelId]; } }
		public List<Roperation> Operations{get{LoadMembersAndOperationsIfNecessary(); return operations.Values.ToList();}}

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
			return objectReferenceData.Equals(robj.objectReferenceData);
		}

		public override int GetHashCode()
		{
			return objectReferenceData.GetHashCode();
		}
	}
}
