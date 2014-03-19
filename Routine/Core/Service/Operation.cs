using System.Collections.Generic;

namespace Routine.Core.Service
{
	public class OperationModel
	{
		public string Id { get; set; }
		public List<string> Marks { get; set; }

		public bool IsHeavy{get;set;}

		public List<ParameterModel> Parameters{get;set;}
		public ResultModel Result{get;set;}

		public OperationModel() 
		{
			Marks = new List<string>();
			Parameters = new List<ParameterModel>(); 
			Result = new ResultModel();
		}

		#region ToString & Equality
		public override string ToString()
		{
			return string.Format("[OperationModel: Id={0}, Marks={1}, IsHeavy={2}, Parameters={3}, Result={4}]", Id, Marks.ToItemString(), IsHeavy, Parameters.ToItemString(), Result);
		}

		public override bool Equals(object obj)
		{
			if(obj == null)
				return false;
			if(ReferenceEquals(this, obj))
				return true;
			if(obj.GetType() != typeof(OperationModel))
				return false;
			OperationModel other = (OperationModel)obj;
			return Id == other.Id && Marks.ItemEquals(other.Marks) && IsHeavy == other.IsHeavy && Parameters.ItemEquals(other.Parameters) && object.Equals(Result, other.Result);
		}
		
		public override int GetHashCode()
		{
			unchecked
			{
				return (Id != null ? Id.GetHashCode() : 0) ^ (Marks != null ? Marks.GetItemHashCode() : 0) ^ IsHeavy.GetHashCode() ^ (Parameters != null ? Parameters.GetItemHashCode() : 0) ^ (Result != null ? Result.GetHashCode() : 0);
			}
		}
		
		#endregion
	}

	public class OperationData
	{
		public string ModelId {get;set;}

		public bool IsAvailable {get;set;}
		public List<ParameterData> Parameters{get;set;}

		public OperationData() { Parameters = new List<ParameterData>(); }

		#region ToString & Equality

		public override string ToString()
		{
			return string.Format("[OperationData: ModelId={0}, Parameters={1}]", ModelId, Parameters.ToItemString());
		}

		public override bool Equals(object obj)
		{
			if(obj == null)
				return false;
			if(ReferenceEquals(this, obj))
				return true;
			if(obj.GetType() != typeof(OperationData))
				return false;
			OperationData other = (OperationData)obj;
			return ModelId == other.ModelId && Parameters.ItemEquals(other.Parameters);
		}
		
		public override int GetHashCode()
		{
			unchecked
			{
				return (ModelId != null ?ModelId.GetHashCode():0) ^ (Parameters != null ?Parameters.GetItemHashCode():0);
			}
		}
				
		#endregion
	}
}
