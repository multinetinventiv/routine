namespace Routine.Core.Reflection
{
	public class PreloadedParameterInfo : ParameterInfo
	{
		private string name;
		private TypeInfo parameterType;
		private int position;

		internal PreloadedParameterInfo(System.Reflection.ParameterInfo parameterInfo)
			: base(parameterInfo) {}

		protected override ParameterInfo Load()
		{
			name = parameterInfo.Name;
			parameterType = TypeInfo.Get(parameterInfo.ParameterType);
			position = parameterInfo.Position;

			return this;
		}

		public override string Name{ get{return name;} }
		public override TypeInfo ParameterType{ get{return parameterType;} }
		public override int Position{get{return position;}}
	}
}
