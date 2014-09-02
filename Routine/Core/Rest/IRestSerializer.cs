using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using Routine.Soa;

namespace Routine.Core.Rest
{
	public interface IRestSerializer
	{
		T Deserialize<T>(string responseString);
		object Deserialize(string responseString);

		string Serialize(object @object);
	}

	public class JsonRestSerializer : IRestSerializer
	{
		private readonly JavaScriptSerializer realSerializer;

		public JsonRestSerializer() : this(new JavaScriptSerializer()) { }
		public JsonRestSerializer(JavaScriptSerializer realSerializer)
		{
			if (realSerializer == null) { throw new ArgumentNullException("realSerializer"); }

			this.realSerializer = realSerializer;
		}

		public T Deserialize<T>(string responseString)
		{
			if (typeof(T) == typeof(ObjectReferenceData))
			{
				return (T)(object)SerializationExtensions.DeserializeObjectReferenceData(realSerializer.DeserializeObject(responseString));
			}

			if (typeof(T) == typeof(ObjectData))
			{
				return (T)(object)SerializationExtensions.DeserializeObjectData(realSerializer.DeserializeObject(responseString));
			}

			if (typeof(T) == typeof(ValueData))
			{
				return (T)(object)SerializationExtensions.DeserializeValueData(realSerializer.DeserializeObject(responseString));
			}

			return realSerializer.Deserialize<T>(responseString);
		}

		public object Deserialize(string responseString)
		{
			return realSerializer.DeserializeObject(responseString);
		}

		public string Serialize(object @object)
		{
			if (@object == null) { return realSerializer.Serialize(null); }

			if (@object.GetType() == typeof(ObjectReferenceData))
			{
				return realSerializer.Serialize(((ObjectReferenceData)@object).ToSerializable());
			}

			if (@object.GetType() == typeof(ObjectData))
			{
				return realSerializer.Serialize(((ObjectData)@object).ToSerializable());
			}

			if (@object.GetType() == typeof(ValueData))
			{
				return realSerializer.Serialize(((ValueData)@object).ToSerializable());
			}

			if (@object.GetType() == typeof(List<ObjectReferenceData>))
			{
				return realSerializer.Serialize(((List<ObjectReferenceData>)@object).Select(s => s.ToSerializable()).ToList());
			}

			if (@object.GetType() == typeof(List<ObjectData>))
			{
				return realSerializer.Serialize(((List<ObjectData>)@object).Select(s => s.ToSerializable()).ToList());
			}

			if (@object.GetType() == typeof(List<ValueData>))
			{
				return realSerializer.Serialize(((List<ValueData>)@object).Select(s => s.ToSerializable()).ToList());
			}

			return realSerializer.Serialize(@object);
		}
	}
}
