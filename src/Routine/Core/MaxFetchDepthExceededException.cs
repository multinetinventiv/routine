using System;

namespace Routine.Core
{
	public class MaxFetchDepthExceededException : Exception
	{
		public MaxFetchDepthExceededException(int maxFetchDepthAllowed, object lastObject)
			: base($"Max fetch depth ({maxFetchDepthAllowed}) is exceeded by {lastObject}") { }
	}
}