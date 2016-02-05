using System;
using System.Collections.Generic;

namespace Routine.Service
{
	public interface IServiceClientConfiguration
	{
		string GetServiceUrlBase();
		
		Exception GetException(ExceptionResult exceptionResult);

		List<string> GetRequestHeaders();
		string GetRequestHeaderValue(string requestHeader);
		List<IHeaderProcessor> GetResponseHeaderProcessors();
	}
}

