using System;
using Routine.Core.Serializer;

namespace Routine.Core.Builder
{
	public class SerializerBuilder<T>
	{
		public DelegateSerializer<T> SerializeBy(Func<T, string> serializerDelegate)
		{
			return new DelegateSerializer<T>().SerializeBy(serializerDelegate);
		}

		public DelegateSerializer<T> DeserializeBy(Func<string, T> deserializerDelegate)
		{
			return new DelegateSerializer<T>().DeserializeBy(deserializerDelegate);
		}
	}
}

