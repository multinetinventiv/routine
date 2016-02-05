using System;

namespace Routine.Engine
{
	public class InitializedTypeDoNotMatchException : Exception
	{
		public InitializedTypeDoNotMatchException(IConstructor constructor, IType expected, IType actual)
			: base(string.Format("{0}.{1}: Expected initialized type is {2}, but given initialized type is {3}", constructor.ParentType.Name, constructor.Name, expected, actual)) { }
	}
}