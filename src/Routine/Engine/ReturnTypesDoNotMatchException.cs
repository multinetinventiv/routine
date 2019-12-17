using System;

namespace Routine.Engine
{
	public class ReturnTypesDoNotMatchException : Exception
	{
		public ReturnTypesDoNotMatchException(IReturnable returnable, IType expected, IType actual)
			: base(string.Format("{0}.{1}: Expected return type is {2}, but given return type is {3}", returnable.ParentType.Name, returnable.Name, expected, actual)) { }
	}
}