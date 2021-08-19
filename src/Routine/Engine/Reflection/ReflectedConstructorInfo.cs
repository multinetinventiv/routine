using System.Linq;
using Routine.Core.Reflection;

namespace Routine.Engine.Reflection
{
	public class ReflectedConstructorInfo : ConstructorInfo
	{
		internal ReflectedConstructorInfo(System.Reflection.ConstructorInfo constructorInfo)
			: base(constructorInfo) { }

		protected override ConstructorInfo Load() { return this; }
		public override ParameterInfo[] GetParameters() { return constructorInfo.GetParameters().Select(p => ParameterInfo.Reflected(p)).ToArray(); }
		public override object[] GetCustomAttributes() { return constructorInfo.GetCustomAttributes(true); }

		public override object Invoke(params object[] parameters)
		{
			return new ReflectionMethodInvoker(constructorInfo).Invoke(null, parameters);
		}

		public override bool IsPublic => constructorInfo.IsPublic;
        public override TypeInfo DeclaringType => TypeInfo.Get(constructorInfo.DeclaringType);
        public override TypeInfo ReflectedType => TypeInfo.Get(constructorInfo.ReflectedType);
    }
}
