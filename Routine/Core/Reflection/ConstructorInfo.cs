using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Routine.Core.Reflection
{
	public abstract class ConstructorInfo : MethodBase
	{
		internal static ConstructorInfo Reflected(System.Reflection.ConstructorInfo constructor)
		{
			return new ReflectedConstructorInfo(constructor).Load();
		}

		internal static ConstructorInfo Preloaded(System.Reflection.ConstructorInfo constructor)
		{
			return new PreloadedConstructorInfo(constructor).Load();
		}

		protected readonly System.Reflection.ConstructorInfo constructorInfo;

		protected ConstructorInfo(System.Reflection.ConstructorInfo constructorInfo)
		{
			this.constructorInfo = constructorInfo;
		}

		public System.Reflection.ConstructorInfo GetActualConstructor()
		{
			return constructorInfo;
		}

		protected abstract ConstructorInfo Load();

		public abstract TypeInfo DeclaringType { get; }

		public abstract object Invoke(params object[] parameters);
	}
}
