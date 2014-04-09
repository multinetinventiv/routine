using System;
using System.Linq;

namespace Routine.Core.Reflection
{
	public abstract class ParameterInfo
	{
		internal static ParameterInfo Reflected(System.Reflection.ParameterInfo parameterInfo)
		{
			return new ReflectedParameterInfo(parameterInfo).Load();
		}

		internal static ParameterInfo Preloaded(System.Reflection.ParameterInfo parameterInfo)
		{
			return new PreloadedParameterInfo(parameterInfo).Load();
		}

		protected readonly System.Reflection.ParameterInfo parameterInfo;

		protected ParameterInfo(System.Reflection.ParameterInfo parameterInfo)
		{
			this.parameterInfo = parameterInfo;
		}

		protected abstract ParameterInfo Load();

		public abstract string Name{ get; }
		public abstract TypeInfo ParameterType{ get; }
		public abstract int Position{get;}
		public abstract object[] GetCustomAttributes();

		public bool Has<TAttribute>() where TAttribute : Attribute { return Has(type.of<TAttribute>()); }
		public bool Has(TypeInfo attributeType)
		{
			return GetCustomAttributes().Any(a => a.GetTypeInfo() == attributeType);
		}
	}
}

