using System;
using Routine.Core;

namespace Routine.Soa
{
	public interface ISoaClientConfiguration
	{
		string ServiceUrlBase { get;}

		IExtractor<SoaExceptionResult, Exception> ExceptionExtractor { get; }
	}
}

