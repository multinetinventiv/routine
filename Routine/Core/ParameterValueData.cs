using System.Collections.Generic;

namespace Routine.Core
{
	public class ParameterValueData
	{
		public bool IsList { get; set; }
		public List<ParameterData> Values { get; set; }

		public ParameterValueData() { Values = new List<ParameterData>(); }

		#region ToString & Equality

		public override string ToString()
		{
			return string.Format("[ParameterValueData: IsList={0}, Values={1}]", IsList, Values.ToItemString());
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(ParameterValueData))
				return false;
			ParameterValueData other = (ParameterValueData)obj;
			return IsList == other.IsList && Values.ItemEquals(other.Values);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return IsList.GetHashCode() ^ (Values != null ? Values.GetItemHashCode() : 0);
			}
		}

		#endregion
	}
}