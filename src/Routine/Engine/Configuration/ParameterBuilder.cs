using System.Reflection;
using Routine.Engine.Virtual;

namespace Routine.Engine.Configuration
{
	public class ParameterBuilder
	{
		private readonly IParametric owner;

		public ParameterBuilder(IParametric owner)
		{
			this.owner = owner;
		}

		public IParametric Owner => owner;

        public VirtualParameter Virtual()
		{
			return new VirtualParameter(owner);
		}

		public VirtualParameter Virtual(ParameterInfo parameterInfo)
		{
			return Virtual()
				.Name.Set(parameterInfo.Name)
				.Index.Set(parameterInfo.Position)
				.ParameterType.Set(parameterInfo.ParameterType.ToTypeInfo())
			;
		}
	}
}