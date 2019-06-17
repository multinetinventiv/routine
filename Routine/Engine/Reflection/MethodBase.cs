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

		IType ITypeComponent.ParentType { get { return ReflectedType; } } 

		#endregion

		#region IParametric implementation

		List<IParameter> IParametric.Parameters { get { return GetParameters().Cast<IParameter>().ToList(); } } 

		#endregion
	}
}
