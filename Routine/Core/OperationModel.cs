using System.Collections.Generic;

namespace Routine.Core
{
	public class OperationModel
	{
		public string Id { get; set; }
		public List<string> Marks { get; set; }
		public int GroupCount { get; set; }

		public List<ParameterModel> Parameters { get; set; }
		public ResultModel Result { get; set; }

		public OperationModel()
		{
			Marks = new List<string>();

			Parameters = new List<ParameterModel>();
			Result = new ResultModel();
		}

		#region ToString & Equality
		public override string ToString()
		{
			return string.Format("[OperationModel: Id={0}, Marks={1}, GroupCount={2}, Parameters={3}, Result={4}]", Id, Marks.ToItemString(), GroupCount, Parameters.ToItemString(), Result);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(OperationModel))
				return false;
			OperationModel other = (OperationModel)obj;
			return Id == other.Id && Marks.ItemEquals(other.Marks) && GroupCount == other.GroupCount && Parameters.ItemEquals(other.Parameters) && object.Equals(Result, other.Result);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Id != null ? Id.GetHashCode() : 0) ^ (Marks != null ? Marks.GetItemHashCode() : 0) ^ GroupCount.GetHashCode() ^ (Parameters != null ? Parameters.GetItemHashCode() : 0) ^ (Result != null ? Result.GetHashCode() : 0);
			}
		}

		#endregion
	}
}