using System;

namespace Routine.Engine
{
	public class ParameterTypesDoNotMatchException : Exception
	{
		public ParameterTypesDoNotMatchException(IParameter parameter, IType expected, IType actual)
			: base(string.Format("{0}.{1}(...,{2},...): Parameter's expected type is {3}, but given parameter has a type of {4}", parameter.Owner.ParentType.Name, parameter.Owner.Name, parameter.Name, expected, actual)) { }
	}
}