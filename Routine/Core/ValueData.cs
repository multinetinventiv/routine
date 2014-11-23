using System.Collections.Generic;

namespace Routine.Core
{
	public class ValueData
	{
		public bool IsList { get; set; }
		public List<ObjectData> Values { get; set; }

		public ValueData() { Values = new List<ObjectData>(); }

		#region ToString & Equality

		public override string ToString()
		{
			return string.Format("[ValueData: IsList={0}, Values={1}]", IsList, Values.ToItemString());
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(ValueData))
				return false;
			ValueData other = (ValueData)obj;
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