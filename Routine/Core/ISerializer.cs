using System;

namespace Routine.Core
{
	public interface ISerializer<T>
	{
		string Serialize(T obj);
		T Deserialize(string objString);
	}

	public interface IOptionalSerializer<T> : ISerializer<T>
	{
		bool CanSerialize(T obj);
		bool CanDeserialize(string objString);

		bool TrySerialize(T obj, out string result);
		bool TryDeserialize(string objString, out T result);
	}

	public class CannotSerializeDeserializeException : Exception
	{
		public CannotSerializeDeserializeException(string message) : base(message) {}
		public CannotSerializeDeserializeException(string message, Exception innerException) : base(message, innerException) {}
	}

	public class CannotSerializeException : CannotSerializeDeserializeException
	{
		private static string MessageFor(object obj) { return "Cannot serialize '" + obj + "'";}

		public CannotSerializeException(object obj) : base(MessageFor(obj)) {}
		public CannotSerializeException(object obj, Exception innerException) : base(MessageFor(obj), innerException) {}
	}

	public class CannotDeserializeException : CannotSerializeDeserializeException
	{
		private static string MessageFor(string objString) { return "Cannot deserialize '" + objString + "'";}

		public CannotDeserializeException(string objString) : base(MessageFor(objString)) {}
		public CannotDeserializeException(string objString, Exception innerException) : base(MessageFor(objString), innerException) {}
	}
}

