namespace Routine.Engine.Reflection
{
	public class PreloadedParameterInfo : ParameterInfo
	{
		private readonly MemberInfo member;

		private string name;
		private TypeInfo parameterType;
		private int position;
        private bool isOptional;
        private bool hasDefaultValue;
        private object defaultValue;
        private object[] customAttributes;

		internal PreloadedParameterInfo(MemberInfo member, System.Reflection.ParameterInfo parameterInfo)
			: base(parameterInfo)
		{
			this.member = member;
		}

		protected override ParameterInfo Load()
		{
			name = parameterInfo.Name;
			parameterType = TypeInfo.Get(parameterInfo.ParameterType);
			position = parameterInfo.Position;
            isOptional = parameterInfo.IsOptional;
            hasDefaultValue = parameterInfo.HasDefaultValue;
            defaultValue = parameterInfo.DefaultValue;
			customAttributes = parameterInfo.GetCustomAttributes(true);

			return this;
		}
		
        public override MemberInfo Member => member;
		public override string Name => name;
        public override TypeInfo ParameterType => parameterType;
        public override int Position => position;
        public override bool IsOptional => isOptional;
        public override bool HasDefaultValue => hasDefaultValue;
        public override object DefaultValue => defaultValue;
        public override object[] GetCustomAttributes() => customAttributes;
    }
}
