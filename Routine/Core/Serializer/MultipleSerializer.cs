using System;
using System.Collections.Generic;

namespace Routine.Core.Serializer
{
	public class MultipleSerializer<TConfigurator, TSerializable> : ISerializer<TSerializable>
	{
		private readonly TConfigurator configurator;

		private readonly List<IOptionalSerializer<TSerializable>> serializers;

		private Func<TSerializable, CannotSerializeDeserializeException> serializationExceptionDelegate;
		private Func<string, CannotSerializeDeserializeException> deserializationExceptionDelegate;

		public MultipleSerializer(TConfigurator configurator)
		{
			this.configurator = configurator;

			this.serializers = new List<IOptionalSerializer<TSerializable>>();
			OnSerializationFailThrow(o => new CannotSerializeException(o));
			OnDeserializationFailThrow(s => new CannotDeserializeException(s));
		}

		public TConfigurator OnFailThrow(CannotSerializeDeserializeException exception) { OnSerializationFailThrow(o => exception); OnDeserializationFailThrow(s => exception); return configurator;}
		public MultipleSerializer<TConfigurator, TSerializable> OnSerializationFailThrow(Func<TSerializable, CannotSerializeDeserializeException> serializationExceptionDelegate) { this.serializationExceptionDelegate = serializationExceptionDelegate; return this; }
		public MultipleSerializer<TConfigurator, TSerializable> OnDeserializationFailThrow(Func<string, CannotSerializeDeserializeException> deserializationExceptionDelegate) { this.deserializationExceptionDelegate = deserializationExceptionDelegate; return this; }

		public TConfigurator Done() { return configurator; }
		public TConfigurator Done(IOptionalSerializer<TSerializable> serializer) { Add(serializer); return configurator; }

		public MultipleSerializer<TConfigurator, TSerializable> Add(IOptionalSerializer<TSerializable> serializer)
		{
			this.serializers.Add(serializer);

			return this;
		}

		public MultipleSerializer<TConfigurator, TSerializable> Merge(MultipleSerializer<TConfigurator, TSerializable> other)
		{
			this.serializers.AddRange(other.serializers);

			return this;
		}

		protected virtual string Serialize(TSerializable obj)
		{
			try
			{
				foreach(var serializer in serializers) 
				{
					string result;
					if(serializer.TrySerialize(obj, out result))
					{
						return result;
					}
				}
			}
			catch(CannotSerializeDeserializeException) { throw; }
			catch(Exception ex) { throw new CannotSerializeException(obj, ex); }

			throw serializationExceptionDelegate(obj);
		}
		
		protected virtual TSerializable Deserialize(string objString)
		{
			try
			{
				foreach(var serializer in serializers) 
				{
					TSerializable result;
					if(serializer.TryDeserialize(objString, out result))
					{
						return result;
					}
				}
			}
			catch(CannotSerializeDeserializeException) { throw; }
			catch(Exception ex) { throw new CannotDeserializeException(objString, ex); }

			throw deserializationExceptionDelegate(objString);
		}

		#region ISerializer implementation
		string ISerializer<TSerializable>.Serialize(TSerializable obj)
		{
			return Serialize(obj);
		}

		TSerializable ISerializer<TSerializable>.Deserialize(string objString)
		{
			return Deserialize(objString);
		}
		#endregion
	}
}

