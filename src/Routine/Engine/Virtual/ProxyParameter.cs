using System;

namespace Routine.Engine.Virtual
{
	public class ProxyParameter : IParameter
	{
		private readonly IParameter real;
		private readonly int index;

		public IParametric Owner { get; }

		public ProxyParameter(IParameter real, IParametric owner) : this(real, owner, 0) { }
		public ProxyParameter(IParameter real, IParametric owner, int index)
		{
            if (index < 0) { throw new ArgumentOutOfRangeException(nameof(index), index, "'index' cannot be less than zero"); }

			this.real = real ?? throw new ArgumentNullException(nameof(real));
			this.index = index;

			Owner = owner ?? throw new ArgumentNullException(nameof(owner));
		}

        public string Name => real.Name;
        public IType ParentType => Owner.ParentType;
        public int Index => index;
        public IType ParameterType => real.ParameterType;
        public bool IsOptional => real.IsOptional;
        public bool HasDefaultValue => real.HasDefaultValue;
        public object DefaultValue => real.DefaultValue;

        public object[] GetCustomAttributes() => real.GetCustomAttributes();
    }
}