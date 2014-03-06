namespace Routine.Core.Service
{
	public class MemberModel
	{
		public string Id{get;set;}

		public string ViewModelId{get;set;}
		public bool IsList{get;set;}
		public bool IsHeavy{get;set;}

		#region ToString & Equality

		public override string ToString()
		{
			return string.Format("[MemberModel: Id={0}, ViewModelId={1}, IsList={2}, IsHeavy={3}]", Id, ViewModelId, IsList, IsHeavy);
		}

		public override bool Equals(object obj)
		{
			if(obj == null)
				return false;
			if(ReferenceEquals(this, obj))
				return true;
			if(obj.GetType() != typeof(MemberModel))
				return false;
			MemberModel other = (MemberModel)obj;
			return Id == other.Id && ViewModelId == other.ViewModelId && IsList == other.IsList && IsHeavy == other.IsHeavy;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Id != null ?Id.GetHashCode():0) ^ (ViewModelId != null ?ViewModelId.GetHashCode():0) ^ IsList.GetHashCode() ^ IsHeavy.GetHashCode();
			}
		}

		#endregion
	}

	public class MemberData
	{
		public string ModelId {get; set;}

		public ValueData Value { get; set; }

		public MemberData() { Value = new ValueData(); }

		#region ToString & Equality

		public override string ToString()
		{
			return string.Format("[MemberData: ModelId={0}, Value={1}]", ModelId, Value);
		}

		public override bool Equals(object obj)
		{
			if(obj == null)
				return false;
			if(ReferenceEquals(this, obj))
				return true;
			if(obj.GetType() != typeof(MemberData))
				return false;
			MemberData other = (MemberData)obj;
			return ModelId == other.ModelId && object.Equals(Value, other.Value);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (ModelId != null ?ModelId.GetHashCode():0) ^ (Value != null ?Value.GetHashCode():0);
			}
		}
		
		#endregion
	}
}
