using System;
using System.Collections.Generic;

namespace Routine.Service
{
	public interface IServiceConfiguration
	{
		string GetRootPath();
		int GetMaxResultLength();
		List<string> GetRequestHeaders();
		List<string> GetResponseHeaders();
		string GetResponseHeaderValue(string responseHeader);
		ExceptionResult GetExceptionResult(Exception exception);
	}
}

