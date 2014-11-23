using System.Collections.Generic;

namespace Routine.Core
{
	public class ObjectData
	{
		public ObjectReferenceData Reference { get; set; }
		public string Value { get; set; }
		public Dictionary<string, ValueData> Members { get; set; }

		public ObjectData()
		{
			Reference = new ObjectReferenceData();
			Members = new Dictionary<string, ValueData>();
		}

		#region ToString & Equality

		public override string ToString()
		{
			return string.Format("[ObjectData: Reference={0}, Value={1}, Members={2}]", Reference, Value, Members.ToKeyValueString());
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(ObjectData))
				return false;
			ObjectData other = (ObjectData)obj;
			return object.Equals(Reference, other.Reference) && Value == other.Value && Members.KeyValueEquals(other.Members);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Reference != null ? Reference.GetHashCode() : 0) ^ (Value != null ? Value.GetHashCode() : 0) ^ (Members != null ? Members.GetKeyValueHashCode() : 0);
			}
		}

		#endregion
	}
}