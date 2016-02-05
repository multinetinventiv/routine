using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Routine.Core
{
	public class ApplicationModel
	{
		internal Dictionary<string, ObjectModel> Model { get; private set; }

		public ApplicationModel()
			: this(new Dictionary<string, object>
			{
				{"Models", new List<Dictionary<string, object>>()}
			}) { }
		public ApplicationModel(IDictionary<string, object> model)
		{
			Models = ((IEnumerable)model["Models"]).Cast<IDictionary<string, object>>().Select(o => new ObjectModel(o)).ToList();
		}

		public List<ObjectModel> Models
		{
			get { return Model.Values.ToList(); }
			set { Model = value.ToDictionary(om => om.Id, om => om); }
		}

		#region ToString & Equality

		public override string ToString()
		{
			return string.Format("[ApplicationModel: [Models: {0}]]", Models.ToItemString());
		}

		protected bool Equals(ApplicationModel other)
		{
			return Models.ItemEquals(other.Models);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;

			return Equals((ApplicationModel)obj);
		}

		public override int GetHashCode()
		{
			return (Models != null ? Models.GetItemHashCode() : 0);
		}

		#endregion
	}
}