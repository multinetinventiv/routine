using Routine.Engine.Virtual;

namespace Routine.Engine.Configuration
{
	public class VirtualTypeBuilder
	{
		public VirtualType FromBasic()
		{
			return new VirtualType()
				.ToStringMethod.Set(o => string.Format("{0} ({1})", o.Id, o.Type))
			;
		}
	}
}