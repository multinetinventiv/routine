using System;
using System.Collections.Generic;

namespace Routine.Soa
{
	public interface ISoaClientConfiguration
	{
		string GetServiceUrlBase();
		
		Exception GetException(SoaExceptionResult exceptionResult);

		List<string> GetHeaders();
		string GetHeaderValue(string header);
	}
}

