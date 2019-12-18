using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Routine.Core
{
	public class OperationModel
	{
		public List<string> Marks { get; set; }
		public int GroupCount { get; set; }

		public string Name { get; set; }
		internal Dictionary<string, ParameterModel> Parameter { get; private set; }
		public ResultModel Result { get; set; }

		public OperationModel()
			: this(new Dictionary<string, object>
			{
				{"Marks", new List<string>()},
				{"GroupCount", 0},
				{"Name", null},
				{"Parameters", new List<Dictionary<string, object>>()},
				{"Result", new Dictionary<string, object>
					{
						{"ViewModelId", null},
						{"IsList", false},
						{"IsVoid", false}
					}
				}
			}) { }
		public OperationModel(IDictionary<string, object> model)
		{
			Marks = ((IEnumerable)model["Marks"]).Cast<string>().ToList();
			GroupCount = (int)model["GroupCount"];

			Name = (string)model["Name"];
			Parameters = ((IEnumerable)model["Parameters"]).Cast<IDictionary<string, object>>().Select(p => new ParameterModel(p)).ToList();
			Result = new ResultModel((IDictionary<string, object>)model["Result"]);
		}

		public List<ParameterModel> Parameters
		{
			get { return Parameter.Values.ToList(); }
			set { Parameter = value.ToDictionary(kvp => kvp.Name, kvp => kvp); }
		}

		#region ToString & Equality

		public override string ToString()
		{
			return string.Format("[OperationModel: [Marks: {0}, GroupCount: {1}, Name: {2}, Parameters: {3}, Result: {4}]]", Marks.ToItemString(), GroupCount, Name, Parameters.ToItemString(), Result);
		}

		protected bool Equals(OperationModel other)
		{
			return Marks.ItemEquals(other.Marks) && GroupCount == other.GroupCount && string.Equals(Name, other.Name) && Parameters.ItemEquals(other.Parameters) && Equals(Result, other.Result);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;

			return Equals((OperationModel)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (Marks != null ? Marks.GetItemHashCode() : 0);
				hashCode = (hashCode * 397) ^ GroupCount;
				hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Parameters != null ? Parameters.GetItemHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Result != null ? Result.GetHashCode() : 0);
				return hashCode;
			}
		}

		#endregion
	}
}