using System;

namespace Routine.Test.Common
{
	[Serializable]
	public struct TypedGuid : IEquatable<TypedGuid>
	{
		public static TypedGuid Parse(string valueString)
		{
			var parts = valueString.Split('|');
			var uid = Guid.Parse(parts[0]);
			var type = Type.GetType(parts[1]);

			return new TypedGuid(uid, type);
		}

		private Guid uid;
		private Type type;

		public TypedGuid(Guid uid, Type type)
		{
			this.uid = uid;
			this.type = type;
		}

		public Guid Uid { get { return uid; } }
		public Type Type { get { return type; } }

		public string Value { get { return ToString(); } }

		public override string ToString()
		{
			return uid.ToString() + "|" + type.AssemblyQualifiedName;
		}

		public static bool operator ==(TypedGuid l, TypedGuid r) { return object.Equals(l, r); }
		public static bool operator !=(TypedGuid l, TypedGuid r) { return !(l == r); }

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(TypedGuid))
				return false;

			return Equals((TypedGuid)obj);
		}

		public bool Equals(TypedGuid other)
		{
			return uid == other.uid && type == other.type;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return uid.GetHashCode() ^ (type != null ? type.GetHashCode() : 0);
			}
		}
	}
}
