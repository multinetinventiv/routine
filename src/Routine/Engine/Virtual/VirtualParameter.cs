using Routine.Core.Configuration;
using System;

namespace Routine.Engine.Virtual
{
	public class VirtualParameter : IParameter
	{
		private readonly IParametric owner;

		public SingleConfiguration<VirtualParameter, string> Name { get; }
		public SingleConfiguration<VirtualParameter, IType> ParameterType { get; }
		public SingleConfiguration<VirtualParameter, int> Index { get; }

		public VirtualParameter(IParametric owner)
		{
			this.owner = owner;

			Name = new SingleConfiguration<VirtualParameter, string>(this, "Name", true);
			ParameterType = new SingleConfiguration<VirtualParameter, IType>(this, "ParameterType", true);
			Index = new SingleConfiguration<VirtualParameter, int>(this, "Index");
		}

		#region ITypeComponent implementation

		object[] ITypeComponent.GetCustomAttributes() { return Array.Empty<object>(); }

		string ITypeComponent.Name => Name.Get();
        IType ITypeComponent.ParentType => owner.ParentType;

        #endregion
		
		#region IParameter implementation

		IParametric IParameter.Owner => owner;
        IType IParameter.ParameterType => ParameterType.Get();
        int IParameter.Index => Index.Get();

        #endregion

	}
}