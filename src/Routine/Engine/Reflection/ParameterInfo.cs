using System;

namespace Routine.Engine.Reflection
{
    public abstract class ParameterInfo : IParameter
    {
        internal static ParameterInfo Reflected(System.Reflection.ParameterInfo parameterInfo) => new ReflectedParameterInfo(parameterInfo).Load();
        internal static ParameterInfo Preloaded(MemberInfo member, System.Reflection.ParameterInfo parameterInfo) => new PreloadedParameterInfo(member, parameterInfo).Load();

        protected readonly System.Reflection.ParameterInfo parameterInfo;

        protected ParameterInfo(System.Reflection.ParameterInfo parameterInfo)
        {
            this.parameterInfo = parameterInfo;
        }

        protected abstract ParameterInfo Load();

        public abstract MemberInfo Member { get; }
        public abstract string Name { get; }
        public abstract TypeInfo ParameterType { get; }
        public abstract int Position { get; }
        public abstract bool IsOptional { get; }
        public abstract bool HasDefaultValue { get; }
        public abstract object DefaultValue { get; }
        public abstract object[] GetCustomAttributes();

        #region ITypeComponent implementation

        IType ITypeComponent.ParentType => Member.ReflectedType;

        #endregion

        #region IParameter implementation

        IParametric IParameter.Owner =>
            Member is MethodBase methodBase
                ? methodBase
                : throw new InvalidOperationException(
                    $"This parameter does not belong to a member that implements IParametric: {Member}"
                );

        int IParameter.Index => Position;
        IType IParameter.ParameterType => ParameterType;

        #endregion
    }
}
