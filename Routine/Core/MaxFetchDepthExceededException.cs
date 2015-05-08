using System;

namespace Routine.Core
{
	public class MaxFetchDepthExceededException : Exception
	{
		public MaxFetchDepthExceededException(int maxFetchDepthAllowed, object lastObject)
			: base(string.Format("Max fetch depth ({0}) is exceeded by {1}", maxFetchDepthAllowed, lastObject)) { }
	}
}