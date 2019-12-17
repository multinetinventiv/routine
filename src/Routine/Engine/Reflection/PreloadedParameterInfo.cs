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

		public override string Name { get { return name; } }
		public override TypeInfo ParameterType { get { return parameterType; } }
		public override MemberInfo Member { get { return member; } }
		public override int Position { get { return position; } }
		public override object[] GetCustomAttributes() { return customAttributes; }
	}
}
