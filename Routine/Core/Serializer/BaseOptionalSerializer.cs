using System;

namespace Routine.Core.Serializer
{
	public abstract class BaseOptionalSerializer<TConcrete, TSerializable> : IOptionalSerializer<TSerializable>
		where TConcrete : BaseOptionalSerializer<TConcrete, TSerializable>
	{
		private Func<TSerializable, bool> serializeWhenDelegate;
		private Func<string, bool> deserializeWhenDelegate;

		protected BaseOptionalSerializer()
		{
			SerializeWhen(s => true);
			DeserializeWhen(s => true);
		}

		public TConcrete SerializeWhen(Func<TSerializable, bool> serializeWhenDelegate) { this.serializeWhenDelegate = serializeWhenDelegate; return (TConcrete)this;}
		public TConcrete DeserializeWhen(Func<string, bool> deserializeWhenDelegate) { this.deserializeWhenDelegate = deserializeWhenDelegate; return (TConcrete)this;}

		protected virtual bool CanSerialize(TSerializable obj)
		{
			return serializeWhenDelegate(obj);
		}
		
		protected virtual bool CanDeserialize(string objString)
		{
			return deserializeWhenDelegate(objString);
		}

		private string SafeSerialize(TSerializable obj)
		{
			if(!CanSerialize(obj)) {throw new CannotSerializeException(obj);}

			return Serialize(obj);
		}

		private TSerializable SafeDeserialize(string objString)
		{
			if(!CanDeserialize(objString)) {throw new CannotDeserializeException(objString);}

			return Deserialize(objString);
		}

		private bool TrySerialize(TSerializable obj, out string result)
		{
			if(!CanSerialize(obj))
			{
				result = null;
				return false;
			}

			result = Serialize(obj);
			return true;
		}

		private bool TryDeserialize(string objString, out TSerializable result)
		{
			if(!CanDeserialize(objString))
			{
				result = default(TSerializable);
				return false;
			}

			result = Deserialize(objString);
			return true;
		}

		protected abstract string Serialize(TSerializable obj);
		protected abstract TSerializable Deserialize(string objString);

		#region IOptionalSerializer implementation
		bool IOptionalSerializer<TSerializable>.CanSerialize(TSerializable obj) { return CanSerialize(obj); }
		bool IOptionalSerializer<TSerializable>.CanDeserialize(string objString) { return CanDeserialize(objString); }

		bool IOptionalSerializer<TSerializable>.TrySerialize(TSerializable obj, out string result) { return TrySerialize(obj, out result); }
		bool IOptionalSerializer<TSerializable>.TryDeserialize(string objString, out TSerializable result) { return TryDeserialize(objString, out result); }
		#endregion

		#region ISerializer implementation
		string ISerializer<TSerializable>.Serialize(TSerializable obj) { return SafeSerialize(obj); }
		TSerializable ISerializer<TSerializable>.Deserialize(string objString) { return SafeDeserialize(objString); }
		#endregion
	}
}
