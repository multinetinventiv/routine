using System.Collections.Generic;

namespace Routine.Core
{
	public class MemberModel
	{
		public string Id { get; set; }
		public List<string> Marks { get; set; }

		public string ViewModelId { get; set; }
		public bool IsList { get; set; }

		public MemberModel() { Marks = new List<string>(); }

		#region ToString & Equality

		public override string ToString()
		{
			return string.Format("[MemberModel: Id={0}, Marks={1}, ViewModelId={2}, IsList={3}]", Id, Marks.ToItemString(), ViewModelId, IsList);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(MemberModel))
				return false;
			var other = (MemberModel)obj;
			return Id == other.Id && Marks.ItemEquals(other.Marks) && ViewModelId == other.ViewModelId && IsList == other.IsList;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Id != null ? Id.GetHashCode() : 0) ^ (Marks != null ? Marks.GetItemHashCode() : 0) ^ (ViewModelId != null ? ViewModelId.GetHashCode() : 0) ^ IsList.GetHashCode();
			}
		}

		#endregion
	}
}