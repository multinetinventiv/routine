using System.Collections.Generic;

namespace Routine.Core
{
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
