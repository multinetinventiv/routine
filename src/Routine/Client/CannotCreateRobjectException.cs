using System;

namespace Routine.Client
{
	public class CannotCreateRobjectException : Exception
	{
		public CannotCreateRobjectException(string message) : base(message) { }
		public CannotCreateRobjectException(string message, Exception innerException) : base(message, innerException) { }
	}
}