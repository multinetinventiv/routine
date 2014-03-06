using System;

namespace Routine.Core.Serializer
{
	public class DelegateSerializer<T> : BaseOptionalSerializer<DelegateSerializer<T>, T>
	{
		private Func<T, string> serializerDelegate;
		private Func<string, T> deserializerDelegate;

		public DelegateSerializer<T> SerializeBy(Func<T, string> serializerDelegate)
		{
			this.serializerDelegate = serializerDelegate;

			return this;
		}

		public DelegateSerializer<T> DeserializeBy(Func<string, T> deserializerDelegate)
		{
			this.deserializerDelegate = deserializerDelegate;

			return this;
		}

		protected override bool CanSerialize(T obj)
		{
			return serializerDelegate != null && base.CanSerialize(obj);
		}

		protected override string Serialize(T obj)
		{	
			return serializerDelegate(obj);
		}

		protected override bool CanDeserialize(string objString)
		{
			return deserializerDelegate != null && base.CanDeserialize(objString);
		}

		protected override T Deserialize(string objString)
		{
			return deserializerDelegate(objString);
		}
	}
}

