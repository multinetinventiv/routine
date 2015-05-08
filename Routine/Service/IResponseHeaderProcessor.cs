using System.Collections.Generic;

namespace Routine.Service
{
	public interface IResponseHeaderProcessor
	{
		void Process(Dictionary<string, string> responseHeaders);
	}
}