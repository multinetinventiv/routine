using System;
using System.Collections.Generic;
using Routine.Core;

namespace Routine.Service
{
	public interface IServiceConfiguration
	{
		string GetRootPath();
		int GetMaxResultLength();

		bool GetAllowGet(ObjectModel objectModel, OperationModel operationModel);

		List<string> GetRequestHeaders();
		List<IHeaderProcessor> GetRequestHeaderProcessors();

		List<string> GetResponseHeaders();
		string GetResponseHeaderValue(string responseHeader);

		ExceptionResult GetExceptionResult(Exception exception);
	}
}

