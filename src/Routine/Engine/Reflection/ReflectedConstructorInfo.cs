using System.Linq;
using Routine.Core.Reflection;

namespace Routine.Engine.Reflection
{
	public class ReflectedConstructorInfo : ConstructorInfo
	{
		internal ReflectedConstructorInfo(System.Reflection.ConstructorInfo constructorInfo)
			: base(constructorInfo) { }

		protected override ConstructorInfo Load() => this;
        public override ParameterInfo[] GetParameters() => constructorInfo.GetParameters().Select(ParameterInfo.Reflected).ToArray();
        public override object[] GetCustomAttributes() => constructorInfo.GetCustomAttributes(true);

        public override object Invoke(params object[] parameters) => new ReflectionMethodInvoker(constructorInfo).Invoke(null, parameters);

        public override bool IsPublic => constructorInfo.IsPublic;
        public override TypeInfo DeclaringType => TypeInfo.Get(constructorInfo.DeclaringType);
        public override TypeInfo ReflectedType => TypeInfo.Get(constructorInfo.ReflectedType);
    }
}
