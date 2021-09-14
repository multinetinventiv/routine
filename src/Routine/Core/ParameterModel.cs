using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Routine.Core
{
	public class ParameterModel
	{
		public List<string> Marks { get; set; }
		public List<int> Groups { get; set; }

		public string Name { get; set; }
		public string ViewModelId { get; set; }
		public bool IsList { get; set; }

		public ParameterModel()
			: this(new Dictionary<string, object>
			{
				{"Marks", new List<string>()},
				{"Groups", new List<int>()},

				{"Name", null},
				{"ViewModelId", null},
				{"IsList", false}
			}) { }
		public ParameterModel(IDictionary<string, object> model)
		{
			Marks = ((IEnumerable)model["Marks"]).Cast<string>().ToList();
			Groups = ((IEnumerable)model["Groups"]).Cast<int>().ToList();

			Name = (string)model["Name"];
			ViewModelId = (string)model["ViewModelId"];
			IsList = (bool)model["IsList"];
		}

		#region ToString & Equality

		public override string ToString()
		{
			return
                $"[ParameterModel: [Marks: {Marks.ToItemString()}, Groups: {Groups.ToItemString()}, Name: {Name}, ViewModelId: {ViewModelId}, IsList: {IsList}]]";
		}

		protected bool Equals(ParameterModel other)
		{
			return Marks.ItemEquals(other.Marks) && Groups.ItemEquals(other.Groups) && string.Equals(Name, other.Name) && string.Equals(ViewModelId, other.ViewModelId) && IsList == other.IsList;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;

			return Equals((ParameterModel)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (Marks != null ? Marks.GetItemHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Groups != null ? Groups.GetItemHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (ViewModelId != null ? ViewModelId.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ IsList.GetHashCode();
				return hashCode;
			}
		}

		#endregion
	}
}