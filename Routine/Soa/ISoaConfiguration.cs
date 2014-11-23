using System;
using System.Collections.Generic;

namespace Routine.Soa
{
	public interface ISoaConfiguration
	{
		string GetActionRouteName();
		int GetMaxResultLength();
		List<string> GetHeaders();
		SoaExceptionResult GetExceptionResult(Exception exception);
	}
}

