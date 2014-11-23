using System.Collections.Generic;

namespace Routine.Core
{
	public class ParameterModel
	{
		public string Id { get; set; }
		public List<string> Marks { get; set; }
		public List<int> Groups { get; set; }

		public string ViewModelId { get; set; }
		public bool IsList { get; set; }

		public ParameterModel() 
		{ 
			Marks = new List<string>();
			Groups = new List<int>();
		}

		#region ToString & Equality

		public override string ToString()
		{
			return string.Format("[ParameterModel: Id={0}, Marks={1}, Groups={2}, ViewModelId={3}, IsList={4}]", Id, Marks.ToItemString(), Groups.ToItemString(), ViewModelId, IsList);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(ParameterModel))
				return false;
			ParameterModel other = (ParameterModel)obj;
			return Id == other.Id && Marks.ItemEquals(other.Marks) && Groups.ItemEquals(other.Groups) && ViewModelId == other.ViewModelId && IsList == other.IsList;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Id != null ? Id.GetHashCode() : 0) ^ (Marks != null ? Marks.GetItemHashCode() : 0) ^ (Groups != null ? Groups.GetItemHashCode() : 0) ^ (ViewModelId != null ? ViewModelId.GetHashCode() : 0) ^ IsList.GetHashCode();
			}
		}

		#endregion
	}
}