using System;

namespace Routine.Engine
{
	public class InitializedTypeDoNotMatchException : Exception
	{
		public InitializedTypeDoNotMatchException(IInitializer initializer, IType expected, IType actual)
			: base(string.Format("{0}.{1}: Expected initialized type is {2}, but given initialized type is {3}", initializer.ParentType.Name, initializer.Name, expected, actual)) { }
	}
}