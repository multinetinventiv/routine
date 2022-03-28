using System;

namespace Routine.Engine.Reflection
{
    public abstract class MemberInfo
    {
        public abstract string Name { get; }
        public abstract TypeInfo ReflectedType { get; }
        public abstract TypeInfo DeclaringType { get; }

        internal static MemberInfo Reflected(System.Reflection.MemberInfo member) =>
            member switch
            {
                System.Reflection.MethodInfo methodInfo => MethodInfo.Reflected(methodInfo),
                System.Reflection.ConstructorInfo constructorInfo => ConstructorInfo.Reflected(constructorInfo),
                System.Reflection.PropertyInfo propertyInfo => PropertyInfo.Reflected(propertyInfo),
                _ => throw new NotImplementedException()
            };
    }
}
