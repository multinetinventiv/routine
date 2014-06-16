using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Routine.Core.Reflection
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
			return constructorInfo.Invoke(parameters);
		}

		public override bool IsPublic { get { return constructorInfo.IsPublic; } }
		public override TypeInfo DeclaringType { get { return TypeInfo.Get(constructorInfo.DeclaringType); } }
	}
}
