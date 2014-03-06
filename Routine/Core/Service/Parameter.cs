namespace Routine.Core.Service
{
	public class ParameterModel
	{
		public string Id{get;set;}

		public string ViewModelId{get;set;}
		public bool IsList{get;set;}

		#region ToString & Equality

		public override string ToString()
		{
			return string.Format("[ParameterModel: Id={0}, ViewModelId={1}, IsList={2}]", Id, ViewModelId, IsList);
		}

		public override bool Equals(object obj)
		{
			if(obj == null)
				return false;
			if(ReferenceEquals(this, obj))
				return true;
			if(obj.GetType() != typeof(ParameterModel))
				return false;
			ParameterModel other = (ParameterModel)obj;
			return Id == other.Id && ViewModelId == other.ViewModelId && IsList == other.IsList;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Id != null ?Id.GetHashCode():0) ^ (ViewModelId != null ?ViewModelId.GetHashCode():0) ^ IsList.GetHashCode();
			}
		}
		
		#endregion
	}

	public class ParameterData
	{
		public string ModelId {get;set;}

		#region ToString & Equality

		public override string ToString()
		{
			return string.Format("[ParameterData: ModelId={0}]", ModelId);
		}

		public override bool Equals(object obj)
		{
			if(obj == null)
				return false;
			if(ReferenceEquals(this, obj))
				return true;
			if(obj.GetType() != typeof(ParameterData))
				return false;
			ParameterData other = (ParameterData)obj;
			return ModelId == other.ModelId;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (ModelId != null ?ModelId.GetHashCode():0);
			}
		}
		
		#endregion
	}

	public class ParameterValueData
	{
		public string ParameterModelId {get;set;}
		public ReferenceData Value{get;set;}

		public ParameterValueData() { Value = new ReferenceData(); }

		#region ToString & Equality

		public override string ToString()
		{
			return string.Format("[ParameterValueData: ParameterModelId={0}, Value={1}]", ParameterModelId, Value);
		}

		public override bool Equals(object obj)
		{
			if(obj == null)
				return false;
			if(ReferenceEquals(this, obj))
				return true;
			if(obj.GetType() != typeof(ParameterValueData))
				return false;
			ParameterValueData other = (ParameterValueData)obj;
			return ParameterModelId == other.ParameterModelId && object.Equals(Value, other.Value);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (ParameterModelId != null ?ParameterModelId.GetHashCode():0) ^ (Value != null ?Value.GetHashCode():0);
			}
		}

		#endregion
	}	
}
