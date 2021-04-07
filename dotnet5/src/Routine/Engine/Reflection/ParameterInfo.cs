using System;

namespace Routine.Engine.Reflection
{
	public abstract class ParameterInfo : IParameter
	{
		internal static ParameterInfo Reflected(System.Reflection.ParameterInfo parameterInfo)
		{
			return new ReflectedParameterInfo(parameterInfo).Load();
		}

		internal static ParameterInfo Preloaded(MemberInfo member, System.Reflection.ParameterInfo parameterInfo)
		{
			return new PreloadedParameterInfo(member, parameterInfo).Load();
		}

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
		public abstract object[] GetCustomAttributes();

		#region ITypeComponent implementation

		IType ITypeComponent.ParentType { get { return Member.ReflectedType; } } 

		#endregion

		#region IParameter implementation

		IParametric IParameter.Owner
		{
			get
			{
				if (Member is MethodBase)
				{
					return Member as MethodBase;
				}

				throw new InvalidOperationException(string.Format("This parameter does not belong to a member that implements IParametric: {0}", Member));
			}
		}
		int IParameter.Index { get { return Position; } }
		IType IParameter.ParameterType { get { return ParameterType; } } 

		#endregion
	}
}

