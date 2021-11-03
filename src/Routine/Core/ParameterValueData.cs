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
			return $"[ParameterValueData: [IsList: {IsList}, Values: {Values.ToItemString()}]]";
		}

		protected bool Equals(ParameterValueData other)
		{
			return IsList == other.IsList && Values.ItemEquals(other.Values);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;

			return Equals((ParameterValueData)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (IsList.GetHashCode() * 397) ^ (Values != null ? Values.GetItemHashCode() : 0);
			}
		}

		#endregion
	}
}