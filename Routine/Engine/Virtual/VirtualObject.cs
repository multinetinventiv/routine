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
	}
}