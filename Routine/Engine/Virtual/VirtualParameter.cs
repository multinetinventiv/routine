using System;

namespace Routine.Engine.Virtual
{
	public class VirtualParameter : IParameter
	{
		public IType ParameterType { get; private set; }
		public string Name { get; private set; }

		public VirtualParameter(IType parameterType, string name)
		{
			if (parameterType == null) { throw new ArgumentNullException("parameterType"); }
			if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException("name"); }

			ParameterType = parameterType;
			Name = name;
		}

		IParametric IParameter.Owner { get { throw new InvalidOperationException("Virtual parameter does not have owner."); } }
		int IParameter.Index { get { return 0; } }
		IType ITypeComponent.ParentType { get { throw new InvalidOperationException("Virtual parameter does not have parent type."); } }
		object[] ITypeComponent.GetCustomAttributes() { return new object[0]; }
	}
}