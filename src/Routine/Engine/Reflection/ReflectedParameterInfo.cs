namespace Routine.Engine.Reflection
{
	public class ReflectedParameterInfo : ParameterInfo
	{
		internal ReflectedParameterInfo(System.Reflection.ParameterInfo parameterInfo)
			: base(parameterInfo) { }

		protected override ParameterInfo Load() { return this; }

		public override MemberInfo Member { get { return MemberInfo.Reflected(parameterInfo.Member); } }
		public override string Name { get { return parameterInfo.Name; } }
		public override TypeInfo ParameterType { get { return TypeInfo.Get(parameterInfo.ParameterType); } }
		public override int Position { get { return parameterInfo.Position; } }
		public override object[] GetCustomAttributes() { return parameterInfo.GetCustomAttributes(true); }
	}

}
