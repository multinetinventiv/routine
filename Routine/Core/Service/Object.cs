using System.Collections.Generic;

namespace Routine.Core.Service
{
	public class ObjectModel
	{
		public string Id{get;set;}
		public string Name{get;set;}
		public string Module{get;set;}
		public bool IsValueModel{get;set;}
		public bool IsViewModel{get;set;}

		public List<MemberModel> Members{get;set;}
		public List<OperationModel> Operations{get;set;}

		public ObjectModel() 
		{
			Members = new List<MemberModel>(); 
			Operations = new List<OperationModel>(); 
		}

		#region ToString & Equality

		public override string ToString()
		{
			return string.Format("[ObjectModel: Id={0}, Name={1}, Module={2}, IsValueModel={3}, IsViewModel={4}, Members={5}, Operations={6}]", Id, Name, Module, IsValueModel, IsViewModel, Members.ToItemString(), Operations.ToItemString());
		}

		public override bool Equals(object obj)
		{
			if(obj == null)
				return false;
			if(ReferenceEquals(this, obj))
				return true;
			if(obj.GetType() != typeof(ObjectModel))
				return false;
			ObjectModel other = (ObjectModel)obj;
			return Id == other.Id && Name == other.Name && Module == other.Module && IsValueModel == other.IsValueModel && IsViewModel == other.IsViewModel && Members.ItemEquals(other.Members) && Operations.ItemEquals(other.Operations);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Id != null ?Id.GetHashCode():0) ^ (Name != null ?Name.GetHashCode():0) ^ (Module != null ?Module.GetHashCode():0) ^ IsValueModel.GetHashCode() ^ IsViewModel.GetHashCode() ^ (Members != null ?Members.GetItemHashCode():0) ^ (Operations != null ?Operations.GetItemHashCode():0);
			}
		}
		
		#endregion
	}

	public class ObjectData
	{
		public ObjectReferenceData Reference {get;set;}
		public string Value {get;set;}
		public List<MemberData> Members {get;set;}
		public List<OperationData> Operations {get;set;}

		public ObjectData()
		{
			Reference = new ObjectReferenceData();
			Members = new List<MemberData>();
			Operations = new List<OperationData>();
		}

		#region ToString & Equality

		public override string ToString()
		{
			return string.Format("[ObjectData: Reference={0}, Value={1}, Members={2}, Operations={3}]", Reference, Value, Members.ToItemString(), Operations.ToItemString());
		}

		public override bool Equals(object obj)
		{
			if(obj == null)
				return false;
			if(ReferenceEquals(this, obj))
				return true;
			if(obj.GetType() != typeof(ObjectData))
				return false;
			ObjectData other = (ObjectData)obj;
			return object.Equals(Reference, other.Reference) && Value == other.Value && Members.ItemEquals(other.Members) && Operations.ItemEquals(other.Operations);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Reference != null ?Reference.GetHashCode():0) ^ (Value != null ?Value.GetHashCode():0) ^ (Members != null ?Members.GetItemHashCode():0) ^ (Operations != null ?Operations.GetItemHashCode():0);
			}
		}

		#endregion
	}
}
