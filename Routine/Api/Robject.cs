using System.Collections.Generic;
using System.Linq;
using Routine.Core.Service;

namespace Routine.Api
{
	public class Robject
	{
		private readonly IObjectService objectService;
		private readonly IFactory factory;

		public Robject(IObjectService objectService, IFactory factory)
		{
			this.objectService = objectService;
			this.factory = factory;
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
		internal Robject With(ObjectReferenceData objectReferenceData, string value)
		{
			this.objectReferenceData = objectReferenceData;
			this.value = value;
			this.members = new Dictionary<string, Rmember>();
			this.operations = new Dictionary<string, Roperation>();

			if(!IsNull)
			{
				this.model = Application.ObjectModel[objectReferenceData.ViewModelId];
			}
			
			return this;
		}

		private bool ModelIsLoaded { get{ return members.Any() || operations.Any(); } }

		private void LoadMembersAndOperationsIfNecessary()
		{
			if(ModelIsLoaded) { return; }
			if(IsNull) { return; } 
			if(!IsDomain) { return; }

			foreach(var member in model.Members)
			{
				members.Add(member.Id, factory.Create<Rmember>().With(this, member));
			}

			foreach(var operation in model.Operations)
			{
				operations.Add(operation.Id, factory.Create<Roperation>().With(this, operation));
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

			value = objectService.GetValue(objectReferenceData);
		}

		internal void LoadObject() 
		{
			if(IsNull) { return; }

			LoadMembersAndOperationsIfNecessary();

			var data = objectService.Get(objectReferenceData);
			value = data.Value;

			foreach(var member in data.Members)
			{
				members[member.ModelId].SetData(member);
			}

			foreach(var operation in data.Operations)
			{
				operations[operation.ModelId].SetData(operation);
			}
		}

		internal ObjectReferenceData ObjectReferenceData {get{return objectReferenceData;}}

		public Rapplication Application{get{return factory.Create<Rapplication>();}}
		public string ActualModelId{get{return objectReferenceData.ActualModelId;}}
		public string ViewModelId{get{return objectReferenceData.ViewModelId;}}
		public string Id {get{return objectReferenceData.Id;}}
		public bool IsNull {get{return objectReferenceData.IsNull;}}
		public bool IsNaked {get{return ActualModelId == ViewModelId;}}
		public bool IsDomain {get{return !model.IsValueModel;}}

		public string Value {get{FetchValueIfNecessary(); return value;}}
		public List<Rmember> Members{get{LoadMembersAndOperationsIfNecessary(); return members.Values.ToList();}}
		public Rmember this[string memberModelId] { get { LoadMembersAndOperationsIfNecessary(); return members[memberModelId]; } }
		public List<Roperation> Operations{get{LoadMembersAndOperationsIfNecessary(); return operations.Values.ToList();}}

		public Rvariable Perform(string operationModelId, params Rvariable[] parameters) { return Perform(operationModelId, parameters.ToList()); }
		public Rvariable Perform(string operationModelId, List<Rvariable> parameters)
		{
			if(IsNull){return factory.Create<Rvariable>().Null();}

			Roperation operation;
			if(ModelIsLoaded)
			{
				operation = operations[operationModelId];
			}
			else
			{
				operation = factory.Create<Roperation>()
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
			foreach(var operation in operations.Values)
			{
				operation.Invalidate();
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
