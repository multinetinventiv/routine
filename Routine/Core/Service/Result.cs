using System.Collections.Generic;

namespace Routine.Core.Service
{
	public class ResultModel
	{
		public string ViewModelId{get;set;}
		public bool IsList{get;set;}
		public bool IsVoid{get;set;}

		#region ToString & Equality

		public override string ToString()
		{
			return string.Format("[ResultModel: ViewModelId={0}, IsList={1}, IsVoid={2}]", ViewModelId, IsList, IsVoid);
		}

		public override bool Equals(object obj)
		{
			if(obj == null)
				return false;
			if(ReferenceEquals(this, obj))
				return true;
			if(obj.GetType() != typeof(ResultModel))
				return false;
			ResultModel other = (ResultModel)obj;
			return ViewModelId == other.ViewModelId && IsList == other.IsList && IsVoid == other.IsVoid;
		}
		
		public override int GetHashCode()
		{
			unchecked
			{
				return (ViewModelId != null ?ViewModelId.GetHashCode():0) ^ IsList.GetHashCode() ^ IsVoid.GetHashCode();
			}
		}
		
		#endregion
	}

	public class ResultData
	{
		public ValueData Value { get; set; }

		public ResultData() { Value = new ValueData(); }

		#region ToString & Equality

		public override string ToString()
		{
			return string.Format("[ResultData: Value={0}]", Value);
		}

		public override bool Equals(object obj)
		{
			if(obj == null)
				return false;
			if(ReferenceEquals(this, obj))
				return true;
			if(obj.GetType() != typeof(ResultData))
				return false;
			ResultData other = (ResultData)obj;
			return object.Equals(Value, other.Value);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Value != null ?Value.GetHashCode():0);
			}
		}
		
		#endregion
	}
}
