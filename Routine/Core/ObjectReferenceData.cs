namespace Routine.Core
{
	public class ObjectReferenceData
	{
		public bool IsNull { get; set; }
		public string ActualModelId { get; set; }
		public string Id { get; set; }
		public string ViewModelId { get; set; }

		#region ToString & Equality

		public override string ToString()
		{
			return string.Format("[ObjectReferenceData: ActualModelId={0}, ViewModelId={1}, Id={2}, IsNull={3}]", ActualModelId, ViewModelId, Id, IsNull);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(ObjectReferenceData))
				return false;
			ObjectReferenceData other = (ObjectReferenceData)obj;
			return (IsNull && other.IsNull) ||
			       (!IsNull && !other.IsNull && ActualModelId == other.ActualModelId && ViewModelId == other.ViewModelId && Id == other.Id);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				if (IsNull) { return IsNull.GetHashCode(); }

				return (ActualModelId != null ? ActualModelId.GetHashCode() : 0) ^ (ViewModelId != null ? ViewModelId.GetHashCode() : 0) ^ (Id != null ? Id.GetHashCode() : 0);
			}
		}

		#endregion
	}
}