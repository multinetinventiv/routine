using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Routine.Core
{
	public class DataModel
	{
		public List<string> Marks { get; set; }

		public string Name { get; set; }
		public string ViewModelId { get; set; }
		public bool IsList { get; set; }

		public DataModel()
			: this(new Dictionary<string, object>
			{
				{"Marks", new List<string>()},
				{"Name", null},
				{"ViewModelId", null},
				{"IsList", false}
			}) { }
		public DataModel(IDictionary<string, object> model)
		{
			Marks = ((IEnumerable)model["Marks"]).Cast<string>().ToList();

			Name = (string)model["Name"];
			ViewModelId = (string)model["ViewModelId"];
			IsList = (bool)model["IsList"];
		}

		#region ToString & Equality

		public override string ToString()
		{
			return
                $"[DataModel: [Marks: {Marks.ToItemString()}, Name: {Name}, ViewModelId: {ViewModelId}, IsList: {IsList}]]";
		}

		protected bool Equals(DataModel other)
		{
			return Marks.ItemEquals(other.Marks) && string.Equals(Name, other.Name) && string.Equals(ViewModelId, other.ViewModelId) && IsList == other.IsList;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;

			return Equals((DataModel)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (Marks != null ? Marks.GetItemHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (ViewModelId != null ? ViewModelId.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ IsList.GetHashCode();
				return hashCode;
			}
		}

		#endregion
	}
}