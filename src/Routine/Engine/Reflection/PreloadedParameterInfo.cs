namespace Routine.Engine.Reflection
{
	public class PreloadedParameterInfo : ParameterInfo
	{
		private readonly MemberInfo member;

		private string name;
		private TypeInfo parameterType;
		private int position;
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
			customAttributes = parameterInfo.GetCustomAttributes(true);

			return this;
		}

		public override string Name => name;
        public override TypeInfo ParameterType => parameterType;
        public override MemberInfo Member => member;
        public override int Position => position;
        public override object[] GetCustomAttributes() { return customAttributes; }
	}
}
