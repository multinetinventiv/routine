using System.Collections.Generic;

namespace Routine.Core
{
	public class InitializerModel
	{
		public List<string> Marks { get; set; }
		public int GroupCount { get; set; }

		public List<ParameterModel> Parameters { get; set; }

		public InitializerModel()
		{
			Marks = new List<string>();
			Parameters = new List<ParameterModel>();
		}

		#region ToString & Equality

		public override string ToString()
		{
			return string.Format("[InitializerModel: Marks={0}, GroupCount={1}, Parameters={2}]", Marks.ToItemString(), GroupCount, Parameters.ToItemString());
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(InitializerModel))
				return false;
			var other = (InitializerModel)obj;
			return Marks.ItemEquals(other.Marks) && GroupCount == other.GroupCount && Parameters.ItemEquals(other.Parameters);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Marks != null ? Marks.GetItemHashCode() : 0) ^ GroupCount.GetHashCode() ^ (Parameters != null ? Parameters.GetItemHashCode() : 0);
			}
		}

		#endregion
	}
}