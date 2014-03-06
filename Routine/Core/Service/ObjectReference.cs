using System.Collections.Generic;

namespace Routine.Core.Service
{
	public class ReferenceData
	{
		public bool IsList{get;set;}
		public List<ObjectReferenceData> References{get;set;}

		public ReferenceData(){References = new List<ObjectReferenceData>();}

		#region ToString & Equality

		public override string ToString()
		{
			return string.Format("[ReferenceData: IsList={0}, References={1}]", IsList, References.ToItemString());
		}

		public override bool Equals(object obj)
		{
			if(obj == null)
				return false;
			if(ReferenceEquals(this, obj))
				return true;
			if(obj.GetType() != typeof(ReferenceData))
				return false;
			ReferenceData other = (ReferenceData)obj;
			return IsList == other.IsList && References.ItemEquals(other.References);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return IsList.GetHashCode() ^ (References != null ?References.GetItemHashCode():0);
			}
		}

		#endregion
	}
	
	public class ObjectReferenceData
	{
		public string ActualModelId{get;set;}
		public string ViewModelId{get;set;}

		public string Id {get;set;}
		public bool IsNull{get;set;}

		#region ToString & Equality

		public override string ToString()
		{
			return string.Format("[ObjectReferenceData: ActualModelId={0}, ViewModelId={1}, Id={2}, IsNull={3}]", ActualModelId, ViewModelId, Id, IsNull);
		}

		public override bool Equals(object obj)
		{
			if(obj == null)
				return false;
			if(ReferenceEquals(this, obj))
				return true;
			if(obj.GetType() != typeof(ObjectReferenceData))
				return false;
			ObjectReferenceData other = (ObjectReferenceData)obj;
			return 	(IsNull && other.IsNull) || 
					(!IsNull && !other.IsNull && ActualModelId == other.ActualModelId && ViewModelId == other.ViewModelId && Id == other.Id);
		}
		

		public override int GetHashCode()
		{
			unchecked
			{
				if(IsNull) { return IsNull.GetHashCode(); }

				return (ActualModelId != null ?ActualModelId.GetHashCode():0) ^ (ViewModelId != null ?ViewModelId.GetHashCode():0) ^ (Id != null ?Id.GetHashCode():0);
			}
		}

		#endregion
	}

}
