using System;
using System.Collections.Generic;
using System.Linq;

namespace Routine.Core.Rest
{
	internal class SerializerHelper
	{
		private readonly IJsonSerializer realSerializer;

		public SerializerHelper(IJsonSerializer realSerializer)
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

			if (typeof(T) == typeof(ParameterData))
			{
				return (T)(object)SerializationExtensions.DeserializeParameterData(realSerializer.DeserializeObject(responseString));
			}

			if (typeof(T) == typeof(ParameterValueData))
			{
				return (T)(object)SerializationExtensions.DeserializeParameterValueData(realSerializer.DeserializeObject(responseString));
			}

			if (typeof(T) == typeof(List<ObjectReferenceData>))
			{
				var array = (object[])realSerializer.DeserializeObject(responseString);

				return (T)(object)array.Select(i => SerializationExtensions.DeserializeObjectReferenceData(i)).ToList();
			}

			if (typeof(T) == typeof(List<ObjectData>))
			{
				var array = (object[])realSerializer.DeserializeObject(responseString);

				return (T)(object)array.Select(i => SerializationExtensions.DeserializeObjectData(i)).ToList();
			}

			if (typeof(T) == typeof(List<ValueData>))
			{
				var array = (object[])realSerializer.DeserializeObject(responseString);

				return (T)(object)array.Select(i => SerializationExtensions.DeserializeValueData(i)).ToList();
			}

			if (typeof(T) == typeof(List<ParameterData>))
			{
				var array = (object[])realSerializer.DeserializeObject(responseString);

				return (T)(object)array.Select(i => SerializationExtensions.DeserializeParameterData(i)).ToList();
			}

			if (typeof(T) == typeof(List<ParameterValueData>))
			{
				var array = (object[])realSerializer.DeserializeObject(responseString);

				return (T)(object)array.Select(i => SerializationExtensions.DeserializeParameterValueData(i)).ToList();
			}

			if (typeof(T) == typeof(Dictionary<string, ObjectReferenceData>))
			{
				var dict = (Dictionary<string, object>)realSerializer.DeserializeObject(responseString);

				return (T)(object)dict.ToDictionary(kvp => kvp.Key, kvp => SerializationExtensions.DeserializeObjectReferenceData(kvp.Value));
			}

			if (typeof(T) == typeof(Dictionary<string, ObjectData>))
			{
				var dict = (Dictionary<string, object>)realSerializer.DeserializeObject(responseString);

				return (T)(object)dict.ToDictionary(kvp => kvp.Key, kvp => SerializationExtensions.DeserializeObjectData(kvp.Value));
			}

			if (typeof(T) == typeof(Dictionary<string, ValueData>))
			{
				var dict = (Dictionary<string, object>)realSerializer.DeserializeObject(responseString);

				return (T)(object)dict.ToDictionary(kvp => kvp.Key, kvp => SerializationExtensions.DeserializeValueData(kvp.Value));
			}

			if (typeof(T) == typeof(Dictionary<string, ParameterData>))
			{
				var dict = (Dictionary<string, object>)realSerializer.DeserializeObject(responseString);

				return (T)(object)dict.ToDictionary(kvp => kvp.Key, kvp => SerializationExtensions.DeserializeParameterData(kvp.Value));
			}

			if (typeof(T) == typeof(Dictionary<string, ParameterValueData>))
			{
				var dict = (Dictionary<string, object>)realSerializer.DeserializeObject(responseString);

				return (T)(object)dict.ToDictionary(kvp => kvp.Key, kvp => SerializationExtensions.DeserializeParameterValueData(kvp.Value));
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

			if (@object.GetType() == typeof(ParameterData))
			{
				return realSerializer.Serialize(((ParameterData)@object).ToSerializable());
			}

			if (@object.GetType() == typeof(ParameterValueData))
			{
				return realSerializer.Serialize(((ParameterValueData)@object).ToSerializable());
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

			if (@object.GetType() == typeof(List<ParameterData>))
			{
				return realSerializer.Serialize(((List<ParameterData>)@object).Select(s => s.ToSerializable()).ToList());
			}

			if (@object.GetType() == typeof(List<ParameterValueData>))
			{
				return realSerializer.Serialize(((List<ParameterValueData>)@object).Select(s => s.ToSerializable()).ToList());
			}

			if (@object.GetType() == typeof(Dictionary<string, ObjectReferenceData>))
			{
				return realSerializer.Serialize(((Dictionary<string, ObjectReferenceData>)@object).ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToSerializable()));
			}

			if (@object.GetType() == typeof(Dictionary<string, ObjectData>))
			{
				return realSerializer.Serialize(((Dictionary<string, ObjectData>)@object).ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToSerializable()));
			}

			if (@object.GetType() == typeof(Dictionary<string, ValueData>))
			{
				return realSerializer.Serialize(((Dictionary<string, ValueData>)@object).ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToSerializable()));
			}

			if (@object.GetType() == typeof(Dictionary<string, ParameterData>))
			{
				return realSerializer.Serialize(((Dictionary<string, ParameterData>)@object).ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToSerializable()));
			}

			if (@object.GetType() == typeof(Dictionary<string, ParameterValueData>))
			{
				return realSerializer.Serialize(((Dictionary<string, ParameterValueData>)@object).ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToSerializable()));
			}

			return realSerializer.Serialize(@object);
		}
	}
}