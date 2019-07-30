using System;

namespace Routine.Service.HandlerActions.Exceptions
{
	public class BadRequestException: Exception
	{
		public BadRequestException(Exception inner) : base(inner.Message, inner) { }
	}
}