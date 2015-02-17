using System;
using System.Collections.Generic;

namespace Routine.Soa
{
	public interface ISoaConfiguration
	{
		string GetRootPath();
		int GetMaxResultLength();
		List<string> GetHeaders();
		SoaExceptionResult GetExceptionResult(Exception exception);
	}
}

