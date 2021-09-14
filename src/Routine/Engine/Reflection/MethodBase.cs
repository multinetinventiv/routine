using System.Collections.Generic;
using System.Linq;

namespace Routine.Engine.Reflection
{
	public abstract class MethodBase : MemberInfo, IParametric
	{
		public abstract bool IsPublic { get; }

		public abstract ParameterInfo[] GetParameters();
		public abstract object[] GetCustomAttributes();

		#region ITypeComponent implementation

		IType ITypeComponent.ParentType => ReflectedType;

        #endregion

		#region IParametric implementation

		List<IParameter> IParametric.Parameters => GetParameters().Cast<IParameter>().ToList();

        #endregion
	}
}
