namespace Routine.Engine.Virtual
{
	public class VirtualObject
	{
		private readonly VirtualType type;

		public string Id { get; private set; }

		public VirtualObject(string id, VirtualType type)
		{
			Id = id;
			this.type = type;
		}

		public IType Type { get { return type; } }

		public override string ToString()
		{
			return type.ToStringMethod.Get()(this);
		}

		protected bool Equals(VirtualObject other)
		{
			return Equals(type, other.type) && string.Equals(Id, other.Id);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;

			return Equals((VirtualObject) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((type != null ? type.GetHashCode() : 0)*397) ^ (Id != null ? Id.GetHashCode() : 0);
			}
		}
	}
}