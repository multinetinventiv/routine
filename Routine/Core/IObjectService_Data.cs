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

	public class ObjectReferenceData
	{
		public bool IsNull { get; set; }
		public string ActualModelId { get; set; }
		public string Id { get; set; }
		public string ViewModelId { get; set; }

		#region ToString & Equality

		public override string ToString()
		{
			return string.Format("[ObjectReferenceData: ActualModelId={0}, ViewModelId={1}, Id={2}, IsNull={3}]", ActualModelId, ViewModelId, Id, IsNull);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(ObjectReferenceData))
				return false;
			ObjectReferenceData other = (ObjectReferenceData)obj;
			return (IsNull && other.IsNull) ||
					(!IsNull && !other.IsNull && ActualModelId == other.ActualModelId && ViewModelId == other.ViewModelId && Id == other.Id);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				if (IsNull) { return IsNull.GetHashCode(); }

				return (ActualModelId != null ? ActualModelId.GetHashCode() : 0) ^ (ViewModelId != null ? ViewModelId.GetHashCode() : 0) ^ (Id != null ? Id.GetHashCode() : 0);
			}
		}

		#endregion
	}

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

	public class ParameterData
	{
		public bool IsNull { get; set; }
		public string ObjectModelId { get; set; }
		public string ReferenceId { get; set; }
		public Dictionary<string, ParameterValueData> InitializationParameters { get; set; }

		public ParameterData() { InitializationParameters = new Dictionary<string, ParameterValueData>(); }

		#region ToString & Equality

		public override string ToString()
		{
			return string.Format("[ParameterData: ObjectModelId={0}, ReferenceId={1}, IsNull={2}, InitializationParameters={3}]", 
												  ObjectModelId, ReferenceId, IsNull, InitializationParameters.ToKeyValueString());
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(ParameterData))
				return false;
			ParameterData other = (ParameterData)obj;
			return (IsNull && other.IsNull) ||
					(!IsNull && !other.IsNull && ObjectModelId == other.ObjectModelId && ReferenceId == other.ReferenceId && InitializationParameters.KeyValueEquals(other.InitializationParameters));
		}

		public override int GetHashCode()
		{
			unchecked
			{
				if (IsNull) { return IsNull.GetHashCode(); }

				return (ObjectModelId != null ? ObjectModelId.GetHashCode() : 0) ^ (ReferenceId != null ? ReferenceId.GetHashCode() : 0) ^ (InitializationParameters != null ? InitializationParameters.GetKeyValueHashCode() : 0);
			}
		}

		#endregion
	}
}
