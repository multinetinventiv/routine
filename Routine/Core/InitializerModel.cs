using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Routine.Core
{
	public class InitializerModel
	{
		public List<string> Marks { get; set; }
		public int GroupCount { get; set; }

		internal Dictionary<string, ParameterModel> Parameter { get; private set; }

		public InitializerModel()
			: this(new Dictionary<string, object>
			{
				{"Marks", new List<string>()},
				{"GroupCount", 0},
				{"Parameters", new List<Dictionary<string, object>>()}
			}) { }
		public InitializerModel(IDictionary<string, object> model)
		{
			Marks = ((IEnumerable)model["Marks"]).Cast<string>().ToList();
			GroupCount = (int)model["GroupCount"];

			Parameters = ((IEnumerable)model["Parameters"]).Cast<IDictionary<string, object>>().Select(p => new ParameterModel(p)).ToList();
		}

		public List<ParameterModel> Parameters
		{
			get { return Parameter.Values.ToList(); }
			set { Parameter = value.ToDictionary(kvp => kvp.Name, kvp => kvp); }
		}

		#region ToString & Equality

		public override string ToString()
		{
			return string.Format("[InitializerModel: [Marks: {0}, GroupCount: {1}, Parameters: {2}]]", Marks.ToItemString(), GroupCount, Parameters.ToItemString());
		}

		protected bool Equals(InitializerModel other)
		{
			return Marks.ItemEquals(other.Marks) && GroupCount == other.GroupCount && Parameters.ItemEquals(other.Parameters);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;

			return Equals((InitializerModel)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (Marks != null ? Marks.GetItemHashCode() : 0);
				hashCode = (hashCode * 397) ^ GroupCount;
				hashCode = (hashCode * 397) ^ (Parameters != null ? Parameters.GetItemHashCode() : 0);
				return hashCode;
			}
		}

		#endregion
	}
}