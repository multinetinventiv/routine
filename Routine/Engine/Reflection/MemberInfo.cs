using System;

namespace Routine.Engine.Reflection
{
	public abstract class MemberInfo
	{
		public abstract string Name { get; }
		public abstract TypeInfo ReflectedType { get; }
		public abstract TypeInfo DeclaringType { get; }

		internal static MemberInfo Reflected(System.Reflection.MemberInfo member)
		{
			if (member is System.Reflection.MethodInfo)
			{
				return MethodInfo.Reflected(member as System.Reflection.MethodInfo);
			}

			if (member is System.Reflection.ConstructorInfo)
			{
				return ConstructorInfo.Reflected(member as System.Reflection.ConstructorInfo);
			}

			if (member is System.Reflection.PropertyInfo)
			{
				return PropertyInfo.Reflected(member as System.Reflection.PropertyInfo);
			}

			throw new NotImplementedException();
		}
	}
}