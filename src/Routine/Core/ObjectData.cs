using System.Collections.Generic;
using System.Linq;

namespace Routine.Core
{
	public class ObjectData
	{
		public string Id { get; set; }
		public string ModelId { get; set; }
		public string Display { get; set; }
		public Dictionary<string, VariableData> Data { get; set; }

		public ObjectData()
			: this(new Dictionary<string, object>
			{
				{"Id", null},
				{"ModelId", null},
				{"Display", null},
				{"Data", new Dictionary<string ,object>()}
			}) { }
		public ObjectData(IDictionary<string, object> data)
		{
			Id = (string)data["Id"];
			ModelId = (string)data["ModelId"];
			Display = (string)data["Display"];

			Data = ((IDictionary<string, object>)data["Data"]).ToDictionary(kvp => kvp.Key, kvp => new VariableData((IDictionary<string, object>)kvp.Value));
		}

		#region ToString & Equality

		public override string ToString()
		{
			return $"[ObjectData: [Id: {Id}, ModelId: {ModelId}, Display: {Display}, Data: {Data.ToKeyValueString()}]]";
		}

		protected bool Equals(ObjectData other)
		{
			return string.Equals(Id, other.Id) && string.Equals(ModelId, other.ModelId) && string.Equals(Display, other.Display) && Data.KeyValueEquals(other.Data);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;

			return Equals((ObjectData)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (Id != null ? Id.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (ModelId != null ? ModelId.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Display != null ? Display.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Data != null ? Data.GetKeyValueHashCode() : 0);
				return hashCode;
			}
		}

		#endregion
	}
}